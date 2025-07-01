using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Torn_Stock_Console_App
{

    public class Stock
    {
        [JsonPropertyName("stock_id")]
        public int StockId { get; set; }

        [JsonPropertyName("total_shares")]
        public int Shares { get; set; }

        [JsonPropertyName("transactions")]
        public required Dictionary<string, Transaction> Transactions { get; set; }
    }

    public class Transaction
    {
        [JsonPropertyName("shares")]
        public long Shares { get; set; }

        [JsonPropertyName("bought_price")]
        public double BoughtPrice { get; set; }

        [JsonPropertyName("time_bought")]
        public long TimeBought {  get; set; }

    }

    public class TornStockResponse
    {
        [JsonPropertyName("stocks")]
        public required Dictionary<string, Stock> Stocks { get; set; }
    }

    
}
