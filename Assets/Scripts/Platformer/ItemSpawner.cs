using System.Collections.Generic;
using Shared.Main_Items;
using UnityEngine;

namespace Platformer
{
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField] private bool isIntermediateRoom;
        
        [SerializeField] private Transform[] mainSpawnTransforms;
        [SerializeField] private Transform[] bonusSpawnTransforms;
        
        [SerializeField] private GameObject pickupMainItemPrefab;
        
        [SerializeField] private GameObject pickupBonusItemPrefab;

        public void SpawnItems(List<MainItem> spawnedItems)
        {
            SpawnMainItem(spawnedItems);
            SpawnBonusItem(spawnedItems);
        }

        private void SpawnBonusItem(List<MainItem> spawnedItems)
        {
            if (bonusSpawnTransforms.Length == 0)
                return;
            Transform itemSpawnerTransform = bonusSpawnTransforms[Random.Range(0, bonusSpawnTransforms.Length)];
            var pickupItem = Instantiate(pickupBonusItemPrefab, itemSpawnerTransform);
            pickupItem.GetComponent<PickupItem>().Initialize(spawnedItems,isIntermediateRoom);
        }

        private void SpawnMainItem(List<MainItem> spawnedItems)
        {
            if (mainSpawnTransforms.Length == 0)
                return;
            Transform itemSpawnerTransform = mainSpawnTransforms[Random.Range(0, mainSpawnTransforms.Length)];
            var pickupItem = Instantiate(pickupMainItemPrefab, itemSpawnerTransform);
            pickupItem.GetComponent<PickupItem>().Initialize(spawnedItems,isIntermediateRoom);
        }
    }
}