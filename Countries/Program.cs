using System;
using System.Collections.Generic;
using RESTCountries.Models;
using RESTCountries.Services;
using System.Threading.Tasks;
using System.Net;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Reflection;

namespace CRMGURUTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MsSQLServerConnection.SetConnection();
            var program = new CountryProgram();
            program.SendMessage += HandlerSendMessage;
            program.GetUserInput += HandlerGetUserInput;

            while(program.State == ProgramState.Executing)
            {
                program.Start();
            }

        }

        private static void HandlerSendMessage(object sender, SendMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private static string HandlerGetUserInput()
        {
            return Console.ReadLine();
        }


    }
}
