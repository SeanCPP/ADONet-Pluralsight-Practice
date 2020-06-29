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
            var dt = db.GetBooksAsDataTable();
            textBox1.Text = db.ResultText;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";

            var viewdata = new ItemViewData();
            var dv = viewdata.GetBooksSortedByAuthorDescending();
            foreach(DataRowView rowView in dv)
            {
                foreach(DataColumn column in dv.Table.Columns)
                {
                    textBox1.Text += $"  {column.ColumnName}: {rowView[column.ColumnName]}  ";
                    textBox1.Text += Environment.NewLine;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";

            var viewdata = new ItemViewData();
            var dv = viewdata.GetTransactionViewsLessThan(46.0m);
            foreach (DataRowView rowView in dv)
            {
                foreach (DataColumn column in dv.Table.Columns)
                {
                    textBox1.Text += $"  {column.ColumnName}: {rowView[column.ColumnName]}  ";
                }
            }
        }
    }
}
