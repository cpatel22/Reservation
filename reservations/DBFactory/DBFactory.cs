using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Reservation.DBFactory
{

    public class DBFactory
    {
        private string _connectionstring = "";
        private DbConnection _connection;
        private DbCommand _command;
        private DbProviderFactory _factory = null;


        public string connectionstring
        {
            get
            {
                return _connectionstring;
            }
            set
            {
                if (value != "")
                {
                    _connectionstring = value;
                }
            }
        }


        public DbConnection connection
        {
            get
            {
                return _connection;
            }
        }


        public DbCommand command
        {
            get
            {
                return _command;
            }
        }




        public void CreateDBObjects(string connectString, Providers providerList)
        {
            switch (providerList)
            {
                case Providers.SqlServer:
                    _factory = SqlClientFactory.Instance;
                    break;


            }
            _connection = _factory.CreateConnection();
            _command = _factory.CreateCommand();
            _connection.ConnectionString = connectString;
            _command.Connection = connection;
        }


        public int AddParameter(string name, object value)
        {
            DbParameter parm = _factory.CreateParameter();
            parm.ParameterName = name;
            parm.Value = value;
            return command.Parameters.Add(parm);
        }


        public int AddParameter(DbParameter parameter)
        {
            return command.Parameters.Add(parameter);
        }


        private void BeginTransaction()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
            command.Transaction = connection.BeginTransaction();
        }


        private void CommitTransaction()
        {
            command.Transaction.Commit();
            connection.Close();
        }


        private void RollbackTransaction()
        {
            command.Transaction.Rollback();
            connection.Close();
        }


        public int ExecuteNonQuery(string query, CommandType commandtype, ConnectionState connectionstate)
        {
            command.CommandText = query;
            command.CommandType = commandtype;
            int i = -1;
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                BeginTransaction();
                i = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                throw (ex);
            }
            finally
            {
                CommitTransaction();
                command.Parameters.Clear();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return i;
        }


        public object ExecuteScaler(string query, CommandType commandtype, ConnectionState connectionstate)
        {
            command.CommandText = query;
            command.CommandType = commandtype;
            object obj = null;
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                BeginTransaction();
                obj = command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                throw (ex);
            }
            finally
            {
                CommitTransaction();
                command.Parameters.Clear();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                    command.Dispose();
                }
            }
            return obj;
        }


        public DbDataReader ExecuteReader(string query, CommandType commandtype, ConnectionState connectionstate)
        {
            command.CommandText = query;
            command.CommandType = commandtype;
            DbDataReader reader = null;
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                if (connectionstate == System.Data.ConnectionState.Open)
                {
                    reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                }
                else
                {
                    reader = command.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                command.Parameters.Clear();
            }
            return reader;
        }


        public DataSet GetDataSet(string query, CommandType commandtype, ConnectionState connectionstate)
        {
            DbDataAdapter adapter = _factory.CreateDataAdapter();
            command.CommandText = query;
            command.CommandType = commandtype;
            adapter.SelectCommand = command;
            DataSet ds = new DataSet();
            try
            {
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                command.Parameters.Clear();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                    command.Dispose();
                }
            }
            return ds;
        }


        public enum Providers
        {
            SqlServer

        }
    }
}