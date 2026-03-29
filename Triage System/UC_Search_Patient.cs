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
                                // Kung nasa pila na or serving, disable yung icon
                                if (status.Equals("Waiting", StringComparison.OrdinalIgnoreCase) ||
                                    status.Equals("Waiting for Doctor", StringComparison.OrdinalIgnoreCase) ||
                                    status.Equals("Serving", StringComparison.OrdinalIgnoreCase))
                                {
                                    actionIcon = Properties.Resources.Add_To_Queue_Not;
                                }
                                else
                                {
                                    actionIcon = Properties.Resources.Add_To_Queue;
                                }

                                Image EditProfile = Properties.Resources.EditProfile;

                                // Idagdag ang items sa row
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

        // --- HANDLE THE ACTION BUTTON CLICK & UPDATE DATABASE ---
        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // ==========================================
                // ACTION 1: ADD TO QUEUE BUTTON (Column Index 6)
                // ==========================================
                if (e.ColumnIndex == 6)
                {
                    string status = guna2DataGridView1.Rows[e.RowIndex].Cells[5].Value?.ToString();

                    if (!string.Equals(status, "Waiting", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(status, "Waiting for Doctor", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(status, "Serving", StringComparison.OrdinalIgnoreCase))
                    {
                        string patientId = guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                        string patientName = guna2DataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

                        // TAWAGIN ANG BAGONG MODAL: Add_To_Queue_Menu
                        using (Add_To_Queue_Menu queueMenu = new Add_To_Queue_Menu(patientName))
                        {
                            queueMenu.StartPosition = FormStartPosition.CenterScreen;

                            // Kapag kinlick ang Save/Add at bumalik ng Yes
                            if (queueMenu.ShowDialog() == DialogResult.Yes)
                            {
                                string selectedCategory = queueMenu.SelectedQueueCategory;
                                UpdatePatientStatus(patientId, "Waiting", selectedCategory);
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
                    string patientId = guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();

                    using (Edit_Profile_Patient editForm = new Edit_Profile_Patient(patientId))
                    {
                        editForm.StartPosition = FormStartPosition.Manual;
                        Point absoluteLocation = this.PointToScreen(Point.Empty);
                        int targetX = absoluteLocation.X + this.Width - editForm.Width;
                        editForm.Location = new Point(targetX, absoluteLocation.Y);
                        editForm.Height = this.Height;

                        if (editForm.ShowDialog() == DialogResult.OK)
                        {
                            LoadPatientData("");
                        }
                    }
                }
            }
        }

        // --- MAG-GENERATE NG QUEUE NUMBER AT I-SAVE ---
        private void UpdatePatientStatus(string id, string newStatus, string queueCategory)
        {
            string prefix = "N"; // Default para sa "Consultation / Follow-Up"
            string referralSource = "Internal Walk-in"; // <-- DEFAULT REFERRAL SOURCE

            // Workflow standard naming validation
            if (queueCategory == "Diagnostics (Lab/Rad)")
            {
                prefix = "T";
                referralSource = "External Recommendation"; // <-- AUTO-ASSUME EXTERNAL REFERRAL
            }
            else if (queueCategory == "Billing & Cashier")
            {
                prefix = "A";
            }

            Random rnd = new Random();
            string queueNum = $"{prefix}-{rnd.Next(1000, 9999)}";

            // UPDATED QUERY: Isinama ang referral_source
            string updateQuery = @"UPDATE patient_registration 
                                   SET status = @status, 
                                       queue_number = @qNum, 
                                       queue_type = @qType,
                                       referral_source = @refSource 
                                   WHERE patient_id = @id";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", newStatus);
                        cmd.Parameters.AddWithValue("@qNum", queueNum);
                        cmd.Parameters.AddWithValue("@qType", queueCategory);
                        cmd.Parameters.AddWithValue("@refSource", referralSource); // <-- ADDED PARAMETER
                        cmd.Parameters.AddWithValue("@id", id);

                        cmd.ExecuteNonQuery();

                        // ========================================================
                        // DITO NATIN TINAWAG YUNG CUSTOM QUEUE RECEIPT MO!
                        // ========================================================
                        using (Queue_Number receiptModal = new Queue_Number(queueNum, id))
                        {
                            // Dahil may Auto-Close timer ang Queue_Number mo, 
                            // mag-e-exit din ito nang kusa after 5 seconds!
                            receiptModal.ShowDialog();
                        }

                        LoadPatientData(""); // Refresh table after summing the modal
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update status: " + ex.Message, "Database Error");
            }
        }

        // --- QUICK ACTION: NEW PATIENT ---
        private void btnTransfer_Click(object sender, EventArgs e)
        {
            using (New_Patient_Registration regPanel = new New_Patient_Registration())
            {
                regPanel.StartPosition = FormStartPosition.Manual;
                Point absoluteLocation = this.PointToScreen(Point.Empty);
                int targetX = absoluteLocation.X + this.Width - regPanel.Width;
                regPanel.Location = new Point(targetX, absoluteLocation.Y);
                regPanel.Height = this.Height;
                regPanel.ShowDialog();
                LoadPatientData("");
            }
        }

        // --- SEARCH BOX IMPLEMENTATIONS ---
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2TextBox searchBox = (Guna.UI2.WinForms.Guna2TextBox)sender;
            LoadPatientData(searchBox.Text.Trim());
        }

        private void txtSearchBox_TextChanged(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2TextBox searchBox = (Guna.UI2.WinForms.Guna2TextBox)sender;
            LoadPatientData(searchBox.Text.Trim());
        }

        // --- FILTER BUTTONS ---
        private void guna2Button1_Click(object sender, EventArgs e) { /* Existing functionality */ }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            currentStatusFilter = "";
            LoadPatientData("");
        }
        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            currentStatusFilter = "Waiting";
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

        // --- DOUBLE CLICK TO EDIT ---
        private void guna2DataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string patientId = guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();

                using (Edit_Profile_Patient editForm = new Edit_Profile_Patient(patientId))
                {
                    editForm.StartPosition = FormStartPosition.Manual;
                    Point absoluteLocation = this.PointToScreen(Point.Empty);
                    int targetX = absoluteLocation.X + this.Width - editForm.Width;
                    editForm.Location = new Point(targetX, absoluteLocation.Y);
                    editForm.Height = this.Height;

                    DialogResult result = editForm.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        LoadPatientData("");
                    }
                }
            }
        }
    }
}