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
    public partial class Generate_Triage_Report : Form
    {
        public Generate_Triage_Report()
        {
            InitializeComponent();
        }

        private void slideOutTimer_Tick(object sender, EventArgs e)
        {
            // 1. Move the form to the right by 25 pixels every split-second
            this.Left += 25;

            // 2. Make it slightly transparent at the same time
            this.Opacity -= 0.1;

            // 3. Once it is completely invisible, stop the timer and officially close the form
            if (this.Opacity <= 0)
            {
                slideOutTimer.Stop();
                this.Close();
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            // Wag muna i-close! I-start muna natin ang pag-slide.
            slideOutTimer.Start();
        }
    }
}
