using System;
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
        
        [FormerlySerializedAs("boardItem")] [SerializeField,HideInInspector]
        private MainItem mainItem;

        public void Initialize()
        {
            mainItem = availableItems[Random.Range(0, availableItems.Length)];
            Functions.SetObjectDirty(mainItem);
            GetComponent<SpriteRenderer>().sprite = mainItem.Sprite;
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
                InventoryManager.Instance.AddBoardItem(mainItem);
                Destroy(gameObject);
            }
        }
    }
}