using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Speech.Synthesis; //

namespace Triage_System
{
    public partial class QueueMonitor : Form
    {
        string connectionString = "server=localhost;user=root;password=;database=triage_system";
        Timer dbTimer = new Timer();
        SpeechSynthesizer reader = new SpeechSynthesizer();

        // Tracker para sa huling timestamp na narinig
        private DateTime lastAlertTime = DateTime.MinValue;

        public QueueMonitor()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void QueueMonitor_Load(object sender, EventArgs e)
        {
            clockTimer.Start();
            dbTimer.Interval = 2000;
            dbTimer.Tick += DbTimer_Tick;
            dbTimer.Start();
            FetchLatestQueueData();
        }

        private void DbTimer_Tick(object sender, EventArgs e) => FetchLatestQueueData();

        private void FetchLatestQueueData()
        {
            // Isinama ang created_at sa SELECT para ma-track ang Call Again
            string query = @"SELECT queue_number, status, created_at 
                             FROM patient_registration 
                             WHERE status IN ('Calling', 'Waiting') 
                             AND queue_number IS NOT NULL 
                             ORDER BY created_at ASC";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader readerObj = cmd.ExecuteReader())
                    {
                        List<string> waitingList = new List<string>();
                        string nowServing = "---";
                        DateTime currentPatientTime = DateTime.MinValue;
                        bool foundCalling = false;

                        while (readerObj.Read())
                        {
                            string qNum = readerObj["queue_number"].ToString();
                            string stat = readerObj["status"].ToString();
                            DateTime recordTime = Convert.ToDateTime(readerObj["created_at"]);

                            if (stat == "Calling" && !foundCalling)
                            {
                                nowServing = qNum;
                                currentPatientTime = recordTime;
                                foundCalling = true;
                            }
                            else
                            {
                                if (waitingList.Count < 8) waitingList.Add(qNum);
                            }
                        }

                        // --- TTS TRIGGER ---
                        // Magsasalita kung may boses at kung ang timestamp ay mas bago sa huling narinig
                        if (nowServing != "---" && currentPatientTime > lastAlertTime)
                        {
                            SpeakNumber(nowServing);
                            lastAlertTime = currentPatientTime;
                        }

                        // Update UI
                        lblNowServing.Text = nowServing;
                        lblNext1.Text = (waitingList.Count > 0) ? waitingList[0] : "---";
                        lblNext2.Text = (waitingList.Count > 1) ? waitingList[1] : "---";
                        lblNext3.Text = (waitingList.Count > 2) ? waitingList[2] : "---";
                        lblNext4.Text = (waitingList.Count > 3) ? waitingList[3] : "---";
                        lblNext5.Text = (waitingList.Count > 4) ? waitingList[4] : "---";
                        lblNext6.Text = (waitingList.Count > 5) ? waitingList[5] : "---";
                        lblNext7.Text = (waitingList.Count > 6) ? waitingList[6] : "---";
                        lblNext8.Text = (waitingList.Count > 7) ? waitingList[7] : "---";
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
        }

        private void SpeakNumber(string queueNumber)
        {
            try
            {
                string text = $"Now serving, patient number, {queueNumber}. Please proceed to Triage Station.";
                reader.Dispose();
                reader = new SpeechSynthesizer();
                reader.SpeakAsync(text);
            }
            catch { }
        }

        private void clockTimer_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("hh:mm:ss tt");
            lblDate.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
        }
    }
}