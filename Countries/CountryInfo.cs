using RESTCountries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRMGURUTest
{
    public class CountryInfo
    {
        public string Name { get; set; }

        public string Alpha3Code { get; set; }

        public string Capital { get; set; }

        public double? Area { get; set; }

        public int Population { get; set; }
        public string Region { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}, AlphaCode: {Alpha3Code}, Capital: {Capital}, Area: {Area}, Population: {Population}, Region: {Region}";
        }

        private CountryInfo(string name, string apla3Code, string capital, double? area, int population, string region)
        {
            Name = name;
            Alpha3Code = apla3Code;
            Capital = capital;
            Area = area;
            Population = population;
            Region = region;
        }

        public static CountryInfo ConvertCountry(Country country)
        {
            return new CountryInfo(country.Name, country.Alpha3Code, country.Capital, country.Area, country.Population, country.Region);
        }

    }
}
