using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyCalculator.Models
{
    class AllCurrencies
    {
        public string Success { get; set; }
        public int Timestamp { get; set; }
        public string Base { get; set; }
        public string Date { get; set; }
        public Dictionary<string, double> Rates { get; set; }  
    }

}
