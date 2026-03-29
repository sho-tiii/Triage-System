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
        // --- ADDED: Changed to string to handle "P-2026-XXXX" format ---
        string activePatientId = "";
        string activeQueueType = ""; // --- ADDED: To track if they are Diagnostics or Consultation ---

        // Updated Constructor to receive ALL patient details when opened
        // --- ADDED: Constructor now accepts string patientId and queueType ---
        public UC_Start_Triage(string passedPatientId, string patientName, string queueId, string sex, int age, string queueType)
        {
            InitializeComponent();

            // 1. Save the ID and Queue Type for your "Send to Doctor" button
            activePatientId = passedPatientId;
            activeQueueType = queueType;

            // 2. Update the UI placeholders with the REAL data!
            lblPatientID.Text = "Patient ID: " + queueId;
            lblAge.Text = "Age: " + age.ToString();
            // NOTE: Kung may lblPatientName at lblSex ka, doon mo ilagay yung name at sex dito.
        }

        private void UC_Start_Triage_Load(object sender, EventArgs e)
        {
            // --- AUTOMATIC KEYPRESS VALIDATIONS ---
            txtSpO2.KeyPress += new KeyPressEventHandler(OnlyNumbers_KeyPress);
            txtHeartRate.KeyPress += new KeyPressEventHandler(OnlyNumbers_KeyPress);
            txtTemperature.KeyPress += new KeyPressEventHandler(Temperature_KeyPress);
            txtblood_Pressure.KeyPress += new KeyPressEventHandler(BloodPressure_KeyPress);

            CalculateTriageScore();

            // --- ADDED: Run the setup to configure UI based on Queue Type ---
            SetupTriageScreenBasedOnQueueType();
        }

        // --- ADDED: THE DYNAMIC UI LOGIC FOR DIAGNOSTICS ---
        private void SetupTriageScreenBasedOnQueueType()
        {
            if (activeQueueType == "Diagnostics (Lab/Rad)")
            {
                // I-disable ang doctor fields
                // NOTE: Replace 'cmbSpecialty' and 'cmbDoctor' with your actual control names if different
                txtchief_Complaint.Enabled = false;
                txtchief_Complaint.Text = "External Referral - For Diagnostics (Lab/Rad). No doctor consultation required.";

                // If you have these combo boxes on your form, uncomment these lines:
                // cmbSpecialty.Enabled = false;  
                // cmbDoctor.Enabled = false;     

                // Palitan ang button text (Assuming guna2Button4 is your 'Send' button)
                guna2Button4.Text = "Send to Diagnostics";
                guna2Button4.FillColor = Color.Orange; // Optional: Change color to indicate a different flow
            }
            else
            {
                // Normal Consultation Flow
                txtchief_Complaint.Enabled = true;
                txtchief_Complaint.Text = "";

                // If you have these combo boxes on your form, uncomment these lines:
                // cmbSpecialty.Enabled = true;
                // cmbDoctor.Enabled = true;

                guna2Button4.Text = "Send to Doctor";
                guna2Button4.FillColor = Color.FromArgb(46, 204, 113); // Back to standard Green
            }
        }

        // --- THE KEYPRESS VALIDATION LOGIC (FIXED FOR GUNA UI) ---
        private void OnlyNumbers_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Temperature_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            // THE BUG FIX: Cast as Guna2TextBox instead of normal TextBox
            if ((e.KeyChar == '.') && ((sender as Guna.UI2.WinForms.Guna2TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void BloodPressure_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
            {
                e.Handled = true;
            }
            // THE BUG FIX: Cast as Guna2TextBox instead of normal TextBox
            if ((e.KeyChar == '/') && ((sender as Guna.UI2.WinForms.Guna2TextBox).Text.IndexOf('/') > -1))
            {
                e.Handled = true;
            }
        }

        // --- YOUR ORIGINAL SAVE BUTTON ---
        private void guna2Button4_Click(object sender, EventArgs e)
        {
            string priorityLevel = "Regular";
            string assignedDepartment = "General Medicine";

            // --- ADDED: Modify assigned department if Diagnostics ---
            if (activeQueueType == "Diagnostics (Lab/Rad)")
            {
                assignedDepartment = "Diagnostics (Lab/Rad)";
                priorityLevel = "N/A"; // Priority isn't typically used for straight lab work
            }

            int systolic = 120, diastolic = 80;
            if (txtblood_Pressure.Text.Contains("/"))
            {
                string[] bpParts = txtblood_Pressure.Text.Split('/');
                int.TryParse(bpParts[0].Trim(), out systolic);
                int.TryParse(bpParts[1].Trim(), out diastolic);
            }

            int.TryParse(txtHeartRate.Text, out int heartRate);
            int.TryParse(txtSpO2.Text, out int oxygen);
            double.TryParse(txtTemperature.Text, out double temp);

            string ageText = lblAge.Text.Replace("Age:", "").Trim();
            int age = 0;
            int.TryParse(ageText, out age);

            // Calculate priority ONLY if it's a consultation
            if (activeQueueType != "Diagnostics (Lab/Rad)")
            {
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

                string complaint = txtchief_Complaint.Text.ToLower();

                if (complaint.Contains("chest pain") || complaint.Contains("heart") || complaint.Contains("palpitation")) { assignedDepartment = "Cardiology"; priorityLevel = "Emergency"; }
                else if (complaint.Contains("breath") || complaint.Contains("asthma") || complaint.Contains("cough")) { assignedDepartment = "Pulmonology"; if (complaint.Contains("difficulty")) priorityLevel = "Emergency"; }
                else if (complaint.Contains("dizzy") || complaint.Contains("headache") || complaint.Contains("stroke") || complaint.Contains("numb")) { assignedDepartment = "Neurology"; if (complaint.Contains("stroke")) priorityLevel = "Emergency"; }
                else if (complaint.Contains("bone") || complaint.Contains("fracture") || complaint.Contains("sprain")) { assignedDepartment = "Orthopedics"; if (complaint.Contains("fracture")) priorityLevel = "Priority"; }
            }

            string targetDestination = (activeQueueType == "Diagnostics (Lab/Rad)") ? "Diagnostics (Lab/Rad)" : "Doctor's queue";

            string resultMessage = $"Calculated Priority: {priorityLevel.ToUpper()}\n" +
                                   $"Assigned Department: {assignedDepartment}\n\n" +
                                   $"Send patient to the {targetDestination}?";

            DialogResult confirm = MessageBox.Show(resultMessage, "Complete Triage", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (confirm == DialogResult.Yes)
            {
                string connectionString = "server=localhost;user=root;password=;database=triage_system";

                string insertQuery = @"INSERT INTO triage_records 
                              (patient_id, blood_pressure, heart_rate, oxygen_level, temperature, weight, height, chief_complaint, priority_level, assigned_department) 
                              VALUES (@id, @bp, @hr, @oxy, @temp, @weight, @height, @complaint, @priority, @dept)";

                // --- ADDED: Determine next status based on Queue Type ---
                string nextStatus = (activeQueueType == "Diagnostics (Lab/Rad)") ? "Waiting for Lab/Rad" : "Waiting for Doctor";
                string updateQuery = "UPDATE patient_registration SET status = @nextStatus WHERE patient_id = @id";

                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();

                        using (MySqlCommand cmdInsert = new MySqlCommand(insertQuery, conn))
                        {
                            cmdInsert.Parameters.AddWithValue("@id", activePatientId); // --- FIXED: Using the string ID ---
                            cmdInsert.Parameters.AddWithValue("@bp", txtblood_Pressure.Text.Trim());
                            cmdInsert.Parameters.AddWithValue("@hr", heartRate);
                            cmdInsert.Parameters.AddWithValue("@oxy", oxygen);
                            cmdInsert.Parameters.AddWithValue("@temp", temp);

                            cmdInsert.Parameters.AddWithValue("@weight", 0);
                            cmdInsert.Parameters.AddWithValue("@height", 0);

                            cmdInsert.Parameters.AddWithValue("@complaint", txtchief_Complaint.Text.Trim());
                            cmdInsert.Parameters.AddWithValue("@priority", priorityLevel);
                            cmdInsert.Parameters.AddWithValue("@dept", assignedDepartment);

                            cmdInsert.ExecuteNonQuery();
                        }

                        using (MySqlCommand cmdUpdate = new MySqlCommand(updateQuery, conn))
                        {
                            cmdUpdate.Parameters.AddWithValue("@id", activePatientId); // --- FIXED: Using the string ID ---
                            cmdUpdate.Parameters.AddWithValue("@nextStatus", nextStatus);
                            cmdUpdate.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show($"Success! Patient routed to {assignedDepartment}.", "Triage Complete");

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

        // --- THE UPGRADED REAL-TIME TRIAGE CALCULATOR WITH COLORS ---
        private void CalculateTriageScore()
        {
            // --- ADDED: Bypass color changing if it's just Diagnostics ---
            if (activeQueueType == "Diagnostics (Lab/Rad)")
            {
                lblPriorityStatus.Text = "DIAGNOSTICS";
                lblPriorityStatus.ForeColor = Color.Orange;
                lblUrgencyScore.Text = "URGENCY SCALE: N/A";
                pbUrgencyScale.Value = 0;
                return; // Stop the rest of the calculation
            }

            // --- 1. RESET ALL COLORS FIRST ---
            Color defaultBorder = Color.FromArgb(213, 218, 223); // Default Guna Gray
            Color defaultFill = Color.White;

            panelSpO2.BorderColor = defaultBorder; panelSpO2.FillColor = defaultFill;
            txtSpO2.BorderColor = defaultBorder; txtSpO2.FillColor = defaultFill;

            panelHeartRate.BorderColor = defaultBorder; panelHeartRate.FillColor = defaultFill;
            txtHeartRate.BorderColor = defaultBorder; txtHeartRate.FillColor = defaultFill;

            panelTemperature.BorderColor = defaultBorder; panelTemperature.FillColor = defaultFill;
            txtTemperature.BorderColor = defaultBorder; txtTemperature.FillColor = defaultFill;

            panelBloodPressure.BorderColor = defaultBorder; panelBloodPressure.FillColor = defaultFill;
            txtblood_Pressure.BorderColor = defaultBorder; txtblood_Pressure.FillColor = defaultFill;


            // --- 2. THE BLANK STATE CHECK ---
            if (string.IsNullOrWhiteSpace(txtSpO2.Text) &&
                string.IsNullOrWhiteSpace(txtHeartRate.Text) &&
                string.IsNullOrWhiteSpace(txtTemperature.Text) &&
                string.IsNullOrWhiteSpace(txtblood_Pressure.Text))
            {
                lblPriorityStatus.Text = "---";
                lblPriorityStatus.ForeColor = Color.Gray;
                lblUrgencyScore.Text = "URGENCY SCALE: ---";
                pbUrgencyScale.Value = 0;
                return;
            }

            // 3. SAFELY PARSE VITALS
            bool hasSpO2 = int.TryParse(txtSpO2.Text, out int spo2);
            bool hasHR = int.TryParse(txtHeartRate.Text, out int heartRate);
            bool hasTemp = double.TryParse(txtTemperature.Text, out double temp);

            int systolic = 0;
            int diastolic = 0;
            bool hasBP = false;

            if (!string.IsNullOrWhiteSpace(txtblood_Pressure.Text))
            {
                if (!txtblood_Pressure.Text.Contains("/") || txtblood_Pressure.Text.Length < 3)
                {
                    lblPriorityStatus.Text = "INVALID BP FORMAT";
                    lblPriorityStatus.ForeColor = Color.Red;
                    lblUrgencyScore.Text = "URGENCY SCALE: ERR";
                    pbUrgencyScale.Value = 0;
                    return;
                }

                string[] bpParts = txtblood_Pressure.Text.Split('/');
                if (bpParts.Length == 2 &&
                    int.TryParse(bpParts[0].Trim(), out systolic) &&
                    int.TryParse(bpParts[1].Trim(), out diastolic))
                {
                    hasBP = true;
                }
            }

            // 4. THE HUMAN BOUNDARY CHECK (ABSURD VALUES)
            if ((hasSpO2 && spo2 > 100) ||
                (hasHR && heartRate > 300) ||
                (hasTemp && (temp > 45.0 || temp < 20.0)) ||
                (hasBP && (systolic > 300 || diastolic > 200)))
            {
                lblPriorityStatus.Text = "INVALID INPUT";
                lblPriorityStatus.ForeColor = Color.Red;
                lblUrgencyScore.Text = "URGENCY SCALE: ERR";
                pbUrgencyScale.Value = 0;
                return;
            }

            // 5. Baseline Status
            int score = 3;
            string statusText = "NORMAL";
            Color statusColor = Color.MediumSeaGreen;

            // --- 6. INDIVIDUAL CARD COLORING & MAIN SCORE CALCULATION ---

            // SpO2 Logic
            if (hasSpO2 && spo2 < 85)
            {
                panelSpO2.BorderColor = Color.Crimson; panelSpO2.FillColor = Color.MistyRose;
                txtSpO2.BorderColor = Color.Crimson; txtSpO2.FillColor = Color.MistyRose;
            }
            else if (hasSpO2 && spo2 <= 90)
            {
                panelSpO2.BorderColor = Color.Crimson; panelSpO2.FillColor = Color.MistyRose;
                txtSpO2.BorderColor = Color.Crimson; txtSpO2.FillColor = Color.MistyRose;
            }

            // Heart Rate Logic
            if (hasHR && heartRate < 40)
            {
                panelHeartRate.BorderColor = Color.Crimson; panelHeartRate.FillColor = Color.MistyRose;
                txtHeartRate.BorderColor = Color.Crimson; txtHeartRate.FillColor = Color.MistyRose;
            }
            else if (hasHR && heartRate > 150)
            {
                panelHeartRate.BorderColor = Color.Crimson; panelHeartRate.FillColor = Color.MistyRose;
                txtHeartRate.BorderColor = Color.Crimson; txtHeartRate.FillColor = Color.MistyRose;
            }
            else if (hasHR && heartRate > 120)
            {
                panelHeartRate.BorderColor = Color.DarkOrange; panelHeartRate.FillColor = Color.LemonChiffon;
                txtHeartRate.BorderColor = Color.DarkOrange; txtHeartRate.FillColor = Color.LemonChiffon;
            }

            // Temperature Logic
            if (hasTemp && temp <= 32.0)
            {
                panelTemperature.BorderColor = Color.Crimson; panelTemperature.FillColor = Color.MistyRose;
                txtTemperature.BorderColor = Color.Crimson; txtTemperature.FillColor = Color.MistyRose;
            }
            else if (hasTemp && (temp >= 40.0 || temp <= 34.0))
            {
                panelTemperature.BorderColor = Color.Crimson; panelTemperature.FillColor = Color.MistyRose;
                txtTemperature.BorderColor = Color.Crimson; txtTemperature.FillColor = Color.MistyRose;
            }
            else if (hasTemp && (temp >= 38.5 || temp <= 35.5))
            {
                panelTemperature.BorderColor = Color.DarkOrange; panelTemperature.FillColor = Color.LemonChiffon;
                txtTemperature.BorderColor = Color.DarkOrange; txtTemperature.FillColor = Color.LemonChiffon;
            }

            // Blood Pressure Logic
            if (hasBP && systolic < 70)
            {
                panelBloodPressure.BorderColor = Color.Crimson; panelBloodPressure.FillColor = Color.MistyRose;
                txtblood_Pressure.BorderColor = Color.Crimson; txtblood_Pressure.FillColor = Color.MistyRose;
            }
            else if (hasBP && (systolic >= 180 || diastolic >= 120 || systolic <= 80))
            {
                panelBloodPressure.BorderColor = Color.Crimson; panelBloodPressure.FillColor = Color.MistyRose;
                txtblood_Pressure.BorderColor = Color.Crimson; txtblood_Pressure.FillColor = Color.MistyRose;
            }
            else if (hasBP && (systolic >= 140 || diastolic >= 90 || systolic <= 90))
            {
                panelBloodPressure.BorderColor = Color.DarkOrange; panelBloodPressure.FillColor = Color.LemonChiffon;
                txtblood_Pressure.BorderColor = Color.DarkOrange; txtblood_Pressure.FillColor = Color.LemonChiffon;
            }

            // --- 7. OVERALL TRIAGE SCORE DECISION ---
            if ((hasSpO2 && spo2 < 85) || (hasHR && heartRate < 40) || (hasBP && systolic < 70) || (hasTemp && temp <= 32.0))
            {
                score = 10;
                statusText = "RESUSCITATION";
                statusColor = Color.DarkRed;
            }
            else if ((hasSpO2 && spo2 <= 90) || (hasTemp && temp >= 40.0) || (hasTemp && temp <= 34.0) || (hasBP && (systolic >= 180 || diastolic >= 120)) || (hasBP && (systolic <= 80)))
            {
                score = 9;
                statusText = "CRITICAL";
                statusColor = Color.Crimson;
            }
            else if ((hasHR && heartRate > 120) || (hasTemp && temp >= 38.5) || (hasTemp && temp <= 35.5) || (hasBP && (systolic >= 140 || diastolic >= 90)) || (hasBP && (systolic <= 90)))
            {
                score = 7;
                statusText = "URGENT";
                statusColor = Color.DarkOrange;
            }

            // 8. Update UI text and Progress Bar
            lblPriorityStatus.Text = statusText;
            lblPriorityStatus.ForeColor = statusColor;
            lblUrgencyScore.Text = $"URGENCY SCALE: {score}/10";

            pbUrgencyScale.Value = score;
            pbUrgencyScale.ProgressColor = statusColor;
            pbUrgencyScale.ProgressColor2 = statusColor;
        }

        // --- TEXT CHANGED EVENTS TO TRIGGER CALCULATION ---
        private void txtOxygen_TextChanged(object sender, EventArgs e) { CalculateTriageScore(); }
        private void txtTemperature_TextChanged(object sender, EventArgs e) { CalculateTriageScore(); }
        private void txtheart_Rate_TextChanged(object sender, EventArgs e) { CalculateTriageScore(); }
        private void txtblood_Pressure_TextChanged(object sender, EventArgs e) { CalculateTriageScore(); }

        // Empty events
        private void txtchief_Complaint_TextChanged(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void label19_Click(object sender, EventArgs e) { }
        private void guna2VSeparator2_Click(object sender, EventArgs e) { }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void lblPriorityStatus_Click(object sender, EventArgs e)
        {

        }
    }
}