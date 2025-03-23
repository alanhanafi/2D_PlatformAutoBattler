using System;
using System.Collections;
using System.Diagnostics;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

namespace Edgar.Unity.Examples.Scripts
{
    public class LevelGeneratorManager : MonoBehaviour
    {
        
        [SerializeField] private PlatformerGeneratorGrid2D generator;

        public UnityEvent OnLevelGenerated = new();

        private void Start()
        {
            LoadNextLevel();
        }

        private void LoadNextLevel()
        {
            Debug.Log("LoadNextLevel");

            // Start the generator coroutine
            StartCoroutine(GeneratorCoroutine(generator));
        }

        /// <summary>
        /// Coroutine that generates the level.
        /// It is sometimes useful to yield return before we hide the loading screen to make sure that
        /// all the scripts that were possibly created during the process are properly initialized.
        /// </summary>
        private IEnumerator GeneratorCoroutine(PlatformerGeneratorGrid2D generator)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var generatorCoroutine = this.StartSmartCoroutine(generator.GenerateCoroutine());

            yield return generatorCoroutine.Coroutine;

            yield return null;

            stopwatch.Stop();

            // Throw an exception if the coroutine was not successful.
            // The point of this custom coroutine is that you can actually catch the exception (unlike with the default coroutines).
            // It makes it possible to run the generator again if needed while still having coroutines and not blocking the main thread.
            generatorCoroutine.ThrowIfNotSuccessful();
            Debug.Log($"Generation time: {stopwatch.ElapsedMilliseconds} ms");
            OnLevelGenerated?.Invoke();
            PlatformerManager.Instance.StartGameAfterProceduralGeneration();
        }
    }
}