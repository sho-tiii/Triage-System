using System;
using System.Windows.Forms;

namespace Triage_System // Make sure this matches your project's namespace!
{
    public partial class Queue_Number : Form
    {
        // Create a timer for the auto-close countdown
        Timer closeTimer = new Timer();

        // Constructor
        public Queue_Number(string incomingQueueNum, string incomingPatientId)
        {
            InitializeComponent();

            // 1. Force the receipt to open exactly in the middle of the screen
            this.StartPosition = FormStartPosition.CenterScreen;

            // 2. Set the labels based on the data sent from NewPatient.cs
            queueNumber.Text = incomingQueueNum;
            patientID.Text = "Your New Patient ID: " + incomingPatientId;
            dateTime.Text = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");

            // 3. Setup the 5-second Auto-Close Timer
            closeTimer.Interval = 5000; // 5000 milliseconds = 5 seconds
            closeTimer.Tick += CloseTimer_Tick; // Tell the timer what to do when it finishes
            closeTimer.Start(); // Start the countdown!
        }

        // 4. The method that actually closes the form when the timer hits 5 seconds
        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            closeTimer.Stop(); // Stop the timer so it doesn't keep ticking
            this.Close(); // Close the receipt
        }
    }
}