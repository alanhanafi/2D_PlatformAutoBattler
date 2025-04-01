using System;
using UnityEngine;

namespace Edgar.Unity.Examples.Scripts
{
    public class RoomEnterTrigger : MonoBehaviour
    {
        [SerializeField] private RoomManager roomManager;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag("Player"))
                roomManager.OnRoomEntered();
        }
    }
}