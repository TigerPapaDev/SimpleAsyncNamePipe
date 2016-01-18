using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AsyncNamedPipe;

namespace ClientTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
        }
        PipeCLient client;
        void Form1_Load(object sender, EventArgs e)
        {
            client = new PipeCLient();
            client.OnMsg += ShowMsg;
            client.OnError += (x) => MessageBox.Show(x);
            client.OnState += ShowMsg;

            client.Start();
        }

        private void ShowMsg(string obj)
        {
            if (this.listBox1.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { listBox1.Items.Add(obj); });
            }
            else
                listBox1.Items.Add(obj);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.Send(this.textBox1.Text);
        }
    }
}
