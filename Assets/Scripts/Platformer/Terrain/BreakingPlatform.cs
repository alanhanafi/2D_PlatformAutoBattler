using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    public class BreakingPlatform : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log($"Player {other.gameObject.name} collided with {gameObject.name}");
                GetComponent<TilemapRenderer>().enabled = false;
                //PlatformerManager.Instance.BumpPlayer();
                //InventoryManager.Instance.AddBonusItem(bonusItem);
                //Destroy(gameObject);
            }
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log($"Player {other.gameObject.name} collided with {gameObject.name}");
                GetComponent<SpriteRenderer>().enabled = false;
                //PlatformerManager.Instance.BumpPlayer();
                //InventoryManager.Instance.AddBonusItem(bonusItem);
                //Destroy(gameObject);
            }
        }
    }
}