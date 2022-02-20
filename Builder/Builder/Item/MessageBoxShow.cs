using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Builder.Item
{
    public partial class MessageBoxShow : Form
    {

        string _Message = "", _Hood = "";
        public MessageBoxShow(string Message,string Hood)
        {
            InitializeComponent();
            _Message = Message;
            _Hood = Hood;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MessageBoxShow_Load(object sender, EventArgs e)
        {
            label1.Text = _Message;
            label2.Text = _Hood;
        }
    }
}
