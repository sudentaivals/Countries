using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CRMGURUTest
{
    public enum ProgramState
    {
        Executing,
        Quit
    }

    public class CountryProgram
    {
        public ProgramState State { get; private set; } = ProgramState.Executing;

        public event Func<string> GetUserInput;

        public event EventHandler<SendMessageEventArgs> SendMessage;

        public void Start()
        {
            OnSendMessage(ShowMainMenuText());
            string input = GetUserInput();
            switch (input)
            {
                case "1":
                    OnSendMessage("Please, enter country name:");
                    string countryName = GetUserInput();
                    CountryContainer country = null;
                    try
                    {
                        country = new CountryContainer(countryName);
                        OnSendMessage(country.Country.ToString() + "\n");
                    }
                    catch
                    {
                        OnSendMessage("Can't find country with this name");
                    }
                    if (country != null && MsSQLServerConnection.IsServerEnable)
                    {
                        OnSendMessage("Would you like to add this country to database?");
                        OnSendMessage("Enter 'Y' or 'y' to add country\nEnter something else to return to main menu");
                        string userInput = GetUserInput();
                        if (userInput == "Y" || userInput == "y")
                        {
                            OnSendMessage(country.TryAddRegionToDatabase());
                            OnSendMessage(country.TryAddCityToDatabase());
                            OnSendMessage(country.TryAddOrUpdateCountry());
                        }
                    }
                    break;
                case "2":
                    if (MsSQLServerConnection.IsServerEnable)
                    {
                        List<string> countries = GetCountriesFromDatabase();
                        OnSendMessage("Name | Alpha | Capital | Area | Population | Region");
                        foreach(var countryInfo in countries)
                        {
                            OnSendMessage(countryInfo);
                        }
                    }
                    break;
                case "3":
                    OnSendMessage(ShowChangeServerText());
                    ChangeServer();
                    break;
                case "4":
                    ExitProgram();
                    break;
                default:
                    OnSendMessage("Use correct input, please");
                    break;
            }
        }

        private List<string> GetCountriesFromDatabase()
        {
            string query = @"SELECT country_name, country_alpha3code, city_name, area, country_population, region_name 
                                            FROM countries 
                                            INNER JOIN regions ON countries.fk_region_id = regions.region_id 
                                            INNER JOIN cities ON fk_capital = city_id;";
            try
            {
                var countries =  QueryExecuter.ExecuteQueryReturnStrings(query);
                return countries;
            }
            catch (Exception ex)
            {
                OnSendMessage(ex.Message);
            }
            return new List<string>();
        }

        private void ExitProgram()
        {
            State = ProgramState.Quit;
        }

        private string ShowMainMenuText()
        {
            return $"Connected to SQL server: { MsSQLServerConnection.IsServerEnable}\n" + "Enter '1' to get information about country\nEnter '2' to get all countries from database\nEnter '3' to change server\nEnter '4' to exit";
        }

        private string ShowChangeServerText()
        {
            return $"Current connection: {MsSQLServerConnection.Connection}\n" + "Enter new connection:";
        }

        private void ChangeServer()
        {
            string conenctionString = GetUserInput();
            MsSQLServerConnection.SetConnection(conenctionString);
        }

        private void OnSendMessage(string message)
        {
            if (SendMessage != null)
            {
                SendMessage(this, new SendMessageEventArgs(message));
            }
        }

    }
}
