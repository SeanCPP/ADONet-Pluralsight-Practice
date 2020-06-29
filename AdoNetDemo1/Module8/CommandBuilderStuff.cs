using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetDemo1.Module8
{
    public class CommandBuilderStuff
    {
        private string SqlDoWithAdapter(Func<SqlConnection, SqlDataAdapter, string> action)
        {
            string result = "";
            try
            {
                using (var cnn = new SqlConnection(Properties.Resources.ConnectionString))
                {
                    using (var adapter = new SqlDataAdapter("SELECT * FROM Book", cnn))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        result = action(cnn, adapter);
                    }
                }
            }
            catch (Exception e)
            {
                result = e.ToString();
            }
            return result;
        }
        public string CreateCrudCommands()
        {
            string result = SqlDoWithAdapter((cnn, adapter) => 
            {
                string commands = string.Empty;
                using (var builder = new SqlCommandBuilder(adapter))
                {
                    commands += $"Insert: {builder.GetInsertCommand(true).CommandText}";
                    commands += Environment.NewLine;
                    commands += $"Update: {builder.GetUpdateCommand(true).CommandText}";
                    commands += Environment.NewLine;
                    commands += $"Delete: {builder.GetDeleteCommand(true).CommandText}";
                    commands += Environment.NewLine;
                }
                return commands;
            });
            return result;
        }

        public string InsertBook(string title, string author, string isbn)
        {
            return SqlDoWithAdapter((cnn, adapter) =>
            {
                string result = string.Empty;
                using (var builder = new SqlCommandBuilder(adapter))
                {
                    using(var cmd = builder.GetInsertCommand(true))
                    {
                        cmd.Parameters["@Name"].Value = title;
                        cmd.Parameters["@Author"].Value = author;
                        cmd.Parameters["@ISBN"].Value = isbn;
                        cmd.Connection = cnn;
                        result = cmd.CommandText;
                        cnn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return result;
            });
        }
    }
}
