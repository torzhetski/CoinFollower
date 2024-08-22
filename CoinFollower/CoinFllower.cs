using System.Text;

namespace CoinFollower
{
    public class CoinFllower
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            
            Parser parser = new Parser();
            TelegramBot bot = new TelegramBot();
            Application application = new Application(parser , bot);
            await application.RunAsync();
        }
    }
}
