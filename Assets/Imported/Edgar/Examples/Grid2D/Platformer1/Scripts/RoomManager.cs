using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Platformer;
using Shared;
using Shared.Main_Items;
using UnityEngine;

namespace Edgar.Unity.Examples.Scripts
{
    public class RoomManager : MonoBehaviour
    {
        private RoomInstanceGrid2D roomInstance;

        internal void Initialize(RoomInstanceGrid2D roomInstance, List<MainItem> spawnedItems)
        {
            this.roomInstance = roomInstance;
            var mainItemSpawned = GetComponent<ItemSpawner>().SpawnItems(spawnedItems);
            roomInstance.UpdateMainItem(mainItemSpawned);
            roomInstance.UpdateRoomCenterTransform();
            if(mainItemSpawned != null)
                PlatformerManager.Instance.AddAvailableMainItem(mainItemSpawned);
            // Enters the start room on game start and on respawn
            if (this.roomInstance.Room.GetDisplayName() == Values.StartRoomName)
            {
                PlatformerManager.Instance.OnGameStart += OnGameStarting;
                PlatformerManager.Instance.OnRespawn += OnRespawning;
                PlatformerManager.Instance.StartRoomTransform = this.roomInstance.RoomCenterTransform;
            }
        }

        private void OnRespawning(object sender, EventArgs e)
        {
            OnRoomEntered();
        }

        private void OnGameStarting(object sender, EventArgs e)
        {
            OnRoomEntered();
        }

        [Button]
        public void OnRoomEntered()
        {
            if(PlatformerManager.Instance.IsGameRunning)
                PlatformerManager.Instance.EnterRoom(FindObjectsInPath());
        }

        private Dictionary<MainItem,Vector3?> FindObjectsInPath()
        {
            var itemsInfoInPath = new Dictionary<MainItem,Vector3?>();
            foreach (var currentRoomDoor in roomInstance.Doors)
            {
                ExploreRooms(roomInstance, currentRoomDoor.ConnectedRoomInstance,
                    currentRoomDoor.ConnectedRoomInstance.IsCorridor?null :currentRoomDoor.ConnectedRoomInstance.RoomCenterTransform.position, itemsInfoInPath);
            }
            /* Do not show item direction if the player is in the item's room
            if(roomInstance.MainItem!=null)
                itemsInfoInPath.Add(roomInstance.MainItem,roomInstance.Position +roomInstance.RoomTemplateInstance.gameObject.transform.position);
            */
            return itemsInfoInPath;
        }

        private void ExploreRooms(RoomInstanceGrid2D roomInstanceGrid2D, RoomInstanceGrid2D connectedRoomInstance, Vector3? nextRoomPosition, Dictionary<MainItem,Vector3?> itemsInfoInPath)
        {
            if (connectedRoomInstance.Room.GetDisplayName() == Values.StartRoomName)
                return;
            if(connectedRoomInstance.MainItem!=null)
                itemsInfoInPath.Add(connectedRoomInstance.MainItem,nextRoomPosition);
            
            foreach (var doorInstance in connectedRoomInstance.Doors)
            {
                if(doorInstance.ConnectedRoomInstance!=roomInstanceGrid2D)
                    ExploreRooms(connectedRoomInstance, doorInstance.ConnectedRoomInstance, nextRoomPosition.HasValue?nextRoomPosition.Value:doorInstance.ConnectedRoomInstance.RoomCenterTransform.position, itemsInfoInPath);
            }
        }
    }
}