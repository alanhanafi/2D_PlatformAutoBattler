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
            // Enters the start room on game start
            if (this.roomInstance.Room.GetDisplayName() == Values.StartRoomName)
                PlatformerManager.Instance.OnGameStart += OnGameStarting;
        }

        private void OnGameStarting(object sender, EventArgs e)
        {
            OnRoomEntered();
        }

        [Button]
        public void OnRoomEntered()
        {
            PlatformerManager.Instance.EnterRoom(FindObjectsInPath());
        }

        // TODO : Update position of the next room properly
        private Dictionary<MainItem,Vector3> FindObjectsInPath()
        {
            var itemsInfoInPath = new Dictionary<MainItem,Vector3>();
            foreach (var currentRoomDoor in roomInstance.Doors)
            {
                ExploreRooms(roomInstance, currentRoomDoor.ConnectedRoomInstance,
                    currentRoomDoor.ConnectedRoomInstance.RoomCenterTransform.position, itemsInfoInPath);
            }
            if(roomInstance.MainItem!=null)
                itemsInfoInPath.Add(roomInstance.MainItem,roomInstance.Position +roomInstance.RoomTemplateInstance.gameObject.transform.position);
            return itemsInfoInPath;
        }

        private void ExploreRooms(RoomInstanceGrid2D roomInstanceGrid2D, RoomInstanceGrid2D connectedRoomInstance, Vector3 nextRoomPosition, Dictionary<MainItem,Vector3> itemsInfoInPath)
        {
            if (connectedRoomInstance.Room.GetDisplayName() == Values.StartRoomName)
            {
                itemsInfoInPath.Add(null!,nextRoomPosition);
                return;
            }
            if(connectedRoomInstance.MainItem!=null)
                itemsInfoInPath.Add(connectedRoomInstance.MainItem,nextRoomPosition);
            
            foreach (var doorInstance in connectedRoomInstance.Doors)
            {
                if(doorInstance.ConnectedRoomInstance!=roomInstanceGrid2D)
                    ExploreRooms(connectedRoomInstance, doorInstance.ConnectedRoomInstance, nextRoomPosition, itemsInfoInPath);
            }
        }
    }
}