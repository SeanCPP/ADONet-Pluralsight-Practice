using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdoNetDemo1.DisconnectedClasses
{
    public class Module6DataAccess
    {
        public string ResultText { get; set; }

        private void SqlDo(string sql, Action<SqlConnection, SqlCommand> action)
        {
            using(var cnn = new SqlConnection(Properties.Resources.ConnectionString))
            {
                using (var cmd = new SqlCommand(sql, cnn))
                {
                    cnn.Open();
                    action(cnn, cmd);
                }
            }
        }
        public DataTable GetBooksAsDataTable()
        {
            string sql = "SELECT * FROM Book";
            DataTable dt = null;
            SqlDo(sql, (cnn, cmd) => 
            {
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    dt = new DataTable();
                    adapter.Fill(dt);
                    ProcessRowsAndColumns(dt);
                }
            });
            return dt;
        }
        public DataTable GetTransactionsAsDataTable()
        {
            string sql = "SELECT * FROM BankTransactions";
            DataTable dt = null;
            SqlDo(sql, (cnn, cmd) =>
            {
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    dt = new DataTable();
                    adapter.Fill(dt);
                    ProcessRowsAndColumns(dt);
                }
            });
            return dt;
        }

        public List<Book> GetBooksAsList(string name)
        {
            var books = new List<Book>();
            ResultText = string.Empty;

            DataTable dt = null;
            string sql = "SELECT * FROM Book";
            sql += " WHERE Name LIKE @Name";

            SqlDo(sql, (cnn, cmd) => 
            {
                cmd.Parameters.Add(new SqlParameter("@Name", name));
                using (var adapter = new SqlDataAdapter())
                {
                    dt = new DataTable();
                    adapter.Fill(dt);
                    if(dt.Rows.Count > 0)
                    {
                        books =
                            (from row in dt.AsEnumerable()
                             select new Book 
                             { 
                                 Name = row.Field<string>("Name"),
                                 Author = row.Field<string>("Author"),
                                 ISBN = row.Field<string>("ISBN")
                             }).ToList();
                    }
                }
            });

            return books;
        }

        public (List<DataRow>, List<DataRow>) GetMultipleResultSets()
        {
            var books = new List<DataRow>();
            var transactions = new List<DataRow>();

            ResultText = string.Empty;
            var ds = new DataSet();
            string sql = "SELECT * FROM Book";
            sql += " SELECT * From BankTransactions;";

            SqlDo(sql, (cnn, cmd) => 
            {
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(ds);
                    if(ds.Tables.Count > 0)
                    {
                        books = ds.Tables[0].AsEnumerable().ToList();
                        transactions = ds.Tables[1].AsEnumerable().ToList();
                    }
                }
            });
            return (books, transactions);
        }

        private void ProcessRowsAndColumns(DataTable dt)
        {
            var sb = new StringBuilder();
            int index = 1;
            foreach(DataRow row in dt.Rows)
            {
                sb.AppendLine($"** Row: {index}");
                foreach(DataColumn col in dt.Columns)
                {
                    sb.AppendLine($"-{col.ColumnName}: {row[col.ColumnName]}");
                }
                sb.AppendLine();
                ++index;
            }
            ResultText = sb.ToString();
        }
    }
}
