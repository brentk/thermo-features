using System.Collections.Generic;

namespace ThermoFeatures.Models {

    public class Device {
        public string Brand {get; set;}
        public string ModelName {get; set;}
        public string ModelNumber {get; set;}
        public List<string> Features {get; set;}
    }

}