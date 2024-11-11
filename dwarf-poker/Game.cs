using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            int secondColumn = 35;
            int playerSectionwidth = Actions.Status(CurrentPlayer.DiceSet).Length + 14;
            string longestName = Players.MaxBy(x => x.Name).Name;
            int betsLongest = Math.Max(longestName.Length + 10, 16);
            int betsWidth = MaxBet.ToString().Length + betsLongest;
            char borderChar = '#';
            string marginLeft = new string((char)32, 2);
            List<string> result = new List<string>();
            result.Add(string.Empty);
            result.Add(marginLeft + new string(borderChar, playerSectionwidth));
            result[result.Count - 1] += new string((char)32, secondColumn - result[result.Count - 1].Length - 1);
            result[result.Count - 1] += marginLeft + new string(borderChar, 11 + Bank.ToString().Length);
            result.Add(marginLeft);
            result[result.Count - 1] += Text.AddBorder(Text.textAlign((int)Text.Align.center, playerSectionwidth, "Current player info"), borderChar);
            result[result.Count - 1] += new string((char)32, secondColumn + 1 - result[result.Count - 1].Length);
            result[result.Count - 1] += $"# Bank: {Bank}$ #";
            result.Add(marginLeft + new string(borderChar, playerSectionwidth));
            result[result.Count - 1] += new string((char)32, secondColumn - result[result.Count - 1].Length - 1);
            result[result.Count - 1] += marginLeft + new string(borderChar, 11 + Bank.ToString().Length);
            result.Add(marginLeft);
            result[result.Count - 1] += $"# Name: {CurrentPlayer.Name}";
            result[result.Count - 1] += new string((char)32, playerSectionwidth + 1 - result[result.Count - 1].Length);
            result[result.Count - 1] += "#";
            result.Add
            (marginLeft + $"# Rolls:  {CurrentPlayer.RollsLeft}");
            result[result.Count - 1] += new string((char)32, playerSectionwidth + 1 - result[result.Count - 1].Length);
            result[result.Count - 1] += "#";
            result[result.Count - 1] += new string((char)32, secondColumn - 1 - result[result.Count - 1].Length);
            result[result.Count - 1] += marginLeft + new string(borderChar, betsWidth + Bank.ToString().Length);
            result.Add(marginLeft + "#" + new string('-', playerSectionwidth - 3));
            result[result.Count - 1] += new string((char)32, playerSectionwidth + 1 - result[result.Count - 1].Length);
            result[result.Count - 1] += "#";
            result[result.Count - 1] += new string((char)32, secondColumn + 1 - result[result.Count - 1].Length);
            result[result.Count - 1] += "#" + new string((char)32, (betsWidth - 4) / 2) + "Bets";
            result[result.Count - 1] += new string((char)32, betsWidth + secondColumn + 1 - result[result.Count - 1].Length);
            result[result.Count - 1] += "#";
            /*
            result.Add
            (
                marginLeft + "#  Finances:" 
                + new string((char)32, secondColumn - 4 - 9 + 1) 
                + $"Biggest bet: {MaxBet}"
            );
            result.Add
            (
                marginLeft + $"#   Bet:    {CurrentPlayer.Bet}$" 
                + new string((char)32, secondColumn - 5 - 9 - CurrentPlayer.Bet.ToString().Length + 1) 
                + "Player Bets:"
            );
            result.Add
            (
                marginLeft + $"#   Money:  {CurrentPlayer.Money}$" 
                + new string((char)32, secondColumn - 5 - 9 - CurrentPlayer.Money.ToString().Length + 2) 
                + $"{Players[0].Name}'s bet: {Players[0].Bet}"
            );
            result.Add
            (
                marginLeft + "#  Dices:" 
                + new string((char)32, secondColumn - 4 - 6 + 2) 
                + $"{Players[1].Name}'s bet: {Players[1].Bet}"
            );
            result.Add
            (
                marginLeft + $"#   Values: {Actions.Status(CurrentPlayer.DiceSet)}" 
                + new string((char)32, secondColumn - 5 - 8 - Actions.Status(CurrentPlayer.DiceSet).Length + 2) 
                + $"{Players[2].Name}'s bet: {Players[2].Bet}");
            result.Add
            (
                marginLeft + $"#   Locked: {Actions.Status(CurrentPlayer.DiceSet, true)}" 
                + new string((char)32, secondColumn - 5 - 8 - Actions.Status(CurrentPlayer.DiceSet, true).Length + 2) 
                + $"{Players[3].Name}'s bet: {Players[3].Bet}"
            );
            /*
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
            result.Add(new string((char)35, 100));*/
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
            int[] parameters = new int[parts.Length - 1];
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
                bool positive = true;
                bool validDiceIndex = true;
                foreach (var parameter in parameters)
                {
                    if (parameter < 0) {  positive = false; break; }
                    if (parameter >= CurrentPlayer.DiceSet.Length) { validDiceIndex = false; }

                }
                if (positive)
                {
                    
                    switch (parts[0])
                    {
                        case "lock":
                            if (validDiceIndex && CurrentPlayer.RollsLeft < 2)
                            {
                                foreach (var parameter in parameters)
                                {
                                    Actions.Lock(CurrentPlayer.DiceSet[parameter]);
                                    result.Add($"Dice {parameter} locked successfully");
                                }
                            }
                            else
                            {
                                result.Add("Numerical parameter out of range");
                            }
                            break;
                        case "unlock":
                            if (validDiceIndex && CurrentPlayer.RollsLeft < 2)
                            {
                                foreach (var parameter in parameters)
                                {
                                    Actions.Unlock(CurrentPlayer.DiceSet[parameter]);
                                    result.Add($"Dice {parameter} unlocked successfully");
                                }
                            }
                            else
                            {
                                result.Add("Numerical parameter out of range");
                            }
                            break;
                        case "raise":
                            if (parameters.Length == 1)
                            {
                                if(parameters[0] <= CurrentPlayer.Money)
                                {
                                    CurrentPlayer.RaiseBet(parameters[0]);
                                    SetBank();
                                    result.Add($"You have raised by:                        {parameters[0]}");
                                    result.Add($"Bank has now value of:                     {Bank}");
                                }
                                else
                                {
                                    result.Add("You don't have enough money");
                                }

                            }
                            else
                            {
                                result.Add("Too many parameters");
                            }
                            break;
                        default:
                            result.Add("Invalid command");
                            break;
                    }
                } else
                {
                    result.Add("Numerical parameter cant be smaller than 0");
                }
            }
            return result;
        }
    }
}
