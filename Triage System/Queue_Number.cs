using System;
using System.Windows.Forms;

namespace Triage_System
{
    public partial class Queue_Number : Form
    {
        // Create a timer for the auto-close countdown
        Timer closeTimer = new Timer();

        // Constructor na tumatanggap ng Queue Number at Patient ID
        public Queue_Number(string incomingQueueNum, string incomingPatientId)
        {
            InitializeComponent();

            // 1. Force the receipt to open exactly in the middle of the screen
            this.StartPosition = FormStartPosition.CenterScreen;

            // 2. I-set ang Date and Time format para kamukha ng nasa design mo
            // Example output: "DATE AND TIME: 03/29/2026 | 09:12 AM"
            dateTime.Text = "DATE AND TIME: " + DateTime.Now.ToString("MM/dd/yyyy | hh:mm tt");

            // 3. I-set ang malaking Queue Number (Ito yung magiging N-1234, T-1234, o A-1234)
            queueNumber.Text = incomingQueueNum;

            // 4. I-set ang Patient ID sa ilalim
            patientID.Text = "Patient ID: " + incomingPatientId;

            // 5. Setup the 5-second Auto-Close Timer
            closeTimer.Interval = 5000; // 5000 milliseconds = 5 seconds
            closeTimer.Tick += CloseTimer_Tick; // Tell the timer what to do when it finishes
            closeTimer.Start(); // Start the countdown!
        }

        // 6. Ang method na magsasara ng form kapag tapos na ang 5 seconds
        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            closeTimer.Stop(); // Stop the timer so it doesn't keep ticking in the background
            this.Close(); // Close the receipt modal
        }

        // Optional: Kung may "X" button ka sa design, pwede mo itong i-link doon para pwedeng i-close manually
        private void btnClose_Click(object sender, EventArgs e)
        {
            closeTimer.Stop();
            this.Close();
        }
    }
}