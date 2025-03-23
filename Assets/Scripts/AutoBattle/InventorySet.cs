using System.Collections.Generic;
using Shared.Main_Items;
using UnityEngine;

namespace AutoBattle
{
    [CreateAssetMenu(fileName = "Inventory Set", menuName = "Items/Inventory Set")]
    public class InventorySet : ScriptableObject
    {
        [field: SerializeField] internal int HealthBonusCount = 10;
        [field: SerializeField] internal int DamageBonusCount = 10;
        [field: SerializeField] internal int AttackSpeedBonusCount = 10;
        
        [field : SerializeField] internal List<MainItem> MainItems;
    }
}