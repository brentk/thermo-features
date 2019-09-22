using System;
using System.IO;

namespace ThermoScrape
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists("local.db")) {
                throw new Exception("local.db not found, please run \"dotnet ef database update\" to build initial database.");
            }

            ScraperService service = new ScraperService();
            service.Scrape();
        }
    }
}
