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

        private void guna2HtmlLabel9_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            // Wag muna i-close! I-start muna natin ang pag-slide.
            slideOutTimer.Start();
        }
        

        private void btnSave_Record_Click(object sender, EventArgs e)
        {
            // 1. Connection String para maka-connect sa XAMPP MySQL mo
            string connString = "server=localhost;user=root;database=triage_system;password=;";

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    // 2. Generate natin yung P-2026-XXXXXX na Patient ID!
                    string currentYear = DateTime.Now.Year.ToString();
                    Random rnd = new Random();
                    string newPatientID = $"P-{currentYear}-{rnd.Next(100000, 999999)}";

                    // 3. I-handa ang SQL Query (Gumagamit tayo ng @parameters para iwas hack/SQL Injection)
                    string query = @"INSERT INTO patient_registration 
                            (patient_id, first_name, middle_name, last_name, suffix, birthdate, gender, address, mobile_number, contact_person, contact_person_number, status) 
                            VALUES 
                            (@id, @fname, @mname, @lname, @suffix, @bdate, @gender, @address, @mobile, @cperson, @cnumber, 'Waiting')";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // 4. Ipasa ang mga text galing sa UI papunta sa Database
                        // PALITAN ANG MGA "txtFirstName.Text", etc. NG MGA TOTOONG NAMES NG TEXTBOX MO SA DESIGNER

                        cmd.Parameters.AddWithValue("@id", newPatientID);
                        cmd.Parameters.AddWithValue("@fname", txtFirstName.Text);
                        cmd.Parameters.AddWithValue("@mname", txtMiddleName.Text);
                        cmd.Parameters.AddWithValue("@lname", txtLastName.Text);
                        cmd.Parameters.AddWithValue("@suffix", txtSuffix.Text);

                        // Para sa DateTimePicker (Kunin lang natin ang date part)
                        cmd.Parameters.AddWithValue("@bdate", dtpBirthdate.Value.ToString("yyyy-MM-dd"));

                        // Para sa Gender (Kung gumamit ka ng ComboBox)
                        cmd.Parameters.AddWithValue("@gender", cmbGender.Text);

                        cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@mobile", txtMobileNumber.Text);
                        cmd.Parameters.AddWithValue("@cperson", txtContactPerson.Text);
                        cmd.Parameters.AddWithValue("@cnumber", txtEmergencyNumber.Text);

                        // 5. Patakbuhin ang Query!
                        cmd.ExecuteNonQuery();

                        // 6. Ipakita ang Success Message
                        MessageBox.Show($"Patient Registration Successful!\n\nAssigned ID: {newPatientID}",
                                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 7. (Optional) Linisin ang form pagkatapos mag-save
                        // txtFirstName.Clear();
                        // txtLastName.Clear();
                        // atbp...
                    }
                }
                catch (Exception ex)
                {
                    // Kung may error sa database connection o syntax, dito lalabas
                    MessageBox.Show("May error sa pag-save: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

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
    }
}
