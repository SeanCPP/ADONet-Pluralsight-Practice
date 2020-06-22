using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetDemo1
{
    public class DataAccessV1
    {
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

        public async Task<int> GetProductCountAsync()
        {
            int total = 0;
            string sql = "SELECT COUNT(*) FROM Book";
            using (var cnn = new SqlConnection(Properties.Resources.ConnectionString))
            {
                using (var cmd = new SqlCommand(sql, cnn))
                {
                    cnn.Open();
                    total = (int)(await cmd.ExecuteScalarAsync());
                }
            }
            ResultText = $"Products: {total.ToString()}";
            return total;
        }
        public int InsertBook()
        {
            int rowsAffected = 0;
            string sql = "INSERT INTO Book(Name, Author, ISBN)";
            sql += "VALUES('asdf', 'asdf', '9999');";
            try
            {
                using (var cnn = new SqlConnection(Properties.Resources.ConnectionString))
                {
                    using (var cmd = new SqlCommand(sql, cnn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cnn.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                        ResultText = $"Rows Affected by change: {rowsAffected.ToString()}";
                    }
                }
            }
            catch(Exception ex)
            {
                ResultText = ex.ToString();
            }
            return rowsAffected;
        }
    }
}
