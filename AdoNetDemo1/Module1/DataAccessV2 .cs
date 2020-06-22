using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetDemo1
{
    public class DataAccessV2
    {
        private void SqlDo(string sql, Action<SqlConnection, SqlCommand> action)
        {
            try
            {
                using (var cnn = new SqlConnection(Properties.Resources.ConnectionString))
                {
                    using (var cmd = new SqlCommand(sql, cnn))
                    {
                        cnn.Open();
                        action(cnn, cmd);
                    }
                }
            }
            catch (Exception ex)
            {

                ResultText = ex.ToString();
            }
        }
        public string ResultText { get; set; }
        public void Connect(string connectionString)
        {
            using (var cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                ResultText = GetConnectionInformation(cnn);
            }
        }

        public void ConnectWithError(string connectionString)
        {

            try
            {
                using (var cnn = new SqlConnection(connectionString + "error"))
                {
                    cnn.Open();
                    ResultText = GetConnectionInformation(cnn);
                }
            }
            catch (Exception ex)
            {

                ResultText = ex.ToString();
            }
        }

        private string GetConnectionInformation(SqlConnection cnn)
        {
            var sb = new StringBuilder(1024);
            sb.AppendLine($"Connection String: {cnn.ConnectionString}")
                .AppendLine($"State: {cnn.State.ToString()}")
                .AppendLine($"Connection Timeout: ${cnn.ConnectionTimeout}")
                .AppendLine($"Database: ${cnn.Database}")
                .AppendLine($"Data Source: ${cnn.DataSource}")
                .AppendLine($"Server Version: ${cnn.ServerVersion}")
                .AppendLine($"Workstation ID: ${cnn.WorkstationId}");
            return sb.ToString();
        }

        public int GetProductCount()
        {
            int total = 0;
            string sql = "SELECT COUNT(*) FROM Book";
            SqlDo(sql, (cnn, cmd) => total = (int)(cmd.ExecuteScalar()));

            ResultText = $"Products: {total.ToString()}";
            return total;
        }
        public int InsertBook()
        {
            int rowsAffected = 0;
            string sql = "INSERT INTO Book(Name, Author, ISBN)";
            sql += "VALUES('asdf', 'asdf', '9999');";

            SqlDo(sql, (cnn, cmd) =>
            {
                cmd.CommandType = CommandType.Text;
                rowsAffected = cmd.ExecuteNonQuery();
                ResultText = $"Rows Affected by change: {rowsAffected.ToString()}";
            });
            return rowsAffected;
        }

        public int GetBookCountUsingName(string name)
        {
            int rowsAffected = 0;
            string sql = "SELECT COUNT(*) FROM Book";
            sql += " WHERE Name LIKE @Name";
            SqlDo(sql, (cnn, cmd) => 
            {
                cmd.Parameters.Add(new SqlParameter("@Name", name));
                rowsAffected = (int)cmd.ExecuteScalar();
                ResultText = $"Rows appearing: {rowsAffected.ToString()}";
            });
            return rowsAffected;
        }

        public int InsertBook(Book book)
        {
            int rowsAffected = 0;
            string sql = "INSERT INTO Book(Name, Author, ISBN)";
            sql += " VALUES(@Name, @Author, @ISBN)";
            SqlDo(sql, (cnn, cmd) => 
            {
                cmd.Parameters.AddWithValue("@Name", book.Name);
                cmd.Parameters.AddWithValue("@Author", book.Author);
                cmd.Parameters.AddWithValue("@ISBN", book.ISBN);

                rowsAffected = cmd.ExecuteNonQuery();
                ResultText = $"Rows Affected by change: {rowsAffected.ToString()}";
            });
            return rowsAffected;
        }
        public int InsertBookOutputId(Book book)
        {
            int rowsAffected = 0;
            string sql = "Book_Insert";
            SqlDo(sql, (cnn, cmd) =>
            {
                cmd.Parameters.AddWithValue("@Name", book.Name);
                cmd.Parameters.AddWithValue("@Author", book.Author);
                cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@BookId", Value = -1, IsNullable = false, DbType=DbType.Int32, Direction = ParameterDirection.Output });
                cmd.CommandType = CommandType.StoredProcedure;
                rowsAffected = cmd.ExecuteNonQuery();

                var bookId = (int)cmd.Parameters["@BookId"].Value;
                ResultText = $"Book ID: {bookId}\nRows Affected by change: {rowsAffected.ToString()}";
            });
            return rowsAffected;
        }
    }
}
