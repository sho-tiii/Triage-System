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
    // Pinalitan ko yung 'Form' ng 'UserControl' para sumakto sa FlowLayoutPanel natin!
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

        // Property para sa Bell Icon (assuming pictureBox1 ang name ng PictureBox mo)
        public Image NotifIcon
        {
            get { return guna2PictureBox1.Image; }
            set { guna2PictureBox1.Image = value; }
        }

        private void guna2Separator1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {
            // Papasabugin/Buburahin ng UserControl na 'to ang sarili niya
            this.Dispose();
        }
    }
}