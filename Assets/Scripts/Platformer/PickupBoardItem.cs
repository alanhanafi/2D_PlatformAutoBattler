using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))][RequireComponent(typeof(Collider2D))]
    public class PickupBoardItem : MonoBehaviour, PickupItem
    {
        [SerializeField] private MainItem[] availableItems;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [SerializeField,HideInInspector]
        private MainItem mainItem;
        
        public MainItem MainItem => mainItem;
        
        public Sprite ItemSprite => mainItem.Sprite;
        public string ItemName => mainItem.Name;
        public string ItemDescription => mainItem.Description;

        public void Initialize(List<MainItem> spawnedItems)
        {
            var spawnableItems = availableItems.Where(item=> !spawnedItems.Contains(item)).ToArray();
            mainItem = spawnableItems[Random.Range(0, spawnableItems.Length)];
            Functions.SetObjectDirty(mainItem);
            spriteRenderer.sprite = mainItem.Sprite;
            spawnedItems.Add(mainItem);
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
                InventoryManager.Instance.AddBoardItem(this);
                Destroy(gameObject);
            }
        }
    }
}