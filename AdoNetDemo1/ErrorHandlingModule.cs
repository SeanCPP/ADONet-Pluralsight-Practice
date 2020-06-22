using AdoNetDemo1.ErrorHandlingModuleController;
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
    public partial class ErrorHandlingModule : Form
    {
        public ErrorHandlingModule()
        {
            InitializeComponent();
        }
        private void ErrorHandlingModule_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            var db = new DataAccessErrors();
            db.RunSelectQuery();
            textBox1.Text = db.ResultText;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var db = new DataAccessErrors();
            db.RunSelectQueryWithBetterError();
            textBox1.Text = db.ResultText;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            var db = new DataAccessErrors();
            db.InsertBookStoredProc();
            textBox1.Text = db.ResultText;
        }
    }
}
