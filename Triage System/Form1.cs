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

            // LOAD QUEUE SCREEN (Create UC_PatientQueue.cs first!)
            //UC_Patient_Queue uc = new UC_Patient_Queue();
            // addUserControl(uc);
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
    }
}