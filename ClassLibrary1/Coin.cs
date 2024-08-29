using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinFollower
{
    public class Coin
    {
        public int Id {  get; set; } 
        public string Name { get; set; }
        public string Material { get; set; }
        public string Denomination { get; set; }
        public string Description { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            var other = obj as Coin; 
            
            return other.Name == this.Name 
                && other.Material == this.Material
                && other.Denomination == this.Denomination;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Name,Material,Denomination);
        }
        public override string ToString()
        {
            return Name + " " + " " + Denomination + " Рублей" + Material;
        }
    }
}
