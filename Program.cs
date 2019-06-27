using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SteamTopSellersLocator
{
    class Program
    {
        static readonly HtmlWeb Web = new HtmlWeb();
        static bool useGlobalTopSellers = true;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Searching...");

            var pagesToCheck = 30;
            var targetGameName = "War for the Overworld";
            var topSellerPosition = 0;
            var foundPosition = false;

            for (var i = 0; i < pagesToCheck; i++)
            {
                var games = await SearchPage(i);

                foreach (var game in games)
                {
                    topSellerPosition++;

                    if (game.Contains(targetGameName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        foundPosition = true;
                        break;
                    }
                }

                if (foundPosition) break;
            }

            if(foundPosition) Console.WriteLine($"Position {topSellerPosition} in {(useGlobalTopSellers ? "globaltopsellers" : "topsellers")}");
            else Console.WriteLine($"Not in the top {topSellerPosition} of {(useGlobalTopSellers ? "globaltopsellers" : "topsellers")} :(");
        }

        static async Task<List<string>> SearchPage(int page = 0)
        {
            var url = "https://store.steampowered.com/search/?";
            url = url + "filter=" + (useGlobalTopSellers ? "globaltopsellers" : "topsellers");

            if (page != 0) url += $"&page={page}";

            Console.WriteLine($"Page {page + 1}");

            var doc = await Web.LoadFromWebAsync(url);
            var nodes = doc.DocumentNode.SelectSingleNode("//body").Descendants()
                .Where(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("search_result_row"))
                .Select(node => node.Descendants("span").First(x => x.Attributes["class"].Value == "title").InnerText)
                .ToList();

            return nodes;
        }
    }
}
