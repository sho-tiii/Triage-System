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
    public partial class New_Patient_Registration : Form
    {
        public New_Patient_Registration()
        {
            InitializeComponent();

            // I-set ang default na Queue Category para hindi mag-error kung makalimutan
            if (cmbQueueCategory.Items.Count > 0)
            {
                cmbQueueCategory.SelectedIndex = 0; // "Consultation / Follow-Up"
            }

            // --- ADDED: Itago muna ang Pregnant checkbox by default ---
            chkPregnant.Visible = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            slideOutTimer.Start();
        }

        // --- ADDED: Event handler para lumabas lang ang Pregnant checkbox kung Female ang pinili ---
        private void cmbGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGender.Text == "Female")
            {
                chkPregnant.Visible = true;
            }
            else
            {
                chkPregnant.Visible = false;
                chkPregnant.Checked = false; // I-reset kung binago ang gender
            }
        }

        // --- VALIDATION LOGIC ---
        private bool IsFormValid()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text) ||
                string.IsNullOrWhiteSpace(txtMobileNumber.Text) ||
                cmbGender.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill out all required fields (Name, Gender, Address, and Mobile Number).",
                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (txtMobileNumber.Text.Trim().Length < 11)
            {
                MessageBox.Show("Please enter a valid 11-digit mobile number.",
                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbQueueCategory.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Queue Category.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnSave_Record_Click(object sender, EventArgs e)
        {
            if (!IsFormValid()) return;

            DialogResult confirm = MessageBox.Show("Are you sure you want to save this patient record?",
                                                   "Confirm Registration", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.No) return;

            // ===============================================================
            // 3. QUEUE AND REFERRAL LOGIC
            // ===============================================================
            string queueCategory = cmbQueueCategory.Text;
            string prefix = "N";
            string referralSource = "Internal Walk-in";

            if (queueCategory == "Diagnostics (Lab/Rad)")
            {
                prefix = "T";
                referralSource = "External Recommendation";
            }
            else if (queueCategory == "Billing & Cashier")
            {
                prefix = "A";
            }

            // ===============================================================
            // 5. CALCULATE PRIORITY AND PREGNANCY
            // ===============================================================
            int age = DateTime.Today.Year - dtpBirthdate.Value.Year;
            if (dtpBirthdate.Value.Date > DateTime.Today.AddYears(-age)) age--;

            string priorityStatus = "Regular";

            if (cmbPWD.Checked)
            {
                priorityStatus = "PWD";
            }
            else if (age >= 60)
            {
                priorityStatus = "Senior Citizen";
            }

            // --- ADDED: Capture Pregnancy Status ---
            int isPregnant = chkPregnant.Checked ? 1 : 0;

            // ===============================================================
            // 6. DATABASE SAVING LOGIC
            // ===============================================================
            string connString = "server=localhost;user=root;database=triage_system;password=;";

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    string currentYear = DateTime.Now.Year.ToString();
                    Random rnd = new Random();
                    string newPatientID = $"P-{currentYear}-{rnd.Next(100000, 999999)}";
                    string queueNum = $"{prefix}-{rnd.Next(1000, 9999)}";

                    // UPDATED SQL QUERY: Added priority and is_pregnant columns
                    string query = @"INSERT INTO patient_registration 
                    (patient_id, first_name, middle_name, last_name, suffix, birthdate, gender, address, mobile_number, contact_person, contact_person_number, status, queue_number, queue_type, referral_source, priority, is_pregnant) 
                    VALUES 
                    (@id, @fname, @mname, @lname, @suffix, @bdate, @gender, @address, @mobile, @cperson, @cnumber, 'Waiting', @qNum, @qType, @refSource, @priority, @isPregnant)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", newPatientID);
                        cmd.Parameters.AddWithValue("@fname", txtFirstName.Text.Trim());
                        cmd.Parameters.AddWithValue("@mname", txtMiddleName.Text.Trim());
                        cmd.Parameters.AddWithValue("@lname", txtLastName.Text.Trim());
                        cmd.Parameters.AddWithValue("@suffix", txtSuffix.Text.Trim());
                        cmd.Parameters.AddWithValue("@bdate", dtpBirthdate.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@gender", cmbGender.Text);
                        cmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                        cmd.Parameters.AddWithValue("@mobile", txtMobileNumber.Text.Trim());
                        cmd.Parameters.AddWithValue("@cperson", txtContactPerson.Text.Trim());
                        cmd.Parameters.AddWithValue("@cnumber", txtEmergencyNumber.Text.Trim());

                        cmd.Parameters.AddWithValue("@qNum", queueNum);
                        cmd.Parameters.AddWithValue("@qType", queueCategory);
                        cmd.Parameters.AddWithValue("@refSource", referralSource);
                        cmd.Parameters.AddWithValue("@priority", priorityStatus);

                        // --- ADDED: Pregnancy Parameter ---
                        cmd.Parameters.AddWithValue("@isPregnant", isPregnant);

                        cmd.ExecuteNonQuery();

                        using (Queue_Number receiptModal = new Queue_Number(queueNum, newPatientID))
                        {
                            receiptModal.ShowDialog();
                        }

                        this.DialogResult = DialogResult.OK;
                        slideOutTimer.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("May error sa pag-save: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void slideOutTimer_Tick(object sender, EventArgs e)
        {
            this.Left += 25;
            this.Opacity -= 0.1;

            if (this.Opacity <= 0)
            {
                slideOutTimer.Stop();
                this.Close();
            }
        }

        private void ClearForm()
        {
            txtFirstName.Clear();
            txtMiddleName.Clear();
            txtLastName.Clear();
            txtSuffix.Clear();
            txtAddress.Clear();
            txtMobileNumber.Clear();
            txtContactPerson.Clear();
            txtEmergencyNumber.Clear();
            dtpBirthdate.Value = DateTime.Now;
            cmbGender.SelectedIndex = -1;
            cmbQueueCategory.SelectedIndex = 0;
            cmbPWD.Checked = false;
            chkPregnant.Checked = false; // Reset
            chkPregnant.Visible = false; // Hide again
        }

        private void cmbGender_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // Chini-check kung "Female" ang eksaktong text sa ComboBox
            if (cmbGender.Text.Trim() == "Female")
            {
                chkPregnant.Visible = true; // Lalabas ang checkbox
            }
            else
            {
                chkPregnant.Visible = false; // Itatago ang checkbox
                chkPregnant.Checked = false; // Sinisiguradong hindi naka-check para hindi magkamali sa database
            }
        }
    }
}