using UnityEngine;

namespace Platformer.Terrain
{
    /// <summary>
    /// When the player touches the collider, he dies.
    /// </summary>
    public class KillZone : MonoBehaviour
    {
        [SerializeField] private bool isSandbox;
        
        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;
            Debug.Log($"Player {other.gameObject.name} collided with {gameObject.name}");
            if (isSandbox)
                SandboxManager.Instance.RespawnPlayer();
            else
                PlatformerManager.Instance.KillPlayer();
        }
    }
}