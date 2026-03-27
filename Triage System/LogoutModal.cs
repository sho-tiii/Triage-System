using System;
using System.Windows.Forms;

namespace Triage_System
{
    public partial class LogoutModal : Form
    {
        public LogoutModal()
        {
            InitializeComponent();
        }

        // CONFIRM LOGOUT BUTTON
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            // Sineset natin to "Yes" para malaman ng Main Form na nag-confirm siya
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        // CANCEL BUTTON
        private void cancelBtn_Click(object sender, EventArgs e)
        {
            // Sineset natin to "No" para malaman ng Main Form na nag-cancel siya
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void guna2HtmlLabel6_Click(object sender, EventArgs e)
        {

        }
    }
}