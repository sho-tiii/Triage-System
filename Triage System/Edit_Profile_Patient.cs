using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Triage_System
{
    public partial class Edit_Profile_Patient : Form
    {
        private readonly string _patientId;
        string connString = "server=localhost;user=root;database=triage_system;password=;";

        public Edit_Profile_Patient()
        {
            InitializeComponent();
        }

        public Edit_Profile_Patient(string patientId) : this()
        {
            _patientId = patientId;

            // --- ADDED: Manual Event Wiring for Gender logic ---
            this.cmbGender.SelectedIndexChanged += new System.EventHandler(this.cmbGender_SelectedIndexChanged);

            LoadPatientDetails();
        }

        // --- ADDED: Logic para sa Visibility ng Pregnant Checkbox ---
        private void cmbGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGender.Text.Trim() == "Female")
            {
                chkPregnant.Visible = true;
            }
            else
            {
                chkPregnant.Visible = false;
                chkPregnant.Checked = false;
            }
        }

        private void LoadPatientDetails()
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM patient_registration WHERE patient_id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", _patientId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtFirstName.Text = reader["first_name"].ToString();
                                txtMiddleName.Text = reader["middle_name"].ToString();
                                txtLastName.Text = reader["last_name"].ToString();
                                txtSuffix.Text = reader["suffix"].ToString();

                                if (reader["birthdate"] != DBNull.Value)
                                    dtpBirthdate.Value = Convert.ToDateTime(reader["birthdate"]);

                                cmbGender.Text = reader["gender"].ToString();
                                txtAddress.Text = reader["address"].ToString();
                                txtMobileNumber.Text = reader["mobile_number"].ToString();
                                txtContactPerson.Text = reader["contact_person"].ToString();
                                txtEmergencyNumber.Text = reader["contact_person_number"].ToString();

                                // --- LOAD PRIORITY & PWD STATUS ---
                                if (reader["priority"].ToString() == "PWD")
                                {
                                    cmbPWD.Checked = true;
                                }
                                else
                                {
                                    cmbPWD.Checked = false;
                                }

                                // --- ADDED: Load Pregnant Status ---
                                if (reader["is_pregnant"] != DBNull.Value && Convert.ToInt32(reader["is_pregnant"]) == 1)
                                {
                                    chkPregnant.Checked = true;
                                    chkPregnant.Visible = true; // Show immediately if already pregnant
                                }
                                else
                                {
                                    chkPregnant.Checked = false;
                                    // Visibility will be handled by the cmbGender logic above
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading patient details: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSave_Record_Click(object sender, EventArgs e)
        {
            // --- CALCULATE PRIORITY & PREGNANCY ---
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

            int isPregnant = chkPregnant.Checked ? 1 : 0;

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    // --- UPDATED: Isinama ang 'priority' at 'is_pregnant' sa UPDATE ---
                    string query = @"UPDATE patient_registration 
                                     SET first_name = @fname, 
                                         middle_name = @mname, 
                                         last_name = @lname, 
                                         suffix = @suffix, 
                                         birthdate = @bdate, 
                                         gender = @gender, 
                                         address = @address, 
                                         mobile_number = @mobile, 
                                         contact_person = @cperson, 
                                         contact_person_number = @cnumber,
                                         priority = @priority,
                                         is_pregnant = @isPregnant
                                     WHERE patient_id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", _patientId);
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

                        cmd.Parameters.AddWithValue("@priority", priorityStatus);
                        cmd.Parameters.AddWithValue("@isPregnant", isPregnant);

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Patient profile updated successfully!", "Update Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.OK;
                        slideOutTimer.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("May error sa pag-update: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            slideOutTimer.Start();
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

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            slideOutTimer.Start();
        }
    }
}