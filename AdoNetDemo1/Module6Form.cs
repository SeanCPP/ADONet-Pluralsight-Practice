using AdoNetDemo1.DisconnectedClasses;
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
    public partial class Module6Form : Form
    {
        public Module6Form()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var db = new Module6DataAccess();
            db.GetBooksAsDataTable();
            textBox1.Text = db.ResultText;
        }
    }
}
