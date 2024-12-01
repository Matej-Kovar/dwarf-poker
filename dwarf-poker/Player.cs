using DwarvenPoker;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DwarvenPoker
{
    public class Player
    {
        public string Name { get; private set; }
        public int Money { get; private set; }
        public int RollsLeft { get; set; }
        public int Bet { get; private set; }
        public bool Surrendered { get; private set; }
        public float Score { get; set; }
        public Player(string name, int money, Dice[] diceSet)
        {
            Name = name;
            Money = money;
            RollsLeft = 2;
            DiceSet = diceSet;
        }

        public Dice[] DiceSet { get; private set; }

        public void RaiseBet(int bet)
        {
            if (bet > 0 && bet <= this.Money)
            {
                Money -= bet;
                Bet += bet;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Not enough money");
            }
        }

        public void Reset()
        {
            Bet = 0;
            RollsLeft = 2;
            Score = 0;
            Surrendered = false;
            foreach (var d in DiceSet)
            {
                d.Value = 0;
                d.Lock = false;
            }
        }

        public void Win(int amount) { Money += amount; }

        public void Surender() { Surrendered = true; }

        public void DiceScore()
        {
            int[] diceCounts = new int[7];
            float maxScore = 0;
            foreach (Dice dice in DiceSet)
            {
                diceCounts[dice.Value]++;
            }
            for (int value = 0; value < diceCounts.Length; value++)
            {
                float tempScore = diceCounts[value] * 10 + value + (diceCounts[value] >= 4 ? 20 : 0);
                if (tempScore > maxScore)
                {
                    maxScore = tempScore;
                }
            }
            if (maxScore > 20 && maxScore < 40)
            {
                int baseScore = (int)maxScore;
                for (int value = 0; value < diceCounts.Length; value++)
                {
                    if (diceCounts[value] == 2 && value != baseScore - 20 && value != baseScore - 30)
                    {
                        float tempScore = baseScore + (value / 10.0f) + (baseScore > 30 ? 20 : 0);
                        maxScore = Math.Max(maxScore, tempScore);
                    }
                }
            }
            maxScore = Math.Max(maxScore, FiveInRowBonus(diceCounts));

            Score = maxScore;
        }
        private float FiveInRowBonus(int[] diceCounts)
        {
            bool isFiveInRow = true;
            for (int value = 2; value <= 6; value++)
            {
                if (diceCounts[value] == 0)
                {
                    isFiveInRow = false;
                    break;
                }
            }
            if (isFiveInRow) return 50;
            isFiveInRow = true;
            for (int value = 1; value <= 5; value++)
            {
                if (diceCounts[value] == 0)
                {
                    isFiveInRow = false;
                    break;
                }
            }
            return isFiveInRow ? 40 : 0;
        }

        public string HandValue()
        {
            string result = "Dices haven't been rolled";
            if (Score > 10)
            {
                if (Score < 20)
                {
                    result = $"ß1 of a kind ({Score - 10})";
                }
                else if (Score < 30)
                {
                    result = $"ß2 of a kind ({Score - 20})";
                    if (Score - Math.Truncate(Score) > 0)
                    {
                        result = $"ß2 pairs ({Math.Truncate(Score) - 20} and {Math.Round((Score - Math.Truncate(Score)) * 10)})";
                    }
                }
                else if (Score < 40)
                {
                    result = $"ß3 of a kind ({Score - 30})";
                }
                else if (Score == 40)
                {
                    result = $"Straight (1, 2, 3, 4, 5)";
                }
                else if (Score == 50)
                {
                    result = $"Straight (2, 3, 4, 5, 6)";
                }
                else if (Score < 60 && Score - Math.Truncate(Score) > 0)
                {
                    result = $"Full House (3 × {Math.Truncate(Score) - 50} and 2 × {Math.Round((Score - Math.Truncate(Score)) * 10)})";
                }
                else if (Score < 70)
                {
                    result = $"ß4 of a kind ({Score - 60})";
                }
                else if (Score < 80)
                {
                    result = $"ß5 of a kind ({Score - 70})";
                }
            }

            return result;
        }
    }
}
