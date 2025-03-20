using System;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Applies poison on each hit.
    /// </summary>
    [CreateAssetMenu(fileName = "Poison Item", menuName = "Items/Main Item/Poison Item")]
    public class PoisonMainItem : MainItem
    {
        [SerializeField] private int percentDamageToPoison = 10;
        
        internal override void OnDealingDamage(object sender,
            (AutoBattlePlayerState target, int damage, bool isDirect) eventArgs)
        {
            if (!eventArgs.isDirect)
                return;
            eventArgs.target.ApplyPoison((int)Math.Round(eventArgs.damage*percentDamageToPoison/100f,MidpointRounding.AwayFromZero));
        }

        internal override void OnReceivingDamage(object sender, 
            (AutoBattlePlayerState source, int damage, bool isDirect) eventArgs)
        {
        }
    }
}