using AngleSharp;
using System.Text;

class Parser
{
    public async Task Parse(string[] args)
    {
        try
        {
            string url = "https://www.nbrb.by/today/services/coins/avail/1";

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(url);
            var table = document.QuerySelector("div.table-block table");

            if (table != null)
            {
                var rows = table.QuerySelectorAll("tbody tr");

                foreach (var row in rows)
                {
                    if (row.QuerySelector("th") != null) continue;

                    var cells = row.QuerySelectorAll("td:not([colspan='3'])");

                    if (cells.Length > 0)
                    {
                        var coinNameCell = cells[0].QuerySelector("a") ?? cells[0];
                        Console.WriteLine($"{coinNameCell.TextContent.Trim()}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Таблица не найдена.");
            }
        }
        catch (Exception ex) 
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void CheckForUpdates()
    {

    }

    private void WriteNewFile()
    {

    }
}