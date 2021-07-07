using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CRMGURUTest
{
    public static class QueryExecuter
    {
        public static int ExecuteQueryReturnInt(string query)
        {
            using (SqlConnection connection = new SqlConnection(MsSQLServerConnection.Connection))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = query;
                    var sqlData = new SqlDataAdapter(command);
                    DataTable data = new DataTable();
                    try
                    {
                        connection.Open();
                        sqlData.Fill(data);
                        return Convert.ToInt32(data.Rows[0].ItemArray[0]);
                    }
                    catch
                    {
                        throw new Exception($"Can't execute query: {query}");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

        }

        public static List<string> ExecuteQueryReturnStrings(string query)
        {
            using (SqlConnection connection = new SqlConnection(MsSQLServerConnection.Connection))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = query;
                    var sqlData = new SqlDataAdapter(command);
                    DataTable countries = new DataTable();
                    try
                    {
                        connection.Open();
                        var strings = new List<string>();
                        sqlData.Fill(countries);
                        foreach (DataRow row in countries.Rows)
                        {
                            List<string> rowData = new List<string>();
                            foreach(var rowItem in row.ItemArray)
                            {
                                rowData.Add($"{rowItem}");
                            }
                            string allColumnsInRows = string.Join(' ', rowData);
                            strings.Add(allColumnsInRows);
                        }
                        return strings;
                    }
                    catch
                    {
                        throw new Exception($"Can't execute query: {query}");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

        }

        public static void ExecuteQuery(string query)
        {
            using (SqlConnection cnn = new SqlConnection(MsSQLServerConnection.Connection))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = cnn;
                    command.CommandType = CommandType.Text;
                    command.CommandText = query;
                    try
                    {
                        cnn.Open();
                        int recordsAffected = command.ExecuteNonQuery();
                    }
                    catch
                    {
                        throw new Exception($"Can't execute query: {query}");
                    }
                    finally
                    {
                        cnn.Close();
                    }
                }
            }

        }



    }
}
