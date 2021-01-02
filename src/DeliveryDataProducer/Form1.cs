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

        private void btnStart_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
            _generator = new Generator((int)numDeliveryTrucks.Value, (int)numPackages.Value, txtInterval.Text, (r) => AppendLog(r));
            _generator.StartNotifyLocationOfAllDevices();
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
            if(_generator != null)
            {
                _generator.StopNotifyLocationOfAllDevices();
            }
        }

        private void btnLose_Click(object sender, EventArgs e)
        {
            if (lstPackages.SelectedItem != null)
            {
                _generator.LostPackages(lstPackages.SelectedItem.ToString());
            }
        }

        private void btnUnlose_Click(object sender, EventArgs e)
        {
            _generator.LostPackages(string.Empty);
        }

        private void txtPackages_TextChanged(object sender, EventArgs e)
        {
            lstPackages.Items.Clear();
            if (!string.IsNullOrEmpty(txtPackages.Text))
            {
                lstPackages.Items.AddRange(_generator.SearchPackagesToBeLost(txtPackages.Text));
            }
        }
    }
    // https://stackoverflow.com/questions/15965166/what-is-the-maximum-length-of-latitude-and-longitude
}
