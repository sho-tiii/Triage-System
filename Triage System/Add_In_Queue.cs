using System;
using System.Windows.Forms;

namespace Triage_System
{
    public partial class Add_In_Queue : Form
    {
        // Constructor that accepts the Name and ID
        public Add_In_Queue(string patientName, string patientId)
        {
            InitializeComponent();

            // Dynamically updates the label text
            lblAdd_In_Queue.Text = $"Add {patientName} ({patientId})\nto the queue?";
        }

        // YES BUTTON
        private void btnYes_Click(object sender, EventArgs e)
        {
            // Sends the "Yes" signal back to UC_Search_Patient
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        // CANCEL BUTTON
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Sends the "Cancel" signal back to UC_Search_Patient
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnYes_Click_1(object sender, EventArgs e)
        {
            // 1. Palalabasin muna natin yung Success Modal mo
            using (Added_Patient_Queue successModal = new Added_Patient_Queue())
            {
                successModal.StartPosition = FormStartPosition.CenterScreen;
                successModal.ShowDialog(); // Magpo-pause dito yung code hanggang i-close yung success modal
            }

            // 2. Pagka-close ng success modal, tsaka niya isesend yung "Yes" sa main form at magco-close
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            // Sends the "Cancel" signal back to UC_Search_Patient
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}