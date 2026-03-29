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
    public partial class frmNotifications : Form
    {
        public frmNotifications()
        {
            InitializeComponent(); // WAG BUBURAHIN TO

            // DITO MO ILAGAY YUNG CODE PARA SURE NA TATAKBO AGAD!
            flowLayoutPanel1.Controls.Clear();

            for (int i = 1; i <= 3; i++)
            {
                UC_Notifcation notifItem = new UC_Notifcation(); // Make sure tama spelling nung UC mo
                notifItem.MessageText = $"Urgent: Patient N-10{i} needs immediate triage.";
                notifItem.TimeText = $"10:0{i} AM";

                flowLayoutPanel1.Controls.Add(notifItem);
            }
        }

        private void frmNotifications_Deactivate(object sender, EventArgs e)
        {
            // Kapag nawalan ng focus yung form (ex. nag-click sa main dashboard), isasara niya sarili niya
            this.Close();
        }
    }
}
