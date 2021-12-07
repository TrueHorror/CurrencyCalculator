using CurrencyCalculator.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurrencyCalculator
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        static void Main(string[] args)
        {

            CurrencyCodesAndValuePromptAsync();
            
        }

        private static void CurrencyCodesAndValuePromptAsync()
        {
            Console.WriteLine("Welcome to Currency Calculator\nPlease enter first currency: ");
            string firstCurrCode = Console.ReadLine();

            Console.WriteLine("Please enter second currency: ");
            string secondCurrCode = Console.ReadLine();

            Console.WriteLine($"Enter value/amount of {firstCurrCode}: ");
            string currencyValInput = Console.ReadLine();

            if (!int.TryParse(currencyValInput, out _))
            {
                CurrencyCodesAndValuePromptAsync(); // Try again with a number when asked to enter value/amount
                
            }else
            {
                int currencyVal = int.Parse(currencyValInput);
                RunCalculation(currencyVal, firstCurrCode, secondCurrCode).GetAwaiter().GetResult(); ;

            }

            Console.ReadLine();
        }

        private static async Task RunCalculation(int currencyVal, string firstCurrCode, string secondCurrCode)
        {
            string accessKey = "5aa758d8aba66377c93879b314ef80cf"; //Usually, i NEVER keep private keys in the code ;)
            client.BaseAddress = new Uri("http://data.fixer.io/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            //Get latest rates

            //Base is EUR, get firstCurrCode value in EUR and then secondCurrCode value in firstCurrCode

            //EUR: 1, NOK:10 10/10.2311243 * secondCurrCode
            
            
            //return firstCurrCode value in secondCurrCode

            try
            {
                //Get exchange rate of selected currencies
                // 
                var toCurrency = await GetExchangeRates($"latest?access_key={accessKey}"); // object with latest rates based on EUR. Need to convert EUR to firstCurrCode then 

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }

        private static async Task<AllCurrencies> GetExchangeRates(string path)
        {
            //TODO: Rename 'currency'
            var jsonString = "";
            
            var response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                jsonString = await response.Content.ReadAsStringAsync();

                AllCurrencies json = JsonSerializer.Deserialize<AllCurrencies>(jsonString);
                Console.WriteLine($"Date: {json.Date}\nSuccess: {json.Success}\nTimestamp: {json.Timestamp}\nBase: {json.Base}\n Rates: {json.Rates}");
                Console.ReadLine();
                return json;
            }

            return null;
           

        }
    }
}
