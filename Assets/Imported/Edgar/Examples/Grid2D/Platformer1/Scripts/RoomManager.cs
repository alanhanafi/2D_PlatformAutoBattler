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
        }

        [Button]
        public void OnRoomEntered()
        {
            PlatformerManager.Instance.EnterRoom(FindObjectsInPath());
        }

        private List<(Vector3, MainItem)> FindObjectsInPath()
        {
            var itemsInfoInPath = new List<(Vector3, MainItem)>();
            foreach (var currentRoomDoor in roomInstance.Doors)
            {
                ExploreRooms(roomInstance, currentRoomDoor.ConnectedRoomInstance,
                    currentRoomDoor.ConnectedRoomInstance.Position, itemsInfoInPath);
            }
            if(roomInstance.MainItem!=null)
                itemsInfoInPath.Add((roomInstance.Position, roomInstance.MainItem));
            return itemsInfoInPath;
        }

        private void ExploreRooms(RoomInstanceGrid2D roomInstanceGrid2D, RoomInstanceGrid2D connectedRoomInstance, Vector3Int nextRoomPosition, List<(Vector3, MainItem)> itemsInfoInPath)
        {
            if (connectedRoomInstance.Room.GetDisplayName() == Values.StartRoomName)
                return;
            if(connectedRoomInstance.MainItem!=null)
                itemsInfoInPath.Add((nextRoomPosition, connectedRoomInstance.MainItem));
            
            foreach (var doorInstance in connectedRoomInstance.Doors)
            {
                if(doorInstance.ConnectedRoomInstance!=roomInstanceGrid2D)
                    ExploreRooms(connectedRoomInstance, doorInstance.ConnectedRoomInstance, nextRoomPosition, itemsInfoInPath);
            }
        }
    }
}