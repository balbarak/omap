using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMap.Data
{
    class Program
    {
        public static string DataPath { get; } = $"{AppDomain.CurrentDomain.BaseDirectory}\\Data";

        static void Main(string[] args)
        {
            var natrualEarthPath = $"{DataPath}\\natrual-earth-country-geojson.json";

            var natrualEarthJson = File.ReadAllText(natrualEarthPath);

            dynamic schema = CreateSchema();
            dynamic countryData = JsonConvert.DeserializeObject(natrualEarthJson);

            int index = 1;
            int count = countryData.features.Count;

            foreach (dynamic item in countryData.features)
            {
                Console.WriteLine($"Proccessing {index} of {count}");
                dynamic feature = CreateCountryFeature(item);

                schema.features.Add(feature);

                index++;
            }


            var jsonData = JsonConvert.SerializeObject(schema);

            File.WriteAllText($"{DataPath}\\country-geojson.json", jsonData);

        }

        public static dynamic CreateSchema()
        {
            dynamic result = new ExpandoObject();

            result.type = "FeatureCollection";
            result.features = new List<dynamic>();

            return result;
        }
        public static dynamic CreateCountryFeature(dynamic feature)
        {
            dynamic result = new ExpandoObject();

            result.type = "Feature";
            result.id = feature.properties.iso_a3;

            result.properties = new ExpandoObject();
            result.properties.name_ar = feature.properties.name_ar;
            result.properties.name_en = feature.properties.name;
            result.properties.iso_two = feature.properties.iso_a2;
            result.properties.iso_three = feature.properties.iso_a3;
            result.properties.scalerank = feature.properties.scalerank;

            result.geometry = new ExpandoObject();
            result.geometry.type = "MultiPolygon";
            result.geometry.coordinates = new List<dynamic>();

            foreach (var item in feature.geometry.coordinates)
            {
                result.geometry.coordinates.Add(item);
            }
            

            return result;
        }
    }
}
