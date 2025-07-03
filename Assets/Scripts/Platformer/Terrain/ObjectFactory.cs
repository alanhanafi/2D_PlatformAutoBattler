using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Platformer.Terrain
{
    /// <summary>
    /// Use object pooling to spawn the same object continuously.
    /// </summary>
    public class ObjectFactory : MonoBehaviour
    {
        [SerializeField] private GameObject spawnedObject;
        [SerializeField] private Transform spawnTransform;
        [SerializeField] private float spawnDelayInSeconds;
        
        private List<GameObject> spawnedObjects = new List<GameObject>();
        
        private float timer = 0;

        private void Start()
        {
            SpawnNewObject();
        }

        private void Update()
        {
            // Create a timer
            timer += Time.deltaTime;
            if (timer >= spawnDelayInSeconds)
            {
                GameObject firstInactiveSpawnedObject = spawnedObjects.FirstOrDefault(obj=>!obj.activeSelf);
                if (firstInactiveSpawnedObject is not null)
                {
                    firstInactiveSpawnedObject.transform.position = spawnTransform.position;
                    firstInactiveSpawnedObject.gameObject.SetActive(true);
                }
                else
                {
                    SpawnNewObject();
                }
                timer-= spawnDelayInSeconds;
            }
                
        }

        private void SpawnNewObject()
        {
            GameObject newObject = Instantiate(spawnedObject, spawnTransform);
            spawnedObjects.Add(newObject);
        }
        
    }
}