using System;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace CurrencyConverter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Добро пожаловать в конвертер валют!");

                string baseCurrency = GetCurrencyInput("Введите код валюты, из которой хотите конвертировать (например, USD, EUR, RUB): ");
                string targetCurrency = GetCurrencyInput("Введите код валюты, в которую хотите конвертировать: ");

                double amount = GetAmountInput("Введите сумму для конвертации: ");
                double exchangeRate = await GetExchangeRate(baseCurrency, targetCurrency);
                if (exchangeRate == 0)
                {
                    Console.WriteLine("Не удалось получить курс валют. Пожалуйста, проверьте код валюты.");
                    continue;
                }

                  double convertedAmount = amount * exchangeRate;
                  Console.WriteLine($"{amount} {baseCurrency} = {convertedAmount:F2} {targetCurrency}");

                bool continueConversion = true;
                while (continueConversion)
                {
                    Console.WriteLine("\nВыберите действие:");
                    Console.WriteLine("1. Завершить программу");
                    Console.WriteLine("2. Конвертировать другую сумму с теми же валютами");
                    Console.WriteLine("3. Поменять местами базовую и целевую валюту");
                    Console.WriteLine("4. Сделать новое конвертирование");

                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            Console.WriteLine("Программа завершена. Нажмите любую клавишу для выхода...");
                            Console.ReadKey();
                            return;
                        case "2":

                            amount = GetAmountInput("Введите сумму для конвертации: ");
                            
                            exchangeRate = await GetExchangeRate(baseCurrency, targetCurrency);
                            if (exchangeRate == 0)
                            {
                                Console.WriteLine("Не удалось получить курс валют. Пожалуйста, проверьте код валюты.");
                                continue;
                            }

                            convertedAmount = amount * exchangeRate;
                            Console.WriteLine($"{amount} {baseCurrency} = " + 
                                $"{convertedAmount:F2} {targetCurrency}");

                            break;
                        case "3":
                            (baseCurrency, targetCurrency) = (targetCurrency, baseCurrency);

                            amount = GetAmountInput("Введите сумму для конвертации: ");

                            exchangeRate = await GetExchangeRate(baseCurrency, targetCurrency);
                            if (exchangeRate == 0)
                            {
                                Console.WriteLine("Не удалось получить курс валют. Пожалуйста, проверьте код валюты.");
                            }
                            else
                            {
                                convertedAmount = amount * exchangeRate;
                                Console.WriteLine($"{amount} {baseCurrency} = " +
                                    $"{convertedAmount:F2} {targetCurrency}");
                            }
                            break;
                        case "4":
                            continueConversion = false;
                            break;
                        default:
                            Console.WriteLine("Некорректный выбор. Пожалуйста, попробуйте еще раз.");
                            break;
                    }
                }
            }
        }

        static double GetAmountInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine().Replace(',', '.');

                if (double.TryParse(input, NumberStyles.Any, 
                    CultureInfo.InvariantCulture, out double amount))
                {
                    return amount;
                }
                else
                {
                    Console.WriteLine("Ошибка: введено некорректное значение суммы. Пожалуйста, введите число.");
                }
            }
        }

        static string GetCurrencyInput(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine().ToUpper();
                    if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Ошибка: код валюты не может " +
                        "быть пустым. Пожалуйста, введите код валюты.");
                    continue;
                }
                return input;

            }
        }
        static async Task<double> GetExchangeRate(string baseCurrency, string targetCurrency)
            {
                var client = new RestClient("https://api.exchangerate-api.com/v4/latest/" + baseCurrency);
                var request = new RestRequest();
                var response = await client.GetAsync(request);

                if (response.IsSuccessful)
                {
                    var json = JObject.Parse(response.Content);
                    if (json["rates"][targetCurrency] != null)
                    {
                        return (double)json["rates"][targetCurrency];
                    }
                }
                return 0;
            }
        
    }
}