using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetDemo1.Module9
{
    public class SqlServerDataAccess : DataAccessBase
    {
        public int RETURN_VALUE { get; set; }
        public SqlServerDataAccess(string connectionStringOrName) : base(connectionStringOrName)
        {
        }
        protected override void Initialize()
        {
            base.Initialize();
            CommandObject = new SqlCommand();
            ParameterToken = "@";
        }
        public override void Reset(CommandType type)
        {
            base.Reset(type);
            if(CommandObject == null)
            {
                CommandObject = new SqlCommand
                {
                    CommandType = type
                };
            }
            RETURN_VALUE = 0;
        }
        public override void AddStandardParameters()
        {
            if(CommandObject.CommandType == CommandType.StoredProcedure)
            {
                AddParameter("RETURN_VALUE", 0, false, DbType.Int32, ParameterDirection.ReturnValue);
            }
        }

        public override IDbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        public override IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public override DbDataAdapter CreateDataAdapter(IDbCommand cmd)
        {
            return new SqlDataAdapter((SqlCommand)cmd);
        }

        public override IDbDataParameter CreateParameter(string name, object value, bool isNullable)
        {
            name = name.Contains(ParameterToken) ? name : $"{ParameterToken}{name}";
            return new SqlParameter
            {
                ParameterName = name,
                Value = value,
                IsNullable = isNullable
            };
        }

        public override IDbDataParameter CreateParameter(string name, object value, bool isNullable, DbType type, ParameterDirection direction = ParameterDirection.Input)
        {
            name = name.Contains(ParameterToken) ? name : $"{ParameterToken}{name}";
            return new SqlParameter
            {
                ParameterName = name,
                Value = value,
                IsNullable = isNullable,
                DbType = type,
                Direction = direction
            };
        }

        public override IDbDataParameter CreateParameter(string name, object value, bool isNullable, DbType type, int size, ParameterDirection direction = ParameterDirection.Input)
        {
            name = name.Contains(ParameterToken) ? name : $"{ParameterToken}{name}";
            return new SqlParameter
            {
                ParameterName = name,
                Value = value,
                IsNullable = isNullable,
                Size = size,
                Direction = direction,
            };
        }

        public override T GetParameterValue<T>(string name, object defaultValue)
        {
            T result;
            string value;
            value = ((SqlParameter)GetParameter(name)).Value.ToString();
            if (string.IsNullOrEmpty(value))
            {
                result = (T)defaultValue;
            }
            else
            {
                result = (T)Convert.ChangeType(value, typeof(T));
            }
            return result;
        }

        public override void GetStandardOutputParameters()
        {
            if(CommandObject.CommandType == CommandType.StoredProcedure)
            {
                RETURN_VALUE = GetParameterValue<int>("RETURN_VALUE", default(int));
            }
        }
    }
}
