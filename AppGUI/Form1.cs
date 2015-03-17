using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TcpClientsSimulator.AppModels;
using TcpClientsSimulator.AppUtilities;

namespace TcpClientsSimulator
{
    public partial class Form1 : Form
    {
        public List<AsyncClientHandlerTask> asyncClientHandlerTaskList;
        public UInt32 countClientType1Total;
        public UInt32 countClientType2Total;
        public UInt32 countClientType3Total;
        public UInt32 countClientType4Total;

        public UInt32 countClientType1Complete;
        public UInt32 countClientType2Complete;
        public UInt32 countClientType3Complete;
        public UInt32 countClientType4Complete;

        #region Constructor
        public Form1()
        {
            InitializeComponent();
            this.Text += "- Host [ " + StaticUtilities.getHostIpAddress() + " ]";
        } 
        #endregion

        #region button1_Click [Start Simulation]
        private async void button1_Click(object sender, EventArgs e)
        {
            Tuple<Boolean, IPAddress> tupleIpFieldValidation = StaticUtilities.isValidIpv4String(textBox1.Text);
            if(!tupleIpFieldValidation.Item1)
            {
                MessageBox.Show(this, "Invalid Server IP Address: " + textBox1.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Tuple<Boolean, int> tuplePortFieldValidation = StaticUtilities.isValidPortNumber(textBox2.Text);
            if(!tuplePortFieldValidation.Item1)
            {
                MessageBox.Show(this, "Invalid Server Port Number: " + textBox2.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Tuple<Boolean, UInt32> tupleClientType1CountValidation = StaticUtilities.isValidUnsignedInt(textBox3.Text);
            if(!tupleClientType1CountValidation.Item1)
            {
                MessageBox.Show(this, "Invalid Client Type 1: " + textBox3.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Tuple<Boolean, UInt32> tupleClientType2CountValidation = StaticUtilities.isValidUnsignedInt(textBox4.Text);
            if (!tupleClientType2CountValidation.Item1)
            {
                MessageBox.Show(this, "Invalid Client Type 2: " + textBox4.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Tuple<Boolean, UInt32> tupleClientType3CountValidation = StaticUtilities.isValidUnsignedInt(textBox5.Text);
            if (!tupleClientType1CountValidation.Item1)
            {
                MessageBox.Show(this, "Invalid Client Type 3: " + textBox5.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Tuple<Boolean, UInt32> tupleClientType4CountValidation = StaticUtilities.isValidUnsignedInt(textBox6.Text);
            if (!tupleClientType4CountValidation.Item1)
            {
                MessageBox.Show(this, "Invalid Client Type 4: " + textBox6.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(button1.Text.StartsWith("Start"))
            {
                button1.Text = "Running...";
                button1.Enabled = false;
            }

            asyncClientHandlerTaskList = new List<AsyncClientHandlerTask>();

            richTextBox1.Text = "";
            richTextBox1.Text += "[Clients Simulation Start]\n";

            countClientType1Total = tupleClientType1CountValidation.Item2;
            countClientType2Total = tupleClientType2CountValidation.Item2;
            countClientType3Total = tupleClientType3CountValidation.Item2;
            countClientType4Total = tupleClientType4CountValidation.Item2;
            countClientType1Complete = 0;
            countClientType2Complete = 0;
            countClientType3Complete = 0;
            countClientType4Complete = 0;

            progressBar1.Value = 0;
            progressBar2.Value = 0;
            progressBar3.Value = 0;
            progressBar4.Value = 0;

            AsyncClientsCreatorTask asyncClientsCreatorTask = new AsyncClientsCreatorTask(this,
                 tupleIpFieldValidation.Item2, tuplePortFieldValidation.Item2,
                 tupleClientType1CountValidation.Item2, tupleClientType2CountValidation.Item2,
                 tupleClientType3CountValidation.Item2, tupleClientType4CountValidation.Item2);
            Boolean result = await asyncClientsCreatorTask.startClientsCreation();
        } 
        #endregion

        #region richTextBox1_MouseDoubleClick
        private void richTextBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            richTextBox1.Text = "";
        } 
        #endregion
    }
}
