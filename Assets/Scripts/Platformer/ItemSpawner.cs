using System.Collections.Generic;
using Shared.Main_Items;
using UnityEngine;

namespace Platformer
{
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnTransforms;
        
        [SerializeField] private GameObject pickupItemPrefab;

        public void SpawnItems(List<MainItem> spawnedItems)
        {
            Transform itemSpawnerTransform = spawnTransforms[Random.Range(0, spawnTransforms.Length)];
            var pickupItem = Instantiate(pickupItemPrefab, itemSpawnerTransform);
            pickupItem.GetComponent<PickupItem>().Initialize(spawnedItems);
        }
    }
}