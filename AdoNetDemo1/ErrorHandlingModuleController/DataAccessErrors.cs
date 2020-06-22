using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetDemo1.ErrorHandlingModuleController
{
    public class DataAccessErrors
    {
        public string ResultText { get; set; }
        public void RunSelectQuery()
        {
            // misspelled table name
            string sql = "SELECT COUNT(*) FROM BanTransactions;";
            try
            {
                using (var cnn = new SqlConnection(Properties.Resources.ConnectionString))
                {
                    using (var cmd = new SqlCommand(sql, cnn))
                    {
                        cnn.Open();
                        cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                ResultText = ex.ToString();
            }
        }
        public void RunSelectQueryWithBetterError()
        {
            // misspelled table name
            string sql = "SELECT COUNT(*) FROM BanTransactions;";
            try
            {
                using (var cnn = new SqlConnection(Properties.Resources.ConnectionString))
                {
                    using (var cmd = new SqlCommand(sql, cnn))
                    {
                        cnn.Open();
                        cmd.ExecuteScalar();
                    }
                }
            }
            catch (SqlException ex)
            {
                var sb = new StringBuilder();
                foreach(SqlError error in ex.Errors)
                {
                    sb.AppendLine($"Type: {error.GetType().FullName}");
                    sb.AppendLine($"Message: {error.Message}");
                    sb.AppendLine($"Number: {error.Number}");
                    sb.AppendLine($"Class: {error.Class}");
                    sb.AppendLine($"Procedure: {error.Procedure}");
                    sb.AppendLine($"LineNumber: {error.LineNumber}");
                }
                ResultText = sb.ToString();
            }
        }
        public int InsertBookStoredProc()
        {

            var book = new Book { Name = "Test", Author="Test", ISBN="Test" };

            int rowsAffected = 0;
            string sql = "Book_Insert";
            SqlConnection cnn = null;
            SqlCommand cmd = null;
            try
            {
                cnn = new SqlConnection(Properties.Resources.ConnectionString);
                cmd = new SqlCommand(sql, cnn);
                  
                cmd.Parameters.AddWithValue("@Nme", book.Name);
                cmd.Parameters.AddWithValue("@Author", book.Author);
                cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@BookId", Value = -1, IsNullable = false, DbType = DbType.Int32, Direction = ParameterDirection.Output });
                cmd.CommandType = CommandType.StoredProcedure;

                cnn.Open();
                rowsAffected = cmd.ExecuteNonQuery();

                var bookId = (int)cmd.Parameters["@BookId"].Value;
                ResultText = $"Book ID: {bookId}\nRows Affected by change: {rowsAffected.ToString()}";

            }
            catch (SqlException ex)
            {
                SqlExceptionManager.Instance.Publish(ex, cmd, "Error Pushing data.");
                ResultText = SqlExceptionManager.Instance.LastException.ToString();
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (cnn != null)
                {
                    cnn.Close();
                    cnn.Dispose();
                }
            }
            return rowsAffected;
        }
    }
}
