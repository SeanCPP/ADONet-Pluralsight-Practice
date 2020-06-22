using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetDemo1.ErrorHandlingModuleController
{
    /// <summary>
    /// This class was written by Paul D Sheriff for his pluralsight course
    /// ADO.net Fundamentals in C#
    /// https://app.pluralsight.com/library/courses/csharp-ado-dotnet-fundamentals/table-of-contents
    /// </summary>
    public class SqlExceptionManager
    {
        #region Instance Property
        private static SqlExceptionManager _Instance;

        public static SqlExceptionManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new SqlExceptionManager();
                }

                return _Instance;
            }
            set { _Instance = value; }
        }
        #endregion

        /// <summary>
        /// Get/Set Last Exception Object Created
        /// </summary>
        public Exception LastException { get; set; }

        #region Publish Methods
        public virtual void Publish(Exception ex)
        {
            LastException = ex;

            // TODO: Implement an exception publisher here
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }

        public virtual void Publish(Exception ex, SqlCommand cmd)
        {
            Publish(ex, cmd, null);
        }

        public virtual void Publish(Exception ex, SqlCommand cmd, string exceptionMsg)
        {
            LastException = ex;

            if (cmd != null)
            {
                LastException = CreateDbException(ex, cmd, null);

                // TODO: Implement an exception publisher here
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region CreateDbException Method
        public virtual SqlServerDataException CreateDbException(Exception ex, SqlCommand cmd, string exceptionMsg)
        {
            SqlServerDataException exc;
            exceptionMsg = string.IsNullOrEmpty(exceptionMsg) ? string.Empty : exceptionMsg + " - ";

            exc = new SqlServerDataException(exceptionMsg + ex.Message, ex)
            {
                ConnectionString = cmd.Connection.ConnectionString,
                Database = cmd.Connection.Database,
                SQL = cmd.CommandText,
                CommandParameters = cmd.Parameters,
                WorkstationId = Environment.MachineName
            };

            return exc;
        }
        #endregion
    }
}
