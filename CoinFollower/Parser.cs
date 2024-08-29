using AngleSharp;
using CoinFollower;
using System.ComponentModel;

namespace CoinFollower
{
    public class Parser
    {
        public event Func<Task>? Changed;

        private readonly string filePath = @"./coins.txt";

        private async Task<List<Coin>> Parse()
        {
            string url = "https://www.nbrb.by/today/services/coins/avail/1";
            List<Coin> listOfCoins = new();
            try
            {
                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);
                var document = await context.OpenAsync(url);
                var table = document.QuerySelector("div.table-block table");

                if (table == null)
                {
                    throw new ArgumentNullException();
                }
                else
                {
                    var rows = table.QuerySelectorAll("tbody tr");
                    List<string> listOfText = new List<string>();
                    foreach (var row in rows)
                    {
                        if (row.QuerySelector("th") != null) continue;

                        var textRows = row.QuerySelectorAll("td:not([colspan='3'])")
                            .Where(td => !string.IsNullOrEmpty(td.TextContent.Trim()));

                        if (textRows != null)
                        {

                            foreach (var text in textRows)
                                listOfText.Add(text.TextContent.Trim());
                        }
                    }

                    for (int i = 0; i < listOfText.Count(); i += 4)
                    {
                        listOfCoins.Add(new Coin()
                        {
                            Name = listOfText[i],
                            Material = listOfText[i + 1],
                            Denomination = listOfText[i + 2],
                            Description = listOfText[i + 3]
                        });
                    }
                    //listOfCoins.Add(new Coin() { Name = "lasl;,s", Material = "lml;aal", Denomination = "l,la,slamm", Description = "sswwdwd" });
                }

                return listOfCoins;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
                return new();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("Скорее всего что то на сайте изменилось");
                return new();
            }
        }

        public async Task CheckForUpdates()
        {
            var newListOfCoins = await Parse();
            var previousListOfCoins = Read();

            var newCoins = newListOfCoins.Except(previousListOfCoins).ToList();
            var oldCoins = previousListOfCoins.Except(newListOfCoins).ToList();

            if (newCoins.Count() > 0 || oldCoins.Count() > 0)
            {
                Write(newCoins, oldCoins);
                await Changed?.Invoke();
            }

        }

        private void Write(List<Coin> newCoins, List<Coin> oldCoins)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                context.Coins.RemoveRange(oldCoins);
                context.Coins.AddRange(newCoins);
                context.SaveChanges();
            }
        }

        private List<Coin> Read()
        {
            var listOfCoins = new List<Coin>();
            using (ApplicationContext context = new ApplicationContext())
            {
                listOfCoins = context.Coins.ToList();
            }
            return listOfCoins;
        }

    }
}