namespace HaptiQ_API
{
    /// <summary>
    /// This class defines the layout of the configuration form
    /// </summary>
    partial class ConfigurationForm
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

        private void InitialiseActuatorsCheckBoxes()
        {
            // TODO
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// TODO - refactor in separate methods
        /// </summary>
        private void InitializeComponent()
        {
            this.Actuator1 = new System.Windows.Forms.CheckBox();
            this.Actuator1MIN = new System.Windows.Forms.TrackBar();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.ServoBoardsComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.InterfaceKitsComboBox = new System.Windows.Forms.ComboBox();
            this.configurationName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.InputIdentifiersComboBox = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.Actuator1MAX = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.Actuator2MAX = new System.Windows.Forms.TrackBar();
            this.Actuator2MIN = new System.Windows.Forms.TrackBar();
            this.Actuator2 = new System.Windows.Forms.CheckBox();
            this.progressBar3 = new System.Windows.Forms.ProgressBar();
            this.Actuator3MAX = new System.Windows.Forms.TrackBar();
            this.Actuator3MIN = new System.Windows.Forms.TrackBar();
            this.Actuator3 = new System.Windows.Forms.CheckBox();
            this.progressBar4 = new System.Windows.Forms.ProgressBar();
            this.Actuator4MAX = new System.Windows.Forms.TrackBar();
            this.Actuator4MIN = new System.Windows.Forms.TrackBar();
            this.Actuator4 = new System.Windows.Forms.CheckBox();
            this.progressBar5 = new System.Windows.Forms.ProgressBar();
            this.Actuator8MAX = new System.Windows.Forms.TrackBar();
            this.Actuator8MIN = new System.Windows.Forms.TrackBar();
            this.Actuator8 = new System.Windows.Forms.CheckBox();
            this.progressBar6 = new System.Windows.Forms.ProgressBar();
            this.Actuator7MAX = new System.Windows.Forms.TrackBar();
            this.Actuator7MIN = new System.Windows.Forms.TrackBar();
            this.Actuator7 = new System.Windows.Forms.CheckBox();
            this.progressBar7 = new System.Windows.Forms.ProgressBar();
            this.Actuator6MAX = new System.Windows.Forms.TrackBar();
            this.Actuator6MIN = new System.Windows.Forms.TrackBar();
            this.Actuator6 = new System.Windows.Forms.CheckBox();
            this.progressBar8 = new System.Windows.Forms.ProgressBar();
            this.Actuator5MAX = new System.Windows.Forms.TrackBar();
            this.Actuator5MIN = new System.Windows.Forms.TrackBar();
            this.Actuator5 = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Pressure1 = new System.Windows.Forms.CheckBox();
            this.Pressure2 = new System.Windows.Forms.CheckBox();
            this.Pressure3 = new System.Windows.Forms.CheckBox();
            this.Pressure4 = new System.Windows.Forms.CheckBox();
            this.Pressure5 = new System.Windows.Forms.CheckBox();
            this.Pressure6 = new System.Windows.Forms.CheckBox();
            this.Pressure7 = new System.Windows.Forms.CheckBox();
            this.Pressure8 = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator1MIN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator1MAX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator2MAX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator2MIN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator3MAX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator3MIN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator4MAX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator4MIN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator8MAX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator8MIN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator7MAX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator7MIN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator6MAX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator6MIN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator5MAX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator5MIN)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Actuator1
            // 
            this.Actuator1.AutoSize = true;
            this.Actuator1.Location = new System.Drawing.Point(12, 43);
            this.Actuator1.Name = "Actuator1";
            this.Actuator1.Size = new System.Drawing.Size(39, 17);
            this.Actuator1.TabIndex = 0;
            this.Actuator1.Text = "#1";
            this.Actuator1.UseVisualStyleBackColor = true;
            this.Actuator1.CheckedChanged += new System.EventHandler(this.Actuator1_CheckedChanged);
            // 
            // Actuator1MIN
            // 
            this.Actuator1MIN.Location = new System.Drawing.Point(57, 37);
            this.Actuator1MIN.Maximum = 220;
            this.Actuator1MIN.Name = "Actuator1MIN";
            this.Actuator1MIN.Size = new System.Drawing.Size(131, 42);
            this.Actuator1MIN.TabIndex = 3;
            this.Actuator1MIN.Scroll += new System.EventHandler(this.Actuator1MIN_Scroll);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(432, 436);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.saveButtonClick);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(542, 436);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 12;
            this.button2.Text = "Reset";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // ServoBoardsComboBox
            // 
            this.ServoBoardsComboBox.FormattingEnabled = true;
            this.ServoBoardsComboBox.Location = new System.Drawing.Point(490, 39);
            this.ServoBoardsComboBox.Name = "ServoBoardsComboBox";
            this.ServoBoardsComboBox.Size = new System.Drawing.Size(121, 21);
            this.ServoBoardsComboBox.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(400, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Servos Board";
            // 
            // InterfaceKitsComboBox
            // 
            this.InterfaceKitsComboBox.FormattingEnabled = true;
            this.InterfaceKitsComboBox.Location = new System.Drawing.Point(490, 85);
            this.InterfaceKitsComboBox.Name = "InterfaceKitsComboBox";
            this.InterfaceKitsComboBox.Size = new System.Drawing.Size(121, 21);
            this.InterfaceKitsComboBox.TabIndex = 16;
            // 
            // configurationName
            // 
            this.configurationName.Location = new System.Drawing.Point(414, 5);
            this.configurationName.Name = "configurationName";
            this.configurationName.Size = new System.Drawing.Size(203, 20);
            this.configurationName.TabIndex = 23;
            this.configurationName.Text = "Configuration Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(400, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Pressure Board";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Input Identifier";
            // 
            // InputIdentifiersComboBox
            // 
            this.InputIdentifiersComboBox.FormattingEnabled = true;
            this.InputIdentifiersComboBox.Location = new System.Drawing.Point(102, 11);
            this.InputIdentifiersComboBox.Name = "InputIdentifiersComboBox";
            this.InputIdentifiersComboBox.Size = new System.Drawing.Size(121, 21);
            this.InputIdentifiersComboBox.TabIndex = 25;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(15, 51);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(76, 41);
            this.button3.TabIndex = 26;
            this.button3.Text = "Get Input Identifier";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.acquireInputIdentifierClick);
            // 
            // Actuator1MAX
            // 
            this.Actuator1MAX.Location = new System.Drawing.Point(184, 37);
            this.Actuator1MAX.Maximum = 220;
            this.Actuator1MAX.Name = "Actuator1MAX";
            this.Actuator1MAX.Size = new System.Drawing.Size(131, 42);
            this.Actuator1MAX.TabIndex = 27;
            this.Actuator1MAX.Scroll += new System.EventHandler(this.Actuator1MAX_Scroll);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(107, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 42;
            this.label4.Text = "MIN POSITION";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(231, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 43;
            this.label5.Text = "MAX POSITION";
            // 
            // progressBar1
            // 
            this.progressBar1.ForeColor = System.Drawing.Color.Firebrick;
            this.progressBar1.Location = new System.Drawing.Point(68, 71);
            this.progressBar1.Maximum = 1000;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(247, 16);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 44;
            // 
            // progressBar2
            // 
            this.progressBar2.ForeColor = System.Drawing.Color.Firebrick;
            this.progressBar2.Location = new System.Drawing.Point(68, 127);
            this.progressBar2.Maximum = 1000;
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(247, 16);
            this.progressBar2.Step = 1;
            this.progressBar2.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar2.TabIndex = 48;
            // 
            // Actuator2MAX
            // 
            this.Actuator2MAX.Location = new System.Drawing.Point(184, 93);
            this.Actuator2MAX.Maximum = 220;
            this.Actuator2MAX.Name = "Actuator2MAX";
            this.Actuator2MAX.Size = new System.Drawing.Size(131, 42);
            this.Actuator2MAX.TabIndex = 47;
            this.Actuator2MAX.Scroll += new System.EventHandler(this.Actuator2MAX_Scroll);
            // 
            // Actuator2MIN
            // 
            this.Actuator2MIN.Location = new System.Drawing.Point(57, 93);
            this.Actuator2MIN.Maximum = 220;
            this.Actuator2MIN.Name = "Actuator2MIN";
            this.Actuator2MIN.Size = new System.Drawing.Size(131, 42);
            this.Actuator2MIN.TabIndex = 46;
            this.Actuator2MIN.Scroll += new System.EventHandler(this.Actuator2MIN_Scroll);
            // 
            // Actuator2
            // 
            this.Actuator2.AutoSize = true;
            this.Actuator2.Location = new System.Drawing.Point(12, 99);
            this.Actuator2.Name = "Actuator2";
            this.Actuator2.Size = new System.Drawing.Size(39, 17);
            this.Actuator2.TabIndex = 45;
            this.Actuator2.Text = "#2";
            this.Actuator2.UseVisualStyleBackColor = true;
            this.Actuator2.CheckedChanged += new System.EventHandler(this.Actuator2_CheckedChanged);
            // 
            // progressBar3
            // 
            this.progressBar3.ForeColor = System.Drawing.Color.Firebrick;
            this.progressBar3.Location = new System.Drawing.Point(68, 183);
            this.progressBar3.Maximum = 1000;
            this.progressBar3.Name = "progressBar3";
            this.progressBar3.Size = new System.Drawing.Size(247, 16);
            this.progressBar3.Step = 1;
            this.progressBar3.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar3.TabIndex = 52;
            // 
            // Actuator3MAX
            // 
            this.Actuator3MAX.Location = new System.Drawing.Point(184, 149);
            this.Actuator3MAX.Maximum = 220;
            this.Actuator3MAX.Name = "Actuator3MAX";
            this.Actuator3MAX.Size = new System.Drawing.Size(131, 42);
            this.Actuator3MAX.TabIndex = 51;
            this.Actuator3MAX.Scroll += new System.EventHandler(this.Actuator3MAX_Scroll);
            // 
            // Actuator3MIN
            // 
            this.Actuator3MIN.Location = new System.Drawing.Point(57, 149);
            this.Actuator3MIN.Maximum = 220;
            this.Actuator3MIN.Name = "Actuator3MIN";
            this.Actuator3MIN.Size = new System.Drawing.Size(131, 42);
            this.Actuator3MIN.TabIndex = 50;
            this.Actuator3MIN.Scroll += new System.EventHandler(this.Actuator3MIN_Scroll);
            // 
            // Actuator3
            // 
            this.Actuator3.AutoSize = true;
            this.Actuator3.Location = new System.Drawing.Point(12, 155);
            this.Actuator3.Name = "Actuator3";
            this.Actuator3.Size = new System.Drawing.Size(39, 17);
            this.Actuator3.TabIndex = 49;
            this.Actuator3.Text = "#3";
            this.Actuator3.UseVisualStyleBackColor = true;
            this.Actuator3.CheckedChanged += new System.EventHandler(this.Actuator3_CheckedChanged);
            // 
            // progressBar4
            // 
            this.progressBar4.ForeColor = System.Drawing.Color.Firebrick;
            this.progressBar4.Location = new System.Drawing.Point(68, 239);
            this.progressBar4.Maximum = 1000;
            this.progressBar4.Name = "progressBar4";
            this.progressBar4.Size = new System.Drawing.Size(247, 16);
            this.progressBar4.Step = 1;
            this.progressBar4.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar4.TabIndex = 56;
            // 
            // Actuator4MAX
            // 
            this.Actuator4MAX.Location = new System.Drawing.Point(184, 205);
            this.Actuator4MAX.Maximum = 220;
            this.Actuator4MAX.Name = "Actuator4MAX";
            this.Actuator4MAX.Size = new System.Drawing.Size(131, 42);
            this.Actuator4MAX.TabIndex = 55;
            this.Actuator4MAX.Scroll += new System.EventHandler(this.Actuator4MAX_Scroll);
            // 
            // Actuator4MIN
            // 
            this.Actuator4MIN.Location = new System.Drawing.Point(57, 205);
            this.Actuator4MIN.Maximum = 220;
            this.Actuator4MIN.Name = "Actuator4MIN";
            this.Actuator4MIN.Size = new System.Drawing.Size(131, 42);
            this.Actuator4MIN.TabIndex = 54;
            this.Actuator4MIN.Scroll += new System.EventHandler(this.Actuator4MIN_Scroll);
            // 
            // Actuator4
            // 
            this.Actuator4.AutoSize = true;
            this.Actuator4.Location = new System.Drawing.Point(12, 211);
            this.Actuator4.Name = "Actuator4";
            this.Actuator4.Size = new System.Drawing.Size(39, 17);
            this.Actuator4.TabIndex = 53;
            this.Actuator4.Text = "#4";
            this.Actuator4.UseVisualStyleBackColor = true;
            this.Actuator4.CheckedChanged += new System.EventHandler(this.Actuator4_CheckedChanged);
            // 
            // progressBar5
            // 
            this.progressBar5.ForeColor = System.Drawing.Color.Firebrick;
            this.progressBar5.Location = new System.Drawing.Point(68, 302);
            this.progressBar5.Maximum = 1000;
            this.progressBar5.Name = "progressBar5";
            this.progressBar5.Size = new System.Drawing.Size(247, 16);
            this.progressBar5.Step = 1;
            this.progressBar5.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar5.TabIndex = 72;
            // 
            // Actuator8MAX
            // 
            this.Actuator8MAX.Location = new System.Drawing.Point(184, 436);
            this.Actuator8MAX.Maximum = 220;
            this.Actuator8MAX.Name = "Actuator8MAX";
            this.Actuator8MAX.Size = new System.Drawing.Size(131, 42);
            this.Actuator8MAX.TabIndex = 71;
            this.Actuator8MAX.Scroll += new System.EventHandler(this.Actuator8MAX_Scroll);
            // 
            // Actuator8MIN
            // 
            this.Actuator8MIN.Location = new System.Drawing.Point(57, 436);
            this.Actuator8MIN.Maximum = 220;
            this.Actuator8MIN.Name = "Actuator8MIN";
            this.Actuator8MIN.Size = new System.Drawing.Size(131, 42);
            this.Actuator8MIN.TabIndex = 70;
            this.Actuator8MIN.Scroll += new System.EventHandler(this.Actuator8MIN_Scroll);
            // 
            // Actuator8
            // 
            this.Actuator8.AutoSize = true;
            this.Actuator8.Location = new System.Drawing.Point(12, 442);
            this.Actuator8.Name = "Actuator8";
            this.Actuator8.Size = new System.Drawing.Size(39, 17);
            this.Actuator8.TabIndex = 69;
            this.Actuator8.Text = "#8";
            this.Actuator8.UseVisualStyleBackColor = true;
            this.Actuator8.CheckedChanged += new System.EventHandler(this.Actuator8_CheckedChanged);
            // 
            // progressBar6
            // 
            this.progressBar6.ForeColor = System.Drawing.Color.Firebrick;
            this.progressBar6.Location = new System.Drawing.Point(68, 358);
            this.progressBar6.Maximum = 1000;
            this.progressBar6.Name = "progressBar6";
            this.progressBar6.Size = new System.Drawing.Size(247, 16);
            this.progressBar6.Step = 1;
            this.progressBar6.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar6.TabIndex = 68;
            // 
            // Actuator7MAX
            // 
            this.Actuator7MAX.Location = new System.Drawing.Point(184, 380);
            this.Actuator7MAX.Maximum = 220;
            this.Actuator7MAX.Name = "Actuator7MAX";
            this.Actuator7MAX.Size = new System.Drawing.Size(131, 42);
            this.Actuator7MAX.TabIndex = 67;
            this.Actuator7MAX.Scroll += new System.EventHandler(this.Actuator7MAX_Scroll);
            // 
            // Actuator7MIN
            // 
            this.Actuator7MIN.Location = new System.Drawing.Point(57, 380);
            this.Actuator7MIN.Maximum = 220;
            this.Actuator7MIN.Name = "Actuator7MIN";
            this.Actuator7MIN.Size = new System.Drawing.Size(131, 42);
            this.Actuator7MIN.TabIndex = 66;
            this.Actuator7MIN.Scroll += new System.EventHandler(this.Actuator7MIN_Scroll);
            // 
            // Actuator7
            // 
            this.Actuator7.AutoSize = true;
            this.Actuator7.Location = new System.Drawing.Point(12, 386);
            this.Actuator7.Name = "Actuator7";
            this.Actuator7.Size = new System.Drawing.Size(39, 17);
            this.Actuator7.TabIndex = 65;
            this.Actuator7.Text = "#7";
            this.Actuator7.UseVisualStyleBackColor = true;
            this.Actuator7.CheckedChanged += new System.EventHandler(this.Actuator7_CheckedChanged);
            // 
            // progressBar7
            // 
            this.progressBar7.ForeColor = System.Drawing.Color.Firebrick;
            this.progressBar7.Location = new System.Drawing.Point(68, 414);
            this.progressBar7.Maximum = 1000;
            this.progressBar7.Name = "progressBar7";
            this.progressBar7.Size = new System.Drawing.Size(247, 16);
            this.progressBar7.Step = 1;
            this.progressBar7.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar7.TabIndex = 64;
            // 
            // Actuator6MAX
            // 
            this.Actuator6MAX.Location = new System.Drawing.Point(184, 324);
            this.Actuator6MAX.Maximum = 220;
            this.Actuator6MAX.Name = "Actuator6MAX";
            this.Actuator6MAX.Size = new System.Drawing.Size(131, 42);
            this.Actuator6MAX.TabIndex = 63;
            this.Actuator6MAX.Scroll += new System.EventHandler(this.Actuator6MAX_Scroll);
            // 
            // Actuator6MIN
            // 
            this.Actuator6MIN.Location = new System.Drawing.Point(57, 324);
            this.Actuator6MIN.Maximum = 220;
            this.Actuator6MIN.Name = "Actuator6MIN";
            this.Actuator6MIN.Size = new System.Drawing.Size(131, 42);
            this.Actuator6MIN.TabIndex = 62;
            this.Actuator6MIN.Scroll += new System.EventHandler(this.Actuator6MIN_Scroll);
            // 
            // Actuator6
            // 
            this.Actuator6.AutoSize = true;
            this.Actuator6.Location = new System.Drawing.Point(12, 330);
            this.Actuator6.Name = "Actuator6";
            this.Actuator6.Size = new System.Drawing.Size(39, 17);
            this.Actuator6.TabIndex = 61;
            this.Actuator6.Text = "#6";
            this.Actuator6.UseVisualStyleBackColor = true;
            this.Actuator6.CheckedChanged += new System.EventHandler(this.Actuator6_CheckedChanged);
            // 
            // progressBar8
            // 
            this.progressBar8.ForeColor = System.Drawing.Color.Firebrick;
            this.progressBar8.Location = new System.Drawing.Point(68, 471);
            this.progressBar8.Maximum = 1000;
            this.progressBar8.Name = "progressBar8";
            this.progressBar8.Size = new System.Drawing.Size(247, 16);
            this.progressBar8.Step = 1;
            this.progressBar8.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar8.TabIndex = 60;
            // 
            // Actuator5MAX
            // 
            this.Actuator5MAX.Location = new System.Drawing.Point(184, 268);
            this.Actuator5MAX.Maximum = 220;
            this.Actuator5MAX.Name = "Actuator5MAX";
            this.Actuator5MAX.Size = new System.Drawing.Size(131, 42);
            this.Actuator5MAX.TabIndex = 59;
            this.Actuator5MAX.Scroll += new System.EventHandler(this.Actuator5MAX_Scroll);
            // 
            // Actuator5MIN
            // 
            this.Actuator5MIN.Location = new System.Drawing.Point(57, 268);
            this.Actuator5MIN.Maximum = 220;
            this.Actuator5MIN.Name = "Actuator5MIN";
            this.Actuator5MIN.Size = new System.Drawing.Size(131, 42);
            this.Actuator5MIN.TabIndex = 58;
            this.Actuator5MIN.Scroll += new System.EventHandler(this.Actuator5MIN_Scroll);
            // 
            // Actuator5
            // 
            this.Actuator5.AutoSize = true;
            this.Actuator5.Location = new System.Drawing.Point(12, 274);
            this.Actuator5.Name = "Actuator5";
            this.Actuator5.Size = new System.Drawing.Size(39, 17);
            this.Actuator5.TabIndex = 57;
            this.Actuator5.Text = "#5";
            this.Actuator5.UseVisualStyleBackColor = true;
            this.Actuator5.CheckedChanged += new System.EventHandler(this.Actuator5_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(331, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 73;
            this.label6.Text = "PRESSURE";
            // 
            // Pressure1
            // 
            this.Pressure1.AutoSize = true;
            this.Pressure1.Location = new System.Drawing.Point(334, 47);
            this.Pressure1.Name = "Pressure1";
            this.Pressure1.Size = new System.Drawing.Size(15, 14);
            this.Pressure1.TabIndex = 74;
            this.Pressure1.UseVisualStyleBackColor = true;
            // 
            // Pressure2
            // 
            this.Pressure2.AutoSize = true;
            this.Pressure2.Location = new System.Drawing.Point(334, 103);
            this.Pressure2.Name = "Pressure2";
            this.Pressure2.Size = new System.Drawing.Size(15, 14);
            this.Pressure2.TabIndex = 75;
            this.Pressure2.UseVisualStyleBackColor = true;
            // 
            // Pressure3
            // 
            this.Pressure3.AutoSize = true;
            this.Pressure3.Location = new System.Drawing.Point(334, 159);
            this.Pressure3.Name = "Pressure3";
            this.Pressure3.Size = new System.Drawing.Size(15, 14);
            this.Pressure3.TabIndex = 76;
            this.Pressure3.UseVisualStyleBackColor = true;
            // 
            // Pressure4
            // 
            this.Pressure4.AutoSize = true;
            this.Pressure4.Location = new System.Drawing.Point(334, 215);
            this.Pressure4.Name = "Pressure4";
            this.Pressure4.Size = new System.Drawing.Size(15, 14);
            this.Pressure4.TabIndex = 77;
            this.Pressure4.UseVisualStyleBackColor = true;
            // 
            // Pressure5
            // 
            this.Pressure5.AutoSize = true;
            this.Pressure5.Location = new System.Drawing.Point(334, 278);
            this.Pressure5.Name = "Pressure5";
            this.Pressure5.Size = new System.Drawing.Size(15, 14);
            this.Pressure5.TabIndex = 78;
            this.Pressure5.UseVisualStyleBackColor = true;
            // 
            // Pressure6
            // 
            this.Pressure6.AutoSize = true;
            this.Pressure6.Location = new System.Drawing.Point(334, 334);
            this.Pressure6.Name = "Pressure6";
            this.Pressure6.Size = new System.Drawing.Size(15, 14);
            this.Pressure6.TabIndex = 79;
            this.Pressure6.UseVisualStyleBackColor = true;
            // 
            // Pressure7
            // 
            this.Pressure7.AutoSize = true;
            this.Pressure7.Location = new System.Drawing.Point(334, 390);
            this.Pressure7.Name = "Pressure7";
            this.Pressure7.Size = new System.Drawing.Size(15, 14);
            this.Pressure7.TabIndex = 80;
            this.Pressure7.UseVisualStyleBackColor = true;
            // 
            // Pressure8
            // 
            this.Pressure8.AutoSize = true;
            this.Pressure8.Location = new System.Drawing.Point(334, 446);
            this.Pressure8.Name = "Pressure8";
            this.Pressure8.Size = new System.Drawing.Size(15, 14);
            this.Pressure8.TabIndex = 81;
            this.Pressure8.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.InputIdentifiersComboBox);
            this.panel1.Location = new System.Drawing.Point(388, 138);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(229, 265);
            this.panel1.TabIndex = 82;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(9, 5);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 83;
            this.label7.Text = "ACTUATORS";
            // 
            // ConfigurationForm
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(634, 490);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Pressure8);
            this.Controls.Add(this.progressBar7);
            this.Controls.Add(this.progressBar8);
            this.Controls.Add(this.Pressure7);
            this.Controls.Add(this.Pressure6);
            this.Controls.Add(this.Pressure5);
            this.Controls.Add(this.Pressure4);
            this.Controls.Add(this.Pressure3);
            this.Controls.Add(this.Pressure2);
            this.Controls.Add(this.Pressure1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.progressBar5);
            this.Controls.Add(this.Actuator8MAX);
            this.Controls.Add(this.Actuator8MIN);
            this.Controls.Add(this.Actuator8);
            this.Controls.Add(this.progressBar6);
            this.Controls.Add(this.Actuator7MAX);
            this.Controls.Add(this.Actuator7MIN);
            this.Controls.Add(this.Actuator7);
            this.Controls.Add(this.Actuator6MAX);
            this.Controls.Add(this.Actuator6MIN);
            this.Controls.Add(this.Actuator6);
            this.Controls.Add(this.Actuator5MAX);
            this.Controls.Add(this.Actuator5MIN);
            this.Controls.Add(this.Actuator5);
            this.Controls.Add(this.progressBar4);
            this.Controls.Add(this.Actuator4MAX);
            this.Controls.Add(this.Actuator4MIN);
            this.Controls.Add(this.Actuator4);
            this.Controls.Add(this.progressBar3);
            this.Controls.Add(this.Actuator3MAX);
            this.Controls.Add(this.Actuator3MIN);
            this.Controls.Add(this.Actuator3);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.Actuator2MAX);
            this.Controls.Add(this.Actuator2MIN);
            this.Controls.Add(this.Actuator2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Actuator1MAX);
            this.Controls.Add(this.configurationName);
            this.Controls.Add(this.InterfaceKitsComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ServoBoardsComboBox);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Actuator1MIN);
            this.Controls.Add(this.Actuator1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurationForm";
            this.Opacity = 0.95D;
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HaptiQ Configuration";
            ((System.ComponentModel.ISupportInitialize)(this.Actuator1MIN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator1MAX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator2MAX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator2MIN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator3MAX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator3MIN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator4MAX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator4MIN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator8MAX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator8MIN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator7MAX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator7MIN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator6MAX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator6MIN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator5MAX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Actuator5MIN)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox Actuator1;
        private System.Windows.Forms.TrackBar Actuator1MIN;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox ServoBoardsComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox InterfaceKitsComboBox;
        private System.Windows.Forms.TextBox configurationName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox InputIdentifiersComboBox;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TrackBar Actuator1MAX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.TrackBar Actuator2MAX;
        private System.Windows.Forms.TrackBar Actuator2MIN;
        private System.Windows.Forms.CheckBox Actuator2;
        private System.Windows.Forms.ProgressBar progressBar3;
        private System.Windows.Forms.TrackBar Actuator3MAX;
        private System.Windows.Forms.TrackBar Actuator3MIN;
        private System.Windows.Forms.CheckBox Actuator3;
        private System.Windows.Forms.ProgressBar progressBar4;
        private System.Windows.Forms.TrackBar Actuator4MAX;
        private System.Windows.Forms.TrackBar Actuator4MIN;
        private System.Windows.Forms.CheckBox Actuator4;
        private System.Windows.Forms.ProgressBar progressBar5;
        private System.Windows.Forms.TrackBar Actuator8MAX;
        private System.Windows.Forms.TrackBar Actuator8MIN;
        private System.Windows.Forms.CheckBox Actuator8;
        private System.Windows.Forms.ProgressBar progressBar6;
        private System.Windows.Forms.TrackBar Actuator7MAX;
        private System.Windows.Forms.TrackBar Actuator7MIN;
        private System.Windows.Forms.CheckBox Actuator7;
        private System.Windows.Forms.ProgressBar progressBar7;
        private System.Windows.Forms.TrackBar Actuator6MAX;
        private System.Windows.Forms.TrackBar Actuator6MIN;
        private System.Windows.Forms.CheckBox Actuator6;
        private System.Windows.Forms.ProgressBar progressBar8;
        private System.Windows.Forms.TrackBar Actuator5MAX;
        private System.Windows.Forms.TrackBar Actuator5MIN;
        private System.Windows.Forms.CheckBox Actuator5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox Pressure1;
        private System.Windows.Forms.CheckBox Pressure2;
        private System.Windows.Forms.CheckBox Pressure3;
        private System.Windows.Forms.CheckBox Pressure4;
        private System.Windows.Forms.CheckBox Pressure5;
        private System.Windows.Forms.CheckBox Pressure6;
        private System.Windows.Forms.CheckBox Pressure7;
        private System.Windows.Forms.CheckBox Pressure8;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label7;
    }
}