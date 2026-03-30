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
    public partial class UC_LabQueue : UserControl
    {
        public UC_LabQueue()
        {
            InitializeComponent();
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            using (DiagnosticForm regPanel = new DiagnosticForm())
            {
                regPanel.StartPosition = FormStartPosition.Manual;
                Point absoluteLocation = this.PointToScreen(Point.Empty);
                int targetX = absoluteLocation.X + this.Width - regPanel.Width;
                regPanel.Location = new Point(targetX, absoluteLocation.Y);
                regPanel.Height = this.Height;
                regPanel.ShowDialog();
            }
        }
    }
}
