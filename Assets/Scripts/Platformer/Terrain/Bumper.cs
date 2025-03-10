using UnityEngine;

namespace DefaultNamespace
{
    public class Bumper : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log($"Player {other.gameObject.name} collided with {gameObject.name}");
                PlatformerManager.Instance.BumpPlayer();
                //InventoryManager.Instance.AddBonusItem(bonusItem);
                //Destroy(gameObject);
            }
        }
        
    }
}