using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))][RequireComponent(typeof(Collider2D))]
    public class PickupBoardItem : MonoBehaviour, PickupItem
    {
        [SerializeField] private BoardItem[] availableItems;
        
        [SerializeField,HideInInspector]
        private BoardItem boardItem;

        public void Initialize()
        {
            boardItem = availableItems[Random.Range(0, availableItems.Length)];
            Functions.SetObjectDirty(boardItem);
            GetComponent<SpriteRenderer>().color = boardItem.Color;
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
                InventoryManager.Instance.AddBoardItem(boardItem);
                Destroy(gameObject);
            }
        }
    }
}