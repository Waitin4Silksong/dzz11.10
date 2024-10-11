using HtmlAgilityPack;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

public class CurrencyConverter
{
    private static decimal usdRate = 0;

    public static async Task GetUsdRate()
    {
        string url = "https://bank.gov.ua/ua/markets/exchangerates";

        HttpClient client = new HttpClient();
        string html = await client.GetStringAsync(url);

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var usdNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='exchangeRates']/tbody/tr[8]/td[5]");

        if (usdNode != null)
        {
            string usdRateText = usdNode.InnerText.Trim();

            if (decimal.TryParse(usdRateText, out decimal parsedRate))
            {
                usdRate = parsedRate;
            }
            else
            {
                Console.WriteLine("Не вдалося розпізнати курс долара.");
            }
        }
        else
        {
            Console.WriteLine("Не вдалося знайти елемент із курсом долара.");
        }
    }

    public static decimal ConvertToUsd(decimal grn)
    {
        if (usdRate == 0)
        {
            Console.WriteLine("Курс долара не встановлено. Неможливо конвертувати.");
            return 0;
        }
        return grn / usdRate;
    }
}

class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Отримання курсу долара з сайту НБУ...");
        await CurrencyConverter.GetUsdRate();

        if (CurrencyConverter.ConvertToUsd(1) != 0)
        {
            Console.WriteLine($"Курс долара: {CurrencyConverter.ConvertToUsd(1):0.00} грн за 1 USD.");
            Console.WriteLine("Введіть суму в гривнях для конвертації в долари:");

            string input = Console.ReadLine();

            if (decimal.TryParse(input, out decimal parsedGrn))
            {
                decimal usd = CurrencyConverter.ConvertToUsd(parsedGrn);
                Console.WriteLine($"{parsedGrn} грн = {usd:0.00} USD.");
            }
            else
            {
                Console.WriteLine("Помилка: введіть коректне число.");
            }
        }
        else
        {
            Console.WriteLine("Помилка: не вдалося отримати курс долара.");
        }
    }
}
