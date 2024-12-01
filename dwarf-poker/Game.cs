using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarvenPoker
{
    public class Game
    {
        public int Player1Wins { get; set; } = 0;
        public int Player2Wins { get; set; } = 0;
        public int Rounds { get; set; } = 0;

        /*public Round AddRound (int size, int count)
        {
            return new Round (size, count);
        }*/

        public void ResolveRound()
        {

        }
    }
}
