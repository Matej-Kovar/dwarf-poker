using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dwarf_poker
{
    class Player
    {
        private string _name;
        public string Name { get { return _name; } }

        private int _money;
        public int Money { get { return _money; } }

        private int _rollsLeft;
        public int RollsLeft {  get { return _rollsLeft; } set { if (value >= 0) _rollsLeft = value; } }

        private int _bet;
        public int Bet { get { return _bet; } }

        private bool _surendered = false;

        public bool Surrendered {  get { return _surendered; } }

        private Dice[] _diceSet;

        public Player(string name, int money, int rollsLeft, Dice[] diceSet)
        {
            _name = name;
            _money = money;
            _rollsLeft = rollsLeft;
            _diceSet = diceSet;
        }

        public Dice[] DiceSet { get { return _diceSet; } }

        public void RaiseBet(int bet)
        {
            if (bet > 0 && bet <= this.Money)
            {
                _money -= bet;
                _bet += bet; 
            } else
            {
                throw new ArgumentOutOfRangeException("Not enough money");
            }
        }

        public void ClearBet(){ _bet = 0; }

        public void Win(int amount) { _money += amount; }

        public void Surender () { _surendered = true; }
    }
}
