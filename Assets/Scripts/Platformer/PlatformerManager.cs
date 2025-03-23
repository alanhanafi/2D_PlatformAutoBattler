using AutoBattle;
using Cysharp.Threading.Tasks;
using Shared;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class PlatformerManager : MonoBehaviour
    {
        [SerializeField] private bool skipFirstPhase = true;
        
        [SerializeField] private float gameTimer = 30;
        
        [SerializeField] private float delayBeforeZoomingIn = 3;
        
        [SerializeField] private TextMeshProUGUI timerText;
        
        [SerializeField] private CinemachineCamera baseMapVirtualCamera;
        
        [SerializeField] private CinemachineCamera gameVirtualCamera;
        
        [SerializeField] private GameObject playerGameObject;
        private Vector3 respawnPosition;
        
        private float timer;
        
        private bool isGameRunning = false;

        private PlayerController playerController;
        
        private bool isGenerating;
        
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
            int layer = LayerMask.NameToLayer($"BaseCameraVisible");
            if (Camera.main != null) 
                Camera.main.cullingMask &= ~(1 << layer);
            else
                Debug.Log("");
            UpdateTimer(gameTimer);
            timerText.gameObject.SetActive(true);
        }

        private void Update()
        {
            //baseMapVirtualCamera.Lens.FieldOfView -= Time.deltaTime;
            if (!isGameRunning)
                return;
            // TODO : Update Input system
            if (Input.GetKeyDown(KeyCode.R))
                AutoBattleGameManager.ReplayGame((int)InventoryManager.CurrentDifficulty);
            UpdateTimer(timer- Time.deltaTime);
            if (timer <=0)
                EndSpeedrun();
        }

        private void UpdateTimer(float newTime)
        {
            timer = newTime;
            timerText.text = timer.ToString("0.00");
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