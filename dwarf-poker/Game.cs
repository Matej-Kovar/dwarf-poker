using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace dwarf_poker
{
    internal class Game
    {
        public Player[] Players;

        private int playerIndex = 0;

        public Player CurrentPlayer { get { return Players[playerIndex]; } }

        public Game(Player[] players)
        {
            Players = players;
        }
        private int _bank;
        public int Bank { get { return _bank; } }
        
        private int _maxBet = 0;
        public int MaxBet {  get { return _maxBet; } }

        private bool _RoundEnd = false;

        public bool RoundEnd { get { return _RoundEnd; } }

        private void SetBank()
        {
            _bank = 0;
            foreach (Player player in Players)
            {
                _bank += player.Bet;
                if(player.Bet > MaxBet)
                {
                    _maxBet = player.Bet;
                }
            }
        }
        public bool NextPlayer()
        {
            CurrentPlayer.RollsLeft--;
            _RoundEnd = false;
            int maxRolls = int.MinValue;
            int? index = null;
            int notSurrendered = 0;
            for (int i = 0; i < Players.Length; i++)
            {
                if (!Players[i].Surrendered)
                {
                    notSurrendered++;
                    if (Players[i].RollsLeft > maxRolls && Players[i].RollsLeft > 0)
                    {
                        index = i;
                        maxRolls = Players[i].RollsLeft;
                    }
                }
            }
            if (index == null || notSurrendered < 2)
            {
                return true;
            } 
            else
            {
                playerIndex = (int)index;
            }
            return false;
        }
        public string[] DisplayStatus()
        {
            List<string> result = new List<string>();
            result.Add(new string((char)35, 100));
            result.Add($"It is now {CurrentPlayer.Name}'s turn");
            result.Add(new string((char)35, 100));
            result.Add($"Bank has value of:                         {Bank}$");
            result.Add($"Biggest bet:                               {MaxBet}$");
            result.Add($"Your bet:                                  {CurrentPlayer.Bet}$");
            result.Add($"Your money:                                {CurrentPlayer.Money}$");
            result.Add(new string((char)35, 100));
            result.Add($"Rolls you have left:                       {CurrentPlayer.RollsLeft}");
            result.Add($"Your dices have value of:                  " + Actions.Status(CurrentPlayer.DiceSet));
            result.Add($"Your dices are locked:                     " + Actions.Status(CurrentPlayer.DiceSet, true));
            result.Add(new string((char)35, 100));
            for (int i = 0; i < Players.Length; i++)
            {
                result.Add($"{Players[i].Name}'s dices & bet:" + new string((char)32, 43 - Players[i].Name.Length - 15) + (string)(!Players[i].Surrendered ? Actions.Status(Players[i].DiceSet) + " & " + Players[i].Bet + "$" : "surrendered"));  //43
            }
            result.Add(new string((char)35, 100));
            if (CurrentPlayer.RollsLeft < 2)
            {
                result.Add($"Type 'unlock <diceIndex>' to unlock a dice");
                result.Add($"Type 'lock <diceIndex>' to lock a dice");

            }
            result.Add($"Type 'raise <amount>' to raise your bet");
            result.Add($"Type 'roll' to roll all unlocked dice, if your bet is lower than the biggest bet, you cannot roll");
            result.Add($"Type 'surrender' to surrender this game");
            result.Add(new string((char)35, 100));
            return result.ToArray();
        }

        public string[] InputHandling(string input)
        {
            List<string> result = new List<string>();
            switch (input)
            {
                case "roll":
                    if(CurrentPlayer.Bet >= MaxBet)
                    {
                        result.AddRange(HandleRoll());
                        _RoundEnd = true;
                    }else
                    {
                        result.Add("Your bet is too low");
                    }
                    break;
                case "surrender":
                    CurrentPlayer.Surender();
                    _RoundEnd = true;
                    break;
                default:
                    result.AddRange(HandleMultiParameterCommand(input));
                    break;
            }
            result.Add("Press [enter] to continue");
            return result.ToArray();
        }

        private List<string> HandleRoll()
        {
            List<string> result = new List<string>();
            Actions.Roll(CurrentPlayer.DiceSet);
            result.Add($"Result of your roll: {Actions.Status(CurrentPlayer.DiceSet)}");
            return result;
        }

        private List<string> HandleMultiParameterCommand(string input)
        {
            List<string> result = new List<string>();
            var parts = input.Split(' ');
            int[] parameters = new int[parts.Length - 2];
            bool parseSucces = true;
            for (int i = 1; i < parts.Length; i++)
            {
                if(!int.TryParse(parts[i], out parameters[i - 1])) { parseSucces = false; break; }
            }
            if (parts.Length < 2 || !parseSucces)
            {
                result.Add("Invalid command or parameter");
            } else
            {
                switch (parts[0])
                {
                    case "lock":
                        if (index >= 0 && index < CurrentPlayer.DiceSet.Length && CurrentPlayer.RollsLeft < 2)
                        {
                            Actions.Lock(CurrentPlayer.DiceSet[index]);
                            result.Add($"Dice {index} locked successfully");
                        }
                        else
                        {
                            result.Add("Second parameter out of range");
                        }
                        break;
                    case "unlock":
                        if (index >= 0 && index < CurrentPlayer.DiceSet.Length && CurrentPlayer.RollsLeft < 2)
                        {
                            Actions.Unlock(CurrentPlayer.DiceSet[index]);
                            result.Add($"Dice {index} unlocked successfully");
                        }
                        else
                        {
                            result.Add("Second parameter out of range");
                        }
                        break;
                    case "raise":
                        if (index > 0 && index <= CurrentPlayer.Money)
                        {
                            CurrentPlayer.RaiseBet(index);
                            SetBank();
                            result.Add($"You have raised by:                        {index}");
                            result.Add($"Bank has now value of:                     {Bank}");
                        }
                        else
                        {
                            result.Add("Second parameter out of range");
                        }
                        break;
                    default:
                        result.Add("Invalid command");
                        break;
                }
            }
            return result;
        }
    }
}
