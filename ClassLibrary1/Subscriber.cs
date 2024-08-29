using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFollower
{
    public class Subscriber
    {
        public long ChatId { get; set; }
        public string Name { get; set; }


        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            var other = obj as Subscriber;
            return other.ChatId == this.ChatId && other.Name == this.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ChatId, Name);
        }

    }
}
