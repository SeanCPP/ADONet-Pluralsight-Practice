using AdoNetDemo1.Module8;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdoNetDemo1
{
    public partial class Module8Form : Form
    {
        public Module8Form()
        {
            InitializeComponent();
            textBox1.Text = Properties.Resources.ConnectionString;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var cnnMgr = new ConnectionStringStuff();

            var lines = cnnMgr.BreakApartConnectionString();
            textBox2.Text = lines;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var mgr = new CommandBuilderStuff();
            textBox2.Text = mgr.CreateCrudCommands();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var mgr = new CommandBuilderStuff();
            textBox2.Text = mgr.InsertBook(titleBox.Text, authorBox.Text, isbnBox.Text);
        }
    }
}
