using UnityEngine;

namespace AutoBattle
{
    public class PoisonState
    {
        private const float PoisonTimeTrigger = 1f;
        
        internal bool IsPoisoned => poisonValue > 0;
        
        private int poisonValue;
        private float poisonTimer;

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