using System.Collections.Generic;
using Shared.Main_Items;
using UnityEngine;

namespace Platformer
{
    public class RoomInPathData
    {
        public Vector3? DoorToStartPosition { get; set; }
        public Vector3 RoomPosition { get; set; }
        public List<DoorInfoInPath> ItemDoorInfoList { get; set; }

        public RoomInPathData(Vector3? doorToStartPosition, Vector3 roomPosition, Vector3Int[] doorsPosToItem)
        {
            DoorToStartPosition = doorToStartPosition;
            RoomPosition = roomPosition;
            List<DoorInfoInPath> itemDoorInfoList = new();
            foreach (var doorPosToItem in doorsPosToItem)
            {
                itemDoorInfoList.Add(new(doorPosToItem));
            }
            ItemDoorInfoList = itemDoorInfoList;
        }
    }

    public class DoorInfoInPath
    {
        public Vector3Int DoorPosToItem { get;}
        
        public MainItem MainItem { get; set; }

        public DoorInfoInPath(Vector3Int doorPosToItem)
        {
            DoorPosToItem = doorPosToItem;
            MainItem = null;
        }
    } 
}