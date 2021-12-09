using CurrencyCalculator.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CurrencyCalculator
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string API_KEY = "5aa758d8aba66377c93879b314ef80cf"; //Usually, i NEVER keep private keys in the code ;)
        static void Main(string[] args)
        {
            client.BaseAddress = new Uri("http://data.fixer.io/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            CurrencyCodesAndValuePromptAsync();
            
        }

        private static void CurrencyCodesAndValuePromptAsync()
        {
            Console.WriteLine("Welcome to Currency Calculator\nPlease enter first currency: ");
            string firstCurrCode = Console.ReadLine().Trim().ToUpper();

            Console.WriteLine("Please enter second currency: ");
            string secondCurrCode = Console.ReadLine().Trim().ToUpper();

            Console.WriteLine($"Enter value/amount of {firstCurrCode}: ");
            string currencyValInput = Console.ReadLine();

            int currencyVal;
            if (int.TryParse(currencyValInput, out currencyVal))
            {
                Console.Write("Do you want to check historical currencies (y/n):");
                string historicalChoice = Console.ReadLine().ToLower();

                string path;
                switch (historicalChoice)
                {
                    case "y":
                        path = HistoricalPath();
                        if(path != "")
                        {
                            RunCurrenciesCalculation(currencyVal, firstCurrCode, secondCurrCode, path).GetAwaiter().GetResult();
                        }
                        else
                        {
                            Console.WriteLine("Wrong date format. Try again.");
                        }
                        break;
                    case "n":
                        RunCurrenciesCalculation(currencyVal, firstCurrCode, secondCurrCode, $"latest?access_key={API_KEY}").GetAwaiter().GetResult();
                        break;
                    default:
                        CurrencyCodesAndValuePromptAsync();
                        break;
                }

            }
            else
            {
                Console.WriteLine("Value/amount needs to be anumber... Restarting...");
                CurrencyCodesAndValuePromptAsync();
                
            }
            
        }

        private static string HistoricalPath()
        {
            //http://data.fixer.io/api/2013-12-24
            //? access_key = API_KEY
            //& base = GBP
            //& symbols = USD,CAD,EUR
            Console.WriteLine("Please enter witch date for currency rates (YYYY-MM-DD)");
            string dateInput = Console.ReadLine().Trim();

            //Regex for matching YYYY-MM-DD
            var r = new Regex(@"^\d{4}-((0\d)|(1[012]))-(([012]\d)|3[01])$");
            if (r.IsMatch(dateInput))
            {
                return $"{dateInput}?access_key={API_KEY}";

            }
            return "";
        }

        private static async Task RunCurrenciesCalculation(int currencyVal, string firstCurrCode, string secondCurrCode, string path)
        {
            try
            {
                var json = await GetJsonWithExchangeRates(path); // object with latest rates based on EUR. Need to convert EUR to firstCurrCode
                

                //Get exchange rate of selected currencies
                double firstCurrCodeRate;
                double secondCurrCodeRate;

                //Find firstCurrCode and secondCurrCode in json.Rates
                if(json != null)
                {

                    if (json.Rates.TryGetValue(firstCurrCode, out firstCurrCodeRate) && json.Rates.TryGetValue(secondCurrCode, out secondCurrCodeRate))
                    {
                    
                        double convertedCurrency = convertCurrency(currencyVal, firstCurrCodeRate, secondCurrCodeRate);
                        Console.WriteLine($"{firstCurrCode}: {currencyVal} -> {secondCurrCode}: {convertedCurrency}");
 
                    }
                    else
                    {
                        //Returns to main menu if keys are wrong
                        Console.WriteLine("One of the selected currencies does not exist. Please try again.");
                    }
                }
                

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private static double convertCurrency(int currencyVal, double firstCurrCodeRate, double secondCurrCodeRate)
        {
            //Base Currency from API is in EUR, so we have to make the selected
            // value into EUR then multiply with the rate of the currency we want to convert to.  
            double firstCurrInBaseCurr = currencyVal / firstCurrCodeRate;
            return firstCurrInBaseCurr * secondCurrCodeRate;
        }

        private static async Task<AllCurrencies> GetJsonWithExchangeRates(string path)
        {
            try
            {

                return await client.GetFromJsonAsync<AllCurrencies>(path);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        
        }
    }
}
