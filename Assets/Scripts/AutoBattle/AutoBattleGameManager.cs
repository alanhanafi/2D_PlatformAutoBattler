using System;
using Cysharp.Threading.Tasks;
using Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AutoBattle
{
    public class AutoBattleGameManager : MonoBehaviour
    {
        [SerializeField] private AutoBattlePlayerState playerState;
        [SerializeField] private AutoBattlePlayerState enemyState;

        [SerializeField]
        private float gameStartDelay = 2f;
        
        // Delay between each timer is raised, allows to get both players' action that would occur at the same time and allow a draw
        [SerializeField]
        private float timerEventDelay = 0.1f;
        
        
        internal EventHandler<float> OnTimePassed;
        
        internal EventHandler OnGameStarted;
        
        internal EventHandler OnGameEnded;

        internal bool IsAutoBattleRunning { get; private set; }= false;
        
        private float currentTimer = 0f;
        
        #region Singleton

        internal static AutoBattleGameManager Instance { get;private set; }

        private void Awake()
        {
            if (Instance != null)
                return;
            Instance = this;
        }

        #endregion

        private void Start()
        {
            Functions.ShowCursor();
            InitializePlayersStats();
            AutoBattleUIManager.Instance.Initialize(playerState,enemyState);
            StartGameAfterDelayAsync().Forget();
        }

        private void Update()
        {
            if (!IsAutoBattleRunning)
                return;
            currentTimer += Time.deltaTime;
            if (currentTimer >= timerEventDelay)
            {
                OnTimePassed?.Invoke(this, timerEventDelay);
                currentTimer -= timerEventDelay;
                if(playerState.IsDead || enemyState.IsDead)
                    EndGame();
            }
        }

        private async UniTask StartGameAfterDelayAsync()
        {
            await UniTask.WaitForSeconds(gameStartDelay);
            StartGame();
        }

        private void InitializePlayersStats()
        {
            if (InventoryManager.Instance != null)
            {
                playerState.BonusItemsList = InventoryManager.Instance.PlayerBonusItemList;
                playerState.MainItemsList = InventoryManager.Instance.PlayerMainItemList;
                enemyState.BonusItemsList = InventoryManager.Instance.EnemyBonusItemList;
                enemyState.MainItemsList = InventoryManager.Instance.EnemyMainItemList;
            }
            playerState.InitializeStats();
            enemyState.InitializeStats();
        }

        internal AutoBattlePlayerState GetPlayerState(Team playerTeam)
        {
            return playerTeam == Team.Player ? playerState : enemyState;
        }
        

        internal void StartGame()
        {
            IsAutoBattleRunning = true;
            OnGameStarted?.Invoke(this, EventArgs.Empty);
        }
        
        private void EndGame()
        {
            IsAutoBattleRunning = false;
            OnGameEnded?.Invoke(this, EventArgs.Empty);
        }

        public static void ReplayGame(int difficulty)
        {
            if(difficulty<0)
                difficulty = 0;
            if(difficulty>(int)Difficulty.Hard)
                difficulty = (int)Difficulty.Hard;
            if(InventoryManager.Instance != null)
                Destroy(InventoryManager.Instance.gameObject);
            InventoryManager.CurrentDifficulty = (Difficulty)difficulty;
            SceneManager.LoadScene("Sandbox");
        }
    }
    internal enum Team { Player, Enemy };
}