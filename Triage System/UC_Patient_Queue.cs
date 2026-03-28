using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Triage_System
{
    public partial class UC_Patient_Queue : UserControl
    {
        string connectionString = "server=localhost;user=root;password=;database=triage_system";
        string currentPatientId = ""; // Remembers the ID of the person currently on screen

        public UC_Patient_Queue()
        {
            InitializeComponent();
            ResetButtons();
        }

        private void UC_Patient_Queue_Load(object sender, EventArgs e)
        {
            FetchNextPatient(); // Load the first person in line immediately
        }

        // --- DATABASE LOGIC ---

        private void FetchNextPatient()
        {
            // Gets the OLDEST patient (FIFO) who is currently "Waiting"
            string query = "SELECT * FROM patient_registration WHERE status = 'Waiting' ORDER BY patient_id ASC LIMIT 1";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            long rawId = Convert.ToInt64(reader["patient_id"]);
                            currentPatientId = rawId.ToString(); // Save the ID for the buttons to use

                            string fName = reader["first_name"].ToString();
                            string mName = reader["middle_name"].ToString();
                            string lName = reader["last_name"].ToString();

                            // Calculate Age dynamically
                            DateTime birthdate = Convert.ToDateTime(reader["birthdate"]);
                            int age = DateTime.Today.Year - birthdate.Year;

                            // Adjust age if they haven't had their birthday yet this year
                            if (birthdate.Date > DateTime.Today.AddYears(-age))
                            {
                                age--;
                            }

                            // Determine Priority
                            string priority = (age >= 60) ? "Senior Citizen" : "Regular";

                            // UPDATE YOUR LABELS HERE
                            lblQueueNumber.Text = "N-" + rawId.ToString("D3");
                            lblPatientName.Text = $"Patient: {fName} {mName} {lName}";

                            // Update Priority Label
                            lblPriority.Text = "Priority: " + priority;

                            // Enable the call button because we have a patient
                            btnCallPatient.Enabled = true;
                        }
                        else
                        {
                            // Nobody is in line!
                            currentPatientId = "";
                            lblQueueNumber.Text = "---";
                            lblPatientName.Text = "Patient: No waiting patients";
                            lblPriority.Text = "Priority: ---"; // Reset priority label
                            btnCallPatient.Enabled = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message);
                }
            }
        }

        private void UpdatePatientStatus(string newStatus)
        {
            if (string.IsNullOrEmpty(currentPatientId)) return;

            string query = "UPDATE patient_registration SET status = @status WHERE patient_id = @id";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@status", newStatus);
                    cmd.Parameters.AddWithValue("@id", currentPatientId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // --- UI BUTTON LOGIC ---

        private void ResetButtons()
        {
            // 1. Reset Text
            btnCallPatient.Text = "CALL PATIENT";

            // 2. Reset Colors (Green)
            Color greenColor = Color.FromArgb(46, 204, 113);
            btnCallPatient.FillColor = greenColor;
            btnCallPatient.FillColor2 = greenColor;

            // 3. THE FIX: Wrap in new Bitmap() para hindi ma-delete ng system sa memory!
            if (Properties.Resources.CallPatient != null)
            {
                btnCallPatient.Image = new Bitmap(Properties.Resources.CallPatient);
            }

            // 4. Lock Start Triage
            btnStartTriage.Enabled = false;
            btnStartTriage.FillColor = Color.DarkGray;
            btnStartTriage.FillColor2 = Color.DarkGray;
        }

        private void btnCallPatient_Click(object sender, EventArgs e)
        {
            Color orangeColor = Color.FromArgb(255, 145, 77);

            if (btnCallPatient.Text == "CALL PATIENT")
            {
                // --- 1. CHANGE TO ORANGE STATE ---
                btnCallPatient.Text = "CALL AGAIN";
                btnCallPatient.FillColor = orangeColor;
                btnCallPatient.FillColor2 = orangeColor;

                // THE FIX: Wrap in new Bitmap() again!
                if (Properties.Resources.CallAgain != null)
                {
                    btnCallPatient.Image = new Bitmap(Properties.Resources.CallAgain);
                }

                btnStartTriage.Enabled = true;
                btnStartTriage.FillColor = Color.FromArgb(46, 204, 113);
                btnStartTriage.FillColor2 = Color.FromArgb(46, 204, 113);

                // Update Database so the Queue Monitor sees them!
                UpdatePatientStatus("Calling");
            }
            else
            {
                // --- 2. LOGIC FOR "CALL AGAIN" ---
                MessageBox.Show("Calling patient again...");
            }
        }

        private void btnNoShow_Click(object sender, EventArgs e)
        {
            // Mark as No Show in the database
            UpdatePatientStatus("No Show");

            // Reset UI and grab the next person in line
            ResetButtons();
            FetchNextPatient();
        }

        private void btnStartTriage_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Start triage?", "Confirm", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                // 1. Update Database to show they are inside with the nurse
                UpdatePatientStatus("Serving");

                // --- 2. FETCH THE REAL PATIENT DATA ---
                string pName = "Unknown";
                string pSex = "Unknown";
                int pAge = 0;
                string pQueueId = lblQueueNumber.Text; // Grabs "N-1008" directly from your screen

                // Quick query to grab their exact details before changing screens
                string query = "SELECT first_name, last_name, sex, birthdate FROM patient_registration WHERE patient_id = @id";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", currentPatientId);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    pName = reader["first_name"].ToString() + " " + reader["last_name"].ToString();
                                    pSex = reader["sex"].ToString();

                                    // Calculate Age dynamically
                                    DateTime bday = Convert.ToDateTime(reader["birthdate"]);
                                    pAge = DateTime.Today.Year - bday.Year;
                                    if (bday.Date > DateTime.Today.AddYears(-pAge)) pAge--;
                                }
                            }
                        }
                    }
                    catch { /* Silent fail */ }
                }

                ResetButtons();

                // --- 3. OPEN THE TRIAGE SCREEN WITH THE REAL DATA ---
                // We pass all the variables we just grabbed into the new screen! No more red error!
                UC_Start_Triage nextScreen = new UC_Start_Triage(Convert.ToInt32(currentPatientId), pName, pQueueId, pSex, pAge);

                // If you are tracking the active session in Form1, uncomment this:
                // Form1.ActiveTriageSession = nextScreen; 

                Control mainPanel = this.Parent;
                if (mainPanel != null)
                {
                    mainPanel.Controls.Clear();
                    nextScreen.Dock = DockStyle.Fill;
                    mainPanel.Controls.Add(nextScreen);
                    nextScreen.BringToFront();
                }
            }
        }
    }
}