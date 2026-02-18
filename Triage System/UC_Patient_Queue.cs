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
    public partial class UC_Patient_Queue : UserControl
    {
        public UC_Patient_Queue()
        {
            InitializeComponent();
            ResetButtons();
        }

        private void UC_Patient_Queue_Load(object sender, EventArgs e)
        {

        }

        private void ResetButtons()
        {
            // --- RESET "CALL PATIENT" BUTTON ---

            // 1. Reset Text
            btnCallPatient.Text = "CALL PATIENT";

            // 2. Reset Colors (Green - #2ecc71 or similar)
            Color greenColor = Color.FromArgb(46, 204, 113);
            btnCallPatient.FillColor = greenColor;
            btnCallPatient.FillColor2 = greenColor; // <--- Crucial for Gradient Buttons!

            // 3. Reset Image (Make sure "Mic_Green" matches your resource name!)
            // If your green mic is named differently, change "Mic_Green" below.
            btnCallPatient.Image = Properties.Resources.CallPatient;

            // --- RESET "START TRIAGE" BUTTON ---

            // 4. Lock it again
            btnStartTriage.Enabled = false;

            // 5. Turn it Gray
            btnStartTriage.FillColor = Color.DarkGray;
            btnStartTriage.FillColor2 = Color.DarkGray;
        }

        private void btnCallPatient_Click(object sender, EventArgs e)
        {
            // Define your Orange Color (#ff914d)
            Color orangeColor = Color.FromArgb(255, 145, 77); // R=255, G=145, B=77

            // Check if we are already in "Call Again" mode
            if (btnCallPatient.Text == "CALL PATIENT")
            {
                // --- 1. CHANGE TO ORANGE STATE ---
                btnCallPatient.Text = "CALL AGAIN";

                // Fix for Gradient Button (Set BOTH to the same color)
                btnCallPatient.FillColor = orangeColor;
                btnCallPatient.FillColor2 = orangeColor;

                // Fix for Space in Filename (Use Underscore)
                btnCallPatient.Image = Properties.Resources.CallAgain;

                // Unlock the Start Triage button
                btnStartTriage.Enabled = true;
                // Set Start Triage to Green (Assuming standard green #2ecc71)
                btnStartTriage.FillColor = Color.FromArgb(46, 204, 113);
                btnStartTriage.FillColor2 = Color.FromArgb(46, 204, 113);
            }
            else
            {
                // --- 2. LOGIC FOR "CALL AGAIN" ---
                // Play sound or logic here
                MessageBox.Show("Calling patient again...");
            }
        }

        private void btnStartTriage_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Start triage?", "Confirm", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                ResetButtons();

                // 1. Create the screen
                UC_Start_Triage nextScreen = new UC_Start_Triage();

                // 2. SAVE IT to the Main Form's "Backpack"
                Form1.ActiveTriageSession = nextScreen;

                // 3. Show it manually using the Parent control
                Control mainPanel = this.Parent;
                mainPanel.Controls.Clear();
                nextScreen.Dock = DockStyle.Fill;
                mainPanel.Controls.Add(nextScreen);
                nextScreen.BringToFront();
            }
        }
    }
}
