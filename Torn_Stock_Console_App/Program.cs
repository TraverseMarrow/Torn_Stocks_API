using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Torn_Stock_API_Script;
using Torn_Stock_Console_App;
using static System.Net.WebRequestMethods;

/* --------------------Description--------------------
 * This is a Console App that asks you for your API from the
 * text-based MMORPG TORN and returns your currently owned
 * stock information. Currently output as a few blocks of 
 * info but could be parsed into sections for excel sheet
 * or bigger app. 
 * 
 * --------------------Security Risks--------------------
 * Security risks could be if a user takes the code and inserts
 * API in place of the manual entry. This could be shared by accident
 * and used by those who are not supposed to have access.
 * 
 * --------------------Comments--------------------
 * Mostly just for practice with API requests.
 * 
 * Warning is present but could be fixed with the simple
 * check loop I made so the value can never be null. 
 * 
 * --------------------Links & Refrence--------------------
 * Stock IDs
 * https://www.torn.com/api.html#
 * 
 * Torn API Docs
 * https://www.torn.com/swagger.php#/Torn/2baae03f953cd57fd5303dd1d04efae0
 * https://tornapi.tornplayground.eu/
 */




namespace Torn_Stock_API_Script
{
    internal class Program()
    {
        public static void Main()
        {
            bool Is_Complete;
            var program = new Program();
            Is_Complete = program.API_Request();
        }

        private bool API_Request()
        {
            string key = "";

            while (string.IsNullOrWhiteSpace(key))
            {
                Console.WriteLine("Please enter your limited access API: ");
                key = Console.ReadLine();
                
            }
            string url = $"https://api.torn.com/user/?key={key}comment=TornAPI&selections=stocks";
            using var client = new HttpClient();
            client.BaseAddress = new Uri(url);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));

            // Get data response
            var response = client.GetAsync(url).Result;
            
            if (response.IsSuccessStatusCode)
            {
                Parse_Response(response);
                
                return true;
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode,
                              response.ReasonPhrase);
                return false;
            }
        }
        
        private async void Parse_Response(HttpResponseMessage response)
        {
            // Parse the response body
            var dataObjects = await response.Content.ReadAsStringAsync();

            if (dataObjects.Contains("\"error\""))
            {
                Console.WriteLine("API returned an error, please check it and try again.");
                return;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var stockData = JsonSerializer.Deserialize<TornStockResponse>(dataObjects, options);

            if (stockData?.Stocks == null)
            {
                Console.WriteLine("No stock data found.");
                return;
            }

            double totalCost = 0;
            var totalCostsByID = new Dictionary<int, double>();

            foreach (var stockEntry in stockData.Stocks)
            {
                var stock = stockEntry.Value;

                if (stock.Transactions != null)
                {
                    foreach (var transaction in stock.Transactions.Values)
                    {
                        totalCost += transaction.Shares * transaction.BoughtPrice;
                    }
                }

                totalCostsByID[stock.StockId] = totalCost;
                totalCost = 0;
            }


            Console.WriteLine("Your Stocks:");
            foreach (var data in stockData.Stocks)
            {
                var stock = data.Value;
                Console.WriteLine($"\nStock ID: {stock.StockId}");
                Console.WriteLine($"Shares: {stock.Shares.ToString("N0")}");
                Console.WriteLine($"Total Cost: ${totalCostsByID[stock.StockId].ToString("N2")}");
            }

        }
    }
}