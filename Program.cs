using System;

namespace ThermoScrape
{
    class Program
    {
        static void Main(string[] args)
        {
            ScraperService service = new ScraperService();
            service.Scrape();
        }
    }
}
