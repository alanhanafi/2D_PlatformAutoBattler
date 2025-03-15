using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace
{
    public class FallingObstacle : KillZone
    {
        public override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (!other.gameObject.CompareTag("Player"))
                gameObject.SetActive(false);
        }
    }
}