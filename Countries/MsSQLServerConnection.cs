using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;

namespace CRMGURUTest
{

    public static class MsSQLServerConnection
    {
        public static bool IsServerEnable { get; private set; }
        public static string Connection { get; private set; }

        public static void SetConnection()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"ServerConnection.txt");
            string connectionString = File.ReadAllLines(path)[0];
            Connection = connectionString;
            CheckConnectionToServer();
        }
        public static void SetConnection(string connection)
        {
            Connection = connection;
            CheckConnectionToServer();
        }




        private static void CheckConnectionToServer()
        {
            using (SqlConnection connection = new SqlConnection(Connection))
            {
                try
                {
                    connection.Open();
                    IsServerEnable = true;
                    connection.Close();
                }
                catch
                {
                    IsServerEnable = false;
                }
            }
        }
    }
}
