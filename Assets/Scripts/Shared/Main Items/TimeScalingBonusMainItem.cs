using System;
using AutoBattle;
using UnityEngine;

namespace Shared.Main_Items
{
    [CreateAssetMenu(fileName = "Time Scaling Item", menuName = "Items/Main Item/Time Scaling Item")]
    public class TimeScalingBonusMainItem : MainItem
    {
        internal float TimePassedAtPickup { get; set; }

        /// <summary>
        /// Gets the bonus type scaling with the time.
        /// Assumes there is only one bonus type per item.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal BonusType GetBonusType()
        {
            if (healthTimePassedScaling > 0)
                return BonusType.Health;
            if(damageTimePassedScaling > 0)
                return BonusType.Damage;
            if(attackSpeedTimePassedScaling > 0)
                return BonusType.AttackSpeed;
            throw new Exception("All scaling bonuses are null");
        }
        
        
        internal float GetBonusValue()
        {
            float baseValue = 0;
            switch (GetBonusType())
            {
                case BonusType.Health:
                    baseValue = healthTimePassedScaling;
                    break;
                case BonusType.Damage:
                    baseValue = damageTimePassedScaling;
                    break;
                case BonusType.AttackSpeed:
                    baseValue = attackSpeedTimePassedScaling;
                    break;
            }
            return baseValue * TimePassedAtPickup;
        }

        [SerializeField] private float healthTimePassedScaling;
        
        [SerializeField] private float damageTimePassedScaling;
        
        [SerializeField] private float attackSpeedTimePassedScaling;

        internal override void InitializeForAutoBattle(AutoBattlePlayerState owner)
        {
            base.InitializeForAutoBattle(owner);
            switch (GetBonusType())
            {
                case BonusType.Health:
                    owner.CurrentHealth += Functions.GetUpRoundedValue(GetBonusValue());
                    owner.MaxHealth += Functions.GetUpRoundedValue(GetBonusValue());
                    break;
                case BonusType.Damage:
                    owner.AttackDamage += Functions.GetUpRoundedValue(GetBonusValue());
                    break;
                case BonusType.AttackSpeed:
                    owner.AttackSpeed += GetBonusValue();
                    break;
            }
        }

        internal override void OnReceivingDamage(object sender, (AutoBattlePlayerState source, int damage, bool isDirect) eventArgs)
        {
        }

        internal override void OnDealingDamage(object sender, (AutoBattlePlayerState target, int damage, bool isDirect) eventArgs)
        {
        }

        internal override string GetDescription()
        {
            return Description.Replace("{type}",GetType().ToString()).Replace("{value}",$"{Functions.GetUpRoundedValue(GetBonusValue())}");
        }
    }
}