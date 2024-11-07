using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dwarf_poker
{
    static class Actions
    {
        public static void Roll(Dice[] diceSet)
        {
            for (int i = 0; i < diceSet.Length; i++)
            {
                if (!diceSet[i].IsLocked)
                {
                    diceSet[i].Roll();
                }
                    
            }
        }
        public static void SetLock (Dice dice, bool val)
        {
            dice.IsLocked = val;
        }
        public static void Unlock (Dice dice)
        {
            SetLock(dice, false);
        }
        public static void Lock(Dice dice)
        {
            SetLock(dice, true);
        }
        public static string Status(Dice[] diceSet, bool returnLocks = false)
        {
            string output = diceSet[0].Status(returnLocks);
            for (int i = 1; i < diceSet.Length; i++)
            {
                output += ", " + diceSet[i].Status(returnLocks);
            }
            return output;
        }
        
    }
}
