using AngleSharp;

namespace CoinFollower
{
    public class Parser
    {
        private Serilog.ILogger _logger;

        public event Func<Task>? Changed;
        
        public Parser(Serilog.ILogger logger ) 
        {
            _logger = logger;
        }

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
                }

                return listOfCoins;
            }
            catch (ArgumentNullException ex)
            {
                _logger.Error($"Таблицы нет: {ex.Message}");
                return new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Скорее всего что то на сайте изменилось{ex.Message}");
                return new();
            }
            finally 
            {
                _logger.Information("Получены новые данные с сайта");
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
            try
            {
                using (ApplicationContext context = new ApplicationContext())
                {
                    context.Coins.RemoveRange(oldCoins);
                    context.Coins.AddRange(newCoins);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка записи в бд {ex.Message}");
            }
        }

        private List<Coin> Read()
        {
            var listOfCoins = new List<Coin>();
            try
            {
                using (ApplicationContext context = new ApplicationContext())
                {
                    listOfCoins = context.Coins.ToList();
                }
                
            }
            catch (Exception ex) 
            {
                _logger.Error($"Ошибка записи в бд {ex.Message}");
            }
            return listOfCoins;
        }

    }
}