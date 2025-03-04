using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))][RequireComponent(typeof(Collider2D))]
    public class PickupItem : MonoBehaviour
    {
        [SerializeField] private Item[] availableItems;
        
        private Item item;

        internal void Initialize()
        {
            item = availableItems[Random.Range(0, availableItems.Length)];
            GetComponent<SpriteRenderer>().color = item.Color;
        }

        /// <summary>
        /// Adds the item to the player's inventory when colliding with the player.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log($"Player {other.gameObject.name} collided with {gameObject.name}");
                other.GetComponent<InventoryManager>().AddItem(item);
                Destroy(gameObject);
            }
        }
    }
}