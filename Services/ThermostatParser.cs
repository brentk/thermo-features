using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ThermoScrape.Models;

namespace ThermoScrape {
    public class ThermostatParser {
        public void Parse(ThermostatLog log) {
            List<Device> devices = JsonConvert.DeserializeObject<List<Device>>(log.Json);

            devices = devices
                .OrderBy(d => d.Brand)
                .ThenBy(d => d.ModelName)
                .ToList();

            List<string> features = GetAllFeatures(devices);

            string line = "Brand, Model, Model #, ";
            line += string.Join(", ", features);
            Console.WriteLine(line);

            foreach (Device device in devices) {
                line = $"{device.Brand}, {device.ModelName}, {device.ModelNumber}, ";

                List<string> supportedFeatures = new List<string>();
                foreach (string feature in features) {
                    string supported = ""; 
                    if (device.Features.Contains(feature)) {
                        supported = "1"; 
                    }
                    supportedFeatures.Add(supported);
                }
                line += string.Join(", ", supportedFeatures);
                Console.WriteLine(line);
            }
        }

        protected List<string> GetAllFeatures(List<Device> devices) {
            List<string> globalFeatures = new List<string>();
            foreach (Device device in devices) {
                foreach (string feature in device.Features) {
                    if (!globalFeatures.Contains(feature)) {
                        globalFeatures.Add(feature);
                    }
                }
            }
            return globalFeatures;
        }


    }
}