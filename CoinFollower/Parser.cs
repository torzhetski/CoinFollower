using AngleSharp;
using System.ComponentModel;

class Parser
{
    public event Action? Changed;

    private readonly string filePath = @"./coins.txt";

    private async Task<List<string>> Parse()
    {
        string url = "https://www.nbrb.by/today/services/coins/avail/1";
        List<string> listOfCoins = new();
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

                foreach (var row in rows)
                {
                    if (row.QuerySelector("th") != null) continue;

                    var rowInfo = row.QuerySelector("td:not([colspan='3'])");

                    if (rowInfo != null)
                    {
                        listOfCoins.Add((rowInfo.QuerySelector("a") ?? rowInfo).TextContent.Trim());
                    }
                }
            }
            if (!File.Exists(filePath))
            {
                WriteToFile(listOfCoins);
            }
            return listOfCoins;
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex.Message);
            return new();
        }
    }

    public async Task CheckForUpdates()
    {
        var newListOfCoins = await Parse();
        var previousListOfCoins = ReadFromFile();

        if(previousListOfCoins.Count == newListOfCoins.Count)
        {
            foreach( var e in newListOfCoins)
            {
                previousListOfCoins.Remove(e);
            }
            if (previousListOfCoins.Count != 0)
            {
                Changed?.Invoke();
            }
        }
        else
        {
            Changed?.Invoke();
        }
        WriteToFile(newListOfCoins);
    }

    private void WriteToFile(List<string> listOfCoins)
    {
        using (StreamWriter sw = new StreamWriter(filePath, false))
        {
            foreach (var e in listOfCoins)
            {
                sw.WriteLine(e.ToString());
            }
        }
    }

    private List<string> ReadFromFile()
    {
        List<string> listOfCoins = new List<string>();

        using (StreamReader sr = new StreamReader(filePath)) 
        {
            while (!sr.EndOfStream) 
            {
                 listOfCoins.Add(sr.ReadLine());
            }
        }
            return listOfCoins;
    }

}