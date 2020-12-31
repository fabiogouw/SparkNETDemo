
namespace DeliveryDataProducer
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtInterval = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.numDeliveryTrucks = new System.Windows.Forms.NumericUpDown();
            this.numPackages = new System.Windows.Forms.NumericUpDown();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPackages = new System.Windows.Forms.TextBox();
            this.btnUnlose = new System.Windows.Forms.Button();
            this.btnLose = new System.Windows.Forms.Button();
            this.lstPackages = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDeliveryTrucks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPackages)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(220, 26);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtInterval);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnStop);
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.numDeliveryTrucks);
            this.groupBox1.Controls.Add(this.numPackages);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(318, 134);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Task Manager";
            // 
            // txtInterval
            // 
            this.txtInterval.Location = new System.Drawing.Point(131, 95);
            this.txtInterval.Mask = "00:00:00";
            this.txtInterval.Name = "txtInterval";
            this.txtInterval.Size = new System.Drawing.Size(52, 23);
            this.txtInterval.TabIndex = 2;
            this.txtInterval.Text = "000030";
            this.txtInterval.ValidatingType = typeof(System.DateTime);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "# of Delivery Trucks";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 15);
            this.label5.TabIndex = 1;
            this.label5.Text = "Interval";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "# of Packages";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(220, 61);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 0;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // numDeliveryTrucks
            // 
            this.numDeliveryTrucks.Location = new System.Drawing.Point(131, 26);
            this.numDeliveryTrucks.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numDeliveryTrucks.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDeliveryTrucks.Name = "numDeliveryTrucks";
            this.numDeliveryTrucks.Size = new System.Drawing.Size(52, 23);
            this.numDeliveryTrucks.TabIndex = 0;
            this.numDeliveryTrucks.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // numPackages
            // 
            this.numPackages.Location = new System.Drawing.Point(131, 61);
            this.numPackages.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numPackages.Name = "numPackages";
            this.numPackages.Size = new System.Drawing.Size(52, 23);
            this.numPackages.TabIndex = 0;
            this.numPackages.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(12, 277);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(776, 153);
            this.txtLog.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lstPackages);
            this.groupBox2.Controls.Add(this.textBox3);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtPackages);
            this.groupBox2.Controls.Add(this.btnUnlose);
            this.groupBox2.Controls.Add(this.btnLose);
            this.groupBox2.Location = new System.Drawing.Point(351, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(437, 236);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Package manager";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(157, 207);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(100, 23);
            this.textBox3.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "Filter";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 207);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 15);
            this.label4.TabIndex = 1;
            this.label4.Text = "Packages Lost / Total:";
            // 
            // txtPackages
            // 
            this.txtPackages.Location = new System.Drawing.Point(57, 25);
            this.txtPackages.Name = "txtPackages";
            this.txtPackages.Size = new System.Drawing.Size(200, 23);
            this.txtPackages.TabIndex = 2;
            this.txtPackages.TextChanged += new System.EventHandler(this.txtPackages_TextChanged);
            // 
            // btnUnlose
            // 
            this.btnUnlose.Location = new System.Drawing.Point(278, 61);
            this.btnUnlose.Name = "btnUnlose";
            this.btnUnlose.Size = new System.Drawing.Size(144, 23);
            this.btnUnlose.TabIndex = 1;
            this.btnUnlose.Text = "\"Un-lose\" package(s)";
            this.btnUnlose.UseVisualStyleBackColor = true;
            this.btnUnlose.Click += new System.EventHandler(this.btnUnlose_Click);
            // 
            // btnLose
            // 
            this.btnLose.Location = new System.Drawing.Point(278, 24);
            this.btnLose.Name = "btnLose";
            this.btnLose.Size = new System.Drawing.Size(144, 23);
            this.btnLose.TabIndex = 1;
            this.btnLose.Text = "Lose package(s)";
            this.btnLose.UseVisualStyleBackColor = true;
            this.btnLose.Click += new System.EventHandler(this.btnLose_Click);
            // 
            // lstPackages
            // 
            this.lstPackages.FormattingEnabled = true;
            this.lstPackages.ItemHeight = 15;
            this.lstPackages.Location = new System.Drawing.Point(57, 63);
            this.lstPackages.Name = "lstPackages";
            this.lstPackages.Size = new System.Drawing.Size(200, 124);
            this.lstPackages.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 442);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDeliveryTrucks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPackages)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numDeliveryTrucks;
        private System.Windows.Forms.NumericUpDown numPackages;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPackages;
        private System.Windows.Forms.Button btnUnlose;
        private System.Windows.Forms.Button btnLose;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MaskedTextBox txtInterval;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox lstPackages;
    }
}

