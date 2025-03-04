using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item")]
    public class Item : ScriptableObject
    {
        [field :SerializeField]
        internal Sprite Sprite { get; private set; }
        
        [field :SerializeField]
        internal Color Color { get; private set; } = Color.white;
        
    }
}