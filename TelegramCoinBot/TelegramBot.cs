using Microsoft.Win32.SafeHandles;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CoinFollower
{
    public class TelegramBot
    {
        private readonly TelegramBotClient _botClient;

        public TelegramBot(string token)
        {
            _botClient = new TelegramBotClient(token);
        }

        public void StartBot()
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message }
            };

            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions
            );

            Console.WriteLine("Bot is running...");
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;
            if (message.Text != null && message.Text == "/start")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    $"Приветствую, {message.Chat.FirstName} {message.Chat.LastName}! Введите /subscribe для подписки на рассылку или /unsubscribe для отписки", 
                    cancellationToken: cancellationToken);
            }
            if (message.Text != null && message.Text == "/subscribe")
            {
                await SubscribeAsync(message);
            }
            if(message.Text != null && message.Text == "/unsubscribe")
            {
                await UnsubscribeAsync(message);
            }
            
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }

        public async Task SendNotificationAsync()
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                var Subscribers = context.Subscribers.ToList();
                //string coinsText = string.Empty;
                //foreach (Coin coin in coins) 
                //{
                //     coinsText += coin.ToString() + "\n";
                //}
                foreach (var subscriber in Subscribers)
                {
                    await _botClient.SendTextMessageAsync(subscriber.ChatId,$"{subscriber.Name}, появились изменения на сайте!");
                }
            }
        }

        public async Task SubscribeAsync(Message message)
        {
            using (ApplicationContext context = new ApplicationContext()) 
            {
                if (message != null) 
                {
                    Subscriber newSubscriber = new Subscriber() {ChatId = message.Chat.Id, Name = message.Chat.FirstName+" "+message.Chat.LastName};
                    if (context.Subscribers.Contains(newSubscriber))
                    {
                        await _botClient.SendTextMessageAsync(newSubscriber.ChatId,$"{newSubscriber.Name}, вы уже подписаны на рассылку, если хотитие отписаться введите /unsubscribe.");
                    }
                    else
                    {
                        context.Subscribers.Add(newSubscriber);
                        context.SaveChanges();
                        await _botClient.SendTextMessageAsync(newSubscriber.ChatId, $"{newSubscriber.Name}, вы успешно подписались на рассылку!");
                    }
                }
            }
        }

        public async Task UnsubscribeAsync(Message message) 
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                if (message != null)
                {
                    Subscriber newSubscriber = new Subscriber() { ChatId = message.Chat.Id, Name = message.Chat.FirstName + " " + message.Chat.LastName };
                    if (context.Subscribers.Contains(newSubscriber))
                    {
                        context.Subscribers.Remove(newSubscriber);
                        context.SaveChanges();
                        await _botClient.SendTextMessageAsync(newSubscriber.ChatId, $"{newSubscriber.Name}, вы успешно отписались от рассылки если хотите снова получать уведомления введите /subscribe");
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(newSubscriber.ChatId, $"{newSubscriber.Name}, вы не подписаны на рассылку если хотите получать уведомления введите /subscribe");
                    }
                }
            }
        }
    }
}