using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Bonus Item", menuName = "Items/Bonus Item")]
    public class BonusItem : ScriptableObject
    {
        [field: SerializeField] 
        internal int BonusHealth { get; private set; } = 10;
        [field: SerializeField] 
        internal int BonusDamage { get; private set; } = 10;
        [field: SerializeField] 
        internal float BonusAttackSpeed { get; private set; } = .1f;
        
        [field: SerializeField] 
        internal Color Color { get; private set; } = Color.white;
        
        [field: SerializeField] 
        internal Sprite Sprite { get; private set; }
        
    }
}