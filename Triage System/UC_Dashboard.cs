using System;
using System.Drawing; // Needed for Color
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Triage_System
{
    public partial class UC_Dashboard : UserControl
    {
        public UC_Dashboard()
        {
            InitializeComponent();
            LoadDummyData();
        }

        private void LoadDummyData()
        {
            // 1. Prevent auto-generation
            guna2DataGridView1.AutoGenerateColumns = false;

            // 2. Define Columns (if not in designer)
            if (guna2DataGridView1.Columns.Count == 0)
            {
                guna2DataGridView1.Columns.Add("QueueNo", "Queue No.");
                guna2DataGridView1.Columns.Add("Name", "Patient Name");
                guna2DataGridView1.Columns.Add("Time", "Time In");
                guna2DataGridView1.Columns.Add("Service", "Service");
                guna2DataGridView1.Columns.Add("Priority", "Priority");
                guna2DataGridView1.Columns.Add("Status", "Status");
            }

            // 3. Clear old rows (optional safety)
            guna2DataGridView1.Rows.Clear();

            // 4. Add Rows
            guna2DataGridView1.Rows.Add("C-030", "Juan dela Cruz", "10:22 AM", "Consultation", "Senior Citizen", "Serving Now");
            guna2DataGridView1.Rows.Add("D-015", "Maria Santos", "10:30 AM", "Diagnostics", "PWD", "Waiting (13 mins)");
            guna2DataGridView1.Rows.Add("C-012", "Pedro Penduko", "10:44 AM", "Consultation", "Regular", "Completed");
            guna2DataGridView1.Rows.Add("C-036", "Ana Reyes", "10:48 AM", "Consultation", "Regular", "Serving Now");
            guna2DataGridView1.Rows.Add("C-023", "Jose Mario", "11:01 AM", "Consultation", "Regular", "Completed");
        }

        private void UC_Dashboard_Load(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}