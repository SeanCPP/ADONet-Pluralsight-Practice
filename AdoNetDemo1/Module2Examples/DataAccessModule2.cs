using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetDemo1.Module2Examples
{
    public class DataAccessModule2
    {
        public string ResultText { get; set; }
        public void GetProductAsDataReader()
        {
            var sb = new StringBuilder(1024);
            string sql = "SELECT * FROM Book";
            using (var cnn = new SqlConnection(Properties.Resources.ConnectionString))
            {
                using (var cmd = new SqlCommand(sql, cnn))
                {
                    cnn.Open();
                    using (var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dr.Read())
                        {
                            sb.AppendLine($"Book: ${dr["Name"]}")
                              .AppendLine($"Author: ${dr["Author"]}")
                              .AppendLine($"ISBN: ${dr["ISBN"]}")
                              .AppendLine();
                        }
                    }
                }
            }
            ResultText = sb.ToString();
        }
        public List<Book> GetProductAsObjects()
        {
            var books = new List<Book>();
            string sql = "SELECT * FROM Book";
            using (var cnn = new SqlConnection(Properties.Resources.ConnectionString))
            {
                using (var cmd = new SqlCommand(sql, cnn))
                {
                    cnn.Open();
                    using (var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dr.Read())
                        {
                            books.Add(new Book 
                            { 
                                Name = dr.GetFieldValue<string>(dr.GetOrdinal("Name")),
                                Author = dr.GetFieldValue<string>(dr.GetOrdinal("Author")),
                                ISBN = dr.GetFieldValue<string>(dr.GetOrdinal("ISBN"))
                            });
                        }
                    }
                }
            }
            return books;
        }
        public (List<Book>, List<BankTransaction>) GetMultipleResultSets()
        {
            var books = new List<Book>();
            var transactions = new List<BankTransaction>();

            string sql = "SELECT * FROM Book;";
            sql += " SELECT * FROM BankTransactions";

            using (var cnn = new SqlConnection(Properties.Resources.ConnectionString))
            {
                using (var cmd = new SqlCommand(sql, cnn))
                {
                    cnn.Open();
                    using (var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dr.Read())
                        {
                            books.Add(new Book
                            {
                                Name = dr.GetFieldValue<string>(dr.GetOrdinal("Name")),
                                Author = dr.GetFieldValue<string>(dr.GetOrdinal("Author")),
                                ISBN = dr.GetFieldValue<string>(dr.GetOrdinal("ISBN"))
                            });
                        }

                        dr.NextResult();

                        while (dr.Read())
                        {
                            transactions.Add(new BankTransaction
                            {
                                Type = dr.GetFieldValue<string>(dr.GetOrdinal("Type")),
                                Amount = dr.GetFieldValue<SqlMoney>(dr.GetOrdinal("Amount")),
                            });
                        }
                    }
                }
            }

            return (books, transactions);
        }
    }
}
