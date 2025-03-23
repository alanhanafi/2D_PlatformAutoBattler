using System;
using AutoBattle;
using UnityEngine;

namespace Shared.Main_Items
{
    /// <summary>
    /// Increase the attack damage with each direct hit received.
    /// </summary>
    [CreateAssetMenu(fileName = "Damage Armor Item", menuName = "Items/Main Item/Damage Armor Item")]
    public class DamageArmorMainItem : MainItem
    {
        [SerializeField] private int attackIncreasePercent = 5;
        
        internal override void OnDealingDamage(object sender,
            (AutoBattlePlayerState target, int damage, bool isDirect) eventArgs)
        {
            return;
        }

        internal override void OnReceivingDamage(object sender, 
            (AutoBattlePlayerState source, int damage, bool isDirect) eventArgs)
        {
            if (!eventArgs.isDirect)
                return;
            AutoBattlePlayerState receiver = sender as AutoBattlePlayerState;
            receiver.IncreaseAttackDamage((int)Math.Round(receiver.AttackDamage *attackIncreasePercent/100f, MidpointRounding.AwayFromZero)) ;
        }
    }
}