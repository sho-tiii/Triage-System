using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

// Guna UI & Charts
using Guna.Charts.WinForms;
using Guna.UI2.WinForms;

// iText 7 Core Libraries
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Geom;
using iText.Kernel.Font; // Para sa FontFactory
using iText.IO.Font.Constants; // Para sa StandardFonts

namespace Triage_System
{
    public partial class UC_Dashboard : UserControl
    {
        string connectionString = "server=localhost;user=root;password=;database=triage_system";

        public UC_Dashboard()
        {
            InitializeComponent();
        }

        private void UC_Dashboard_Load(object sender, EventArgs e)
        {
            LoadDashboardMetrics();
            LoadQueueVolumeChart();
            LoadEfficiencyDoughnutChart();
        }

        // --- 1. LIVE NUMBERS ---
        public void LoadDashboardMetrics()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT 
                                        SUM(CASE WHEN status = 'Waiting' THEN 1 ELSE 0 END) AS waiting_count,
                                        SUM(CASE WHEN status = 'Serving' THEN 1 ELSE 0 END) AS serving_count,
                                        SUM(CASE WHEN status = 'Completed' AND DATE(created_at) = CURDATE() THEN 1 ELSE 0 END) AS completed_today,
                                        SUM(CASE WHEN DATE(created_at) = CURDATE() THEN 1 ELSE 0 END) AS total_today
                                     FROM patient_registration";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblWaiting.Text = reader["waiting_count"] != DBNull.Value ? reader["waiting_count"].ToString() : "0";
                            lblServing.Text = reader["serving_count"] != DBNull.Value ? reader["serving_count"].ToString() : "0";
                            lblCompletedToday.Text = reader["completed_today"] != DBNull.Value ? reader["completed_today"].ToString() : "0";
                            lblPatientsToday.Text = reader["total_today"] != DBNull.Value ? reader["total_today"].ToString() : "0";
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error metrics: " + ex.Message); }
            }
        }

        // --- 2. LIVE QUEUE VOLUME CHART ---
        private void LoadQueueVolumeChart()
        {
            gunaChart1.Datasets.Clear();
            var volumeDataset = new GunaSplineAreaDataset();
            volumeDataset.Label = "Queue Volume";
            volumeDataset.BorderColor = Color.FromArgb(24, 87, 135);
            volumeDataset.FillColor = Color.FromArgb(80, 150, 200, 250);
            volumeDataset.PointRadius = 4;

            Dictionary<string, int> hourlyData = new Dictionary<string, int>()
            {
                {"8 AM", 0}, {"9 AM", 0}, {"10 AM", 0}, {"11 AM", 0}, {"12 PM", 0},
                {"1 PM", 0}, {"2 PM", 0}, {"3 PM", 0}, {"4 PM", 0}, {"5 PM", 0}
            };

            string query = @"SELECT HOUR(created_at) AS hr, COUNT(*) AS patient_count 
                             FROM patient_registration 
                             WHERE DATE(created_at) = CURDATE() 
                             GROUP BY HOUR(created_at) 
                             ORDER BY hr ASC";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int hour24 = Convert.ToInt32(reader["hr"]);
                            int count = Convert.ToInt32(reader["patient_count"]);
                            string amPm = hour24 >= 12 ? "PM" : "AM";
                            int displayHour = hour24 > 12 ? hour24 - 12 : (hour24 == 0 ? 12 : hour24);
                            string timeLabel = $"{displayHour} {amPm}";
                            if (hourlyData.ContainsKey(timeLabel)) hourlyData[timeLabel] = count;
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Chart error: " + ex.Message); }
            }

            foreach (var item in hourlyData) { volumeDataset.DataPoints.Add(item.Key, item.Value); }
            gunaChart1.Datasets.Add(volumeDataset);
            gunaChart1.Legend.Display = false;
            gunaChart1.Update();
        }

        // --- 3. LIVE EFFICIENCY CHART ---
        private void LoadEfficiencyDoughnutChart()
        {
            gunaChart2.Datasets.Clear();
            var efficiencyDataset = new GunaDoughnutDataset();
            string query = @"SELECT 
                                SUM(CASE WHEN status = 'Completed' THEN 1 ELSE 0 END) AS done_count,
                                SUM(CASE WHEN status IN ('Waiting', 'Serving') THEN 1 ELSE 0 END) AS remaining_count
                             FROM patient_registration 
                             WHERE DATE(created_at) = CURDATE()";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int done = reader["done_count"] != DBNull.Value ? Convert.ToInt32(reader["done_count"]) : 0;
                            int remaining = reader["remaining_count"] != DBNull.Value ? Convert.ToInt32(reader["remaining_count"]) : 0;
                            if (done == 0 && remaining == 0)
                            {
                                efficiencyDataset.DataPoints.Add("No Patients", 1);
                                efficiencyDataset.FillColors.Add(Color.FromArgb(235, 235, 235));
                            }
                            else
                            {
                                efficiencyDataset.DataPoints.Add("Done", done);
                                efficiencyDataset.DataPoints.Add("Remaining", remaining);
                                efficiencyDataset.FillColors.Add(Color.FromArgb(46, 204, 113));
                                efficiencyDataset.FillColors.Add(Color.FromArgb(235, 235, 235));
                            }
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Doughnut error: " + ex.Message); }
            }
            gunaChart2.Datasets.Add(efficiencyDataset);
            gunaChart2.Legend.Position = Guna.Charts.WinForms.LegendPosition.Bottom;
            gunaChart2.Update();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            LoadDashboardMetrics();
            LoadQueueVolumeChart();
            LoadEfficiencyDoughnutChart();
        }

        // --- QUICK ACTIONS ---

        // ADD NEW PATIENT
        private void btnTransfer_Click(object sender, EventArgs e)
        {
            using (New_Patient_Registration regPanel = new New_Patient_Registration())
            {
                regPanel.StartPosition = FormStartPosition.Manual;
                System.Drawing.Point absLoc = this.PointToScreen(System.Drawing.Point.Empty);
                regPanel.Location = new System.Drawing.Point(absLoc.X + this.Width - regPanel.Width, absLoc.Y);
                regPanel.Height = this.Height;
                if (regPanel.ShowDialog() == DialogResult.OK) timer1_Tick(null, null);
            }
        }

        // SEARCH PATIENT
        private void guna2GradientButton1_Click(object sender, EventArgs e)
        {
            var mainForm = this.FindForm() as Form1; // Palitan ang Form1 kung iba ang pangalan ng Main Form mo
            if (mainForm != null) mainForm.SwitchToSearchPatient();
        }

        // GENERATE DAILY REPORT (PDF)
        private void guna2GradientButton3_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF Files (*.pdf)|*.pdf";
            sfd.FileName = "Daily_Triage_Report_" + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Siguraduhin na sarado ang file bago simulan
                    FileInfo file = new FileInfo(sfd.FileName);

                    using (PdfWriter writer = new PdfWriter(file.FullName))
                    {
                        using (PdfDocument pdf = new PdfDocument(writer))
                        {
                            Document document = new Document(pdf);

                            // Simpleng text lang muna
                            document.Add(new Paragraph("NORTH METRO GENERAL HOSPITAL DAILY REPORT"));
                            document.Add(new Paragraph("Date: " + DateTime.Now.ToShortDateString()));
                            document.Add(new Paragraph("--------------------------------------------------"));

                            Table table = new Table(4); // 4 columns muna para simple
                            table.AddHeaderCell("ID");
                            table.AddHeaderCell("Name");
                            table.AddHeaderCell("Time");
                            table.AddHeaderCell("Status");

                            using (MySqlConnection conn = new MySqlConnection(connectionString))
                            {
                                conn.Open();
                                string query = "SELECT patient_id, first_name, last_name, created_at, status FROM patient_registration WHERE DATE(created_at) = CURDATE()";

                                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                                using (MySqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        table.AddCell(reader["patient_id"].ToString());
                                        table.AddCell(reader["first_name"].ToString() + " " + reader["last_name"].ToString());
                                        table.AddCell(Convert.ToDateTime(reader["created_at"]).ToString("hh:mm tt"));
                                        table.AddCell(reader["status"].ToString());
                                    }
                                }
                            }

                            document.Add(table);
                            document.Close();
                        }
                    }

                    MessageBox.Show("Report Saved!", "Success");
                }
                catch (Exception ex)
                {
                    // Dito natin makikita ang totoong error details
                    MessageBox.Show("Details: " + ex.ToString(), "Error Found");
                }
            }
        }

        // Designer-generated stubs (Wag burahin kung naka-bind sa UI)
        private void guna2TextBox1_TextChanged(object sender, EventArgs e) { }
        private void btnQueue_Click(object sender, EventArgs e) { }
        private void guna2Panel2_Paint(object sender, PaintEventArgs e) { }
        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e) { }
    }
}