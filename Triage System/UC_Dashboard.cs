using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Guna.Charts.WinForms; // Ensure this is here for the chart
using MySql.Data.MySqlClient;

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
            // Load the visuals as soon as the dashboard opens
            SetupQueueVolumeChart();
            SetupEfficiencyDoughnutChart(); // Now calling the new Doughnut Chart method
        }

        private void SetupQueueVolumeChart()
        {
            // 1. Clear default data
            gunaChart1.Datasets.Clear();

            // 2. Create the Spline Area Dataset
            var volumeDataset = new GunaSplineAreaDataset();
            volumeDataset.Label = "Queue Volume";

            // 3. Apply Styling 
            volumeDataset.BorderColor = Color.FromArgb(24, 87, 135);
            volumeDataset.FillColor = Color.FromArgb(80, 150, 200, 250);

            // Hide the colored dots on the line (Removed the HoverRadius line that caused the error)
            volumeDataset.PointRadius = 0;

            // 4. Add Sample Data 
            volumeDataset.DataPoints.Add("8AM", 15);
            volumeDataset.DataPoints.Add("9AM", 35);
            volumeDataset.DataPoints.Add("10AM", 20);
            volumeDataset.DataPoints.Add("12PM", 95);
            volumeDataset.DataPoints.Add("1PM", 40);
            volumeDataset.DataPoints.Add("2PM", 65);
            volumeDataset.DataPoints.Add("3PM", 30);
            volumeDataset.DataPoints.Add("4PM", 60);

            // 5. Add dataset to chart and clean up the UI
            gunaChart1.Datasets.Add(volumeDataset);

            gunaChart1.Legend.Display = false;
            gunaChart1.YAxes.GridLines.Display = false;
            gunaChart1.XAxes.GridLines.Display = true;

            gunaChart1.Update();
        }

        private void SetupEfficiencyDoughnutChart()
        {
            // 1. Clear any default data
            gunaChart2.Datasets.Clear();

            // 2. Create the Doughnut Dataset
            var efficiencyDataset = new GunaDoughnutDataset();

            // 3. Add the Data (85% Progress, 15% Remaining space)
            efficiencyDataset.DataPoints.Add("Done", 85);
            efficiencyDataset.DataPoints.Add("Remaining", 15);

            // 4. Set the Colors (Green for progress, Light Gray for the track)
            efficiencyDataset.FillColors.Clear();
            efficiencyDataset.FillColors.Add(Color.FromArgb(46, 204, 113));  // Green
            efficiencyDataset.FillColors.Add(Color.FromArgb(235, 235, 235)); // Light Gray

            // 5. Add to the chart
            gunaChart2.Datasets.Add(efficiencyDataset);

            // 6. Clean up the Chart UI
            gunaChart2.Legend.Display = false;
            gunaChart2.Update();
        }

        // --- Your existing empty event handlers below ---

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnQueue_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }
    }
}