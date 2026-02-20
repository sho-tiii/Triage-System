using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient; // Make sure this is here!

namespace Triage_System
{
    public partial class UC_Dashboard : UserControl
    {
        string connectionString = "server=localhost;user=root;password=;database=triage_system";

        public UC_Dashboard()
        {
            InitializeComponent();
            LoadQueueData(); // We changed this from LoadDummyData!
        }

        private void LoadQueueData()
        {
            // 1. Prevent auto-generation (keeps your beautiful designer columns)
            guna2DataGridView1.AutoGenerateColumns = false;

            // 2. Clear out any old rows from the designer
            guna2DataGridView1.Rows.Clear();

            // 3. The Query: Grab everyone, sorting them so the oldest Waiting patients are at the top
            string query = @"SELECT patient_id, first_name, last_name, created_at, birthdate, status 
                             FROM patient_registration 
                             ORDER BY 
                                FIELD(status, 'Calling', 'Serving', 'Waiting', 'Waiting for Doctor', 'Completed', 'No Show'), 
                                created_at ASC";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // --- Grab and Format the Data ---

                            // 1. Queue Number
                            long rawId = Convert.ToInt64(reader["patient_id"]);
                            string queueNo = "N-" + rawId.ToString("D3");

                            // 2. Full Name
                            string patientName = reader["first_name"].ToString() + " " + reader["last_name"].ToString();

                            // 3. Time In (Formats to e.g., "10:30 AM")
                            DateTime timeIn = Convert.ToDateTime(reader["created_at"]);
                            string timeString = timeIn.ToString("hh:mm tt");

                            // 4. Service 
                            string service = "Consultation"; // Standard default

                            // 5. Priority (Calculate Senior Citizen status dynamically)
                            DateTime birthdate = Convert.ToDateTime(reader["birthdate"]);
                            int age = DateTime.Today.Year - birthdate.Year;
                            if (birthdate.Date > DateTime.Today.AddYears(-age)) age--;
                            string priority = (age >= 60) ? "Senior Citizen" : "Regular";

                            // 6. Status
                            string status = reader["status"].ToString();

                            // --- Add the Row to the DataGridView ---
                            // Ensure this matches the exact order of your columns in the screenshots!
                            guna2DataGridView1.Rows.Add(queueNo, patientName, timeString, service, priority, status);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UC_Dashboard_Load(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnQueue_Click(object sender, EventArgs e)
        {
         
        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}