using System;
using System.Collections.Generic;
using Shared.Main_Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Platformer
{
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField] private bool isIntermediateRoom;
        
        [SerializeField] private Transform[] mainSpawnTransforms;
        [SerializeField] private Transform[] bonusSpawnTransforms;
        
        [SerializeField] private PickupMainItem pickupMainItemPrefab;
        
        [SerializeField] private PickupBonusItem pickupBonusItemPrefab;

        public MainItem SpawnItems(List<MainItem> spawnedItems)
        {
            SpawnBonusItem(spawnedItems);
            return SpawnMainItem(spawnedItems);
        }

        private void SpawnBonusItem(List<MainItem> spawnedItems)
        {
            if (bonusSpawnTransforms.Length == 0)
                return;
            Transform itemSpawnerTransform = bonusSpawnTransforms[Random.Range(0, bonusSpawnTransforms.Length)];
            var pickupItem = Instantiate(pickupBonusItemPrefab, itemSpawnerTransform);
            pickupItem.GetComponent<PickupItem>().Initialize(spawnedItems,isIntermediateRoom);
        }

        private MainItem SpawnMainItem(List<MainItem> spawnedItems)
        {
            if (mainSpawnTransforms.Length == 0)
                return null;
            Transform itemSpawnerTransform = mainSpawnTransforms[Random.Range(0, mainSpawnTransforms.Length)];
            var pickupItem = Instantiate(pickupMainItemPrefab, itemSpawnerTransform);
            pickupItem.Initialize(spawnedItems,isIntermediateRoom);
            return pickupItem.MainItem;
        }

        private void OnDrawGizmos()
        {
            if (mainSpawnTransforms.Length > 0)
            {
                Gizmos.color = Color.blue;
                foreach (Transform mainTransform in mainSpawnTransforms)
                {
                    Gizmos.DrawSphere(mainTransform.position, .4f);   
                }
            }
            if (bonusSpawnTransforms.Length > 0)
            {
                Gizmos.color = Color.green;
                foreach (Transform mainTransform in bonusSpawnTransforms)
                {
                    Gizmos.DrawSphere(mainTransform.position, .4f);   
                }
            }
        }
    }
}