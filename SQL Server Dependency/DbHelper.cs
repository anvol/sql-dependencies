using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SQL_Server_Dependency
{
    public class DbHelper
    {
        private string connectionStringFormat = "server={0};Integrated Security=true";
        private string connectionString = "";

        public DbHelper(string server)
        {
            connectionString = String.Format(connectionStringFormat, server);
        }

        public List<String> GetAllDBs(bool bExcludeSysDbs)
        {
            var select = "SELECT name FROM master..sysdatabases";
            if (bExcludeSysDbs) select += " WHERE name NOT IN ('master','model','msdb','tempdb')";
            var list = new List<String>();

            using (var connection = new SqlConnection(connectionString))
            {
                // Create the command and set its properties.
                var command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = String.Format(select);
                command.CommandType = CommandType.Text;

                // Open the connection and execute the reader.
                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        list.Add(reader[0].ToString());
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();
            }
            return list;
        }

        public List<DatabaseObject> GetAllObjects(bool bExcludeSysDBs)
        {
            var list = new List<DatabaseObject>();
            var dbs = GetAllDBs(bExcludeSysDBs);

            foreach (var db in dbs)
            {
                var select = String.Format(@"Select o.[name] as ObjectName, o.[type_desc] as ObjectType, '{0}' as ObjectDatabase, s.[definition] as ObjectText
From [{0}].Sys.Objects o left join [{0}].Sys.sql_modules s on o.object_id=s.object_id  WHERE o.[type_desc] NOT IN ('system_table')", db);
                using (var connection = new SqlConnection(connectionString))
                {
                    // Create the command and set its properties.
                    var command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = String.Format(select.ToString());
                    command.CommandType = CommandType.Text;

                    // Open the connection and execute the reader.
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            list.Add(new DatabaseObject(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), "n/a", reader[3].ToString()));
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    reader.Close();
                }
            }
            return list;
        }
    }
}
