using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        public void addUserControl(UserControl userControl)
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

            // LOAD SEARCH SCREEN 
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
                UC_Patient_Queue myQueue = new UC_Patient_Queue();
                myQueue.Dock = DockStyle.Fill;
                mainPanel.Controls.Add(myQueue);
            }
        }

        private void triageBtn_Click(object sender, EventArgs e)
        {
            MoveIndicator(sender as Control);

            // LOAD HISTORY SCREEN 
            UC_Triage_History uc = new UC_Triage_History();
            addUserControl(uc);
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
                darkOverlay.Opacity = 0.50;
                darkOverlay.BackColor = System.Drawing.Color.Black;
                darkOverlay.Size = this.Size;
                darkOverlay.Location = this.Location;
                darkOverlay.ShowInTaskbar = false;

                darkOverlay.Show();

                using (LogoutModal modal = new LogoutModal())
                {
                    modal.Owner = darkOverlay;

                    if (modal.ShowDialog() == DialogResult.Yes)
                    {
                        this.Hide();
                        LoginForm login = new LoginForm();
                        login.Show();
                    }
                }
            }
            finally
            {
                darkOverlay.Dispose();
            }
        }

        private void lblTime_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Update the time label 
            lblTime.Text = DateTime.Now.ToString("hh:mm tt");

            // Update the day label 
            lblDate.Text = DateTime.Now.ToString("dddd");

            // Update the full date label 
            lblDateTwo.Text = DateTime.Now.ToString("MMMM dd, yyyy");
        }

        private void imgBell_Click(object sender, EventArgs e)
        {
            // Itago yung red notification dot pag na-click
            redNotif.Visible = false;

            frmNotifications notifForm = new frmNotifications();
            notifForm.StartPosition = FormStartPosition.Manual;

            // Kunin ang exact screen location ng bell icon
            Point bellLocation = this.PointToScreen(imgBell.Location);

            // PERFECT RIGHT-ALIGN FORMULA: 
            // (X position ng bell + lapad ng bell) MINUS (lapad ng notification form)
            int spawnX = (bellLocation.X + imgBell.Width) - notifForm.Width;

            // Kung gusto mo pa siyang i-usog pakaliwa para may konting "margin" sa gilid ng screen:
            // Pwede mong gawing: spawnX = spawnX - 10; 

            int spawnY = bellLocation.Y + imgBell.Height + 5;

            notifForm.Location = new Point(spawnX, spawnY);
            notifForm.Show();
        }

        private void redNotif_Click(object sender, EventArgs e)
        {

        }

        public void SwitchToSearchPatient()
        {
            // 1. Tawagin mo yung button click event ng Search Patient button mo
            // Halimbawa, kung 'btnSearchPatient' ang pangalan ng button mo sa Sidebar:
            searchPatientBtn.PerformClick();
        }
    }
}