using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient; // Importante para sa Database

namespace Triage_System
{
    public partial class Edit_Profile_Patient : Form
    {
        private readonly string _patientId; // Taga-hawak ng Patient ID
        string connString = "server=localhost;user=root;database=triage_system;password=;";

        public Edit_Profile_Patient()
        {
            InitializeComponent();
        }

        // Constructor na tumatanggap ng Patient ID galing sa Search Form
        public Edit_Profile_Patient(string patientId) : this()
        {
            _patientId = patientId;
            LoadPatientDetails(); // I-load agad ang data pagkabukas!
        }

        // --- METHOD: KUNIN ANG DATA SA DB AT ILAGAY SA TEXTBOXES ---
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

                                // Para sa Date at ComboBox
                                if (reader["birthdate"] != DBNull.Value)
                                    dtpBirthdate.Value = Convert.ToDateTime(reader["birthdate"]);

                                cmbGender.Text = reader["gender"].ToString();

                                txtAddress.Text = reader["address"].ToString();
                                txtMobileNumber.Text = reader["mobile_number"].ToString();
                                txtContactPerson.Text = reader["contact_person"].ToString();
                                txtEmergencyNumber.Text = reader["contact_person_number"].ToString();
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

        // --- METHOD: I-SAVE ANG MGA BINAGO SA DATABASE ---
        private void btnSave_Record_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    // Gagamit tayo ng UPDATE imbes na INSERT
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
                                         contact_person_number = @cnumber 
                                     WHERE patient_id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Ipasa ang mga text galing sa UI papunta sa Database
                        cmd.Parameters.AddWithValue("@id", _patientId);
                        cmd.Parameters.AddWithValue("@fname", txtFirstName.Text);
                        cmd.Parameters.AddWithValue("@mname", txtMiddleName.Text);
                        cmd.Parameters.AddWithValue("@lname", txtLastName.Text);
                        cmd.Parameters.AddWithValue("@suffix", txtSuffix.Text);
                        cmd.Parameters.AddWithValue("@bdate", dtpBirthdate.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@gender", cmbGender.Text);
                        cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@mobile", txtMobileNumber.Text);
                        cmd.Parameters.AddWithValue("@cperson", txtContactPerson.Text);
                        cmd.Parameters.AddWithValue("@cnumber", txtEmergencyNumber.Text);

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Patient profile updated successfully!", "Update Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // I-set ang result para malaman ng Search form na kailangan nitong mag-refresh
                        this.DialogResult = DialogResult.OK;

                        // I-trigger ang sliding out animation bago tuluyang mag-close
                        slideOutTimer.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("May error sa pag-update: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // --- METHOD: CLOSE BUTTON (X) ---
        private void guna2Button1_Click(object sender, EventArgs e) // Assuming ito yung X o Cancel button mo
        {
            slideOutTimer.Start();
        }

        // --- METHOD: SLIDE OUT ANIMATION ---
        private void slideOutTimer_Tick(object sender, EventArgs e)
        {
            // 1. Move the form to the right by 25 pixels every split-second
            this.Left += 25;

            // 2. Make it slightly transparent at the same time
            this.Opacity -= 0.1;

            // 3. Once it is completely invisible, stop the timer and officially close the form
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