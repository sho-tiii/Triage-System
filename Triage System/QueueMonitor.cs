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
    public partial class QueueMonitor : Form
    {
        public QueueMonitor()
        {
            InitializeComponent();
        }

        private void clockTimer_Tick(object sender, EventArgs e)
        {
            // Update the Time label (e.g., 10:45:30 AM)
            // "hh" = 12-hour format, "mm" = minutes, "ss" = seconds, "tt" = AM/PM
            lblTime.Text = DateTime.Now.ToString("hh:mm:ss tt");

            // Update the Date label (e.g., Thursday, October 24, 2025)
            // "dddd" = Full day name, "MMMM" = Full month name, "dd" = Day number, "yyyy" = Year
            lblDate.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
        }
    }
}
