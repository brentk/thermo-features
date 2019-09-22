using System;
using System.IO;

namespace ThermoFeatures
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists("local.db")) {
                throw new Exception("local.db not found, please run \"dotnet ef database update\" or the \"Run Migrations\" build step to build initial database.");
            }

            ApiService service = new ApiService();
            ThermostatLog log = service.Save();
            ThermostatParser parser = new ThermostatParser();
            parser.Parse(log);
        }
    }
}
