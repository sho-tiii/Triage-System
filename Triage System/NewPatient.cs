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
    public partial class NewPatient : Form
    {
        public NewPatient()
        {
            InitializeComponent();
        }

        private void btnSave_Record_Click(object sender, EventArgs e)
        {
            // 1. Define your connection string (Check your XAMPP password, usually it's empty)
            string connectionString = "server=localhost;user=root;password=;database=triage_system";

            // 2. The SQL Query
            string query = "INSERT INTO patient_registration (first_name, middle_name, last_name, birthdate, sex, mobile_number) " +
                           "VALUES (@fname, @mname, @lname, @bdate, @sex, @mobile)";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // 3. Add Parameters (This prevents hacking/SQL Injection)
                        cmd.Parameters.AddWithValue("@fname", txtFirst_Name.Text.Trim());

                        // Handle Middle Name (If empty, save as NULL)
                        if (string.IsNullOrWhiteSpace(txtMiddle_Name.Text))
                        {
                            cmd.Parameters.AddWithValue("@mname", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@mname", txtMiddle_Name.Text.Trim());
                        }

                        cmd.Parameters.AddWithValue("@lname", txtLast_Name.Text.Trim());

                        // DateTimePicker gives a full date/time, we just need the Date part
                        cmd.Parameters.AddWithValue("@bdate", dtpBirthdate.Value.Date);

                        cmd.Parameters.AddWithValue("@sex", cbSex.Text); // Make sure ComboBox isn't empty!
                        cmd.Parameters.AddWithValue("@mobile", txtMobile_Number.Text.Trim());

                        // 4. Execute the command
                        cmd.ExecuteNonQuery();

                        // 5. Success Message
                        MessageBox.Show("New patient record saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Optional: Clear fields after save
                        ClearFields();
                    }
                }
            }
            catch (Exception ex)
            {
                // If XAMPP is off or code is wrong, this tells you why
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            txtFirst_Name.Clear();
            txtMiddle_Name.Clear();
            txtLast_Name.Clear();
            txtMobile_Number.Clear();
            cbSex.SelectedIndex = -1; // Reset dropdown
            dtpBirthdate.Value = DateTime.Now; // Reset date
        }
    }
}
