using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetDemo1
{
    public class DataAccessWithTransactions
    {
        public string ResultText { get; set; }

        public int InsertAsTransaction(Book book)
        {  
            int rowsAffected = 0;

            string sql = "INSERT INTO Book(Name, Author, ISBN)";
            sql += " VALUES(@Name, @Author, @ISBN);";

            var book2 = new Book { Name = $"[temp]{book.Name}", Author = $"[temp]{book.Author}", ISBN = $"[temp]{book.ISBN}"};

            try
            {
                using (var cnn = new SqlConnection(Properties.Resources.ConnectionString))
                {
                    cnn.Open();
                    using (var trn = cnn.BeginTransaction())
                    {
                        try
                        {
                            using (var cmd = new SqlCommand(sql, cnn))
                            {
                                cmd.Transaction = trn;

                                cmd.Parameters.AddWithValue("@Name", book.Name);
                                cmd.Parameters.AddWithValue("@Author", book.Author);
                                cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
                                cmd.CommandType = CommandType.Text;

                                rowsAffected = cmd.ExecuteNonQuery();
                                ResultText = $"Rows Affected by change: {rowsAffected.ToString()}";
                                cmd.Parameters.Clear();

                                // second command execution
                                cmd.Parameters.AddWithValue("@Name", book2.Name);
                                cmd.Parameters.AddWithValue("@Author", book2.Author);
                                cmd.Parameters.AddWithValue("@ISBN", book2.ISBN);
                                rowsAffected = cmd.ExecuteNonQuery();
                                ResultText += $"\nRows Affected by change: {rowsAffected.ToString()}";

                                trn.Commit();
                            }
                        }
                        catch(Exception e)
                        {
                            trn.Rollback();
                            ResultText = $"Transaction error: {e.ToString()}";
                        }
                    }
                }
            }
            catch(Exception e)
            {
                ResultText = e.ToString();
            }

            return rowsAffected;
        }
    }
}
