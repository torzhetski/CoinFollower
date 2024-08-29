using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFollower
{
    class Application
    {
        private readonly Parser _parser;
        private readonly TelegramBot _bot;
        private static TaskCompletionSource<bool> _completionSource = new TaskCompletionSource<bool>();

        public Application(Parser parser, TelegramBot bot)
        {
            _parser = parser;
            _bot = bot;
            _parser.Changed += _bot.SendNotificationAsync;

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                _completionSource.SetResult(true);
            };
        }

        public async Task RunAsync()
        {
            // Запуск бота
            _bot.StartBot();

            // Запуск парсера каждые 30 минут
            while (!_completionSource.Task.IsCompleted)
            {
                await _parser.CheckForUpdates();
                await Task.Delay(TimeSpan.FromSeconds(60));
            }
            await _completionSource.Task;
        }
    }

}
