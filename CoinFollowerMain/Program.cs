using System.Text;
using Serilog;

namespace CoinFollower
{
    public class CoinFollowerMain
    {
        static async Task Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            string token = Environment.GetEnvironmentVariable("TelegramBotToken");
            Log.Logger = new LoggerConfiguration()
                            .WriteTo.File("logs/mybotlog.txt", rollingInterval: RollingInterval.Day)
                            .CreateLogger();

            TelegramBot telegramBot = new TelegramBot(token, Log.Logger);
            Parser parser = new Parser(Log.Logger);
            Application application = new Application(parser, telegramBot);          

            await application.RunAsync();
        }
    }
}