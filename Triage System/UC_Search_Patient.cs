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
    public partial class UC_Search_Patient : UserControl
    {
        string connectionString = "server=localhost;user=root;password=;database=triage_system";
        private NewPatient _newPatientForm = null;

        public UC_Search_Patient()
        {
            InitializeComponent();

            // Load everyone when the screen first opens!
            LoadPatientData("");
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (_newPatientForm == null || _newPatientForm.IsDisposed)
            {
                _newPatientForm = new NewPatient();
                _newPatientForm.Show();
            }
            else
            {
                _newPatientForm.BringToFront();
            }
        }

        // --- DATABASE LOAD & SEARCH LOGIC ---

        private void LoadPatientData(string searchTerm)
        {
            guna2DataGridView1.AutoGenerateColumns = false;
            guna2DataGridView1.Rows.Clear();

            // 1. Changed to 'mobile_number' here!
            string query = @"SELECT patient_id, first_name, last_name, birthdate, mobile_number, created_at, status 
                             FROM patient_registration 
                             WHERE CONCAT(first_name, ' ', last_name) LIKE @search 
                             OR patient_id LIKE @search
                             ORDER BY patient_id DESC";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                long rawId = Convert.ToInt64(reader["patient_id"]);
                                string formattedId = "N-" + rawId.ToString("D3");
                                string fullName = reader["first_name"].ToString() + " " + reader["last_name"].ToString();

                                DateTime dob = Convert.ToDateTime(reader["birthdate"]);
                                string dobString = dob.ToString("MMM dd, yyyy");

                                // 2. Changed to 'mobile_number' here too!
                                string contact = reader["mobile_number"].ToString();

                                DateTime lastVisit = Convert.ToDateTime(reader["created_at"]);
                                string lastVisitString = lastVisit.ToString("MMM dd, yyyy");

                                string status = reader["status"].ToString();

                                guna2DataGridView1.Rows.Add(formattedId, fullName, dobString, contact, lastVisitString, status);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error");
            }
        }

        // --- MAKE THE SEARCH BOX WORK ---
        // Double-click your Search text box in the designer. 
        // If the event it generates has a different name, just paste the line inside it!
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            // Cast 'sender' to a Guna2TextBox to grab its text dynamically
            Guna.UI2.WinForms.Guna2TextBox searchBox = (Guna.UI2.WinForms.Guna2TextBox)sender;
            LoadPatientData(searchBox.Text.Trim());
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            // 1. Check if the nurse actually clicked on a patient row
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                // 2. Grab the ID from the first column of the selected row (e.g., "N-1011")
                string selectedIdText = guna2DataGridView1.SelectedRows[0].Cells[0].Value.ToString();

                // Strip the "N-" to get the raw database number (1011)
                int patientId = 0;
                int.TryParse(selectedIdText.Replace("N-", ""), out patientId);

                // 3. Update the database!
                string connectionString = "server=localhost;user=root;password=;database=triage_system";
                string query = "UPDATE patient_registration SET status = 'Waiting' WHERE patient_id = @id";

                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", patientId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show($"Patient {selectedIdText} successfully added to today's queue!", "Queue Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh the table so the nurse sees the status change instantly
                    LoadPatientData(txtSearchBox.Text.Trim());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select a patient from the table first.", "Select Patient", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}