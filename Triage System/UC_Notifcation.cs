using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Triage_System
{
    public partial class UC_Notifcation : UserControl
    {
        public UC_Notifcation()
        {
            InitializeComponent();
        }

        // Property para sa Notification Message
        public string MessageText
        {
            get { return lblNotifMessage.Text; }
            set { lblNotifMessage.Text = value; }
        }

        // Property para sa Oras
        public string TimeText
        {
            get { return lblNotifTime.Text; }
            set { lblNotifTime.Text = value; }
        }

        // Property para sa Bell Icon (assuming guna2PictureBox1 ang name ng PictureBox mo)
        public Image NotifIcon
        {
            get { return guna2PictureBox1.Image; }
            set { guna2PictureBox1.Image = value; }
        }

        private void guna2Separator1_Click(object sender, EventArgs e)
        {
            // Pwedeng walang laman kung design lang 'to
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {
            // Kapag kinlick yung "X" icon/label, ididispose (buburahin) niya sarili niya sa flow layout panel
            this.Dispose();
        }
    }
}