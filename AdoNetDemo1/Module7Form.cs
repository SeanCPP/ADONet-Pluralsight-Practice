using AdoNetDemo1.Module7;
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
    public partial class Module7Form : Form
    {
        private readonly Module7DataAccess data = new Module7DataAccess();
        public Module7Form()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dt = data.BuildDataTable();
            textBox1.Text = data.ProcessRowsAndColumns(dt);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var dt = data.CloneDataTable();
            textBox1.Text = data.ProcessRowsAndColumns(dt);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var dt = data.CopyDataTable();
            textBox1.Text = data.ProcessRowsAndColumns(dt);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var dt = data.GetSubsetOfDataTable();
            textBox1.Text = data.ProcessRowsAndColumns(dt);
        }
    }
}
