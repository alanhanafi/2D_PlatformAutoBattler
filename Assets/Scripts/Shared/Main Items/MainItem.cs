﻿using AutoBattle;
using UnityEngine;

namespace Shared.Main_Items
{
    public abstract class MainItem : ScriptableObject
    {
        [field :SerializeField]
        internal Sprite Sprite { get; private set; }
        
        [field :SerializeField]
        internal string Name { get; private set; }
        
        [field :SerializeField]
        internal string Description { get; private set; }


        internal virtual string GetDescription()
        {
            return Description;
        }
        internal virtual void InitializeForAutoBattle(AutoBattlePlayerState owner)
        {
            owner.OnDamageDealt += OnDealingDamage;
            owner.OnDamageReceived += OnReceivingDamage;
        }

        internal abstract void OnReceivingDamage(object sender, 
            (AutoBattlePlayerState source, int damage, bool isDirect) eventArgs);

        internal abstract void OnDealingDamage(object sender,
            (AutoBattlePlayerState target, int damage, bool isDirect) eventArgs);

    }
}