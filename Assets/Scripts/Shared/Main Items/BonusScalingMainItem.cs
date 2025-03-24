using System;
using System.Linq;
using AutoBattle;
using UnityEngine;

namespace Shared.Main_Items
{
    [CreateAssetMenu(fileName = "Bonus Scaling Item", menuName = "Items/Main Item/Bonus Scaling Item")]
    public class BonusScalingMainItem : MainItem
    {
        [field : SerializeField]
        internal BonusType BonusType { get; private set; }

        [SerializeField] private float valuePerBonus;

        internal override void InitializeForAutoBattle(AutoBattlePlayerState owner)
        {
            base.InitializeForAutoBattle(owner);
            int bonusesOwned = owner.BonusItemsList.Count(item=>item.BonusType == BonusType);
            switch (BonusType)
            {
                case BonusType.Health:
                    owner.MaxHealth += Functions.GetUpRoundedValue(valuePerBonus*bonusesOwned);
                    owner.CurrentHealth += owner.MaxHealth;
                    break;
                case BonusType.Damage:
                    owner.AttackDamage += Functions.GetUpRoundedValue(valuePerBonus*bonusesOwned);
                    break;
                case BonusType.AttackSpeed:
                    owner.AttackSpeed += valuePerBonus*bonusesOwned;
                    break;
            }
        }

        internal override string GetDescription()
        {
            return Description.Replace("{type}", BonusType.ToString()).Replace("{value}", valuePerBonus.ToString());
        }

        internal override void OnReceivingDamage(object sender, (AutoBattlePlayerState source, int damage, bool isDirect) eventArgs)
        {
            throw new System.NotImplementedException();
        }

        internal override void OnDealingDamage(object sender, (AutoBattlePlayerState target, int damage, bool isDirect) eventArgs)
        {
            throw new System.NotImplementedException();
        }
    }
}