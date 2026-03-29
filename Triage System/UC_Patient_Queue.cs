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
        string currentPatientId = "";
        string currentQueueType = "";

        private string currentCategoryFilter = "";
        private string currentSearchTerm = "";

        public UC_Patient_Queue()
        {
            InitializeComponent();
            ResetButtons();
        }

        private void UC_Patient_Queue_Load(object sender, EventArgs e)
        {
            FetchNextPatient();
            searchData("");
        }

        private void FetchNextPatient()
        {
            // LIMIT 1 para laging yung pinaka-matagal nang naghihintay ang unang lilitaw
            string query = "SELECT * FROM patient_registration WHERE status IN ('Waiting', 'Calling') AND queue_number IS NOT NULL ORDER BY created_at ASC LIMIT 1";

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
                            currentPatientId = reader["patient_id"].ToString();
                            lblQueueNumber.Text = reader["queue_number"].ToString();
                            currentQueueType = reader["queue_type"].ToString();
                            string status = reader["status"].ToString();

                            // I-update ang UI labels (Siguraduhin na ang labels ay tama sa designer mo)
                            lblPatientName.Text = $"Patient: {reader["first_name"]} {reader["last_name"]}\nService: {currentQueueType}";
                            lblPriority.Text = "Priority: " + reader["priority"];

                            // SMART UI: I-set ang kulay base sa status mula sa DB
                            if (status == "Calling")
                            {
                                SetCallingUI();
                            }
                            else
                            {
                                ResetButtons();
                            }

                            btnCallPatient.Enabled = true;
                        }
                        else
                        {
                            currentPatientId = "";
                            lblQueueNumber.Text = "---";
                            lblPatientName.Text = "Patient: No waiting patients\nService: ---";
                            lblPriority.Text = "Priority: ---";
                            ResetButtons();
                            btnCallPatient.Enabled = false;
                        }
                    }
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Error FetchNextPatient: " + ex.Message); }
            }
        }

        private void searchData(string searchTerm)
        {
            currentSearchTerm = searchTerm;
            if (this.Controls.Find("guna2DataGridView1", true).FirstOrDefault() is DataGridView grid)
            {
                grid.Rows.Clear();
                string query = @"SELECT patient_id, queue_number, first_name, last_name, queue_type, priority 
                                 FROM patient_registration 
                                 WHERE status = 'Waiting' 
                                 AND queue_number IS NOT NULL 
                                 AND (CONCAT_WS(' ', first_name, last_name) LIKE @search OR queue_number LIKE @search)";

                if (!string.IsNullOrEmpty(currentCategoryFilter)) query += " AND queue_type = @category";
                query += " ORDER BY created_at ASC";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");
                            if (!string.IsNullOrEmpty(currentCategoryFilter)) cmd.Parameters.AddWithValue("@category", currentCategoryFilter);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    grid.Rows.Add(
                                        reader["patient_id"].ToString(),
                                        reader["queue_number"].ToString(),
                                        reader["first_name"].ToString() + " " + reader["last_name"].ToString(),
                                        reader["queue_type"].ToString(),
                                        reader["priority"].ToString()
                                    );
                                }
                            }
                        }
                    }
                    catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Search Error: " + ex.Message); }
                }
            }
        }

        private void UpdatePatientStatus(string newStatus)
        {
            if (string.IsNullOrEmpty(currentPatientId)) return;
            string query = "UPDATE patient_registration SET status = @status WHERE patient_id = @id";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", newStatus);
                        cmd.Parameters.AddWithValue("@id", currentPatientId);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Status Update Error: " + ex.Message); }
            }
        }

        // --- UI STATES ---
        private void ResetButtons()
        {
            btnCallPatient.Text = "CALL PATIENT";
            btnCallPatient.FillColor = Color.FromArgb(46, 204, 113); // Solid Green
            btnCallPatient.FillColor2 = Color.FromArgb(46, 204, 113);

            btnStartTriage.Enabled = false;
            btnStartTriage.FillColor = Color.DarkGray;
            btnStartTriage.FillColor2 = Color.DarkGray;
        }

        private void SetCallingUI()
        {
            btnCallPatient.Text = "CALL AGAIN";
            btnCallPatient.FillColor = Color.FromArgb(255, 145, 77); // Solid Orange
            btnCallPatient.FillColor2 = Color.FromArgb(255, 145, 77);

            btnStartTriage.Enabled = true;
            btnStartTriage.FillColor = Color.FromArgb(46, 204, 113); // Solid Green
            btnStartTriage.FillColor2 = Color.FromArgb(46, 204, 113);
        }

        private void btnCallPatient_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentPatientId)) return;

            // I-apply ang visual changes
            SetCallingUI();

            // I-update ang status at i-refresh ang timestamp para mag-trigger ang Monitor voice
            string query = "UPDATE patient_registration SET status = 'Calling', created_at = CURRENT_TIMESTAMP WHERE patient_id = @id";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", currentPatientId);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex) { MessageBox.Show("DB Error: " + ex.Message); }
            }
            searchData(currentSearchTerm);
        }

        private void btnNoShow_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentPatientId)) return;

            if (MessageBox.Show("Mark as No Show?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                UpdatePatientStatus("No Show");
                ResetButtons();
                FetchNextPatient();
                searchData(currentSearchTerm);
            }
        }

        private void btnStartTriage_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Start triage?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                UpdatePatientStatus("Serving");

                string pName = "", pSex = "", qId = lblQueueNumber.Text;
                int pAge = 0;
                string query = "SELECT first_name, last_name, gender, birthdate FROM patient_registration WHERE patient_id = @id";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", currentPatientId);
                        using (MySqlDataReader r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                pName = r["first_name"].ToString() + " " + r["last_name"].ToString();
                                pSex = r["gender"].ToString();
                                DateTime bday = Convert.ToDateTime(r["birthdate"]);
                                pAge = DateTime.Today.Year - bday.Year;
                                if (bday.Date > DateTime.Today.AddYears(-pAge)) pAge--;
                            }
                        }
                    }
                }

                UC_Start_Triage nextScreen = new UC_Start_Triage(currentPatientId, pName, qId, pSex, pAge, currentQueueType);
                Control p = this.Parent;
                p.Controls.Clear();
                nextScreen.Dock = DockStyle.Fill;
                p.Controls.Add(nextScreen);
            }
        }

        // Filters and Search
        private void txtSearch_TextChanged_1(object sender, EventArgs e) => searchData(txtSearch.Text.Trim());
        private void btnAll_Click_1(object sender, EventArgs e) { currentCategoryFilter = ""; searchData(currentSearchTerm); }
        private void btnConsultation_Click_1(object sender, EventArgs e) { currentCategoryFilter = "Consultation / Follow-Up"; searchData(currentSearchTerm); }
        private void btnDiagnostics_Click_1(object sender, EventArgs e) { currentCategoryFilter = "Diagnostics (Lab/Rad)"; searchData(currentSearchTerm); }
        private void btbnBilling_Click(object sender, EventArgs e) { currentCategoryFilter = "Billing & Cashier"; searchData(currentSearchTerm); }
    }
}