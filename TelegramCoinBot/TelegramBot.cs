using Telegram.Bot;

namespace CoinFollower
{
    public class TelegramBot
    {
        private readonly TelegramBotClient botClient;

        public TelegramBot()
        {
            string token = Environment.GetEnvironmentVariable("TelegramBotToken");
            botClient = new TelegramBotClient(token);
        }

        public void SendNotification()
        {

        }
        static void Main()
        {
            Console.ReadLine();
        }
    }
}