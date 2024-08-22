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

        public Application(Parser parser, TelegramBot bot)
        {
            _parser = parser;
            _bot = bot;
        }

        public async Task RunAsync()
        {
            _parser.Changed += _bot.SendNotification;
            await _parser.CheckForUpdates();
        }
    }

}
