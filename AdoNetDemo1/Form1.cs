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
    public partial class Form1 : Form
    {
        private readonly DataAccessV2 db = new DataAccessV2();

        public Book Book { get; set; }

        public Form1()
        {
            InitializeComponent();
            UpdateBookModel();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            db.Connect(Properties.Resources.ConnectionString);
            label1.Text = db.ResultText;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            db.ConnectWithError(Properties.Resources.ConnectionString);
            label1.Text = db.ResultText;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int result = db.GetProductCount();
            label1.Text = db.ResultText;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            db.InsertBook();
            label1.Text = db.ResultText;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            db.GetBookCountUsingName(textBox1.Text);
            label1.Text = db.ResultText;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            UpdateBookModel();
            db.InsertBook(Book);
            label1.Text = db.ResultText;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            UpdateBookModel();
            db.InsertBookOutputId(Book);
            label1.Text = db.ResultText;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            UpdateBookModel();
            var db = new DataAccessWithTransactions();
            db.InsertAsTransaction(Book);
            label1.Text = db.ResultText;
        }

        private void UpdateBookModel()
        {
            if (Book is null)
            {
                Book = new Book();
            }
            Book.Name = textBoxName.Text;
            Book.Author = textBoxAuthor.Text;
            Book.ISBN = textBoxISBN.Text;
        }
    }
}
