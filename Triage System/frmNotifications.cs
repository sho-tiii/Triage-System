using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Triage_System
{
    public partial class frmNotifications : Form
    {
        // Palitan kung iba ang password o connection string mo
        string connectionString = "server=localhost;user=root;password=;database=triage_system";

        public frmNotifications()
        {
            InitializeComponent();
            LoadRealTimeNotifications();
        }

        private void LoadRealTimeNotifications()
        {
            flowLayoutPanel1.Controls.Clear();

            // Kukuha tayo ng totoong notification mula sa database (Mga pasyenteng 'Waiting' pa)
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // QUERY: Kukunin natin yung mga pasyente na kakatala lang o naghihintay ng Triage/Doctor.
                    // Note: Paki-adjust ang column names (ex. date_registered) depende sa totoong table structure mo.
                    string query = @"SELECT patient_id, status 
                                     FROM patient_registration 
                                     WHERE status LIKE '%Waiting%' OR status = 'Pending' 
                                     ORDER BY patient_id DESC LIMIT 10";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            bool hasData = false;

                            while (reader.Read())
                            {
                                hasData = true;
                                string pId = reader["patient_id"].ToString();
                                string pStatus = reader["status"].ToString();

                                UC_Notifcation notifItem = new UC_Notifcation();

                                // Gagawa tayo ng message base sa status niya
                                notifItem.MessageText = $"Patient ID: {pId} is currently {pStatus}.";

                                // Ilalagay natin ang current time (pwede mo rin kunin sa database yung oras ng registration niya kung meron)
                                notifItem.TimeText = DateTime.Now.ToString("hh:mm tt");

                                flowLayoutPanel1.Controls.Add(notifItem);
                            }

                            // Kung walang pasyenteng naghihintay, maglalagay tayo ng placeholder notification
                            if (!hasData)
                            {
                                UC_Notifcation emptyNotif = new UC_Notifcation();
                                emptyNotif.MessageText = "No new notifications at the moment.";
                                emptyNotif.TimeText = DateTime.Now.ToString("hh:mm tt");
                                flowLayoutPanel1.Controls.Add(emptyNotif);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading notifications: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void frmNotifications_Deactivate(object sender, EventArgs e)
        {
            // Kapag nawalan ng focus yung form (ex. nag-click sa main dashboard o ibang window), isasara niya sarili niya para hindi makalat
            this.Close();
        }
    }
}