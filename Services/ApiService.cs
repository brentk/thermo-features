using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ThermoFeatures.Models;

namespace ThermoFeatures {

    public class ApiService {

        public ApiService (){
        }

        public ThermostatLog Save() {

            ThermostatLog response = null;

            using (var db = new ThermoFeaturesDbContext()) {

                DateTimeOffset tolerance = DateTimeOffset.Now.AddHours(-5);

                response = db.ThermostatLogs
                    .Where(l => l.Stamp > tolerance)
                    .OrderByDescending(l => l.Stamp).FirstOrDefault();

                if (response == null) {
                    Console.WriteLine("No recent thermostat log found, pulling new records:");
                    List<Device> devices = GetDetails();
                    string json = JsonConvert.SerializeObject(devices);

                    response = new ThermostatLog() {
                        Id = Guid.NewGuid().ToString(),
                        Json = json,
                        Stamp = DateTimeOffset.Now,
                    };

                    db.Add(response);
                    db.SaveChanges();
                } else {
                    Console.WriteLine($"Using thermostat log pulled at: {response.Stamp.ToString("M/d/yyyy h:mmtt")}");
                }
            }

            return response;
        }

        protected List<Device> GetDetails() {
            List<Device> response = new List<Device>();

            WebRequest webRequest = WebRequest.Create("https://data.energystar.gov/resource/7p2p-wkbf.json");
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            string json = null;
            using(var reader = new StreamReader(webResponse.GetResponseStream())) {
                json = reader.ReadToEnd();
            }

            List<EnergyStarThermostat> thermostats = JsonConvert.DeserializeObject<List<EnergyStarThermostat>>(json);

            foreach (EnergyStarThermostat thermostat in thermostats) {
                Device device = new Device() {
                    Brand = thermostat.Brand,
                    ModelName = thermostat.ModelName,
                    ModelNumber = thermostat.ModelNumber,
                    Features = GetFeatures(thermostat),
                };
                response.Add(device);
            }

            return response;
        }

        protected List<string> GetFeatures(EnergyStarThermostat thermostat) {
            List<string> response = new List<string>();

            string featuresString = thermostat.Features ?? "";
            string otherFeaturesString = thermostat.OtherFeatures ?? "";
            string communicationString = thermostat.CommunicationMethods ?? "";

            List<string> featureTokens = featuresString.Split(',').ToList();
            foreach (string token in featureTokens) {
                string value = token.Trim();
                if (!string.IsNullOrEmpty(value)) {
                    response.Add(value);
                }
            }

            // These are separated by linebreaks for some reason?
            /* These are not normalized, commenting out for now
            List<string> otherFeatureTokens = otherFeaturesString.Split('\n').ToList();
            foreach (string token in otherFeatureTokens) {
                string value = token.Trim();
                if (!string.IsNullOrEmpty(value)) {
                    response.Add(value);
                }
            }
            */

            List<string> communicationTokens = communicationString.Split(',').ToList();
            foreach (string token in communicationTokens) {
                string value = token.Trim();
                if (!string.IsNullOrEmpty(value)) {
                    response.Add(value);
                }
            }

            response = response.Distinct().ToList();
            return response;
        }
    }
}