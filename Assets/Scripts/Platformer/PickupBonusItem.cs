using System.Collections.Generic;
using Shared;
using Shared.Main_Items;
using UnityEngine;

namespace Platformer
{
    public class PickupBonusItem : MonoBehaviour, PickupItem
    {
        [SerializeField] private SpriteRenderer bonusItemSpriteRenderer;
        [SerializeField] private BonusItem[] availableBonusItems;
        
        public Sprite ItemSprite => bonusItem.Sprite;
        public string ItemName => bonusItem.Name;
        public string ItemDescription => bonusItem.Description;
        
        [SerializeField,HideInInspector]
        private BonusItem bonusItem;
        
        internal BonusItem BonusItem => bonusItem;

        public void Initialize(List<MainItem> spawnedItems)
        {
            bonusItem = availableBonusItems[Random.Range(0, availableBonusItems.Length)];
            #if UNITY_EDITOR
            Functions.SetObjectDirty(bonusItem);
            #endif
            GetComponent<SpriteRenderer>().color = bonusItem.Color;
            bonusItemSpriteRenderer.sprite = bonusItem.Sprite;
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
                InventoryManager.Instance.AddBonusItem(this);
                Destroy(gameObject);
            }
        }
        
    }
}