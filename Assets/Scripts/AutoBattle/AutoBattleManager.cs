using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class AutoBattleManager : MonoBehaviour
    {
        [SerializeField] private Button replayButton;
        [SerializeField] private TextMeshProUGUI endGameText;
        [SerializeField] private TextMeshProUGUI playerHealthText;
        [SerializeField] private TextMeshProUGUI enemyHealthText;
        
        [SerializeField] private Transform playerBoardItemsContainer;
        [SerializeField] private GameObject battleItemPrefab;
        
        [SerializeField] private AutoBattlePlayerState playerState;
        [SerializeField] private AutoBattlePlayerState enemyState;

        [SerializeField]
        private float gameStartDelay = 2f;
        
        // Delay between each timer is raised, allows to get both players' action that would occur at the same time and allow a draw
        [SerializeField]
        private float timerEventDelay = 0.1f;
        
        // TODO : Use SO 
        [SerializeField]
        private int additionalEnemyPlayerHealth = 50;
        [SerializeField]
        private int additionalEnemyAttackDamage = 25;
        [SerializeField]
        private float additionalEnemyAttackSpeed = .3f;
        
        
        internal EventHandler<float> OnTimePassed;
        
        internal EventHandler OnGameStarted;
        
        internal EventHandler OnGameEnded;

        internal bool IsAutoBattleRunning { get; private set; }= false;
        
        private float currentTimer = 0f;
        
        #region Singleton

        internal static AutoBattleManager Instance { get;private set; }

        private void Awake()
        {
            if (Instance != null)
                return;
            Instance = this;
        }

        #endregion

        private void Start()
        {
            InitializePlayersStats();
            InitializePlayerBoardItems();
            playerHealthText.text = playerState.CurrentHealth.ToString();
            enemyHealthText.text = enemyState.MaxHealth.ToString();
            playerState.OnHealthChanged += OnChangingPlayerHealth;
            enemyState.OnHealthChanged += OnChangingEnemyHealth;
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

        private void InitializePlayerBoardItems()
        {
            // NO board for now
            return;
            if (InventoryManager.Instance == null)
                return;
            foreach (MainItem boardItem in InventoryManager.Instance.BoardItemList)
            {
                Instantiate(battleItemPrefab, playerBoardItemsContainer).GetComponent<BattleBoardItem>().Initialize(boardItem);
            }
        }

        private void InitializePlayersStats()
        {
            int additionalPlayerHealth = 0;
            int additionalAttackDamage = 0;
            float additionalAttackSpeed = 0;
            if (InventoryManager.Instance == null)
            {
                additionalPlayerHealth = additionalEnemyPlayerHealth;
                additionalAttackDamage =additionalEnemyAttackDamage;
                additionalAttackSpeed = additionalEnemyAttackSpeed;
            }
            else
            {
                foreach (BonusItem bonusItem in InventoryManager.Instance.BonusItemList)
                {
                    additionalPlayerHealth += bonusItem.BonusHealth;
                    additionalAttackDamage += bonusItem.BonusDamage;
                    additionalAttackSpeed += bonusItem.BonusAttackSpeed;
                }
                playerState.mainItemsList = InventoryManager.Instance.BoardItemList;
            }
            playerState.InitializeStats(additionalPlayerHealth, additionalAttackDamage, additionalAttackSpeed);
            enemyState.InitializeStats(additionalEnemyPlayerHealth,additionalEnemyAttackDamage, additionalEnemyAttackSpeed);
        }

        internal AutoBattlePlayerState GetPlayerState(Team playerTeam)
        {
            return playerTeam == Team.Player ? playerState : enemyState;
        }
        

        private void OnChangingEnemyHealth(object sender, int newHealth)
        {
            enemyHealthText.text = newHealth.ToString();
        }

        private void OnChangingPlayerHealth(object sender, int newHealth)
        {
            playerHealthText.text = newHealth.ToString();
        }

        internal void StartGame()
        {
            IsAutoBattleRunning = true;
            OnGameStarted?.Invoke(this, EventArgs.Empty);
        }
        
        private void EndGame()
        {
            IsAutoBattleRunning = false;
            switch (playerState.IsDead)
            {
                case true when enemyState.IsDead:
                    endGameText.text = "DRAW !";
                    break;
                case true:
                    endGameText.text = "YOU LOST !";
                    break;
                default:
                {
                    if(enemyState.IsDead)
                        endGameText.text = "YOU WON !";
                    break;
                }
            }
            endGameText.gameObject.SetActive(true);
            OnGameEnded?.Invoke(this, EventArgs.Empty);
            replayButton.gameObject.SetActive(true);
        }

        public void ReplayGame()
        {
            if(InventoryManager.Instance != null)
                Destroy(InventoryManager.Instance.gameObject);
            SceneManager.LoadScene("Test Generation");
        }
    }
    internal enum Team { Player, Enemy };
}