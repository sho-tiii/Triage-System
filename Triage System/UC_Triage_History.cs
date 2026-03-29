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
    public partial class UC_Triage_History : UserControl
    {
        public UC_Triage_History()
        {
            InitializeComponent();
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            using (Generate_Triage_Report regPanel = new Generate_Triage_Report())
            {
                // 1. Double check it's set to Manual
                regPanel.StartPosition = FormStartPosition.Manual;

                // 2. Get the exact screen location of the top-left corner of THIS white area (the UserControl)
                Point absoluteLocation = this.PointToScreen(Point.Empty);

                // 3. The New Math:
                // X = Right edge of the white area MINUS the slide-out panel's width
                int targetX = absoluteLocation.X + this.Width - regPanel.Width;

                // Y = The exact top edge of the white area (right below your blue header)
                int targetY = absoluteLocation.Y;

                // Apply the exact location
                regPanel.Location = new Point(targetX, targetY);

                // 4. Match the height of the white area perfectly!
                regPanel.Height = this.Height;

                // 5. Show it!
                regPanel.ShowDialog();
            }
        }
    }
}
