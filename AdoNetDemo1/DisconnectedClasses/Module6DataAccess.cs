using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
