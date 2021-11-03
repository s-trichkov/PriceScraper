using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using HtmlAgilityPack;

namespace PriceScraper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            List<string> links = new List<string>
            {
                "https://bookoholic.net/product/10844/spy-x-family-vol-1.html",
                "https://bookoholic.net/product/11947/naruto-3-in-1-edition-vol-22-includes-vols-64-65-66.html",
                "https://bookoholic.net/product/13601/berserk-deluxe-volume-8.html"
            };

            Stopwatch stopwatch = new Stopwatch();

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            stopwatch.Start();
            var result = await InvokeAsync(links);
            stopwatch.Stop();

            PrintBooks(result);
            Console.WriteLine("Задачата се изпълни за: " + stopwatch.ElapsedMilliseconds / 1000.0 + " секунди");
        }

        static async Task<List<Book>> InvokeAsync(List<string> links)
        {
            List<Task<Book>> booksToWork = new List<Task<Book>>();

            foreach (string link in links)
            {
                booksToWork.Add(Task.Run(() => Worker(link)));
            }

            var result = await Task.WhenAll(booksToWork);
            return result.ToList();
        }

        static HtmlDocument GetProducts(string link)
        {
            HtmlWeb web = new HtmlWeb();
            return web.Load(link);
        }

        static Book Worker(string link)
        {
            Book book = new Book();
            try
            {
                HtmlNode htmlNode = GetProducts(link).DocumentNode;
                string productTitle = htmlNode.SelectSingleNode(".//h1[@class='title-right fn']").InnerText.Trim();
                string price = htmlNode.SelectSingleNode(".//span[@class='taxed-price-value price-value']").InnerText.Trim();
                book.Title = productTitle;
                book.Price = price;
                return book;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

        }

        static void PrintBooks(List<Book> books)
        {
            foreach (var item in books)
            {
                Console.WriteLine(item.Title + " струва " + item.Price);
            }
        }
    }
}