using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdoNetDemo1
{
    public partial class Launcher : Form
    {
        public Launcher()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var module1 = new Form1();
            module1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var module1 = new Module2();
            module1.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var module1 = new ErrorHandlingModule();
            module1.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var module6 = new Module6Form();
            module6.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var module7 = new Module7Form();
            module7.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var module8 = new Module8Form();
            module8.Show();
        }
    }
}
