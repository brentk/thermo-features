using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Newtonsoft.Json;

namespace ThermoScrape {

    public class Device {
        public string Brand {get; set;}
        public string ModelName {get; set;}
        public string ModelNumber {get; set;}
        public List<string> Features {get; set;}
    }

    public class ScraperService {

        public ScraperService (){
        }

        public void Scrape() {
            List<IDocument> pages = GetAllPages();
            List<string> deviceUrls = GetDetailUrls(pages);
            List<Device> devices = GetDetails(deviceUrls);
            string json = JsonConvert.SerializeObject(devices);
        }

        protected List<Device> GetDetails(List<string> urls) {
            List<Device> response = new List<Device>();

            ParallelOptions options = new ParallelOptions() {
                MaxDegreeOfParallelism = 4
            };

            Parallel.ForEach(urls.GetRange(0, 8), options, url => {
                IDocument deviceDocument = GetDocument(url);
                var detail = GetDeviceDetail(deviceDocument);
                response.Add(detail);
                Console.WriteLine($"Device \"{detail.Brand}-{detail.ModelName}\" detail retrieved.");
            });

            return response;
        }

        protected Device GetDeviceDetail(IDocument deviceDocument) {
            Device response = new Device();
            response.Features = new List<string>();

            IElement table = deviceDocument.QuerySelector("#product-data-table");
            if (table == null) {
                throw new Exception("Unable to find product-data-table");
            }

            List<IElement> rows = table.QuerySelectorAll("tr").ToList();

            response.Brand = GetValue(rows, "Thermostat Brand Name");
            response.ModelName = GetValue(rows, "Thermostat Model Name");
            response.ModelNumber = GetValue(rows, "Thermostat Model Number");
            response.Features = GetFeatures(rows);


            return response;
        }

        protected List<string> GetFeatures(List<IElement> rows) {
            List<string> response = new List<string>();

            string featuresString = GetValue(rows, "Thermostat Heating and Cooling Control Features") ?? "";
            string communicationString = GetValue(rows, "Thermostat Communication Method") ?? "";

            List<string> featureTokens = featuresString.Split(',').ToList();
            foreach (string token in featureTokens) {
                response.Add(token.Trim());
            }

            List<string> communicationTokens = communicationString.Split(',').ToList();
            foreach (string token in communicationTokens) {
                response.Add(token.Trim());
            }

            response = response.Distinct().ToList();
            return response;
        }

        protected string GetValue(List<IElement> rows, string key) {
            IElement row = rows.Where(r => r.QuerySelectorAll("td").Where(td => td.InnerHtml.Trim().StartsWith(key)).Count() > 0).SingleOrDefault();
            if (row == null) {
                throw new Exception($"Unable to find detail row for {key}");
            }

            List<IElement> cells = row.QuerySelectorAll("td").ToList();
            if (cells.Count < 2) {
                throw new Exception($"Row for \"{key}\" does not cotain at least two cells");
            }

            if (cells[1].InnerHtml == null) {
                throw new Exception($"Value cell for row \"{key}\" is null");
            }

            string value = cells[1].InnerHtml.Trim();
            return value;
        }

        protected List<string> GetDetailUrls(List<IDocument> pages) {
            List<string> links = new List<string>();

            Console.WriteLine("Retrieving list of device urls...");
            foreach (IDocument document in pages) {
                List<IElement> detailButtons = document.QuerySelectorAll(".detailsbutton").ToList();
                foreach (IElement button in detailButtons) {
                    string url = button.GetAttribute("href");
                    url = CorrectDeviceUrl(url);
                    links.Add(url);
                }
            }
            Console.WriteLine($"\tDone, found {links.Count} device urls");

            return links;
        }

        protected string CorrectDeviceUrl(string url) {
            string deviceId = url.Replace("../../product/certified-connected-thermostats/details-plus/", "");
            deviceId = deviceId.Replace("#PriceAndLocation", "");
            url = $"https://www.energystar.gov/productfinder/product/certified-connected-thermostats/details-plus/{deviceId}";
            return url;
        }

        protected List<IDocument> GetAllPages() {
            List<IDocument> pages = new List<IDocument>();

            // Pull the first page so we can a) get the page count at the bottom and b) cache the contents for scraping
            Console.WriteLine("Getting first page with page count...");
            IDocument firstPage = GetPage(0);
            List<IElement> pageNumbers = firstPage.QuerySelectorAll(".page_number").ToList();
            int pageCount = pageNumbers.Count;
            Console.WriteLine($"\tDone, found {pageCount} pages.");

            //Skipping first index since we already got it when pulling the page count
            Console.WriteLine("Getting other pages... ");
            for (int i = 1; i < pageCount; i++) {
                Console.WriteLine($"\tGetting page {i+1}...");
                pages.Add(GetPage(i));
                Console.WriteLine($"\tDone.");
            }
            Console.WriteLine($"{pageCount} pages retrieved.");

            return pages;
        }

        protected IDocument GetPage(int pageNumber) {
            string mainUrl = @"https://www.energystar.gov/productfinder/product/certified-connected-thermostats/results"
                + "?formId=bd3f0862-ca59-4611-8f28-d636dd956d1d"
                + "&markets_filter=United+States&page_number=%PAGE_NUMBER%";

            string url = mainUrl.Replace("%PAGE_NUMBER%", pageNumber.ToString());
            IDocument document = GetDocument(url);
            return document;
        }

        protected IDocument GetDocument(string url) {
            IConfiguration config = Configuration.Default.WithDefaultLoader();
            IDocument document = BrowsingContext
                .New(config)
                .OpenAsync(url)
                .GetAwaiter()
                .GetResult();

            return document;

        }



    }
}