using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoogleServiceAccount
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //Get Visibility
        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                GoogleDriveFileInfo info = GoogleManager.GetDriveFileInfo(textBox1.Text);

                if (info?.Permissions != null)
                {
                    string visibility = string.Join("\r\n", info.Permissions);
                    MessageBox.Show(visibility);
                }
                else
                {
                    MessageBox.Show("EMPTY");
                }
            }
        }

        //Set Visibility
        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text))
            {
                if (GoogleManager.SetDriveFilePermission(textBox1.Text, textBox2.Text))
                {
                    MessageBox.Show("DONE");
                }
                else
                {
                    MessageBox.Show("ERROR");
                }
            }
        }
    }
}
