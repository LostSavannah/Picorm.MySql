using MySql.Data.MySqlClient;
using Picorm.Common.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Picorm.MySql
{
    public class MySqlDbInterface : IDBInterface
    {
        public MySqlDbInterface(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; }

        public IDataReader SelectAll(string tableName)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            connection.Open();
            MySqlCommand command = new MySqlCommand(
                    $"SELECT * FROM {tableName};",
                    connection
                );
            return new ReaderWrapper(connection, command.ExecuteReader());
        }

        public IDataReader SelectWhere(string tableName, IEnumerable<QueryParameter> parameters)
        {
            string filterTemplate = string.Join(" AND ", parameters.Select(p => $"{p.Name} = @{p.Name}"));

            MySqlConnection connection = new MySqlConnection(ConnectionString);
            connection.Open();

            MySqlCommand command = new MySqlCommand()
            {
                Connection = connection
            };
            if (parameters.Any())
            {
                command.CommandText = $"SELECT * FROM {tableName} WHERE {filterTemplate};";
                parameters.ToList().ForEach(p => command.Parameters.AddWithValue(p.Name, p.Value));
            }
            else
            {
                command.CommandText = $"SELECT * FROM {tableName};";
            }
            return new ReaderWrapper(connection, command.ExecuteReader());
        }

        public int Insert(string tableName, IEnumerable<QueryParameter> parameters)
        {
            string insertColumns = string.Join(", ", parameters.Select(p => p.Name));
            string insertParameters = string.Join(", ", parameters.Select(p => $"@{p.Name}"));
            string query = $"INSERT INTO {tableName}({insertColumns}) VALUES ({insertParameters});";
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(query, connection);
                parameters.ToList().ForEach(p => command.Parameters.AddWithValue(p.Name, p.Value));
                return command.ExecuteNonQuery();
            }
        }

        public int Update(
            string tableName,
            IEnumerable<QueryParameter> updates,
            IEnumerable<QueryParameter> where)
        {
            string updateParameters = string.Join(", ", updates.Select(p => $"{p.Name} = @{p.Name}.update_p"));
            string whereParameters = string.Join(" AND ", where.Select(p => $"{p.Name} = @{p.Name}.where_p"));
            string query = $"UPDATE {tableName} SET {updateParameters} WHERE {whereParameters}";
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(query, connection);
                updates.ToList().ForEach(p => command.Parameters.AddWithValue($"{ p.Name}.update_p", p.Value));
                where.ToList().ForEach(p => command.Parameters.AddWithValue($"{ p.Name}.where_p", p.Value));
                return command.ExecuteNonQuery();
            }
        }

        public int Delete(
            string tableName,
            IEnumerable<QueryParameter> where
            )
        {
            string whereParameters = string.Join(" AND ", where.Select(p => $"{p.Name} = @{p.Name}"));
            string query = $"DELETE FROM {tableName} WHERE {whereParameters}";
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(query, connection);
                where.ToList().ForEach(p => command.Parameters.AddWithValue($"{ p.Name}", p.Value));
                return command.ExecuteNonQuery();
            }
        }
    }
}
