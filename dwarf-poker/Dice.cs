using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace DwarvenPoker
{
    public class Dice
    {
        private int[] _sides;
        public Dice(int[] sides)
        {
            this.Sides = sides;
        }
        public int Value { get; set; } = 0;
        public int[] Sides
        {
            get { return _sides; }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] < 0)
                    {
                        throw new ArgumentOutOfRangeException("Side value cannot be lower than 1");
                    }
                }
                _sides = value;
            }
        }
        public bool Lock { get; set; } = false;
        public void Roll()
        {
            Random rnd = new Random();
            Value = Sides[rnd.Next(0, Sides.Length)];
        }
        public string Status(bool returnLocked)
        {
            if (returnLocked)
            {
                if (Lock)
                {
                    return "Y";
                }
                else
                {
                    return "N";
                }
            }
            else
            {
                return Value.ToString();
            }
        }
    }
}
