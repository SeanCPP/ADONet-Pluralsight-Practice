using AdoNetDemo1.Module2Examples;
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
    public partial class Module2 : Form
    {
        public Module2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var db = new DataAccessModule2();
            db.GetProductAsDataReader();
            label1.Text = db.ResultText;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var db = new DataAccessModule2();
            var books = db.GetProductAsObjects();
            label1.Text = $"First Book in list: {books[0].Name}";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var db = new DataAccessModule2();
            var (books, transactions) = db.GetMultipleResultSets();

            label1.Text = $"{books[0].Name} by {books[0].Author}\n";
            label1.Text += $"{transactions[0].Type} for {transactions[0].Amount.ToDecimal():C}";
        }
    }
}
