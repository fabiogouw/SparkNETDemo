using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeliveryDataProducer
{
    public partial class Form1 : Form
    {
        private Generator _generator;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _generator = new Generator(0, "00:00:00", (r) => AppendLog(r));
        }

        private void btnGetLast_Click(object sender, EventArgs e)
        {
            txtCmds.Text = txtLog.Lines.LastOrDefault(s => !string.IsNullOrWhiteSpace(s));
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            txtLog.Clear();
            _generator = new Generator((int)numDeliveryTrucks.Value, txtInterval.Text, (r) => AppendLog(r));
            _generator.StartNotifyLocationOfAllDevices();
            btnStop.Enabled = true;
        }

        private void AppendLog(string log)
        {
            if(txtLog.Lines.Count() > 1000)
            {
                txtLog.Lines = txtLog.Lines.TakeLast(100).ToArray();
            }
            txtLog.AppendText(log + Environment.NewLine);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            _generator.StopNotifyLocationOfAllDevices();
            btnStart.Enabled = true;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            _generator.SendCommands(txtCmds.Lines);
        }
    }
    // https://stackoverflow.com/questions/15965166/what-is-the-maximum-length-of-latitude-and-longitude
}
