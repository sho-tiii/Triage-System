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

                                string contact = reader["mobile_number"].ToString();

                                DateTime lastVisit = Convert.ToDateTime(reader["created_at"]);
                                string lastVisitString = lastVisit.ToString("MMM dd, yyyy");

                                string status = reader["status"].ToString();

                                // Determine which IMAGE to show based on status
                                Image actionIcon;
                                if (status.Equals("Waiting", StringComparison.OrdinalIgnoreCase) ||
                                    status.Equals("Waiting for Doctor", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Patient is already in line - show the disabled/check icon
                                    actionIcon = Properties.Resources.Add_To_Queue_Not;
                                }
                                else
                                {
                                    // Patient is NOT in line - show the add button
                                    actionIcon = Properties.Resources.Add_To_Queue;
                                }

                                // Adds the row (Make sure your DataGridView has exactly 7 columns created in the designer!)
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
            Guna.UI2.WinForms.Guna2TextBox searchBox = (Guna.UI2.WinForms.Guna2TextBox)sender;
            LoadPatientData(searchBox.Text.Trim());
        }

        // --- HANDLE THE ACTION BUTTON CLICK & UPDATE DATABASE ---
        // --- HANDLE THE ACTION BUTTON CLICK & UPDATE DATABASE ---
        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 6)
                {
                    string status = guna2DataGridView1.Rows[e.RowIndex].Cells[5].Value?.ToString();

                    // Check the condition: only clickable if NOT waiting
                    if (!string.Equals(status, "Waiting", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(status, "Waiting for Doctor", StringComparison.OrdinalIgnoreCase))
                    {
                        string formattedId = guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                        string patientName = guna2DataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                        string rawIdString = formattedId.Replace("N-", "");

                        // TINAWAG NATIN YUNG CUSTOM FORM MO DITO!
                        using (Add_In_Queue confirmModal = new Add_In_Queue(patientName, formattedId))
                        {
                            // Starts the form right in the center of the screen
                            confirmModal.StartPosition = FormStartPosition.CenterScreen;

                            // ShowDialog() freezes the main form until this modal is closed
                            DialogResult result = confirmModal.ShowDialog();

                            if (result == DialogResult.Yes)
                            {
                                string updateQuery = "UPDATE patient_registration SET status = 'Waiting' WHERE patient_id = @id";

                                try
                                {
                                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                                    {
                                        conn.Open();
                                        using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                                        {
                                            cmd.Parameters.AddWithValue("@id", rawIdString);
                                            int rowsAffected = cmd.ExecuteNonQuery();

                                            if (rowsAffected > 0)
                                            {
                                                LoadPatientData("");
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Failed to update status: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    else
                    {
                        // TINAWAG NATIN YUNG CUSTOM "ALREADY IN QUEUE" FORM MO DITO!
                        using (Already_In_Queue noticeModal = new Already_In_Queue())
                        {
                            noticeModal.StartPosition = FormStartPosition.CenterScreen;
                            noticeModal.ShowDialog();
                        }
                    }
                }
            }
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            using (New_Patient_Registration regPanel = new New_Patient_Registration())
            {
                // 1. Double check it's set to Manual
                regPanel.StartPosition = FormStartPosition.Manual;

                // 2. Get the exact screen location of the top-left corner of THIS white area (the UserControl)
                Point absoluteLocation = this.PointToScreen(Point.Empty);

                // 3. The New Math:
                // X = Right edge of the white area MINUS the slide-out panel's width
                int targetX = absoluteLocation.X + this.Width - regPanel.Width;

                // Y = The exact top edge of the white area (right below your blue header)
                int targetY = absoluteLocation.Y;

                // Apply the exact location
                regPanel.Location = new Point(targetX, targetY);

                // 4. Match the height of the white area perfectly!
                regPanel.Height = this.Height;

                // 5. Show it!
                regPanel.ShowDialog();
            }
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {

        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {

        }
    }
}