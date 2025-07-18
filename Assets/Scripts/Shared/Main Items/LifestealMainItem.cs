﻿using System;
using AutoBattle;
using UnityEngine;

namespace Shared.Main_Items
{
    /// <summary>
    /// Heals the owner of the item every time he deals damage.
    /// </summary>
    [CreateAssetMenu(fileName = "LifeSteal Item", menuName = "Items/Main Item/LifeSteal Item")]
    public class LifestealMainItem : MainItem
    {
        [SerializeField] private int lifeStealPercentage = 10;
        
        internal override void OnDealingDamage(object sender,
            (AutoBattlePlayerState target, int damage, bool isDirect) eventArgs)
        {
            (sender as AutoBattlePlayerState)?.SelfHeal((int)Math.Round(eventArgs.damage*lifeStealPercentage/100f,MidpointRounding.AwayFromZero),true);
        }

        internal override void OnReceivingDamage(object sender, 
            (AutoBattlePlayerState source, int damage, bool isDirect) eventArgs)
        {
        }
    }
}