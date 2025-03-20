using UnityEngine;

namespace DefaultNamespace
{
    public class PoisonState
    {
        internal const float PoisonTimeTrigger = 1f;
        
        internal bool IsPoisoned => poisonValue > 0;
        
        internal int poisonValue;
        internal float poisonTimer;

        internal void PassTime(float secondsPassed, AutoBattlePlayerState poisonedPlayer)
        {
            Debug.Log($"Poisoned {secondsPassed}, {poisonTimer}");
            poisonTimer += secondsPassed;
            if (poisonTimer >= PoisonTimeTrigger)
            {
                poisonedPlayer.TriggerPoison(poisonValue);
                poisonTimer -= PoisonTimeTrigger;
            }
        }

        internal void AddPoisonValue(int addedPoisonValue)
        {
            poisonValue += addedPoisonValue;
        }

        internal PoisonState()
        {
            poisonValue = 0;
            poisonTimer = 0;
        }
    }
}