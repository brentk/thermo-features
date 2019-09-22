using System;
using Newtonsoft.Json;

namespace ThermoFeatures.Models {

    public class EnergyStarThermostat
    {
        [JsonProperty("pd_id")]
        public string Id { get; set; }
        public string energy_star_partner { get; set; }
        [JsonProperty("brand_name")]
        public string Brand { get; set; }
        [JsonProperty("model_name")]
        public string ModelName { get; set; }
        [JsonProperty("model_number")]
        public string ModelNumber { get; set; }
        public string additional_model_information { get; set; }
        public string ct_device_brand_owner { get; set; }
        public string ct_device_brand_name { get; set; }
        public string ct_device_model_name { get; set; }
        public string ct_device_model_number { get; set; }
        public string additional_ct_device_model_numbers { get; set; }
        public string family_id { get; set; }
        public string static_temperature_accuracy { get; set; }
        public string network_standby_average_power_consumption { get; set; }
        public string time_to_enter_network_standby_after_user_interaction { get; set; }
        [JsonProperty("ct_product_heating_and_cooling_control_features")]
        public string Features { get; set; }
        [JsonProperty("other_heating_and_cooling_control_features")]
        public string OtherFeatures { get; set; }
        public string CommunicationMethods { get; set; }
        [JsonProperty("communication_method_other")]
        public string communication_method_other { get; set; }
        public string demand_response_summary { get; set; }
        public string demand_response_product_variations { get; set; }
        public DateTime date_available_on_market { get; set; }
        public DateTime date_qualified { get; set; }
        public string markets { get; set; }
        public string energy_star_model_identifier { get; set; }
    }
}