using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// Make sure you have this if using Guna
using Guna.UI2.WinForms;

namespace Triage_System
{
    public partial class Form1 : Form
    {

        public static UserControl ActiveTriageSession = null;

        public Form1()
        {
            InitializeComponent();
        }

        // --- CORE NAVIGATION ENGINE (Adds the screens) ---
        private void addUserControl(UserControl userControl)
        {
            userControl.Dock = DockStyle.Fill;
            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(userControl);
            userControl.BringToFront();
        }

        // --- VISUAL EFFECTS (Your Indicator & Shadow Logic) ---
        private void MoveIndicator(Control btn)
        {
            // 1. Move the white strip
            img_slide.Top = btn.Top;
            img_slide.Height = btn.Height;

            // 2. Turn off all shadows
            ResetShadows();

            // 3. Turn on the shadow for THIS button
            if (btn is Guna2Button currentBtn)
            {
                currentBtn.ShadowDecoration.Enabled = true;
            }
        }

        private void ResetShadows()
        {
            // Reset shadows for all sidebar buttons
            dashboardBtn.ShadowDecoration.Enabled = false;
            patientQueueBtn.ShadowDecoration.Enabled = false;
            searchPatientBtn.ShadowDecoration.Enabled = false;
            doctorsBtn.ShadowDecoration.Enabled = false;
            triageHistoryBtn.ShadowDecoration.Enabled = false;
        }

        // --- BUTTON CLICKS (Navigation + Visuals) ---

        private void dashboardBtn_Click(object sender, EventArgs e)
        {
            MoveIndicator(sender as Control);

            // LOAD DASHBOARD SCREEN
            UC_Dashboard uc = new UC_Dashboard();
            addUserControl(uc);
        }

        private void searchpatientBtn_Click(object sender, EventArgs e)
        {
            MoveIndicator(sender as Control);

            // LOAD SEARCH SCREEN (Create UC_SearchPatient.cs first!)
            UC_Search_Patient uc = new UC_Search_Patient();
            addUserControl(uc);
        }

        private void patientqueueBtn_Click(object sender, EventArgs e)
        {
            MoveIndicator(sender as Control);

            // Clear the panel first
            mainPanel.Controls.Clear();

            // CHECK: Do we have an active triage session saved?
            if (ActiveTriageSession != null)
            {
                // YES: Load the SAVED screen (with all your typed data)
                ActiveTriageSession.Dock = DockStyle.Fill;
                mainPanel.Controls.Add(ActiveTriageSession);
                ActiveTriageSession.BringToFront();
            }
            else
            {
                // NO: Load the normal Queue list
                UC_Patient_Queue myQueue = new UC_Patient_Queue(); // <--- Create it
                myQueue.Dock = DockStyle.Fill;                     // <--- Use the variable "myQueue"
                mainPanel.Controls.Add(myQueue);                   // <--- Add the variable
            }
        }

        private void doctorBtn_Click(object sender, EventArgs e)
        {
            MoveIndicator(sender as Control);

            // LOAD DOCTORS SCREEN (Create UC_Doctors.cs first!)
            // UC_Doctors uc = new UC_Doctors();
            // addUserControl(uc);
        }

        private void triageBtn_Click(object sender, EventArgs e)
        {
            MoveIndicator(sender as Control);

            // LOAD HISTORY SCREEN (Create UC_History.cs first!)
            // UC_History uc = new UC_History();
            // addUserControl(uc);
        }

        // --- APP STARTUP ---
        private void Form1_Load(object sender, EventArgs e)
        {
            // --- FIX FOR NOTIFICATION BADGE OVERLAY ---
            // Set the parent of the red dot to the bell picturebox
            redNotif.Parent = imgBell;

            // Adjust the location so it stays in the right spot relative to the bell
            redNotif.Location = new Point(redNotif.Location.X - imgBell.Location.X, redNotif.Location.Y - imgBell.Location.Y);

            // Make sure it sits on top
            redNotif.BringToFront();
            // ------------------------------------------

            // 1. Visually select Dashboard
            MoveIndicator(dashboardBtn);
            dashboardBtn.Checked = true;

            // 2. Actually load the Dashboard screen
            UC_Dashboard uc = new UC_Dashboard();
            addUserControl(uc);
        }

        private void guna2VSeparator1_Click(object sender, EventArgs e)
        {
            // Unused event
        }

        private void mainPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            // Gagawa tayo ng temporary dark overlay form
            Form darkOverlay = new Form();

            try
            {
                // Setup ng Dark Overlay para mag-dim yung likod
                darkOverlay.StartPosition = FormStartPosition.Manual;
                darkOverlay.FormBorderStyle = FormBorderStyle.None;
                darkOverlay.Opacity = 0.50; // 50% transparency. Pwede mong i-adjust (ex. 0.70)
                darkOverlay.BackColor = System.Drawing.Color.Black;
                darkOverlay.Size = this.Size;       // Same size as main form
                darkOverlay.Location = this.Location; // Same position as main form
                darkOverlay.ShowInTaskbar = false;

                darkOverlay.Show(); // Ipakita yung dim background

                // I-open na natin yung Logout Modal mo
                using (LogoutModal modal = new LogoutModal())
                {
                    // Siguraduhing nasa ibabaw ng dim overlay yung modal
                    modal.Owner = darkOverlay;

                    // Ang ShowDialog ay i-pa-pause yung code dito hanggang sa mag-close yung modal
                    if (modal.ShowDialog() == DialogResult.Yes)
                    {
                        // Kung nag "Yes" (Confirm Logout) ang user:
                        this.Hide(); // Itago itong current dashboard/form

                        // I-open ang LoginForm
                        LoginForm login = new LoginForm();
                        login.Show();
                    }
                    // Kung nag "No" (Cancel), wala siyang gagawin. Magk-close lang yung modal.
                }
            }
            finally
            {
                // IMPORTANT: Siguraduhing ma-delete yung dark overlay kapag nag-close na yung modal
                darkOverlay.Dispose();
            }
        }

        private void lblTime_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Update the time label (e.g., "10:45 AM")
            lblTime.Text = DateTime.Now.ToString("hh:mm tt");

            // Update the day label (e.g., "Tuesday")
            lblDate.Text = DateTime.Now.ToString("dddd");

            // Update the full date label (e.g., "March 25, 2026")
            lblDateTwo.Text = DateTime.Now.ToString("MMMM dd, yyyy");
        }
    }
}