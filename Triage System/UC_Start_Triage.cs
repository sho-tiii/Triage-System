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
        string activePatientId = "";
        string activeQueueType = "";

        // --- ADDED: Connection string sa taas para magamit ng Combobox at Save button ---
        string connectionString = "server=localhost;user=root;password=;database=triage_system";

        public UC_Start_Triage(string passedPatientId, string patientName, string queueId, string sex, int age, string queueType)
        {
            InitializeComponent();

            activePatientId = passedPatientId;
            activeQueueType = queueType;

            // --- FIXED: Bind the passed variables to your UI Labels ---
            lblPatientID.Text = "Patient ID: " + passedPatientId;
            lblName.Text = patientName;
            lblGender.Text = "Sex: " + sex;
            lblAge.Text = "Age: " + age.ToString();

            // (Optional) Update the last visit to current date so it's not hardcoded
            lblLastVisit.Text = "Triage Date: " + DateTime.Now.ToString("MMMM dd, yyyy");

            // --- ADDED: Extract initials for the avatar circle ---
            if (!string.IsNullOrWhiteSpace(patientName))
            {
                var nameParts = patientName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string initials = "";
                if (nameParts.Length > 0) initials += nameParts[0][0]; // First initial
                if (nameParts.Length > 1) initials += nameParts[nameParts.Length - 1][0]; // Last initial
                guna2CircleButton1.Text = initials.ToUpper();
            }

            // --- FIXED: Wire up the event handler so the Department Dropdown actually triggers ---
            cmbDepartment.SelectedIndexChanged += cmbDepartment_SelectedIndexChanged;
        }

        private void UC_Start_Triage_Load(object sender, EventArgs e)
        {
            // --- AUTOMATIC KEYPRESS VALIDATIONS ---
            txtSpO2.KeyPress += new KeyPressEventHandler(OnlyNumbers_KeyPress);
            txtHeartRate.KeyPress += new KeyPressEventHandler(OnlyNumbers_KeyPress);
            txtTemperature.KeyPress += new KeyPressEventHandler(Temperature_KeyPress);
            txtblood_Pressure.KeyPress += new KeyPressEventHandler(BloodPressure_KeyPress);

            // --- ADDED: Keypress para sa weight at height ---
            txtWeight.KeyPress += new KeyPressEventHandler(Temperature_KeyPress);
            txtHeight.KeyPress += new KeyPressEventHandler(Temperature_KeyPress);

            CalculateTriageScore();
            SetupTriageScreenBasedOnQueueType();
        }

        // --- ADDED: DYNAMIC DOCTOR LOADING BASED SA DEPARTMENT ---
        private void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 1. Clear existing items and reset the default placeholder
            cmbPhysician.Items.Clear();
            cmbPhysician.Items.Add("--Select Doctor--");
            cmbPhysician.SelectedIndex = 0;

            // 2. If they picked "--Select Department--", just stop here
            if (cmbDepartment.SelectedIndex <= 0) return;

            // 3. Fetch doctors from the database
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // DITO YUNG BAGONG QUERY (staffaccounts ang gamit)
                    string query = "SELECT FullName FROM staffaccounts WHERE DepartmentSector = @dept AND Role = 'Doctor' AND Status = 'Active'";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dept", cmbDepartment.Text);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // DITO YUNG READER: FullName na ang kinukuha imbes na doctor_name
                                cmbPhysician.Items.Add(reader["FullName"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading doctors: " + ex.Message);
                }
            }
        }

        private void SetupTriageScreenBasedOnQueueType()
        {
            if (activeQueueType == "Diagnostics (Lab/Rad)")
            {
                txtchief_Complaint.Enabled = false;
                txtchief_Complaint.Text = "External Referral - For Diagnostics (Lab/Rad). No doctor consultation required.";

                // --- UNCOMMENTED AND UPDATED ---
                cmbDepartment.Enabled = false;
                cmbPhysician.Enabled = false;

                guna2Button4.Text = "Send to Diagnostics";
                guna2Button4.FillColor = Color.Orange;
            }
            else
            {
                txtchief_Complaint.Enabled = true;
                txtchief_Complaint.Text = "";

                // --- UNCOMMENTED AND UPDATED ---
                cmbDepartment.Enabled = true;
                cmbPhysician.Enabled = true;

                guna2Button4.Text = "Send to Doctor";
                guna2Button4.FillColor = Color.FromArgb(46, 204, 113);
            }
        }

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
            if ((e.KeyChar == '/') && ((sender as Guna.UI2.WinForms.Guna2TextBox).Text.IndexOf('/') > -1))
            {
                e.Handled = true;
            }
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            // --- KINUHA YUNG DOCTOR SA COMBOBOX ---
            string selectedDoctor = (activeQueueType == "Diagnostics (Lab/Rad)") ? "N/A" : cmbPhysician.Text;

            string priorityLevel = "Regular";
            string assignedDepartment = "General Medicine";

            if (activeQueueType == "Diagnostics (Lab/Rad)")
            {
                assignedDepartment = "Diagnostics (Lab/Rad)";
                priorityLevel = "N/A";
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

            // --- ADDED: PARSING PARA SA WEIGHT AT HEIGHT ---
            double.TryParse(txtWeight.Text, out double weight);
            double.TryParse(txtHeight.Text, out double height);

            string ageText = lblAge.Text.Replace("Age:", "").Trim();
            int age = 0;
            int.TryParse(ageText, out age);

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

                // --- ADDED: If the user manually chose a department from the combobox, use that instead of the default/keyword one. ---
                if (cmbDepartment.SelectedIndex > 0 && !string.IsNullOrEmpty(cmbDepartment.Text))
                {
                    assignedDepartment = cmbDepartment.Text;
                }
            }

            string targetDestination = (activeQueueType == "Diagnostics (Lab/Rad)") ? "Diagnostics (Lab/Rad)" : "Doctor's queue";

            string resultMessage = $"Calculated Priority: {priorityLevel.ToUpper()}\n" +
                                   $"Assigned Department: {assignedDepartment}\n\n" +
                                   $"Send patient to the {targetDestination}?";

            DialogResult confirm = MessageBox.Show(resultMessage, "Complete Triage", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (confirm == DialogResult.Yes)
            {
                // --- ADDED: assigned_physician sa insert query ---
                string insertQuery = @"INSERT INTO triage_records 
                              (patient_id, blood_pressure, heart_rate, oxygen_level, temperature, weight, height, chief_complaint, priority_level, assigned_department, assigned_physician) 
                              VALUES (@id, @bp, @hr, @oxy, @temp, @weight, @height, @complaint, @priority, @dept, @doc)";

                string nextStatus = (activeQueueType == "Diagnostics (Lab/Rad)") ? "Waiting for Lab/Rad" : "Waiting for Doctor";
                string updateQuery = "UPDATE patient_registration SET status = @nextStatus WHERE patient_id = @id";

                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();

                        using (MySqlCommand cmdInsert = new MySqlCommand(insertQuery, conn))
                        {
                            cmdInsert.Parameters.AddWithValue("@id", activePatientId);
                            cmdInsert.Parameters.AddWithValue("@bp", txtblood_Pressure.Text.Trim());
                            cmdInsert.Parameters.AddWithValue("@hr", heartRate);
                            cmdInsert.Parameters.AddWithValue("@oxy", oxygen);
                            cmdInsert.Parameters.AddWithValue("@temp", temp);

                            // --- FIXED: Ipapasok na yung tunay na variables imbes na 0 ---
                            cmdInsert.Parameters.AddWithValue("@weight", weight);
                            cmdInsert.Parameters.AddWithValue("@height", height);

                            cmdInsert.Parameters.AddWithValue("@complaint", txtchief_Complaint.Text.Trim());
                            cmdInsert.Parameters.AddWithValue("@priority", priorityLevel);
                            cmdInsert.Parameters.AddWithValue("@dept", assignedDepartment);

                            // --- ADDED: Parameter para sa doctor ---
                            cmdInsert.Parameters.AddWithValue("@doc", selectedDoctor);

                            cmdInsert.ExecuteNonQuery();
                        }

                        using (MySqlCommand cmdUpdate = new MySqlCommand(updateQuery, conn))
                        {
                            cmdUpdate.Parameters.AddWithValue("@id", activePatientId);
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

        // --- THE UPGRADED REAL-TIME TRIAGE CALCULATOR WITH COLORS (WALANG GINALAW DITO) ---
        private void CalculateTriageScore()
        {
            if (activeQueueType == "Diagnostics (Lab/Rad)")
            {
                lblPriorityStatus.Text = "DIAGNOSTICS";
                lblPriorityStatus.ForeColor = Color.Orange;
                lblUrgencyScore.Text = "URGENCY SCALE: N/A";
                pbUrgencyScale.Value = 0;
                return;
            }

            Color defaultBorder = Color.FromArgb(213, 218, 223);
            Color defaultFill = Color.White;

            panelSpO2.BorderColor = defaultBorder; panelSpO2.FillColor = defaultFill;
            txtSpO2.BorderColor = defaultBorder; txtSpO2.FillColor = defaultFill;

            panelHeartRate.BorderColor = defaultBorder; panelHeartRate.FillColor = defaultFill;
            txtHeartRate.BorderColor = defaultBorder; txtHeartRate.FillColor = defaultFill;

            panelTemperature.BorderColor = defaultBorder; panelTemperature.FillColor = defaultFill;
            txtTemperature.BorderColor = defaultBorder; txtTemperature.FillColor = defaultFill;

            panelBloodPressure.BorderColor = defaultBorder; panelBloodPressure.FillColor = defaultFill;
            txtblood_Pressure.BorderColor = defaultBorder; txtblood_Pressure.FillColor = defaultFill;

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

            int score = 3;
            string statusText = "NORMAL";
            Color statusColor = Color.MediumSeaGreen;

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

            lblPriorityStatus.Text = statusText;
            lblPriorityStatus.ForeColor = statusColor;
            lblUrgencyScore.Text = $"URGENCY SCALE: {score}/10";

            pbUrgencyScale.Value = score;
            pbUrgencyScale.ProgressColor = statusColor;
            pbUrgencyScale.ProgressColor2 = statusColor;
        }

        private void txtOxygen_TextChanged(object sender, EventArgs e) { CalculateTriageScore(); }
        private void txtTemperature_TextChanged(object sender, EventArgs e) { CalculateTriageScore(); }
        private void txtheart_Rate_TextChanged(object sender, EventArgs e) { CalculateTriageScore(); }
        private void txtblood_Pressure_TextChanged(object sender, EventArgs e) { CalculateTriageScore(); }

        private void txtchief_Complaint_TextChanged(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void label19_Click(object sender, EventArgs e) { }
        private void guna2VSeparator2_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void lblPriorityStatus_Click(object sender, EventArgs e) { }

        private void guna2Button4_Click_1(object sender, EventArgs e)
        {
            // 1. I-refresh ang calculation para sure na updated ang priority status bago i-save
            CalculateTriageScore();

            string priorityLevel = lblPriorityStatus.Text;
            string dept = (activeQueueType == "Diagnostics (Lab/Rad)") ? "Diagnostics (Lab/Rad)" : cmbDepartment.Text;
            string physician = (activeQueueType == "Diagnostics (Lab/Rad)") ? "N/A" : cmbPhysician.Text;

            // 2. Validation: Kung consultation, bawal ang blangkong department o complaint
            if (activeQueueType != "Diagnostics (Lab/Rad)" && (cmbDepartment.SelectedIndex <= 0 || string.IsNullOrWhiteSpace(txtchief_Complaint.Text)))
            {
                MessageBox.Show("Please select a Department and enter the Chief Complaint.", "Incomplete Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Confirmation Message
            DialogResult confirm = MessageBox.Show($"Are you sure you want to send this patient to {dept}?", "Confirm Triage", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            // 4. Ligtas na pag-convert ng Vitals (Para hindi mag-error kung blangko)
            double.TryParse(txtWeight.Text, out double weight);
            double.TryParse(txtHeight.Text, out double height);
            double.TryParse(txtTemperature.Text, out double temp);
            int.TryParse(txtHeartRate.Text, out int hr);
            int.TryParse(txtSpO2.Text, out int oxy);

            // 5. Database Saving & Updating
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string insertSql = @"INSERT INTO patient_queue
(patient_id, fullname, sex, age, blood_pressure, heart_rate, oxygen_level, temperature, weight, height, chief_complaint, priority_level, assigned_department, assigned_physician) 
VALUES (@pid, @fullname, @sex, @age, @bp, @hr, @oxy, @temp, @w, @h, @comp, @prio, @dept, @doc)";
                    using (MySqlCommand cmd = new MySqlCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", activePatientId);
                        cmd.Parameters.AddWithValue("@bp", txtblood_Pressure.Text.Trim());
                        cmd.Parameters.AddWithValue("@hr", hr);
                        cmd.Parameters.AddWithValue("@oxy", oxy);
                        cmd.Parameters.AddWithValue("@temp", temp);
                        cmd.Parameters.AddWithValue("@w", weight);
                        cmd.Parameters.AddWithValue("@h", height);
                        cmd.Parameters.AddWithValue("@comp", txtchief_Complaint.Text.Trim());
                        cmd.Parameters.AddWithValue("@prio", priorityLevel);
                        cmd.Parameters.AddWithValue("@dept", dept);
                        cmd.Parameters.AddWithValue("@doc", physician);

                        cmd.Parameters.AddWithValue("@fullname", lblName.Text);
                        cmd.Parameters.AddWithValue("@sex", lblGender.Text);
                        cmd.Parameters.AddWithValue("@age", lblAge.Text);
                        cmd.ExecuteNonQuery();
                    }

                    // A. INSERT DATA SA TRIAGE_RECORDS TABLE
                    insertSql = @"INSERT INTO triage_records 
                (patient_id, blood_pressure, heart_rate, oxygen_level, temperature, weight, height, chief_complaint, priority_level, assigned_department, assigned_physician) 
                VALUES (@pid, @bp, @hr, @oxy, @temp, @w, @h, @comp, @prio, @dept, @doc)";

                    using (MySqlCommand cmd = new MySqlCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", activePatientId);
                        cmd.Parameters.AddWithValue("@bp", txtblood_Pressure.Text.Trim());
                        cmd.Parameters.AddWithValue("@hr", hr);
                        cmd.Parameters.AddWithValue("@oxy", oxy);
                        cmd.Parameters.AddWithValue("@temp", temp);
                        cmd.Parameters.AddWithValue("@w", weight);
                        cmd.Parameters.AddWithValue("@h", height);
                        cmd.Parameters.AddWithValue("@comp", txtchief_Complaint.Text.Trim());
                        cmd.Parameters.AddWithValue("@prio", priorityLevel);
                        cmd.Parameters.AddWithValue("@dept", dept);
                        cmd.Parameters.AddWithValue("@doc", physician);
                        cmd.ExecuteNonQuery();
                    }

                    // B. UPDATE STATUS SA PATIENT_REGISTRATION TABLE
                    string status = (activeQueueType == "Diagnostics (Lab/Rad)") ? "Waiting for Lab" : "Waiting for Doctor";
                    string updateSql = "UPDATE patient_registration SET status = @s WHERE patient_id = @id";

                    using (MySqlCommand cmdUp = new MySqlCommand(updateSql, conn))
                    {
                        cmdUp.Parameters.AddWithValue("@s", status);
                        cmdUp.Parameters.AddWithValue("@id", activePatientId);
                        cmdUp.ExecuteNonQuery();
                    }

                    // 6. Success Message & Balik sa Queue Screen
                    MessageBox.Show("Patient Triage Completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    UC_Patient_Queue queueScreen = new UC_Patient_Queue();
                    Control mainPanel = this.Parent;
                    if (mainPanel != null)
                    {
                        mainPanel.Controls.Clear();
                        queueScreen.Dock = DockStyle.Fill;
                        mainPanel.Controls.Add(queueScreen);
                        queueScreen.BringToFront();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cmbDepartment_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // 1. Clear existing items and reset the default placeholder
            cmbPhysician.Items.Clear();
            cmbPhysician.Items.Add("--Select Doctor--");
            cmbPhysician.SelectedIndex = 0;

            // 2. If they picked "--Select Department--", just stop here
            if (cmbDepartment.SelectedIndex <= 0) return;

            // 3. Fetch doctors from the database
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT doctor_name FROM doctors WHERE specialization = @dept AND status = 'Available'";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dept", cmbDepartment.Text);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cmbPhysician.Items.Add(reader["doctor_name"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading doctors: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}