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
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Triage_System
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        public class Class1
        {
            // Dito natin ilalagay ang database connection para isang bago na lang sa lahat
            public static string connString = "server=localhost;database=triage_system;uid=root;pwd=;";

            public static string LoggedInUser = "";
            public static string LoggedInUserID = "";
            public static string LoggedInUserPassword = "";
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string usernameInput = staffID.Text.Trim();
            string passwordInput = password.Text;

            // Tinatawag na natin yung connection string galing sa Class1
            using (MySqlConnection conn = new MySqlConnection(Class1.connString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT StaffID, FullName, PasswordHash FROM staffaccounts WHERE Username=@user AND PasswordHash=@pass AND Role='Nurse' AND Status='Active'";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", usernameInput);
                        cmd.Parameters.AddWithValue("@pass", passwordInput);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Class1.LoggedInUserID = reader["StaffID"].ToString();
                                Class1.LoggedInUser = reader["FullName"].ToString();
                                Class1.LoggedInUserPassword = reader["PasswordHash"].ToString();

                                Form1 frm = new Form1();
                                frm.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Invalid Nurse credentials.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
