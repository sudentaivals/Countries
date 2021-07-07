using RESTCountries.Models;
using RESTCountries.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace CRMGURUTest
{
    public class CountryContainer
    {

        public CountryInfo Country { get; private set; }

        public CountryContainer(string countryName)
        {
            Task setCountry = SetCountry(countryName);
            setCountry.Wait();
        }

        private async Task SetCountry(string countryName)
        {
                    Country country = await RESTCountriesAPI.GetCountryByFullNameAsync(countryName);
                    Country = CountryInfo.ConvertCountry(country);
        }

        public string TryAddOrUpdateCountry()
        {
            //check for existing in db
            bool isCountryExistsInDb;
            try
            {
                isCountryExistsInDb = IsCountryExistsInDatabase();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            int countryId = CalculateCountryId();
            if (countryId == -1)
            {
                return "Failed to calculate country Id";
            }
            int capitalId = GetCapitalId();
            if (capitalId == -1)
            {
                return "Failed to get capital Id";
            }
            int regionId = GetRegionId();
            if (regionId == -1)
            {
                return "Failed to get region Id";
            }


            if (isCountryExistsInDb)
            {
                return UpdateCountry(countryId, capitalId, regionId);
            }
            else
            {
                return AddCountryToDatabase(countryId, capitalId, regionId);
            }
        }

        private string UpdateCountry(int countryId, int capitalId, int regionId)
        {
            try
            {
                string query = $"UPDATE countries " +
                    $"SET country_alpha3code = '{Country.Alpha3Code}', fk_capital = '{capitalId}', fk_region_id = '{regionId}', area = {Country.Area}, country_population = {Country.Population} " +
                    $"WHERE country_id = {countryId} ";
                QueryExecuter.ExecuteQuery(query);
                return $"{Country.Name} successfully updated!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string AddCountryToDatabase(int countryId, int capitalId, int regionId)
        {
            try
            {
                string query = $"INSERT INTO countries VALUES ({countryId}, '{Country.Name}', {capitalId}, {regionId}, '{Country.Alpha3Code}', {Country.Area}, {Country.Population})";
                QueryExecuter.ExecuteQuery(query);
                return $"{Country.Name} successfully added to database!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        private int GetRegionId()
        {
            try
            {
                string getRegionIdQuery = $"SELECT region_id FROM regions WHERE region_name = '{Country.Region}'";
                int regionId = QueryExecuter.ExecuteQueryReturnInt(getRegionIdQuery);
                return regionId;
            }
            catch
            {
                return -1;
            }
        }

        private int GetCapitalId()
        {
            try
            {
                string getCapitalIdQuery = $"SELECT city_id FROM cities WHERE city_name = '{Country.Capital}'";
                int capitalId = QueryExecuter.ExecuteQueryReturnInt(getCapitalIdQuery);
                return capitalId;
            }
            catch
            {
                return -1;
            }
        }

        private int CalculateCountryId()
        {
            try
            {
                string getCountryIdQuery = "SELECT COUNT(country_id) FROM countries";
                int currentNumberOfCountries = QueryExecuter.ExecuteQueryReturnInt(getCountryIdQuery);
                if (currentNumberOfCountries == 0)
                {
                    return 1;
                }
                else
                    try
                    {
                        string maxCountryIdQuery = "SELECT MAX(country_id) FROM countries";
                        return QueryExecuter.ExecuteQueryReturnInt(maxCountryIdQuery) + 1;
                    }
                    catch (Exception ex)
                    {
                        return -1;
                    }
            }
            catch
            {
                return -1;
            }

        }

        public string TryAddCityToDatabase()
        {
            //check for existing in db
            bool iscityExistsInDb;
            try
            {
                iscityExistsInDb = IsCityExistsInDatabase();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            if (iscityExistsInDb)
            {
                return $"City({Country.Capital}) already exists in database";
            }
            //check city id - if count = 0, id = 1
            int cityId;
            try
            {
                string citiesCountQuery = "SELECT COUNT(city_id) FROM cities";
                int currentNumberOfCities = QueryExecuter.ExecuteQueryReturnInt(citiesCountQuery);
                if (currentNumberOfCities == 0)
                {
                    cityId = 1;
                }
                else
                {
                    try
                    {
                        string calculateCityIdQuery = "SELECT MAX(city_id) FROM cities";
                        cityId = QueryExecuter.ExecuteQueryReturnInt(calculateCityIdQuery) + 1;
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            //add city to db
            try
            {
                string addCityQuery = $"INSERT INTO cities VALUES ({cityId}, '{Country.Capital}')";
                QueryExecuter.ExecuteQuery(addCityQuery);
                return $"Successful added city({Country.Capital}) to database!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public string TryAddRegionToDatabase()
        {
            //check for existing in db
            bool isRegionExistsInDb;
            try
            {
                isRegionExistsInDb = IsRegionExistsInDatabase();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            if (isRegionExistsInDb)
            {
                return $"Region({Country.Region}) already exists in database";
            }

            //check region id - if count = 0, id = 1
            int regionId;
            try
            {
                string regionsCountQuery = "SELECT COUNT(region_id) FROM regions";
                int currentNumberOfRegions = QueryExecuter.ExecuteQueryReturnInt(regionsCountQuery);
                if(currentNumberOfRegions == 0)
                {
                    regionId = 1;
                }
                else
                {
                    try
                    {
                        string calculateRegionIdQuery = "SELECT MAX(region_id) FROM regions";
                        regionId = QueryExecuter.ExecuteQueryReturnInt(calculateRegionIdQuery) + 1;
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            //add region to db
            try
            {
                string query = $"INSERT INTO regions VALUES ({regionId}, '{Country.Region}')";
                QueryExecuter.ExecuteQuery(query);
                return $"Successful added region({Country.Region}) to database!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        private bool IsRegionExistsInDatabase()
        {
            string query = $"SELECT COUNT(region_id) FROM regions WHERE region_name = '{Country.Region}'";
            try
            {
                int numberOfRegions = QueryExecuter.ExecuteQueryReturnInt(query);
                if(numberOfRegions == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool IsCityExistsInDatabase()
        {
            try
            {
                string query = $"SELECT COUNT(city_id) FROM cities WHERE city_name = '{Country.Capital}'";
                int numberOfRegions = QueryExecuter.ExecuteQueryReturnInt(query);
                if (numberOfRegions == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool IsCountryExistsInDatabase()
        {
            try
            {
                string query = $"SELECT COUNT(country_id) FROM countries WHERE country_name = '{Country.Name}'";
                int numberOfRegions = QueryExecuter.ExecuteQueryReturnInt(query);
                if (numberOfRegions == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


    }
}
