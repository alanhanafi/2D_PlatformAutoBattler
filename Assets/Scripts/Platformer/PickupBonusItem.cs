using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class PickupBonusItem : MonoBehaviour, PickupItem
    {
        [SerializeField] private BonusItem[] availableBonusItems;
        
        [SerializeField,HideInInspector]
        private BonusItem bonusItem;

        public void Initialize()
        {
            bonusItem = availableBonusItems[Random.Range(0, availableBonusItems.Length)];
            Functions.SetObjectDirty(bonusItem);
            GetComponent<SpriteRenderer>().color = bonusItem.Color;
        }

        /// <summary>
        /// Adds the item to the player's inventory when colliding with the player.
        /// </summary>
        /// <param name="other"></param>
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log($"Player {other.gameObject.name} collided with {gameObject.name}");
                InventoryManager.Instance.AddBonusItem(bonusItem);
                Destroy(gameObject);
            }
        }
        
    }
}