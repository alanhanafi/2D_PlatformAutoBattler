using UnityEngine;

namespace Platformer
{
    public class RoomCenterTracker : MonoBehaviour
    {
        [SerializeField] private Transform roomCenterTransform;

        public Transform RoomCenterTransform => roomCenterTransform;

        private void OnDrawGizmos()
        {
            if (roomCenterTransform == null) 
                return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(roomCenterTransform.position, Vector3.one);
        }
    }
}