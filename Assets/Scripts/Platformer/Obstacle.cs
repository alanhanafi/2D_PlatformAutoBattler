using UnityEngine;

namespace DefaultNamespace
{
    public class Obstacle : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log($"Player {other.gameObject.name} collided with {gameObject.name}");
                PlatformerManager.Instance.KillPlayer();
                //InventoryManager.Instance.AddBonusItem(bonusItem);
                //Destroy(gameObject);
            }
        }
    }
}