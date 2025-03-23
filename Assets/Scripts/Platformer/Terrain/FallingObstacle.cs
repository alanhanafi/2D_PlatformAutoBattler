using UnityEngine;

namespace Platformer.Terrain
{
    public class FallingObstacle : KillZone
    {
        [SerializeField]
        private float rotationSpeed = 200f;

        void Update()
        {
            // Rotate the obstacle around the Z-axis
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }

        public override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            if (!other.gameObject.CompareTag("Player"))
                gameObject.SetActive(false);
        }
    }
}