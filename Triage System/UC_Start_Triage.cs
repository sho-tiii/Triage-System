using MySql.Data.MySqlClient;
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
    public partial class UC_Start_Triage : UserControl
    {
        // Holds the real database ID so we can save their vitals later
        int activePatientId = 0;

        // Updated Constructor to receive ALL patient details when opened
        public UC_Start_Triage(int passedPatientId, string patientName, string queueId, string sex, int age)
        {
            InitializeComponent();

            // 1. Save the ID for your "Send to Doctor" button
            activePatientId = passedPatientId;

            // 2. Update the UI placeholders with the REAL data!
            lblName.Text = patientName;
            lblPatientID.Text = "Patient ID: " + queueId;
            lblSex.Text = "Sex: " + sex;
            lblAge.Text = "Age: " + age.ToString();
        }

        private void UC_Start_Triage_Load(object sender, EventArgs e)
        {
   
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            // 1. Set Default Status
            string priorityLevel = "Regular";
            string assignedDepartment = "General Medicine";

            // 2. Safely Parse Vitals Data
            int systolic = 120, diastolic = 80;
            if (txtblood_Pressure.Text.Contains("/"))
            {
                string[] bpParts = txtblood_Pressure.Text.Split('/');
                int.TryParse(bpParts[0].Trim(), out systolic);
                int.TryParse(bpParts[1].Trim(), out diastolic);
            }

            int.TryParse(txtheart_Rate.Text, out int heartRate);
            int.TryParse(txtOxygen.Text, out int oxygen);
            double.TryParse(txtTemperature.Text, out double temp);

            // --- SMART EXTRACTION ---
            // Make sure your Patient ID label is named lblPatientID for this to work!
            // Takes "Patient ID: N-1001", removes the letters, leaving just "1001"
            string idText = lblPatientID.Text.Replace("Patient ID:", "").Replace("N-", "").Trim();
            int patientId = 0;
            int.TryParse(idText, out patientId);

            string ageText = lblAge.Text.Replace("Age:", "").Trim();
            int age = 0;
            int.TryParse(ageText, out age);

            // --- 3. AUTO-TRIAGE ALGORITHM ---
            if ((oxygen > 0 && oxygen < 90) ||
                (heartRate > 130 || (heartRate > 0 && heartRate < 40)) ||
                (systolic >= 180 || diastolic >= 120) ||
                (temp >= 40.0))
            {
                priorityLevel = "Emergency";
            }
            else if (priorityLevel != "Emergency")
            {
                if ((oxygen >= 90 && oxygen <= 94) ||
                    (heartRate >= 110 && heartRate <= 130) ||
                    (systolic >= 140 || diastolic >= 90) ||
                    (temp >= 38.5) ||
                    (age >= 60))
                {
                    priorityLevel = "Priority";
                }
            }

            // --- 4. CHIEF COMPLAINT ROUTING ---
            string complaint = txtchief_Complaint.Text.ToLower();

            if (complaint.Contains("chest pain") || complaint.Contains("heart") || complaint.Contains("palpitation")) { assignedDepartment = "Cardiology"; priorityLevel = "Emergency"; }
            else if (complaint.Contains("breath") || complaint.Contains("asthma") || complaint.Contains("cough")) { assignedDepartment = "Pulmonology"; if (complaint.Contains("difficulty")) priorityLevel = "Emergency"; }
            else if (complaint.Contains("dizzy") || complaint.Contains("headache") || complaint.Contains("stroke") || complaint.Contains("numb")) { assignedDepartment = "Neurology"; if (complaint.Contains("stroke")) priorityLevel = "Emergency"; }
            else if (complaint.Contains("bone") || complaint.Contains("fracture") || complaint.Contains("sprain")) { assignedDepartment = "Orthopedics"; if (complaint.Contains("fracture")) priorityLevel = "Priority"; }

            // --- 5. CONFIRMATION & DATABASE SAVING ---
            string resultMessage = $"Calculated Priority: {priorityLevel.ToUpper()}\n" +
                                   $"Assigned Department: {assignedDepartment}\n\n" +
                                   $"Send patient to the doctor's queue?";

            DialogResult confirm = MessageBox.Show(resultMessage, "Complete Triage", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (confirm == DialogResult.Yes)
            {
                string connectionString = "server=localhost;user=root;password=;database=triage_system";

                // Query 1: Save all vitals to the new table
                string insertQuery = @"INSERT INTO triage_records 
                              (patient_id, blood_pressure, heart_rate, oxygen_level, temperature, weight, height, chief_complaint, priority_level, assigned_department) 
                              VALUES (@id, @bp, @hr, @oxy, @temp, @weight, @height, @complaint, @priority, @dept)";

                // Query 2: Change status so they drop off the Nurse Queue
                string updateQuery = "UPDATE patient_registration SET status = 'Waiting for Doctor' WHERE patient_id = @id";

                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();

                        // 1. Execute Insert
                        using (MySqlCommand cmdInsert = new MySqlCommand(insertQuery, conn))
                        {
                            cmdInsert.Parameters.AddWithValue("@id", patientId);
                            cmdInsert.Parameters.AddWithValue("@bp", txtblood_Pressure.Text.Trim());
                            cmdInsert.Parameters.AddWithValue("@hr", heartRate);
                            cmdInsert.Parameters.AddWithValue("@oxy", oxygen);
                            cmdInsert.Parameters.AddWithValue("@temp", temp);

                            // Safely parse weight and height
                            double.TryParse(txtWeight.Text, out double weight);
                            cmdInsert.Parameters.AddWithValue("@weight", weight);

                            double.TryParse(txtHeight.Text, out double height);
                            cmdInsert.Parameters.AddWithValue("@height", height);

                            cmdInsert.Parameters.AddWithValue("@complaint", txtchief_Complaint.Text.Trim());
                            cmdInsert.Parameters.AddWithValue("@priority", priorityLevel);
                            cmdInsert.Parameters.AddWithValue("@dept", assignedDepartment);

                            cmdInsert.ExecuteNonQuery();
                        }

                        // 2. Execute Update
                        using (MySqlCommand cmdUpdate = new MySqlCommand(updateQuery, conn))
                        {
                            cmdUpdate.Parameters.AddWithValue("@id", patientId);
                            cmdUpdate.ExecuteNonQuery();
                        }
                    }

                    // --- 6. RETURN TO QUEUE DASHBOARD ---
                    MessageBox.Show($"Success! Patient routed to {assignedDepartment}.", "Triage Complete");

                    // Load the Queue Screen back up
                    UC_Patient_Queue queueScreen = new UC_Patient_Queue();
                    Control mainPanel = this.Parent;
                    mainPanel.Controls.Clear();
                    queueScreen.Dock = DockStyle.Fill;
                    mainPanel.Controls.Add(queueScreen);
                    queueScreen.BringToFront();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
