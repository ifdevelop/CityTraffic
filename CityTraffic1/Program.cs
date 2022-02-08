using System;
using System.Collections.Generic;
using System.Linq;

namespace CityTraffic1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Тестовые строки
            string s1 = "[\"1:[5]\", \"2:[5]\", \"3:[5]\", \"4:[5]\", \"5:[1,2,3,4]\"]";
            string s2 = "[\"1:[5]\", \"2:[5,18]\", \"3:[5,12]\", \"4:[5]\", \"5:[1,2,3,4]\", \"18:[2]\", \"12:[3]\"]";
            string s3 = "[\"1:[5]\", \"4:[5]\", \"3:[5]\", \"5:[1,4,3,2]\", \"2:[5,15,7]\", \"7:[2,8]\", \"8:[7,38]\", \"15:[2]\", \"38:[8]\"]";

            Console.WriteLine(CityTraffic(s1));
            Console.ReadKey();

        }

        static string CityTraffic(string strArr)
        {
            Dictionary<int, City> populationCities = ParsePopulationCitiesFromString(strArr);

            Dictionary<City, int> citiesTraffic = GetCitiesTraffic(populationCities);

            return MakeResultString(citiesTraffic); 
        }

        static Dictionary<int, City> ParsePopulationCitiesFromString(string strArr)
        {
            Dictionary<int, City> populationCities = new Dictionary<int, City>();

            string[] cities = strArr.Split("\"").ToArray();

            foreach(var city in cities)
            {
                // Информация о городе должна содержать не меньше 5 символов. Например, 1:[5]
                if (city.Length < 5)
                    continue;

                string[] populationAndNeighbours = city.Split(new char[] { ':', ',', '[', ']' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

                int population = Int32.Parse(populationAndNeighbours[0]);
                
                HashSet<int> adjacentCities = new HashSet<int>();

                for (int i = 1; i < populationAndNeighbours.Length; i++)
                {
                    adjacentCities.Add(Int32.Parse(populationAndNeighbours[i].ToString()));
                }

                City newCity = new City(population, adjacentCities, populationCities);
                populationCities.Add(population, newCity);
            }

            return populationCities;
        }

        static Dictionary<City, int> GetCitiesTraffic(Dictionary<int, City> populationCities)
        {
            Dictionary<City, int> citiesTraffic = new Dictionary<City, int>();

            foreach (var populationCity in populationCities)
            {
                int maxTraffic = populationCity.Value.GetMaxTraffic(0);

                citiesTraffic.Add(populationCity.Value, maxTraffic);
            }

            return citiesTraffic;
        }

        public static string MakeResultString(Dictionary<City, int> citiesTraffic)
        {
            return string.Join(",", citiesTraffic.OrderBy(c => c.Key.Population).Select(c => $"{c.Key.Population}:{c.Value}"));
        }
    }

    public struct City
    {
        private readonly int population;
        private readonly HashSet<int> adjacentCities;
        private readonly Dictionary<int, City> allCities;

        public int Population => population;

        public HashSet<int> AdjacentCities => adjacentCities;

        
        public City(int population, HashSet<int> adjacentCities, Dictionary<int, City> allCities)
        {
            this.population= population;
            this.adjacentCities = adjacentCities;
            this.allCities = allCities;
        }

        
        public int GetMaxTraffic(int trafficTo)
        {
            int maxTraffic = 0;
            int traffic = 0;

            foreach (var city in AdjacentCities)
            {
                // Пропускаем город, для которого идет поиск
                if (city == trafficTo) 
                    continue;

                // Если у города нет других соседей и это не начальный город
                if (allCities[city].AdjacentCities.Count < 2 && trafficTo != 0)
                    traffic += allCities[city].Population;
                // Если это начальный город и у соседнего города нет других соседей
                else if(allCities[city].AdjacentCities.Count < 2 && trafficTo == 0)
                    traffic = allCities[city].Population;
                else
                {
                    traffic += allCities[city].GetMaxTraffic(Population);
                }

                if (traffic > maxTraffic)
                    maxTraffic = traffic;

                if (trafficTo == 0)
                    traffic = 0;
            }

            if (trafficTo == 0)
                return maxTraffic;

            return Population + maxTraffic;
        }
    }
}

