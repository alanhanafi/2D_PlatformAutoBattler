using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// When the player touches the collider, he dies.
    /// </summary>
    public class KillZone : MonoBehaviour
    {
        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log($"Player {other.gameObject.name} collided with {gameObject.name}");
                PlatformerManager.Instance.KillPlayer();
            }
        }
    }
}