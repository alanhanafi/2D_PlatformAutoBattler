using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Board Item", menuName = "Items/Board Item")]
    public class BoardItem : ScriptableObject
    {
        [field :SerializeField]
        internal Sprite Sprite { get; private set; }
        
        [field :SerializeField]
        internal Color Color { get; private set; } = Color.white;

        [field: SerializeField] 
        internal float CooldownInSeconds { get; private set; } = 2;

        [SerializeField] private int damage = 10;
        
        // Should be handled differently in a trigger function
        internal int Damage => damage;
        
        


    }
}