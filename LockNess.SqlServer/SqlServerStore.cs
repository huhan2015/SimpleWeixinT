using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Dapper;

namespace LockNess.WeiXin.Core.Db
{
    public class SqlServerStore
    {
        private string _connectionString;

        public SqlServerStore(string connString)
        {
            _connectionString = connString;
        }

       private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }


        public DataTable GetDataReader(string sql)
        {
            var datatable = new DataTable();
            using (var connection = GetConnection())
            {
                var reader = connection.ExecuteReader(sql);
                datatable.Load(reader);
            }
            return datatable;
        }

        public void ExcuteSql(string sql)
        {
            using (var connection = GetConnection())
            {
                connection.Execute(sql);
            }
        }
    }
}
