using DwarvenPoker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DwarvenPoker
{
    public class Round
    {
        public Player[] Players;

        private int playerIndex = 0;

        public Player CurrentPlayer { get { return Players[playerIndex]; } }

        public Round(Player[] players)
        {
            Players = players;
        }
        public int Bank { get; private set; }
        public int MaxBet { get; private set; }
        public bool RoundEnd { get; private set; }
        public bool InMenu { get; set; } = true;
        public bool End { get; private set; } = false;
        private void SetBank()
        {
            Bank = 0;
            foreach (Player player in Players)
            {
                Bank += player.Bet;
                if (player.Bet > MaxBet)
                {
                    MaxBet = player.Bet;
                }
            }
        }
        public bool NextPlayer()
        {
            CurrentPlayer.RollsLeft--;
            RoundEnd = false;
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
            int gap = 2;
            char borderChar = '-';
            int marginLeft = 2;
            List<string> result = new List<string>();
            Style.Format(ref result, Style.Margin(Style.Margin(Style.SectionedArticle(CurrentPlayerData(), "Current Player Info", borderChar), 1, (int)Style.Direction.top), marginLeft, (int)Style.Direction.right), 0);
            Style.Format(ref result, Style.Margin(Style.SectionedArticle([[$"  Bank: {Bank}$"], [$"  Biggest bet: {MaxBet}$"]]), gap, (int)Style.Direction.right), 13);
            Style.Format(ref result, Style.Margin(Style.Margin(Style.Table(["Name", "Bet", "Dice Values", "Hand Value"], PlayerData(), borderChar), 1, (int)Style.Direction.right, ' '), 1, (int)Style.Direction.top), 0);
            Style.Format(ref result, Style.Margin(Style.Table(["Syntax", "Effect"], CommandData(), borderChar), 1, (int)Style.Direction.right, ' '), 5 + Players.Length);
            result.Add("  " + new string('─', Console.WindowWidth - 4));
            return result.ToArray();
        }

        private string[][] PlayerData()
        {
            string[][] result;
            if (InMenu)
            {
                result = new string[3][];
            }
            else
            {
                result = new string[4][];
            }
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new string[Players.Length];
            }
            for (int i = 0; i < Players.Length; i++)
            {
                if (InMenu)
                {
                    result[0][i] = i.ToString();
                    result[1][i] = Players[i].Name;
                    result[2][i] = Players[i].Money.ToString() + "$";
                }
                else
                {
                    result[0][i] = Players[i].Name;
                    result[1][i] = Players[i].Bet.ToString() + "$";
                    result[2][i] = Status(Players[i].DiceSet);
                    result[3][i] = Players[i].HandValue();
                    if (Players[i].Surrendered)
                    {
                        result[2][i] = "surrendered";
                    }
                }
            }
            return result;
        }

        private string[][] CurrentPlayerData()
        {
            string[][] result = new string[3][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new string[2];
            }
            result[0][0] = $"  Name: {CurrentPlayer.Name}";
            result[0][1] = $"  Rolls: {CurrentPlayer.RollsLeft}";
            result[1][0] = $"  Money: {CurrentPlayer.Money}$";
            result[1][1] = $"  Bet: {CurrentPlayer.Bet}$";
            result[2][0] = $"  Dice Values: {Status(CurrentPlayer.DiceSet)}";
            result[2][1] = $"  Locked?:     {Status(CurrentPlayer.DiceSet, true)}";
            return result;
        }

        private string[][] CommandData()
        {
            string[][] result = new string[2][];
            for (int i = 0; i < result.Length; i++)
            {
                if (CurrentPlayer.RollsLeft < 2)
                {
                    result[i] = new string[5];
                }
                else
                {
                    result[i] = new string[3];
                }

            }
            result[0][0] = $"roll";
            result[1][0] = $"Rolls all your unlock dices";
            result[0][1] = $"surrender";
            result[1][1] = $"'Folds' your dices";
            result[0][2] = $"raise <amount>";
            result[1][2] = $"Raises your bet by <amount>";
            if (CurrentPlayer.RollsLeft < 2)
            {
                result[0][3] = $"lock <diceindex>...";
                result[1][3] = $"Lock dices with indexes you provided";
                result[0][4] = $"unlock <diceindex>...";
                result[1][4] = $"Unlock dices with indexes you provided";
            }
            return result;
        }

        public string[] InGameInputs(string input)
        {
            List<string> result = new List<string>();
            switch (input)
            {
                case "roll":
                    if (CurrentPlayer.Bet >= MaxBet || CurrentPlayer.Money == 0)
                    {
                        result.AddRange(HandleRoll());
                        CurrentPlayer.DiceScore();
                        RoundEnd = true;
                    }
                    else
                    {
                        result.Add("  Your bet is too low");
                    }
                    break;
                case "surrender":
                    CurrentPlayer.Surender();
                    RoundEnd = true;
                    break;
                default:
                    result.AddRange(HandleMultiParameterCommand(input));
                    break;
            }
            result.Add("  Press [enter] to continue");
            return result.ToArray();
        }

        private List<string> HandleRoll()
        {
            List<string> result = new List<string>();
            Roll(CurrentPlayer.DiceSet);
            result.Add($"  Result of your roll: {Status(CurrentPlayer.DiceSet)}");
            return result;
        }

        private List<string> HandleMultiParameterCommand(string input)
        {
            List<string> result = new List<string>();
            var parts = input.Split(' ');
            int[] parameters = new int[parts.Length - 1];
            if (parts.Length > 1)
            {
                bool parseSucces = int.TryParse(parts[1], out parameters[0]);
                if (parts[0] == "add")
                {
                    parseSucces = true;
                }
                for (int i = 2; i < parts.Length; i++)
                {
                    if (!int.TryParse(parts[i], out parameters[i - 1])) { parseSucces = false; break; }
                }
                if (parts.Length < 2 || !parseSucces)
                {
                    result.Add("  Invalid command or parameter");
                }
                else
                {
                    bool positive = true;
                    bool validDiceIndex = true;
                    foreach (var parameter in parameters)
                    {
                        if (parameter < 0) { positive = false; break; }
                        if (Players.Length > 0)
                        {
                            if (parameter >= CurrentPlayer.DiceSet.Length) { validDiceIndex = false; }
                        }
                    }
                    if (positive)
                    {

                        switch (parts[0])
                        {
                            case "lock":
                                if (validDiceIndex && CurrentPlayer.RollsLeft < 2 && !InMenu)
                                {
                                    foreach (var parameter in parameters)
                                    {
                                        Lock(CurrentPlayer.DiceSet[parameter]);
                                        result.Add($"  Dice {parameter} locked successfully");
                                    }
                                }
                                else
                                {
                                    result.Add("  Numerical parameter out of range");
                                }
                                break;
                            case "unlock":
                                if (validDiceIndex && CurrentPlayer.RollsLeft < 2 && !InMenu)
                                {
                                    foreach (var parameter in parameters)
                                    {
                                        Unlock(CurrentPlayer.DiceSet[parameter]);
                                        result.Add($"  Dice {parameter} unlocked successfully");
                                    }
                                }
                                else
                                {
                                    result.Add("  Numerical parameter out of range");
                                }
                                break;
                            case "raise":
                                if (parameters.Length == 1 && !InMenu)
                                {
                                    if (parameters[0] <= CurrentPlayer.Money && parameters[0] > 0)
                                    {
                                        CurrentPlayer.RaiseBet(parameters[0]);
                                        SetBank();
                                        result.Add($"  You have raised by:                        {parameters[0]}$");
                                        result.Add($"  Bank has now value of:                     {Bank}$");
                                    }
                                    else
                                    {
                                        result.Add("  You don't have enough money");
                                    }

                                }
                                else
                                {
                                    result.Add("  Too many parameters");
                                }
                                break;
                            case "remove":
                                if (parameters.Length == 1 && InMenu)
                                {
                                    if (parameters[0] < Players.Length && Players.Length > 0)
                                    {
                                        Player[] temp = new Player[Players.Length - 1];
                                        for (int i = 0; i < parameters[0]; i++)
                                        {
                                            temp[i] = Players[i];
                                        }
                                        for (int i = parameters[0] + 1; i < Players.Length; i++)
                                        {
                                            temp[i - 1] = Players[i];
                                        }
                                        Players = temp;
                                        result.Add($"  Player number {parameters[0]} was removed");
                                    }
                                    else
                                    {
                                        result.Add("  No such Player exist");
                                    }

                                }
                                else
                                {
                                    result.Add("  Too many parameters");
                                }
                                break;
                            case "add":
                                if (parameters.Length == 2 && InMenu)
                                {
                                    Player[] temp = new Player[Players.Length + 1];
                                    if (Players.Length > 0)
                                    {
                                        for (int i = 0; i < Players.Length; i++)
                                        {
                                            temp[i] = Players[i];
                                        }
                                    }
                                    Dice[] dices = new Dice[5];
                                    for (int i = 0; i < dices.Length; i++)
                                    {
                                        dices[i] = new Dice([1, 2, 3, 4, 5, 6]);
                                    }
                                    temp[temp.Length - 1] = new Player(parts[1], parameters[1], dices);
                                    Players = temp;
                                    result.Add($"  Player {Players[Players.Length - 1].Name} added");
                                }
                                else
                                {
                                    if (parameters.Length < 2)
                                    {
                                        result.Add("  Not enought parameters");
                                    }
                                    else
                                    {
                                        result.Add("  Too many parameters");
                                    }
                                }
                                break;
                            default:
                                result.Add("  Invalid command");
                                break;
                        }
                    }
                    else
                    {
                        result.Add("  Numerical parameter cant be smaller than 0");
                    }
                }
            }
            else
            {
                result.Add("  Invalid command");
            }
            return result;
        }
        public void Roll(Dice[] diceSet)
        {
            for (int i = 0; i < diceSet.Length; i++)
            {
                if (!diceSet[i].Lock)
                {
                    diceSet[i].Roll();
                }

            }
        }
        public void SetLock(Dice dice, bool val)
        {
            dice.Lock = val;
        }
        public void Unlock(Dice dice)
        {
            SetLock(dice, false);
        }
        public void Lock(Dice dice)
        {
            SetLock(dice, true);
        }
        public string Status(Dice[] diceSet, bool returnLocks = false)
        {
            string output = diceSet[0].Status(returnLocks);
            for (int i = 1; i < diceSet.Length; i++)
            {
                output += ", " + diceSet[i].Status(returnLocks);
            }
            return output;
        }

        public string[] FinishRound()
        {
            float maxScore = 0;
            int winnerIndex = 0;
            List<string[]> winnerInfo = new List<string[]>();
            List<string> result = new List<string>();
            List<Player> temp = new List<Player>();
            bool distributed = false;
            temp.AddRange(Players);
            while (!distributed)
            {
                for (int i = 0; i < temp.Count; i++)
                {
                    if (!temp[i].Surrendered && temp[i].Score > maxScore)
                    {
                        maxScore = temp[i].Score;
                        winnerIndex = i;
                    }
                }
                if (temp[winnerIndex].Bet < MaxBet)
                {
                    int maxWin = temp[winnerIndex].Bet * temp.Count;
                    int win = 0;
                    if (maxWin < Bank)
                    {
                        win = maxWin;
                    }
                    else if (maxWin >= Bank)
                    {
                        win = Bank;
                        distributed = true;
                    }
                    temp[winnerIndex].Win(win);
                    winnerInfo.Add([$"  Name: {temp[winnerIndex].Name}", $"  Win amount: {win}", $"  Winning Dices: {Status(temp[winnerIndex].DiceSet)}"]);
                    Bank -= maxWin;
                    temp.Remove(temp[winnerIndex]);
                }
                else
                {
                    temp[winnerIndex].Win(Bank);
                    distributed = true;
                    winnerInfo.Add([$"  Name: {temp[winnerIndex].Name}", $"  Win amount: {Bank}", $"  Winning Dices: {Status(temp[winnerIndex].DiceSet)}"]);
                }
            }
            result.AddRange(Style.Margin(Style.SectionedArticle(winnerInfo.ToArray(), "Winner"), 5, (int)Style.Direction.top));
            Bank = 0;
            MaxBet = 0;
            RoundEnd = false;
            playerIndex = 0;
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i].Reset();
            }
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = Style.textAlign(2, Console.WindowWidth, result[i]);
            }
            result.Add("  " + new string('─', Console.WindowWidth - 4));
            return result.ToArray();
        }

        public string[] MenuInput(string input)
        {
            List<string> result = new List<string>();
            if (input == "play")
            {
                if (Players.Length > 1)
                {
                    InMenu = false;
                    result.Add("  Round will start after you press [enter]");
                }
                else
                {
                    result.Add("  Not enought players to start");
                }

            }
            else if (input == "exit")
            {
                End = true;
                InMenu = false;
                result.Add("  Game will end after you press [enter]");
            }
            else
            {
                result.AddRange(HandleMultiParameterCommand(input));
            }
            result.Add("  Press [enter] to continue");
            return result.ToArray();
        }

        public string[] MenuOutput()
        {
            List<string> result = new List<string>();
            result.AddRange(Style.SectionedArticle([], "   Welcome to Dwarwen Poker   "));
            result.AddRange(Style.Table(["Syntax", "Effect"], [["play", "add <name> <moneyAmount>", "remove <index>"], ["Starts next round", "Add new player with <name> and <moneyAmount>", "Removes player by index"]]));
            if (Players.Length > 0)
            {
                result.AddRange(Style.Table(["Index", "Name", "Money"], PlayerData()));
            }
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = Style.textAlign(2, Console.WindowWidth, result[i]);
            }
            result.Add("  " + new string('─', Console.WindowWidth - 4));
            return result.ToArray();
        }
    }
}

