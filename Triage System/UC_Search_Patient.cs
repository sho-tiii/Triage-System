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
    public partial class UC_Search_Patient : UserControl
    {
        public UC_Search_Patient()
        {
            InitializeComponent();
        }
        private NewPatient _newPatientForm = null;
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (_newPatientForm == null || _newPatientForm.IsDisposed)
            {
                _newPatientForm = new NewPatient();
                _newPatientForm.Show();
            }
            else
            {
                _newPatientForm.BringToFront();
            }
        }
    }
}
