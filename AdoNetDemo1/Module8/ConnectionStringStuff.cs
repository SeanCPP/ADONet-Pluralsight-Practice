using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdoNetDemo1.Module8
{
    public class ConnectionStringStuff
    {
        public string BreakApartConnectionString()
        {
            var result = new StringBuilder();

            var cnnBuilder = new SqlConnectionStringBuilder(Properties.Resources.ConnectionString);

            result.AppendLine($"Application Name: {cnnBuilder.ApplicationName}");
            result.AppendLine($"Data Source: {cnnBuilder.DataSource}");
            result.AppendLine($"Initial Catalog: {cnnBuilder.InitialCatalog}");
            result.AppendLine($"Integrated Security: {cnnBuilder.IntegratedSecurity}");

            return result.ToString();
        }
    }
}
