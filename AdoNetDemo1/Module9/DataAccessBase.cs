using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetDemo1.Module9
{
    public abstract class DataAccessBase : IDisposable
    {
        #region Properties
        public Exception LastException { get; protected set; }
        public int RowsAffected { get; protected set; }
        public IDbCommand CommandObject { get; protected set; }
        public DataSet DataSetObject { get; protected set; }
        public string ConnectionString { get; private set; }
        public string ConnectionStringName { get; private set; }
        public string SQL { get; protected set; }
        public object IdentityGenerated { get; protected set; }
        public string ParameterToken { get; private set; }
        public List<ValidationResponse> ValidationResponses { get; set; } = new List<ValidationResponse>();
        public bool IsInTransaction { get; protected set; }
        #endregion
        public DataAccessBase(string connectionStringOrName)
        {
            Initialize();
            InitializeConnectionString(connectionStringOrName);
        }

        protected virtual void Initialize()
        {
            ParameterToken = "@";
            IdentityGenerated = null;
        }

        public IDbTransaction BeginTransaction()
        {
            IsInTransaction = true;
            CheckAndInitializeCommand(CommandObject);
            if(CommandObject.Connection.State != ConnectionState.Open)
            {
                CommandObject.Connection.Open();
            }
            CommandObject.Transaction = CommandObject.Connection.BeginTransaction();
            return CommandObject.Transaction;
        }
        public void CommitTransaction()
        {
            IsInTransaction = false;
            CommandObject.Transaction.Commit();
        }
        public void RollbackTransaction()
        {
            IsInTransaction = false;
            CommandObject.Transaction.Rollback();
        }
        public virtual void CheckAndInitializeCommand(IDbCommand cmd)
        {
            if(cmd == null)
            {
                cmd = CreateCommand();
            }
            if(cmd.Connection == null)
            {
                cmd.Connection = CreateConnection();
            }
        }
        public virtual IDbConnection CreateConnection()
        {
            return CreateConnection(ConnectionString);
        }
        public abstract IDbConnection CreateConnection(string connectionString);
        public abstract IDbCommand CreateCommand();
        public abstract DbDataAdapter CreateDataAdapter(IDbCommand cmd);
        public virtual void Reset(CommandType type)
        {
            if(CommandObject != null)
            {
                CommandObject.CommandText = string.Empty;
                CommandObject.CommandType = type;
                CommandObject.Parameters.Clear();
            }
            LastException = null;
            SQL = string.Empty;
            RowsAffected = 0;
            IdentityGenerated = null;
            ValidationResponses.Clear();
        }
        public virtual void Reset()
        {
            Reset(CommandType.Text);
        }

        #region CreateParameter
        public abstract IDbDataParameter CreateParameter(string name, object value, bool isNullable);
        public abstract IDbDataParameter CreateParameter(string name, object value, bool isNullable, DbType type, ParameterDirection direction = ParameterDirection.Input);
        public abstract IDbDataParameter CreateParameter(string name, object value, bool isNullable, DbType type, int size, ParameterDirection direction = ParameterDirection.Input);
        #endregion

        #region AddParameter
        public virtual void AddParameter(string name, object value, bool isNullable)
        {
            CommandObject.Parameters.Add(CreateParameter(name, value, isNullable));
        }
        public virtual void AddParameter(string name, object value, bool isNullable, DbType type, ParameterDirection direction = ParameterDirection.Input)
        {
            CommandObject.Parameters.Add(CreateParameter(name, value, isNullable, type, direction));
        }
        public virtual void AddParameter(string name, object value, bool isNullable, DbType type, int size, ParameterDirection direction = ParameterDirection.Input)
        {
            CommandObject.Parameters.Add(CreateParameter(name, value, isNullable, type, size, direction));
        }
        #endregion

        public virtual IDataParameter GetParameter(string name)
        {
            if(!name.Contains(ParameterToken))
            {
                name = $"{ParameterToken}{name}";
            }
            return (IDataParameter)CommandObject.Parameters[name];
        }

        public abstract T GetParameterValue<T>(string name, object defaultValue);
        public virtual T GetIdentityValue<T>()
        {
            return GetIdentityValue<T>((T)default);
        }
        public virtual T GetIdentityValue<T>(object defaultValue)
        {
            T result = (T)defaultValue;
            if(IdentityGenerated != null)
            {
                result = (T)Convert.ChangeType(IdentityGenerated, typeof(T));
            }
            return result;
        }

        #region GetRecord
        public virtual T GetRecord<T>(string sql) where T : new()
        {
            return GetRecord<T>(sql, CommandType.Text, null);
        }
        public virtual T GetRecord<T>(string sql, CommandType type) where T : new()
        {
            return GetRecord<T>(sql, type, null);
        }
        public virtual T GetRecord<T>(string sql, params IDbDataParameter[] parameters) where T : new()
        {
            return GetRecord<T>(sql, CommandType.Text, parameters);
        }
        public virtual T GetRecord<T>(string sql, CommandType type, params IDbDataParameter[] parameters) where T: new()
        {
            var result = new T();
            Reset(type);
            SQL = sql;
            if(parameters != null)
            {
                foreach(var param in parameters)
                {
                    CommandObject.Parameters.Add(param);
                }
            }
            AddStandardParameters();
            using(var dr = GetDataReader())
            {
                var items = ToList<T>(dr);
                if (items.Any())
                {
                    result = items[0];
                    RowsAffected = items.Count;
                }
            }
            GetStandardOutputParameters();

            return result;
        }
        #endregion
        #region GetRecordFromDataSet
        public virtual T GetRecordFromDataSet<T>(string sql) where T : new()
        {
            return GetRecord<T>(sql, CommandType.Text, null);
        }
        public virtual T GetRecordFromDataSet<T>(string sql, CommandType type) where T : new()
        {
            return GetRecord<T>(sql, type, null);
        }
        public virtual T GetRecordFromDataSet<T>(string sql, params IDbDataParameter[] parameters) where T : new()
        {
            return GetRecord<T>(sql, CommandType.Text, parameters);
        }
        public virtual T GetRecordFromDataSet<T>(string sql, CommandType type, params IDbDataParameter[] parameters) where T : new()
        {
            var result = new T();
            Reset(type);
            SQL = sql;
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    CommandObject.Parameters.Add(param);
                }
            }
            AddStandardParameters();
            DataSetObject = GetDataSet();
            if(DataSetObject.Tables.Count > 0)
            {
                var items = ToList<T>(DataSetObject.Tables[0]);
                if (items.Any())
                {
                    result = items[0];
                    RowsAffected = items.Count;
                }
            }
            GetStandardOutputParameters();

            return result;
        }
        #endregion
        #region GetRecords
        public virtual List<T> GetRecords<T>(string sql)
        {
            return GetRecords<T>(sql, CommandType.Text);
        }
        public virtual List<T> GetRecords<T>(string sql, CommandType type)
        {
            return GetRecords<T>(sql, type, null);
        }
        public virtual List<T> GetRecords<T>(string sql, params IDbDataParameter[] parameters)
        {
            return GetRecords<T>(sql, CommandType.Text, parameters);
        }
        public virtual List<T> GetRecords<T>(string sql, CommandType type, params IDbDataParameter[] parameters)
        {
            var result = new List<T>();
            Reset(type);
            SQL = sql;
            AddStandardParameters();
            if(parameters != null)
            {
                foreach(var param in parameters)
                {
                    CommandObject.Parameters.Add(param);
                }
            }
            using(var dr = GetDataReader())
            {
                result = ToList<T>(dr);
                RowsAffected = result.Count;
            }
            GetStandardOutputParameters();
            return result;
        }
        #endregion
        #region GetRecordsFromDataSet
        public virtual List<T> GetRecordsFromDataSet<T>(string sql)
        {
            return GetRecordsFromDataSet<T>(sql, CommandType.Text, null);
        }
        public virtual List<T> GetRecordsFromDataSet<T>(string sql, CommandType type)
        {
            return GetRecordsFromDataSet<T>(sql, type, null);
        }
        public virtual List<T> GetRecordsFromDataSet<T>(string sql, params IDbDataParameter[] parameters)
        {
            return GetRecordsFromDataSet<T>(sql, CommandType.Text, parameters);
        }
        public virtual List<T> GetRecordsFromDataSet<T>(string sql, CommandType type, params IDbDataParameter[] parameters)
        {
            List<T> result;
            Reset(type);
            SQL = sql;
            AddStandardParameters();
            if (parameters != null)
            {
                foreach(var param in parameters)
                {
                    CommandObject.Parameters.Add(param);
                }
            }
            DataSetObject = GetDataSet();
            result = ToList<T>(DataSetObject.Tables[0]);
            RowsAffected = result.Count;
            return result;
        }
        #endregion
        #region CountRecords
        public virtual int CountRecords(string sql)
        {
            return CountRecords(sql, CommandType.Text, null);
        }
        public virtual int CountRecords(string sql, CommandType type)
        {
            return CountRecords(sql, type, null);
        }
        public virtual int CountRecords(string sql, params IDbDataParameter[] parameters)
        {
            return CountRecords(sql, CommandType.Text, parameters);
        }
        public virtual int CountRecords(string sql, CommandType type, params IDbDataParameter[] parameters)
        {
            Reset(type);
            SQL = sql;
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    CommandObject.Parameters.Add(param);
                }
            }

            RowsAffected = ExecuteScalar<int>();
            return RowsAffected;
        }
        #endregion

        public virtual T ExecuteScalar<T>(string exceptionMsg = "", T defaultValue = default)
        {
            return ExecuteScalar<T>(CommandObject, exceptionMsg, defaultValue);
        }
        public virtual T ExecuteScalar<T>(IDbCommand cmd, string exceptionMsg = "", T defaultValue = default)
        {
            T result = defaultValue;
            bool isConnectionOpen = false;

            RowsAffected = 0;
            IdentityGenerated = null;
            try
            {
                CheckAndInitializeCommand(cmd);
                isConnectionOpen = cmd.Connection.State == ConnectionState.Open;
                
                // dont interfere with existing connection state, if it is already open
                if(!isConnectionOpen)
                {
                    cmd.Connection.Open();
                }

                result = (T)cmd.ExecuteScalar();

                if (!isConnectionOpen)
                {
                    cmd.Connection.Close();
                }
            }
            catch(Exception ex)
            {
                ThrowDbException(ex, exceptionMsg);
            }

            return result;
        }

        #region ExecuteNonQuery
        public virtual int ExecuteNonQuery()
        {
            return ExecuteNonQuery(CommandObject, false, string.Empty, string.Empty);
        }
        public virtual int ExecuteNonQuery(string exceptionMsg)
        {
            return ExecuteNonQuery(CommandObject, false, string.Empty, exceptionMsg);
        }
        public virtual int ExecuteNonQuery(bool retrieveIdentity = false)
        {
            return ExecuteNonQuery(retrieveIdentity, string.Empty, string.Empty);
        }

        public virtual int ExecuteNonQuery(bool retrieveIdentity = false, string identityParamName = "", string exceptionMsg = "")
        {
            return ExecuteNonQuery(CommandObject, retrieveIdentity, identityParamName);
        }
        public virtual int ExecuteNonQuery(IDbCommand cmd, bool retrieveIdentity = false, string identityParamName = "", string exceptionMsg = "")
        {
            bool isConnectionOpen = false;
            RowsAffected = 0;
            IdentityGenerated = null;
            try
            {
                CheckAndInitializeCommand(cmd);
                isConnectionOpen = cmd.Connection.State == ConnectionState.Open;
                if (!isConnectionOpen)
                {
                    cmd.Connection.Open();
                }
                if (retrieveIdentity)
                {
                    if (string.IsNullOrEmpty(identityParamName))
                    {
                        RowsAffected = ExecuteNonQueryFromDataSet(cmd);
                    }
                    else
                    {
                        RowsAffected = cmd.ExecuteNonQuery();
                        IdentityGenerated = ((IDbDataParameter)cmd.Parameters[identityParamName]).Value;
                    }
                }
                else
                {
                    RowsAffected = cmd.ExecuteNonQuery();
                }

                
                if (!isConnectionOpen)
                {
                    cmd.Connection.Close();
                }
            }
            catch(Exception ex)
            {
                ThrowDbException(ex, exceptionMsg);
            }
            return RowsAffected;
        }
        #endregion
        public int ExecuteNonQueryFromDataSet(IDbCommand cmd)
        {
            return ExecuteNonQueryFromDataSet(cmd, string.Empty);
        }
        public int ExecuteNonQueryFromDataSet(IDbCommand cmd, string exceptionMsg = "")
        {
            DataSetObject = new DataSet();
            RowsAffected = 0;
            IdentityGenerated = null;
            cmd.CommandText += ";SELECT @@ROWCOUNT AS RowsAffected, SCOPE_IDENTITY() AS IdentityGenerated";
            try
            {
                using(DbDataAdapter adapter = CreateDataAdapter(cmd))
                {
                    adapter.Fill(DataSetObject);
                    if(DataSetObject.Tables.Count > 0)
                    {
                        RowsAffected = (int)DataSetObject.Tables[0].Rows[0]["RowsAffected"];
                        IdentityGenerated = DataSetObject.Tables[0].Rows[0]["IdentityGenerated"];
                    }
                }
            }
            catch(Exception ex)
            {
                ThrowDbException(ex, exceptionMsg);
            }
            return RowsAffected;
        }
        public virtual DataSet GetDataSet()
        {
            return GetDataSet(string.Empty);
        }
        public virtual DataSet GetDataSet(string exceptionMsg)
        {
            return GetDataSet(CommandObject, exceptionMsg);
        }
        public virtual DataSet GetDataSet(IDbCommand cmd, string exceptionMsg = "")
        {
            DataSetObject = new DataSet();
            try
            {
                CheckAndInitializeCommand(cmd);
                using (var adapter = CreateDataAdapter(cmd))
                {
                    adapter.Fill(DataSetObject);
                }
            }
            catch(Exception ex)
            {
                ThrowDbException(ex, exceptionMsg);
            }
            return DataSetObject;
        }
        public virtual IDataReader GetDataReader(string exceptionMsg = "")
        {
            return GetDataReader(CommandObject, exceptionMsg);
        }
        public virtual IDataReader GetDataReader(IDbCommand cmd, string exceptionMsg = "")
        {
            var dr = null as IDataReader;
            try
            {
                CheckAndInitializeCommand(cmd);
                cmd.Connection.Open();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch(Exception ex)
            {
                ThrowDbException(ex, exceptionMsg);
            }
            return dr;
        }
        public abstract void AddStandardParameters();
        public abstract void GetStandardOutputParameters();
        protected virtual void InitializeConnectionString(string connectionStringOrName)
        {
            if (connectionStringOrName.Contains("="))
            {
                ConnectionString = connectionStringOrName;
                return;
            }
            ConnectionStringName = connectionStringOrName;
            try
            {
                ConnectionString = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            }
            catch(Exception ex)
            {
                LastException = new ArgumentException($"Connection string by the name of {ConnectionStringName} could not be found. Check for spelling errors.", ex);
                throw LastException;
            }
        }
        public virtual void ThrowDbException(Exception ex, string msg = "")
        {
            LastException = new ApplicationException($"Invalid call to the database: {msg}", ex);
            throw LastException;
        }

        public virtual List<T> ToList<T>(IDataReader reader)
        {
            var result = new List<T>();
            var columns = new List<ColumnMapper>();
            var type = typeof(T);
            var properties = type.GetProperties();
            var attributes = properties.Where(p => p.GetCustomAttributes(typeof(ColumnAttribute), false).Any()).ToArray();

            for (int fieldIndx = 0; fieldIndx < reader.FieldCount; ++fieldIndx)
            {
                string fieldName = reader.GetName(fieldIndx);
                var column = properties.FirstOrDefault(c => c.Name == fieldName);

                if (column == null)
                {
                    foreach (var attribute in attributes)
                    {
                        var customAttribute = attribute.GetCustomAttribute(typeof(ColumnAttribute));
                        if (customAttribute != null && ((ColumnAttribute)customAttribute).Name == fieldName)
                        {
                            column = properties.FirstOrDefault(c => c.Name == attribute.Name);
                            break;
                        }
                    }
                }
                if (column != null)
                {
                    columns.Add(new ColumnMapper
                    {
                        ColumnName = fieldName,
                        ColumnProperty = column,
                    });
                }
            }
            while (reader.Read())
            {
                var item = Activator.CreateInstance<T>();
                foreach (var columnMapper in columns)
                {
                    if (reader[columnMapper.ColumnName].Equals(DBNull.Value))
                    {
                        columnMapper.ColumnProperty.SetValue(item, null, null);
                    }
                    else
                    {
                        columnMapper.ColumnProperty.SetValue(item, reader[columnMapper.ColumnName], null);
                    }
                }
                result.Add(item);
            }
            return result;
        }
        public virtual List<T> ToList<T>(DataTable dt)
        {
            var result = new List<T>();
            var columns = new List<ColumnMapper>();
            var type = typeof(T);
            var properties = type.GetProperties();
            var attributes = properties.Where(p => p.GetCustomAttributes(typeof(ColumnAttribute), false).Any()).ToArray();

            for (int fieldIndx = 0; fieldIndx < dt.Columns.Count; ++fieldIndx)
            {
                string fieldName = dt.Columns[fieldIndx].ColumnName;
                var column = properties.FirstOrDefault(c => c.Name == fieldName);

                if (column == null)
                {
                    foreach (var attribute in attributes)
                    {
                        var customAttribute = attribute.GetCustomAttribute(typeof(ColumnAttribute));
                        if (customAttribute != null && ((ColumnAttribute)customAttribute).Name == fieldName)
                        {
                            column = properties.FirstOrDefault(c => c.Name == attribute.Name);
                            break;
                        }
                    }
                }
                if (column != null)
                {
                    columns.Add(new ColumnMapper
                    {
                        ColumnName = fieldName,
                        ColumnProperty = column,
                    });
                }
            }
            foreach(var row in dt.AsEnumerable())
            {
                var item = Activator.CreateInstance<T>();
                foreach(var col in columns)
                {
                    if (row[col.ColumnName].Equals(DBNull.Value))
                    {
                        col.ColumnProperty.SetValue(item, null, null);
                    }
                    else
                    {
                        col.ColumnProperty.SetValue(item, row[col.ColumnName], null);
                    }
                }
                result.Add(item);
            }

            return result;
        }
        public ValidationResponse AddValidationResponse(string propertyName, string message)
        {
            var response = new ValidationResponse
            {
                PropertyName = propertyName,
                Message = message
            };
            ValidationResponses.Add(response);
            return response;
        }
        public virtual bool Validate<T>(T item)
        {
            string propName = string.Empty;
            ValidationResponses.Clear();

            if (item != null)
            {
                ValidationContext context = new ValidationContext(item, serviceProvider: null, items: null);
                List<ValidationResult> results = new List<ValidationResult>();

                if (!Validator.TryValidateObject(item, context, results, true))
                {
                    foreach (ValidationResult result in results)
                    {
                        if (((string[])result.MemberNames).Length > 0)
                        {
                            propName = ((string[])result.MemberNames)[0];
                        }
                        ValidationResponses.Add(new ValidationResponse { Message = result.ErrorMessage, PropertyName = propName });
                    }
                }
            }

            return (ValidationResponses.Count > 0);
        }
        public virtual void Dispose()
        {
            if(CommandObject != null)
            {
                if(CommandObject.Connection != null)
                {
                    if(CommandObject.Transaction != null)
                    {
                        CommandObject.Transaction.Dispose();
                    }
                    CommandObject.Connection.Close();
                    CommandObject.Connection.Dispose();
                }
                CommandObject.Dispose();
            }
        }
    }
}
