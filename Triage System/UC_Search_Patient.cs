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
        // Connection string sa iyong XAMPP database
        string connectionString = "server=localhost;user=root;password=;database=triage_system";
        private NewPatient _newPatientForm = null;

        public UC_Search_Patient()
        {
            InitializeComponent();
            // I-load agad ang lahat ng pasyente pagkabukas ng module
            LoadPatientData("");
        }

        // --- DATABASE LOAD & SEARCH LOGIC ---
        private void LoadPatientData(string searchTerm)
        {
            guna2DataGridView1.AutoGenerateColumns = false;
            guna2DataGridView1.Rows.Clear();

            // SQL Query: Supporta para sa P-2026-XXXXXX format at real-time search
            string query = @"SELECT patient_id, first_name, last_name, birthdate, mobile_number, created_at, status 
                 FROM patient_registration 
                 WHERE (CONCAT_WS(' ', first_name, last_name) LIKE @search 
                 OR patient_id LIKE @search)
                 ORDER BY created_at DESC";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Ang '%' wildcard ay para sa partial matching sa search
                        cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Gamitin ang direct string dahil VARCHAR na ang patient_id
                                string formattedId = reader["patient_id"].ToString();
                                string fullName = reader["first_name"].ToString() + " " + reader["last_name"].ToString();

                                DateTime dob = Convert.ToDateTime(reader["birthdate"]);
                                string dobString = dob.ToString("MMM dd, yyyy");

                                string contact = reader["mobile_number"].ToString();

                                DateTime lastVisit = Convert.ToDateTime(reader["created_at"]);
                                string lastVisitString = lastVisit.ToString("MMM dd, yyyy");

                                string status = reader["status"].ToString();

                                // Determine which IMAGE to show based on status
                                Image actionIcon;
                                if (status.Equals("Waiting", StringComparison.OrdinalIgnoreCase) ||
                                    status.Equals("Waiting for Doctor", StringComparison.OrdinalIgnoreCase))
                                {
                                    actionIcon = Properties.Resources.Add_To_Queue_Not;
                                }
                                else
                                {
                                    actionIcon = Properties.Resources.Add_To_Queue;
                                }

                                // Siguraduhing may 7 columns ang DataGridView mo sa designer
                                guna2DataGridView1.Rows.Add(formattedId, fullName, dobString, contact, lastVisitString, status, actionIcon);
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
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            // 1. Tama ito, keep this line. 
            // Kinukuha nito ang mismong textbox na nag-type ka.
            Guna.UI2.WinForms.Guna2TextBox searchBox = (Guna.UI2.WinForms.Guna2TextBox)sender;

            // 2. I-pass ang trimmed text sa LoadPatientData.
            // Siguraduhin na ang LoadPatientData mo ay gamit na ang CONCAT_WS sa SQL query.
            LoadPatientData(searchBox.Text.Trim());
        }

        // --- HANDLE THE ACTION BUTTON CLICK & UPDATE DATABASE ---
        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 6) // Action Button Column
            {
                string status = guna2DataGridView1.Rows[e.RowIndex].Cells[5].Value?.ToString();

                // Clickable lang kung wala pa sa queue
                if (!string.Equals(status, "Waiting", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(status, "Waiting for Doctor", StringComparison.OrdinalIgnoreCase))
                {
                    string patientId = guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                    string patientName = guna2DataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

                    // Confirmation modal
                    using (Add_In_Queue confirmModal = new Add_In_Queue(patientName, patientId))
                    {
                        confirmModal.StartPosition = FormStartPosition.CenterScreen;
                        if (confirmModal.ShowDialog() == DialogResult.Yes)
                        {
                            UpdatePatientStatus(patientId, "Waiting");
                        }
                    }
                }
                else
                {
                    // Modal kapag nasa queue na ang pasyente
                    using (Already_In_Queue noticeModal = new Already_In_Queue())
                    {
                        noticeModal.StartPosition = FormStartPosition.CenterScreen;
                        noticeModal.ShowDialog();
                    }
                }
            }
        }

        private void UpdatePatientStatus(string id, string newStatus)
        {
            string updateQuery = "UPDATE patient_registration SET status = @status WHERE patient_id = @id";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", newStatus);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                        LoadPatientData(""); // I-refresh ang listahan
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update status: " + ex.Message, "Database Error");
            }
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            // Pagbubukas ng New Patient Registration side panel
            using (New_Patient_Registration regPanel = new New_Patient_Registration())
            {
                regPanel.StartPosition = FormStartPosition.Manual;
                Point absoluteLocation = this.PointToScreen(Point.Empty);
                int targetX = absoluteLocation.X + this.Width - regPanel.Width;
                regPanel.Location = new Point(targetX, absoluteLocation.Y);
                regPanel.Height = this.Height;
                regPanel.ShowDialog();
                LoadPatientData(""); // I-refresh pagkatapos mag-register
            }
        }

        // Iba pang buttons
        private void guna2Button1_Click(object sender, EventArgs e) { /* Existing functionality */ }
        private void guna2Button5_Click(object sender, EventArgs e) { }
        private void guna2Button1_Click_1(object sender, EventArgs e) { }
        private void guna2Button3_Click(object sender, EventArgs e) { }
        private void guna2Button4_Click(object sender, EventArgs e) { }

        private void txtSearchBox_TextChanged(object sender, EventArgs e)
        {
            // 1. Tama ito, keep this line. 
            // Kinukuha nito ang mismong textbox na nag-type ka.
            Guna.UI2.WinForms.Guna2TextBox searchBox = (Guna.UI2.WinForms.Guna2TextBox)sender;

            // 2. I-pass ang trimmed text sa LoadPatientData.
            // Siguraduhin na ang LoadPatientData mo ay gamit na ang CONCAT_WS sa SQL query.
            LoadPatientData(searchBox.Text.Trim());
        }
    }
}