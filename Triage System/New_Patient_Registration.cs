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
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            slideOutTimer.Start();
        }

        // --- VALIDATION LOGIC ---
        private bool IsFormValid()
        {
            // I-check kung blangko ang mga REQUIRED fields
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text) ||
                string.IsNullOrWhiteSpace(txtMobileNumber.Text) ||
                cmbGender.SelectedIndex == -1) // Walang pinili sa Gender
            {
                MessageBox.Show("Please fill out all required fields (Name, Gender, Address, and Mobile Number).",
                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Optional: I-check kung 11 digits ang phone number
            if (txtMobileNumber.Text.Trim().Length < 11)
            {
                MessageBox.Show("Please enter a valid 11-digit mobile number.",
                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnSave_Record_Click(object sender, EventArgs e)
        {
            // 1. Patakbuhin muna ang Validation
            if (!IsFormValid()) return;

            // 2. Confirmation Prompt (Iwas accidental click)
            DialogResult confirm = MessageBox.Show("Are you sure you want to save this patient record?",
                                                   "Confirm Registration", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.No) return;

            // ===============================================================
            // 3. TAWAGIN ANG QUEUE MENU PARA TANUNGIN KUNG SAAN PUPUNTA
            // ===============================================================
            string patientFullName = txtFirstName.Text.Trim() + " " + txtLastName.Text.Trim();
            string queueCategory = "";
            string prefix = "N"; // Default

            using (Add_To_Queue_Menu queueMenu = new Add_To_Queue_Menu(patientFullName))
            {
                queueMenu.StartPosition = FormStartPosition.CenterScreen;
                if (queueMenu.ShowDialog() == DialogResult.Yes)
                {
                    queueCategory = queueMenu.SelectedQueueCategory;
                }
                else
                {
                    // Kung kinancel ng nurse yung pamimili ng department, i-abort ang pag-save
                    return;
                }
            }

            // 4. Tukuyin ang tamang Prefix base sa pinili
            if (queueCategory == "Diagnostics (Lab/Rad)") prefix = "T";
            else if (queueCategory == "Billing & Cashier") prefix = "A";

            // ===============================================================
            // 5. DATABASE SAVING LOGIC
            // ===============================================================
            string connString = "server=localhost;user=root;database=triage_system;password=;";

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    // Generate Patient ID at Queue Number
                    string currentYear = DateTime.Now.Year.ToString();
                    Random rnd = new Random();
                    string newPatientID = $"P-{currentYear}-{rnd.Next(100000, 999999)}";
                    string queueNum = $"{prefix}-{rnd.Next(1000, 9999)}";

                    // IN-UPDATE KO YUNG SQL QUERY PARA ISAMA YUNG queue_number AT queue_type
                    string query = @"INSERT INTO patient_registration 
                    (patient_id, first_name, middle_name, last_name, suffix, birthdate, gender, address, mobile_number, contact_person, contact_person_number, status, queue_number, queue_type) 
                    VALUES 
                    (@id, @fname, @mname, @lname, @suffix, @bdate, @gender, @address, @mobile, @cperson, @cnumber, 'Waiting', @qNum, @qType)";

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

                        // Mga bagong parameters para sa Queue
                        cmd.Parameters.AddWithValue("@qNum", queueNum);
                        cmd.Parameters.AddWithValue("@qType", queueCategory);

                        cmd.ExecuteNonQuery();

                        // ===============================================================
                        // 6. IPAKITA ANG MAGANDANG QUEUE RECEIPT IMBES NA MESSAGEBOX!
                        // ===============================================================
                        using (Queue_Number receiptModal = new Queue_Number(queueNum, newPatientID))
                        {
                            receiptModal.ShowDialog(); // Magsasara ito automatic after 5 seconds dahil sa timer mo!
                        }

                        // 7. I-set ang result at simulan ang slide out animation
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
        }
    }
}