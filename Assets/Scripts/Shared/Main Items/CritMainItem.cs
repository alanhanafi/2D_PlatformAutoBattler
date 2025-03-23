using AutoBattle;
using UnityEngine;

namespace Shared.Main_Items
{
    [CreateAssetMenu(fileName = "Crit Item", menuName = "Items/Main Item/Crit Item")]
    public class CritMainItem : MainItem
    {
        internal override void OnDealingDamage(object sender,
            (AutoBattlePlayerState target, int damage, bool isDirect) eventArgs)
        {
        }

        internal override void OnReceivingDamage(object sender, 
            (AutoBattlePlayerState source, int damage, bool isDirect) eventArgs)
        {
        }
    }
}