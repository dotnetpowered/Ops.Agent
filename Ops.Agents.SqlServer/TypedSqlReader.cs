using System;
using Microsoft.Data.SqlClient;

namespace Ops.Agents.SqlServer
{
    public class TypedSqlReader
    {
        SqlDataReader reader;

        public TypedSqlReader(SqlDataReader reader)
        {
            this.reader = reader;
        }

        public bool Read()
        {
            return reader.Read();

        }

        public string? GetString(string column) 
        {
            var value = reader[column];
            if (value == DBNull.Value)
                return null;
            return value.ToString();
        }

        public T? GetValue<T>(string column) where T : struct
        {
            try
            {
                var value = reader[column];
                if (value == DBNull.Value)
                    return null;
                return (T)value;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}

