
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
            this.btnStop = new System.Windows.Forms.Button();
            this.numDeliveryTrucks = new System.Windows.Forms.NumericUpDown();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnGetLast = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtCmds = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDeliveryTrucks)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(16, 92);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(58, 23);
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
            this.groupBox1.Controls.Add(this.btnStop);
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.numDeliveryTrucks);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(169, 134);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Task Manager";
            // 
            // txtInterval
            // 
            this.txtInterval.Location = new System.Drawing.Point(104, 58);
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
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "# Transactions";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 15);
            this.label5.TabIndex = 1;
            this.label5.Text = "Interval";
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(97, 92);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(59, 23);
            this.btnStop.TabIndex = 0;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // numDeliveryTrucks
            // 
            this.numDeliveryTrucks.Location = new System.Drawing.Point(104, 26);
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
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(12, 161);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(1059, 212);
            this.txtLog.TabIndex = 3;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnGetLast);
            this.groupBox3.Controls.Add(this.btnSend);
            this.groupBox3.Controls.Add(this.txtCmds);
            this.groupBox3.Location = new System.Drawing.Point(196, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(875, 134);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Send";
            // 
            // btnGetLast
            // 
            this.btnGetLast.Location = new System.Drawing.Point(819, 22);
            this.btnGetLast.Name = "btnGetLast";
            this.btnGetLast.Size = new System.Drawing.Size(50, 39);
            this.btnGetLast.TabIndex = 2;
            this.btnGetLast.Text = "Get Last";
            this.btnGetLast.UseVisualStyleBackColor = true;
            this.btnGetLast.Click += new System.EventHandler(this.btnGetLast_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(819, 76);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(50, 39);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtCmds
            // 
            this.txtCmds.Location = new System.Drawing.Point(16, 22);
            this.txtCmds.Multiline = true;
            this.txtCmds.Name = "txtCmds";
            this.txtCmds.Size = new System.Drawing.Size(797, 93);
            this.txtCmds.TabIndex = 0;
            this.txtCmds.Text = "{\"transaction\":\"431\",\"number\":\"0015-0000-0000-0000\",\"lat\":5.1618,\"lng\":0.47201,\"a" +
    "mount\":91.01487,\"category\":\"lazer\",\"eventTime\":\"2021-01-05T19:07:19.3888\"}";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1083, 385);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDeliveryTrucks)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numDeliveryTrucks;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.MaskedTextBox txtInterval;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtCmds;
        private System.Windows.Forms.Button btnGetLast;
    }
}

