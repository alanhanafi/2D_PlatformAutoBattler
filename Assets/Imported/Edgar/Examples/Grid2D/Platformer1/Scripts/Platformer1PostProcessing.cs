using System;
using System.Collections.Generic;
using System.Linq;
using Platformer;
using Shared;
using Shared.Main_Items;
using UnityEngine;

namespace Edgar.Unity.Examples.Scripts
{
    #region codeBlock:2d_platformer1_postProcessing

    [CreateAssetMenu(menuName = "Edgar/Examples/Platformer 1/Post-processing", fileName = "Platformer1PostProcessing")]
    public class Platformer1PostProcessing : DungeonGeneratorPostProcessingGrid2D
    {
        public override void Run(DungeonGeneratorLevelGrid2D level)
        {
            SetSpawnPosition(level);
            RemoveWallsFromDoors(level);
        }
        /// <summary>
        /// Move the player to the spawn point of the level.
        /// </summary>
        /// <param name="level"></param>
        private void SetSpawnPosition(DungeonGeneratorLevelGrid2D level)
        {
            // Find the room with the Entrance type
            var entranceRoomInstance = level
                .RoomInstances
                .FirstOrDefault(x => x.Room.GetDisplayName() == Values.StartRoomName);

            if (entranceRoomInstance == null)
            {
                throw new InvalidOperationException("Could not find Entrance room");
            }

            var roomTemplateInstance = entranceRoomInstance.RoomTemplateInstance;

            // Find the spawn position marker
            var spawnPosition = roomTemplateInstance.transform.Find("SpawnPosition");

            // Move the player to the spawn position
            var player = GameObject.FindWithTag("Player");
            player.transform.position = spawnPosition.position;
        }

        private void RemoveWallsFromDoors(DungeonGeneratorLevelGrid2D level)
        {
            // Get the tilemap that we want to delete tiles from
            var walls = level.GetSharedTilemaps().Single(x => x.name == "Walls");

            List<MainItem> spawnedItems = new List<MainItem>();
            
            // Go through individual rooms
            foreach (var roomInstance in level.RoomInstances)
            {
                // Spawn items in each room
                Transform itemSpawnerTransform = roomInstance.RoomTemplateInstance.transform.Find("ItemSpawner");
                if (itemSpawnerTransform != null)
                    itemSpawnerTransform.GetComponent<ItemSpawner>().SpawnItems(spawnedItems);
                
                // Go through individual doors
                foreach (var doorInstance in roomInstance.Doors)
                {
                    // Remove all the wall tiles from door positions
                    foreach (var point in doorInstance.DoorLine.GetPoints())
                    {
                        walls.SetTile(point + roomInstance.Position, null);
                    }
                }
            }
        }
    }

    #endregion
}