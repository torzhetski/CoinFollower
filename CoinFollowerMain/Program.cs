namespace CoinFollower
{
    public class CoinFollowerMain
    {
        

        static async Task Main()
        {
            string token = Environment.GetEnvironmentVariable("TelegramBotToken");
            TelegramBot telegramBot = new TelegramBot(token);
            Parser parser = new Parser();
            Application application = new Application(parser, telegramBot);          

            await application.RunAsync();
        }
    }
}