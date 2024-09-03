using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Serilog;

namespace CoinFollower
{
    public class TelegramBot
    {
        private readonly TelegramBotClient _botClient;
        private Serilog.ILogger _logger;

        public TelegramBot(string token, Serilog.ILogger logger)
        {
            _botClient = new TelegramBotClient(token);
            _logger = logger;
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

            _logger.Information("Бот запущен");
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
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
                if (message.Text != null && message.Text == "/unsubscribe")
                {
                    await UnsubscribeAsync(message);
                }
            }
            catch (Exception ex) 
            {
                _logger.Error($"Ошибка обновления: {ex.Message}");
            }
            
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.Error("Ошибка подключения: " + exception.Message);
            return Task.CompletedTask;
        }

        public async Task SendNotificationAsync()
        {
            try
            {
                using (ApplicationContext context = new ApplicationContext())
                {

                    var Subscribers = context.Subscribers.ToList();
                    foreach (var subscriber in Subscribers)
                    {
                        await _botClient.SendTextMessageAsync(subscriber.ChatId, $"{subscriber.Name}, появились изменения на сайте! \n https://www.nbrb.by/today/services/coins/avail/1");
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Ошибка рассылки: {ex.Message}");
            }
        }

        public async Task SubscribeAsync(Message message)
        {
            try
            {
                using (ApplicationContext context = new ApplicationContext())
                {
                    if (message != null)
                    {
                        Subscriber newSubscriber = new Subscriber() { ChatId = message.Chat.Id, Name = message.Chat.FirstName + " " + message.Chat.LastName };
                        if (context.Subscribers.Contains(newSubscriber))
                        {
                            await _botClient.SendTextMessageAsync(newSubscriber.ChatId, $"{newSubscriber.Name}, вы уже подписаны на рассылку, если хотитие отписаться введите /unsubscribe.");
                        }
                        else
                        {
                            context.Subscribers.Add(newSubscriber);
                            context.SaveChanges();
                            await _botClient.SendTextMessageAsync(newSubscriber.ChatId, $"{newSubscriber.Name}, вы успешно подписались на рассылку! Для отписки введите /unsubscribe.");
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                _logger.Error($"Ошибка подписки: {ex.Message}");
            }
        }

        public async Task UnsubscribeAsync(Message message) 
        {
            try
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
            catch (Exception ex) 
            {
                _logger.Error($"Ошибка отписки: {ex.Message}");
            }
        }
    }
}