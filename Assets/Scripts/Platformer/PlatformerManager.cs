using AutoBattle;
using Cysharp.Threading.Tasks;
using Shared;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Platformer
{
    public class PlatformerManager : MonoBehaviour
    {
        [SerializeField] private bool skipFirstPhase = true;
        [SerializeField] private float gameTimer = 90;
        [SerializeField] private float delayBeforeZoomingIn = 3;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private CinemachineCamera baseMapVirtualCamera;
        [SerializeField] private CinemachineCamera gameVirtualCamera;
        [SerializeField] private GameObject playerGameObject;
        [SerializeField] private PlatformerInput platformerInput;
        
        private Vector3 respawnPosition;
        
        private float remainingTime;
        
        private bool isGameRunning = false;

        private PlayerController playerController;
        
        private bool isGenerating;
        
        internal float TimePassed => gameTimer - remainingTime;
        
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
            // Hide the cursor during the platformer
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (skipFirstPhase)
                StartGameAfterProceduralGeneration();
        }

        public void StartGameAfterProceduralGeneration()
        {
            respawnPosition = playerGameObject.transform.position;
            WaitForZoomThenStartGame().Forget();
        }

        private void UnlockInputs()
        {
            playerController.UnlockInputs();
        }

        private async UniTask WaitForZoomThenStartGame()
        {
            if (skipFirstPhase)
            {
                baseMapVirtualCamera.Priority -= 2;
                UnlockInputs();
                isGameRunning = true;
                UpdateTimer(gameTimer);
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
            UnlockInputs();
            isGameRunning = true;
            // Hide big sized main items from the game camera
            int layer = LayerMask.NameToLayer($"BaseCameraVisible");
            if (Camera.main != null) 
                Camera.main.cullingMask &= ~(1 << layer);
            else
                Debug.Log("Camera.main is null");
            UpdateTimer(gameTimer);
            timerText.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!isGameRunning)
                return;
            if (platformerInput.GetResetPressed())
                AutoBattleGameManager.ReplayGame((int)InventoryManager.CurrentDifficulty);
            UpdateTimer(remainingTime- Time.deltaTime);
            if (remainingTime <=0)
                EndSpeedrun();
        }

        private void UpdateTimer(float newTime)
        {
            remainingTime = newTime;
            timerText.text = remainingTime.ToString("0.00");
        }

        private void EndSpeedrun()
        {
            isGameRunning = false;
            SceneManager.LoadScene("AutoBattle");
        }

        internal void KillPlayer()
        {
            Respawn();
        }

        private void Respawn()
        {
            playerGameObject.transform.position = respawnPosition;
        }

        public void BumpPlayer()
        {
            playerController.ExecuteBumper();
        }
    }
}