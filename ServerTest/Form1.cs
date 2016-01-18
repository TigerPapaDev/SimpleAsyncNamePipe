using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AsyncNamedPipe;

namespace ServerTest
{
    public partial class Form1 : Form
    {
        PipeServer server = null;
        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
        }

        void Form1_Load(object sender, EventArgs e)
        {
            server = new PipeServer();
            server.OnMsg += ShowMsg;
            server.OnError += (x) => MessageBox.Show(x);
            server.OnState += ShowMsg;
            server.StartListen();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            server.Send(this.textBox1.Text);
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
    }
}
