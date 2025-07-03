using System;
using System.Collections.Generic;
using System.Linq;
using AutoBattle;
using Cysharp.Threading.Tasks;
using Shared;
using Shared.Main_Items;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Platformer
{
    public class PlatformerManager : MonoBehaviour
    {
        [SerializeField] private bool isMinimapActive;
        [SerializeField] private bool skipFirstPhase = true;
        [SerializeField] private float gameTimer = 90;
        [SerializeField] private float delayBeforeZoomingIn = 3;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private CinemachineCamera baseMapVirtualCamera;
        [SerializeField] private CinemachineCamera gameVirtualCamera;
        [SerializeField] private GameObject playerGameObject;
        [SerializeField] private PlatformerInput platformerInput;
        [SerializeField] private GameObject centerMinimapGameObject;
        [SerializeField] private GameObject cornerMinimapGameObject;
        [SerializeField] private Camera centerMinimapCamera;
        [SerializeField] private Camera cornerMinimapCamera;
        [SerializeField] private MainItemDirection[] mainItemDirections;
        [SerializeField] private GameObject mainCanvas;
        [SerializeField] private GameObject mainMenu;

        public EventHandler OnGameStart;
        public EventHandler OnRespawn;
        
        internal float TimePassed => gameTimer - remainingTime;
        internal Vector3 PlayerPosition => playerGameObject.transform.position;
        
        internal readonly List<MainItem> AvailableMainItems = new List<MainItem>();
        private Vector3 respawnPosition;
        private float remainingTime;
        public bool IsGameRunning { get; private set; }
        private PlayerController playerController;
        private bool isGenerating;
        private bool isMenuOpen;
        public Transform StartRoomTransform { get; set; }
        
        
        
        
        private bool isMinimapCentered => centerMinimapGameObject.activeSelf;
        
        #region Singleton

        public static PlatformerManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
                return;
            Instance = this;
            playerController = playerGameObject.GetComponent<PlayerController>();
        }
        
        #endregion
        
        
        
        

        private void Start()
        {
            if (!isMinimapActive)
            {
                centerMinimapCamera.gameObject.SetActive(false);
                cornerMinimapCamera.gameObject.SetActive(false);
                centerMinimapGameObject.SetActive(false);
                cornerMinimapGameObject.SetActive(false);
            }
            Functions.HideCursor();
            if (skipFirstPhase)
                StartGameAfterProceduralGeneration();
        }

        public void StartGameAfterProceduralGeneration()
        {
            centerMinimapCamera.transform.position = respawnPosition;
            playerGameObject.transform.position = respawnPosition;
            if (SandboxManager.Instance != null)
                SandboxManager.Instance.OnSandboxExit += OnExitingSandbox;
            else
                WaitForZoomThenStartGame().Forget();
        }

        private void OnExitingSandbox(object sender, EventArgs e)
        {
            WaitForZoomThenStartGame().Forget();
        }

        private void UnlockInputs()
        {
            playerController.UnlockInputs();
        }

        internal void PickupMainItem(MainItem mainItem)
        {
            AvailableMainItems.Remove(mainItem);
        }
        
        public void AddAvailableMainItem(MainItem mainItem)
        {
            AvailableMainItems.Add(mainItem);
        }

        private async UniTask WaitForZoomThenStartGame()
        {
            if (skipFirstPhase)
            {
                baseMapVirtualCamera.Priority -= 2;
                StartGame();
                return;
            }
            
            timerText.gameObject.SetActive(false);
            
            // Wait a bit at min zoom to see the whole map
            await UniTask.WaitForSeconds(delayBeforeZoomingIn);
            // Blends to the main game camera
            baseMapVirtualCamera.Priority -= 2;
            await UniTask.WaitUntil(() => !baseMapVirtualCamera.IsParticipatingInBlend());
            
            // Start game at the end of the blend
            Debug.Log($"Start game");
            // Hide big sized main items from the game camera
            int layer = LayerMask.NameToLayer($"BaseCameraVisible");
            if (Camera.main != null) 
                Camera.main.cullingMask &= ~(1 << layer);
            else
                Debug.Log("Camera.main is null");
            StartGame();
        }

        private void StartGame()
        {
            UnlockInputs();
            IsGameRunning = true;
            OnGameStart?.Invoke(this, EventArgs.Empty);
            UpdateTimer(gameTimer);
            timerText.gameObject.SetActive(true);
            mainCanvas.SetActive(true);
        }

        private void Update()
        {
            if (!IsGameRunning)
                return;
            if (platformerInput.GetMinimapButtonPressed() && isMinimapActive)
                SwapMinimap();
            if (platformerInput.GetMenuButtonPressed())
            {
                if(isMenuOpen)
                    CloseMenu();
                else
                    OpenMenu();
            }
            UpdateTimer(remainingTime- Time.deltaTime);
            if (remainingTime <=0)
                EndSpeedrun();
        }

        /// <summary>
        /// Replay the game in hard difficulty
        /// </summary>
        public void ReplayGameInHardMode()
        {
            AutoBattleGameManager.ReplayGame((int)Difficulty.Hard);
        }
        
        private void OpenMenu()
        {
            Functions.ShowCursor();
            mainMenu.gameObject.SetActive(true);
            isMenuOpen = true;
        }

        private void CloseMenu()
        {
            Functions.HideCursor();
            mainMenu.gameObject.SetActive(false);
            isMenuOpen = false;
        }
        
        

        private void SwapMinimap()
        {
            if (isMinimapCentered)
            {
                centerMinimapGameObject.SetActive(false);
                centerMinimapCamera.gameObject.SetActive(false);
                cornerMinimapGameObject.SetActive(true);
                cornerMinimapCamera.gameObject.SetActive(true);
            }
            else
            {
                centerMinimapGameObject.SetActive(true);
                centerMinimapCamera.gameObject.SetActive(true);
                cornerMinimapGameObject.SetActive(false);
                cornerMinimapCamera.gameObject.SetActive(true);
            }
        }

        private void UpdateTimer(float newTime)
        {
            remainingTime = newTime;
            timerText.text = remainingTime.ToString("0.00");
        }

        private void EndSpeedrun()
        {
            IsGameRunning = false;
            SceneManager.LoadScene("AutoBattle");
        }

        internal void KillPlayer()
        {
            Respawn();
        }

        private void Respawn()
        {
            playerGameObject.transform.position = respawnPosition;
            OnRespawn?.Invoke(this, EventArgs.Empty);
        }

        public void BumpPlayer()
        {
            playerController.ExecuteBumper();
        }

        public void EnterRoom(Dictionary<MainItem,Vector3?> itemsInPath)
        {
            if (!IsGameRunning)
                return;
            Dictionary<Vector3?, List<MainItem>> itemsDirectionInfo = new();

            foreach (MainItem availableMainItem in AvailableMainItems)
            {
                if (itemsInPath.Keys.Contains(availableMainItem))
                {
                    if(itemsDirectionInfo.Keys.Contains(itemsInPath[availableMainItem]))
                        itemsDirectionInfo[itemsInPath[availableMainItem]].Add(availableMainItem);
                    else
                        itemsDirectionInfo.TryAdd(itemsInPath[availableMainItem],new List<MainItem> {availableMainItem});
                }
                /* Do not show the items that are not on the current path
                else
                {
                    if(itemsDirectionInfo.Keys.Contains(StartRoomTransform.position))
                        itemsDirectionInfo[StartRoomTransform.position].Add(availableMainItem);
                    else
                        itemsDirectionInfo.TryAdd(StartRoomTransform.position,new List<MainItem> {availableMainItem});
                }*/
            }

            int j = 0;
            foreach (var itemDirectionInfo in itemsDirectionInfo)
            {
                mainItemDirections[j].UpdateRoomTargetPosition(itemDirectionInfo.Key.Value, itemDirectionInfo.Value);
                j++;
            }

            for (int i = j; i < mainItemDirections.Length; i++)
            {
                mainItemDirections[i].gameObject.SetActive(false);
            }
        }

        public void SetRespawnPosition(Vector3 respawnPosition)
        {
            this.respawnPosition = respawnPosition;
        }
    }
}