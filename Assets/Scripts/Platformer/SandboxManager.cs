using System;
using Cysharp.Threading.Tasks;
using Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class SandboxManager : MonoBehaviour
    {
        [SerializeField] private Transform respawnTransform;
        
        [SerializeField] private GameObject sandboxPlayerPrefab;
        
        private PlayerController playerController;

        internal EventHandler OnSandboxExit;
        
        static internal SandboxManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
                return;
            Instance = this;
        }

        private void Start()
        {
            Functions.HideCursor();
            ResetPlayerPosition();
            playerController = sandboxPlayerPrefab.GetComponent<PlayerController>();
            SceneManager.LoadScene("Test Generation", LoadSceneMode.Additive);
        }

        internal void RespawnPlayer()
        {
            ResetPlayerPosition();
        }

        private void ResetPlayerPosition()
        {
            sandboxPlayerPrefab.transform.position = respawnTransform.position;
        }

        internal void BumpPlayer()
        {                
            playerController.ExecuteBumper();
        }

        internal async UniTask QuitSandbox()
        {
            await SceneManager.UnloadSceneAsync(sceneName:"Sandbox");
            OnSandboxExit?.Invoke(this, EventArgs.Empty);
        }
    }
}