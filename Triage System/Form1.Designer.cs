namespace Triage_System
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.sideBar = new Guna.UI2.WinForms.Guna2Panel();
            this.mainPanel = new Guna.UI2.WinForms.Guna2Panel();
            this.img_slide = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2ControlBox1 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2ControlBox2 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2ControlBox3 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.topBar = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.guna2VSeparator1 = new Guna.UI2.WinForms.Guna2VSeparator();
            this.guna2HtmlLabel3 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.triageHistoryBtn = new Guna.UI2.WinForms.Guna2Button();
            this.doctorsBtn = new Guna.UI2.WinForms.Guna2Button();
            this.patientQueueBtn = new Guna.UI2.WinForms.Guna2Button();
            this.searchPatientBtn = new Guna.UI2.WinForms.Guna2Button();
            this.dashboardBtn = new Guna.UI2.WinForms.Guna2Button();
            this.guna2CirclePictureBox1 = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.sideBar.SuspendLayout();
            this.topBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // sideBar
            // 
            this.sideBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(110)))), ((int)(((byte)(164)))));
            this.sideBar.Controls.Add(this.triageHistoryBtn);
            this.sideBar.Controls.Add(this.doctorsBtn);
            this.sideBar.Controls.Add(this.patientQueueBtn);
            this.sideBar.Controls.Add(this.searchPatientBtn);
            this.sideBar.Controls.Add(this.img_slide);
            this.sideBar.Controls.Add(this.dashboardBtn);
            this.sideBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.sideBar.Location = new System.Drawing.Point(0, 110);
            this.sideBar.Name = "sideBar";
            this.sideBar.Size = new System.Drawing.Size(252, 690);
            this.sideBar.TabIndex = 1;
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.mainPanel.CustomizableEdges.BottomLeft = false;
            this.mainPanel.CustomizableEdges.TopRight = false;
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(252, 110);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.ShadowDecoration.CustomizableEdges.TopLeft = false;
            this.mainPanel.ShadowDecoration.CustomizableEdges.TopRight = false;
            this.mainPanel.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.mainPanel.Size = new System.Drawing.Size(1028, 690);
            this.mainPanel.TabIndex = 2;
            // 
            // img_slide
            // 
            this.img_slide.BorderRadius = 9;
            this.img_slide.CustomizableEdges.BottomLeft = false;
            this.img_slide.CustomizableEdges.TopLeft = false;
            this.img_slide.FillColor = System.Drawing.Color.White;
            this.img_slide.Location = new System.Drawing.Point(0, 32);
            this.img_slide.Name = "img_slide";
            this.img_slide.Size = new System.Drawing.Size(10, 50);
            this.img_slide.TabIndex = 0;
            // 
            // guna2ControlBox1
            // 
            this.guna2ControlBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBox1.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;
            this.guna2ControlBox1.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox1.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox1.Location = new System.Drawing.Point(1130, 2);
            this.guna2ControlBox1.Name = "guna2ControlBox1";
            this.guna2ControlBox1.Size = new System.Drawing.Size(45, 41);
            this.guna2ControlBox1.TabIndex = 0;
            // 
            // guna2ControlBox2
            // 
            this.guna2ControlBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBox2.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MaximizeBox;
            this.guna2ControlBox2.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox2.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox2.Location = new System.Drawing.Point(1181, 2);
            this.guna2ControlBox2.Name = "guna2ControlBox2";
            this.guna2ControlBox2.Size = new System.Drawing.Size(45, 41);
            this.guna2ControlBox2.TabIndex = 1;
            // 
            // guna2ControlBox3
            // 
            this.guna2ControlBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBox3.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox3.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox3.Location = new System.Drawing.Point(1232, 2);
            this.guna2ControlBox3.Name = "guna2ControlBox3";
            this.guna2ControlBox3.Size = new System.Drawing.Size(45, 41);
            this.guna2ControlBox3.TabIndex = 2;
            // 
            // topBar
            // 
            this.topBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(171)))), ((int)(((byte)(239)))));
            this.topBar.Controls.Add(this.guna2HtmlLabel3);
            this.topBar.Controls.Add(this.guna2VSeparator1);
            this.topBar.Controls.Add(this.guna2HtmlLabel2);
            this.topBar.Controls.Add(this.guna2HtmlLabel1);
            this.topBar.Controls.Add(this.guna2CirclePictureBox1);
            this.topBar.Controls.Add(this.guna2ControlBox3);
            this.topBar.Controls.Add(this.guna2ControlBox2);
            this.topBar.Controls.Add(this.guna2ControlBox1);
            this.topBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.topBar.Location = new System.Drawing.Point(0, 0);
            this.topBar.Name = "topBar";
            this.topBar.Size = new System.Drawing.Size(1280, 110);
            this.topBar.TabIndex = 0;
            // 
            // guna2HtmlLabel1
            // 
            this.guna2HtmlLabel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel1.Font = new System.Drawing.Font("Poppins", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel1.ForeColor = System.Drawing.Color.White;
            this.guna2HtmlLabel1.Location = new System.Drawing.Point(84, 30);
            this.guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            this.guna2HtmlLabel1.Size = new System.Drawing.Size(149, 44);
            this.guna2HtmlLabel1.TabIndex = 4;
            this.guna2HtmlLabel1.Text = "North Metro";
            // 
            // guna2HtmlLabel2
            // 
            this.guna2HtmlLabel2.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel2.Font = new System.Drawing.Font("Poppins Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel2.ForeColor = System.Drawing.Color.White;
            this.guna2HtmlLabel2.Location = new System.Drawing.Point(82, 61);
            this.guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            this.guna2HtmlLabel2.Size = new System.Drawing.Size(151, 30);
            this.guna2HtmlLabel2.TabIndex = 5;
            this.guna2HtmlLabel2.Text = "General Hospital";
            // 
            // guna2BorderlessForm1
            // 
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // guna2VSeparator1
            // 
            this.guna2VSeparator1.BackColor = System.Drawing.Color.Transparent;
            this.guna2VSeparator1.FillColor = System.Drawing.Color.White;
            this.guna2VSeparator1.FillThickness = 2;
            this.guna2VSeparator1.Location = new System.Drawing.Point(249, 30);
            this.guna2VSeparator1.Name = "guna2VSeparator1";
            this.guna2VSeparator1.Size = new System.Drawing.Size(8, 63);
            this.guna2VSeparator1.TabIndex = 6;
            this.guna2VSeparator1.Click += new System.EventHandler(this.guna2VSeparator1_Click);
            // 
            // guna2HtmlLabel3
            // 
            this.guna2HtmlLabel3.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel3.Font = new System.Drawing.Font("Poppins", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel3.ForeColor = System.Drawing.Color.White;
            this.guna2HtmlLabel3.Location = new System.Drawing.Point(293, 27);
            this.guna2HtmlLabel3.Name = "guna2HtmlLabel3";
            this.guna2HtmlLabel3.Size = new System.Drawing.Size(254, 64);
            this.guna2HtmlLabel3.TabIndex = 7;
            this.guna2HtmlLabel3.Text = "Triage Station";
            // 
            // triageHistoryBtn
            // 
            this.triageHistoryBtn.BackColor = System.Drawing.Color.Transparent;
            this.triageHistoryBtn.BorderRadius = 13;
            this.triageHistoryBtn.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.triageHistoryBtn.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(180)))), ((int)(((byte)(245)))));
            this.triageHistoryBtn.CustomizableEdges.BottomRight = false;
            this.triageHistoryBtn.CustomizableEdges.TopRight = false;
            this.triageHistoryBtn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.triageHistoryBtn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.triageHistoryBtn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.triageHistoryBtn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.triageHistoryBtn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(110)))), ((int)(((byte)(164)))));
            this.triageHistoryBtn.Font = new System.Drawing.Font("Poppins", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.triageHistoryBtn.ForeColor = System.Drawing.Color.White;
            this.triageHistoryBtn.Image = global::Triage_System.Properties.Resources.Triage_History;
            this.triageHistoryBtn.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.triageHistoryBtn.ImageOffset = new System.Drawing.Point(10, 0);
            this.triageHistoryBtn.ImageSize = new System.Drawing.Size(30, 30);
            this.triageHistoryBtn.Location = new System.Drawing.Point(25, 317);
            this.triageHistoryBtn.Name = "triageHistoryBtn";
            this.triageHistoryBtn.ShadowDecoration.BorderRadius = 15;
            this.triageHistoryBtn.ShadowDecoration.Depth = 15;
            this.triageHistoryBtn.ShadowDecoration.Enabled = true;
            this.triageHistoryBtn.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.triageHistoryBtn.Size = new System.Drawing.Size(243, 50);
            this.triageHistoryBtn.TabIndex = 4;
            this.triageHistoryBtn.Text = "Triage History";
            this.triageHistoryBtn.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.triageHistoryBtn.TextOffset = new System.Drawing.Point(15, 0);
            this.triageHistoryBtn.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.triageHistoryBtn.Click += new System.EventHandler(this.triageBtn_Click);
            // 
            // doctorsBtn
            // 
            this.doctorsBtn.BackColor = System.Drawing.Color.Transparent;
            this.doctorsBtn.BorderRadius = 13;
            this.doctorsBtn.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.doctorsBtn.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(180)))), ((int)(((byte)(245)))));
            this.doctorsBtn.CustomizableEdges.BottomRight = false;
            this.doctorsBtn.CustomizableEdges.TopRight = false;
            this.doctorsBtn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.doctorsBtn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.doctorsBtn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.doctorsBtn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.doctorsBtn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(110)))), ((int)(((byte)(164)))));
            this.doctorsBtn.Font = new System.Drawing.Font("Poppins", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.doctorsBtn.ForeColor = System.Drawing.Color.White;
            this.doctorsBtn.Image = global::Triage_System.Properties.Resources.Doctors_on_Duty;
            this.doctorsBtn.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.doctorsBtn.ImageOffset = new System.Drawing.Point(10, 0);
            this.doctorsBtn.ImageSize = new System.Drawing.Size(30, 30);
            this.doctorsBtn.Location = new System.Drawing.Point(25, 245);
            this.doctorsBtn.Name = "doctorsBtn";
            this.doctorsBtn.ShadowDecoration.BorderRadius = 15;
            this.doctorsBtn.ShadowDecoration.Depth = 15;
            this.doctorsBtn.ShadowDecoration.Enabled = true;
            this.doctorsBtn.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.doctorsBtn.Size = new System.Drawing.Size(243, 50);
            this.doctorsBtn.TabIndex = 3;
            this.doctorsBtn.Text = "Doctors";
            this.doctorsBtn.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.doctorsBtn.TextOffset = new System.Drawing.Point(15, 0);
            this.doctorsBtn.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.doctorsBtn.Click += new System.EventHandler(this.doctorBtn_Click);
            // 
            // patientQueueBtn
            // 
            this.patientQueueBtn.BackColor = System.Drawing.Color.Transparent;
            this.patientQueueBtn.BorderRadius = 13;
            this.patientQueueBtn.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.patientQueueBtn.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(180)))), ((int)(((byte)(245)))));
            this.patientQueueBtn.CustomizableEdges.BottomRight = false;
            this.patientQueueBtn.CustomizableEdges.TopRight = false;
            this.patientQueueBtn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.patientQueueBtn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.patientQueueBtn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.patientQueueBtn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.patientQueueBtn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(110)))), ((int)(((byte)(164)))));
            this.patientQueueBtn.Font = new System.Drawing.Font("Poppins", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.patientQueueBtn.ForeColor = System.Drawing.Color.White;
            this.patientQueueBtn.Image = global::Triage_System.Properties.Resources.Patient_Queue;
            this.patientQueueBtn.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.patientQueueBtn.ImageOffset = new System.Drawing.Point(10, 0);
            this.patientQueueBtn.ImageSize = new System.Drawing.Size(30, 30);
            this.patientQueueBtn.Location = new System.Drawing.Point(25, 173);
            this.patientQueueBtn.Name = "patientQueueBtn";
            this.patientQueueBtn.ShadowDecoration.BorderRadius = 15;
            this.patientQueueBtn.ShadowDecoration.Depth = 15;
            this.patientQueueBtn.ShadowDecoration.Enabled = true;
            this.patientQueueBtn.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.patientQueueBtn.Size = new System.Drawing.Size(243, 50);
            this.patientQueueBtn.TabIndex = 2;
            this.patientQueueBtn.Text = "Patient Queue";
            this.patientQueueBtn.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.patientQueueBtn.TextOffset = new System.Drawing.Point(15, 0);
            this.patientQueueBtn.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.patientQueueBtn.Click += new System.EventHandler(this.patientqueueBtn_Click);
            // 
            // searchPatientBtn
            // 
            this.searchPatientBtn.BackColor = System.Drawing.Color.Transparent;
            this.searchPatientBtn.BorderRadius = 13;
            this.searchPatientBtn.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.searchPatientBtn.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(180)))), ((int)(((byte)(245)))));
            this.searchPatientBtn.CustomizableEdges.BottomRight = false;
            this.searchPatientBtn.CustomizableEdges.TopRight = false;
            this.searchPatientBtn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.searchPatientBtn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.searchPatientBtn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.searchPatientBtn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.searchPatientBtn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(110)))), ((int)(((byte)(164)))));
            this.searchPatientBtn.Font = new System.Drawing.Font("Poppins", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchPatientBtn.ForeColor = System.Drawing.Color.White;
            this.searchPatientBtn.Image = global::Triage_System.Properties.Resources.Search_Patient;
            this.searchPatientBtn.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.searchPatientBtn.ImageOffset = new System.Drawing.Point(10, 0);
            this.searchPatientBtn.ImageSize = new System.Drawing.Size(30, 30);
            this.searchPatientBtn.Location = new System.Drawing.Point(25, 103);
            this.searchPatientBtn.Name = "searchPatientBtn";
            this.searchPatientBtn.ShadowDecoration.BorderRadius = 15;
            this.searchPatientBtn.ShadowDecoration.Depth = 15;
            this.searchPatientBtn.ShadowDecoration.Enabled = true;
            this.searchPatientBtn.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.searchPatientBtn.Size = new System.Drawing.Size(243, 50);
            this.searchPatientBtn.TabIndex = 1;
            this.searchPatientBtn.Text = "Search Patient";
            this.searchPatientBtn.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.searchPatientBtn.TextOffset = new System.Drawing.Point(15, 0);
            this.searchPatientBtn.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.searchPatientBtn.Click += new System.EventHandler(this.searchpatientBtn_Click);
            // 
            // dashboardBtn
            // 
            this.dashboardBtn.BackColor = System.Drawing.Color.Transparent;
            this.dashboardBtn.BorderRadius = 13;
            this.dashboardBtn.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.dashboardBtn.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(180)))), ((int)(((byte)(245)))));
            this.dashboardBtn.CustomizableEdges.BottomRight = false;
            this.dashboardBtn.CustomizableEdges.TopRight = false;
            this.dashboardBtn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.dashboardBtn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.dashboardBtn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.dashboardBtn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.dashboardBtn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(110)))), ((int)(((byte)(164)))));
            this.dashboardBtn.Font = new System.Drawing.Font("Poppins", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dashboardBtn.ForeColor = System.Drawing.Color.White;
            this.dashboardBtn.Image = global::Triage_System.Properties.Resources.Dashboard;
            this.dashboardBtn.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.dashboardBtn.ImageOffset = new System.Drawing.Point(10, 0);
            this.dashboardBtn.ImageSize = new System.Drawing.Size(30, 30);
            this.dashboardBtn.Location = new System.Drawing.Point(25, 32);
            this.dashboardBtn.Name = "dashboardBtn";
            this.dashboardBtn.ShadowDecoration.BorderRadius = 15;
            this.dashboardBtn.ShadowDecoration.Depth = 15;
            this.dashboardBtn.ShadowDecoration.Enabled = true;
            this.dashboardBtn.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.dashboardBtn.Size = new System.Drawing.Size(243, 50);
            this.dashboardBtn.TabIndex = 0;
            this.dashboardBtn.Text = "Dashboard";
            this.dashboardBtn.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.dashboardBtn.TextOffset = new System.Drawing.Point(15, 0);
            this.dashboardBtn.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.dashboardBtn.Click += new System.EventHandler(this.dashboardBtn_Click);
            // 
            // guna2CirclePictureBox1
            // 
            this.guna2CirclePictureBox1.FillColor = System.Drawing.Color.Transparent;
            this.guna2CirclePictureBox1.Image = global::Triage_System.Properties.Resources.Triage_History1;
            this.guna2CirclePictureBox1.ImageRotate = 0F;
            this.guna2CirclePictureBox1.Location = new System.Drawing.Point(12, 27);
            this.guna2CirclePictureBox1.Name = "guna2CirclePictureBox1";
            this.guna2CirclePictureBox1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CirclePictureBox1.Size = new System.Drawing.Size(64, 64);
            this.guna2CirclePictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.guna2CirclePictureBox1.TabIndex = 3;
            this.guna2CirclePictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 800);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.sideBar);
            this.Controls.Add(this.topBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.sideBar.ResumeLayout(false);
            this.topBar.ResumeLayout(false);
            this.topBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Guna.UI2.WinForms.Guna2Panel sideBar;
        private Guna.UI2.WinForms.Guna2Panel mainPanel;
        private Guna.UI2.WinForms.Guna2Button dashboardBtn;
        private Guna.UI2.WinForms.Guna2Button triageHistoryBtn;
        private Guna.UI2.WinForms.Guna2Button doctorsBtn;
        private Guna.UI2.WinForms.Guna2Button patientQueueBtn;
        private Guna.UI2.WinForms.Guna2Button searchPatientBtn;
        private Guna.UI2.WinForms.Guna2Panel img_slide;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox1;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox2;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox3;
        private Guna.UI2.WinForms.Guna2Panel topBar;
        private Guna.UI2.WinForms.Guna2CirclePictureBox guna2CirclePictureBox1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Guna.UI2.WinForms.Guna2VSeparator guna2VSeparator1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel3;
    }
}

