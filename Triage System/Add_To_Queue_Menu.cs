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
    public partial class Add_To_Queue_Menu : Form
    {
        // Dito natin ise-save yung pinili sa ComboBox para makuha ng UC_Search_Patient
        public string SelectedQueueCategory { get; private set; }

        // Tumatanggap ito ng patientName mula sa Search panel
        public Add_To_Queue_Menu(string patientName)
        {
            InitializeComponent();

            // I-set agad sa first item ("Consultation / Follow-Up") para hindi blanko
            if (cmbQueueCategory.Items.Count > 0)
            {
                cmbQueueCategory.SelectedIndex = 0;
            }
        }

        // CONFIRM / ADD TO QUEUE BUTTON
        private void btnSave_Record_Click(object sender, EventArgs e)
        {
            // 1. Kunin kung ano ang naka-select sa ComboBox
            SelectedQueueCategory = cmbQueueCategory.Text;

            // 2. I-set ang result as "Yes" at isara ang modal
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        // CANCEL BUTTON
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            // I-cancel at isara ang modal
            this.DialogResult = DialogResult.No;
            this.Close();
        }
    }
}