using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace TyreDegradation
{
    /// <summary>
    /// Part of the Model - holds tyre data
    /// </summary>
    public class Track
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public int[] DegCoefficients { get; set; }

        public int Temperature { get; set; }

        HttpClient client = new HttpClient();

        public Track(string name, string location, int[] degCoefficients, int temperature)
        {
            Name = name;
            Location = location;
            DegCoefficients = degCoefficients;
            Temperature = temperature;

        }
        public async void GetCurrentTempAsync()
        {
            try
            {
                string requestURL = String.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&appid=77e2991a0ab1fb9a2e9698513d743475&mode=xml", Location);
                HttpResponseMessage response = await client.GetAsync(requestURL);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                XDocument weatherData = XDocument.Parse(responseBody);

                Temperature = (int)double.Parse(weatherData.Element("current").Element("temperature").Attribute("value").Value);
                Temperature -= 273;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }


        }

    }
}
