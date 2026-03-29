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
        private string currentStatusFilter = "";

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

            // SQL Query: CONCAT_WS na para iwas error sa search, tapos may Dynamic Filter
            string query = @"SELECT patient_id, first_name, last_name, birthdate, mobile_number, created_at, status 
                             FROM patient_registration 
                             WHERE (CONCAT_WS(' ', first_name, last_name) LIKE @search 
                             OR patient_id LIKE @search)";

            // Kung hindi "All" ang pinili, idadagdag natin yung filter sa query!
            if (!string.IsNullOrEmpty(currentStatusFilter))
            {
                query += " AND status = @statusFilter";
            }

            query += " ORDER BY created_at DESC";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");

                        // I-pass natin ang status kung may piniling filter button
                        if (!string.IsNullOrEmpty(currentStatusFilter))
                        {
                            cmd.Parameters.AddWithValue("@statusFilter", currentStatusFilter);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string formattedId = reader["patient_id"].ToString();
                                string fullName = reader["first_name"].ToString() + " " + reader["last_name"].ToString();
                                DateTime dob = Convert.ToDateTime(reader["birthdate"]);
                                string dobString = dob.ToString("MMM dd, yyyy");
                                string contact = reader["mobile_number"].ToString();
                                DateTime lastVisit = Convert.ToDateTime(reader["created_at"]);
                                string lastVisitString = lastVisit.ToString("MMM dd, yyyy");
                                string status = reader["status"].ToString();

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

                                // DITO NA YUNG BAGONG LOGO MO! 
                                Image EditProfile = Properties.Resources.EditProfile;

                                // Idagdag ang editIcon sa dulo (pang-8 na item)
                                guna2DataGridView1.Rows.Add(formattedId, fullName, dobString, contact, lastVisitString, status, actionIcon, EditProfile);
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
            // Siguraduhing hindi header ang na-click
            if (e.RowIndex >= 0)
            {
                // ==========================================
                // ACTION 1: ADD TO QUEUE BUTTON (Column Index 6)
                // ==========================================
                if (e.ColumnIndex == 6)
                {
                    string status = guna2DataGridView1.Rows[e.RowIndex].Cells[5].Value?.ToString();

                    if (!string.Equals(status, "Waiting", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(status, "Waiting for Doctor", StringComparison.OrdinalIgnoreCase))
                    {
                        string patientId = guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                        string patientName = guna2DataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

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
                        using (Already_In_Queue noticeModal = new Already_In_Queue())
                        {
                            noticeModal.StartPosition = FormStartPosition.CenterScreen;
                            noticeModal.ShowDialog();
                        }
                    }
                }

                // ==========================================
                // ACTION 2: EDIT PROFILE BUTTON (Column Index 7)
                // ==========================================
                else if (e.ColumnIndex == 7)
                {
                    // Kunin ang Patient ID mula sa unang column
                    string patientId = guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();

                    // Tawagin yung Edit Profile form nang may slide-in animation natin kanina
                    using (Edit_Profile_Patient editForm = new Edit_Profile_Patient(patientId))
                    {
                        editForm.StartPosition = FormStartPosition.Manual;
                        Point absoluteLocation = this.PointToScreen(Point.Empty);
                        int targetX = absoluteLocation.X + this.Width - editForm.Width;
                        editForm.Location = new Point(targetX, absoluteLocation.Y);
                        editForm.Height = this.Height;

                        if (editForm.ShowDialog() == DialogResult.OK)
                        {
                            // Mag-refresh kapag may binago at na-save
                            LoadPatientData("");
                        }
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
        private void guna2Button5_Click(object sender, EventArgs e)
        {
            currentStatusFilter = ""; // Blank means kunin lahat
            LoadPatientData(""); // Refresh table
        }
        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            currentStatusFilter = "Waiting"; // Dapat eksaktong tugma sa spelling sa database mo!
            LoadPatientData("");
        }
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            currentStatusFilter = "Serving";
            LoadPatientData("");
        }
        private void guna2Button4_Click(object sender, EventArgs e)
        {
            currentStatusFilter = "Completed";
            LoadPatientData("");
        }

        private void txtSearchBox_TextChanged(object sender, EventArgs e)
        {
            // 1. Tama ito, keep this line. 
            // Kinukuha nito ang mismong textbox na nag-type ka.
            Guna.UI2.WinForms.Guna2TextBox searchBox = (Guna.UI2.WinForms.Guna2TextBox)sender;

            // 2. I-pass ang trimmed text sa LoadPatientData.
            // Siguraduhin na ang LoadPatientData mo ay gamit na ang CONCAT_WS sa SQL query.
            LoadPatientData(searchBox.Text.Trim());
        }

        private void guna2DataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string patientId = guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();

                using (Edit_Profile_Patient editForm = new Edit_Profile_Patient(patientId))
                {
                    // 1. Set to Manual para tayo ang magdikta kung saan siya lilitaw
                    editForm.StartPosition = FormStartPosition.Manual;

                    // 2. Kunin ang exact screen location ng white panel mo
                    Point absoluteLocation = this.PointToScreen(Point.Empty);

                    // 3. I-calculate ang X (Right edge minus width ng edit form) at Y
                    int targetX = absoluteLocation.X + this.Width - editForm.Width;

                    editForm.Location = new Point(targetX, absoluteLocation.Y);

                    // 4. Pantayin ang height niya sa main panel mo
                    editForm.Height = this.Height;

                    // I-show ang form
                    DialogResult result = editForm.ShowDialog();

                    // Refresh table kapag nag-save at nag-close
                    if (result == DialogResult.OK)
                    {
                        LoadPatientData("");
                    }
                }
            }
        }
    }
}