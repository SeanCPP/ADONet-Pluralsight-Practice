using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetDemo1.Module2Examples
{
    public static class DataAccessHelpers
    {
        public static T GetFieldValue<T>(this SqlDataReader dataReader, string name)
        {
            T result = default;
            if (!dataReader[name].Equals(DBNull.Value))
            {
                result = (T)dataReader[name];
            }
            return result;
        }
    }
}
