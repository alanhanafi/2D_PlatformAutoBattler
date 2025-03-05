using UnityEngine;

namespace DefaultNamespace
{
    public interface PickupItem
    {
        public void Initialize();
        
        public void OnTriggerEnter2D(Collider2D other);
    }
}