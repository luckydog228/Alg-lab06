using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace WeatherApp
{
    // Структура Weather
    public struct Weather
    {
        public string Country;
        public string Name;
        public double Temp;
        public string Description;
    }

    class Program
    {
        // Метод для получения случайных значений координат
        private static double GetRandomCoordinate(Random rand, double minValue, double maxValue)
        {
            return rand.NextDouble() * (maxValue - minValue) + minValue;
        }

        // Метод для получения погоды по координатам
        private static async Task<Weather> GetWeatherAsync(double latitude, double longitude)
        {
            // Вставьте ваш API key
            string apiKey = "476e3f5a9650b91350c682a966f8d4e6";
            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<dynamic>();
                    Weather weather = new Weather
                    {
                        Country = (string)data.sys.country,
                        Name = (string)data.name,
                        Temp = (double)data.main.temp,
                        Description = (string)data.weather[0].description
                    };
                    return weather;
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        static async Task Main(string[] args)
        {
            List<Weather> weatherData = new List<Weather>();

            Random rand = new Random();
            while (weatherData.Count < 50)
            {
                double latitude = GetRandomCoordinate(rand, -90, 90);
                double longitude = GetRandomCoordinate(rand, -180, 180);

                try
                {
                    Weather weather = await GetWeatherAsync(latitude, longitude);
                    if (string.IsNullOrWhiteSpace(weather.Country) || string.IsNullOrWhiteSpace(weather.Name))
                    {
                        continue;
                    }

                    weatherData.Add(weather);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }

            // Задание 1: Страна с максимальной температурой
            var maxTempCountry = weatherData.OrderByDescending(w => w.Temp).First().Country;
            Console.WriteLine($"Страна с максимальной температурой: {maxTempCountry}");

            // Задание 1: Страна с минимальной температурой
            var minTempCountry = weatherData.OrderBy(w => w.Temp).First().Country;
            Console.WriteLine($"Страна с минимальной температурой: {minTempCountry}");

            // Задание 2: Средняя температура в мире
            var averageTemp = weatherData.Select(w => w.Temp).Average();
            Console.WriteLine($"Средняя температура в мире: {averageTemp}");

            // Задание 3: Количество стран в коллекции
            var distinctCountries = weatherData.Select(w => w.Country).Distinct().Count();
            Console.WriteLine($"Количество стран в коллекции: {distinctCountries}");

            // Задание 4: Первая найденная страна и название местности с Description "clear sky", "rain", "few clouds"
            var weatherWithClearSky = weatherData.FirstOrDefault(w => w.Description == "clear sky");
            if (weatherWithClearSky.Country != null && weatherWithClearSky.Name != null)
            {
                Console.WriteLine($"Страна с погодой 'clear sky': {weatherWithClearSky.Country}");
                Console.WriteLine($"Местность с погодой 'clear sky': {weatherWithClearSky.Name}");
            }

            var weatherWithRain = weatherData.FirstOrDefault(w => w.Description == "rain");
            if (weatherWithRain.Country != null && weatherWithRain.Name != null)
            {
                Console.WriteLine($"Страна с погодой 'rain': {weatherWithRain.Country}");
                Console.WriteLine($"Местность с погодой 'rain': {weatherWithRain.Name}");
            }

            var weatherWithFewClouds = weatherData.FirstOrDefault(w => w.Description == "few clouds");
            if (weatherWithFewClouds.Country != null && weatherWithFewClouds.Name != null)
            {
                Console.WriteLine($"Страна с погодой 'few clouds': {weatherWithFewClouds.Country}");
                Console.WriteLine($"Местность с погодой 'few clouds': {weatherWithFewClouds.Name}");
            }

        }
    }
}