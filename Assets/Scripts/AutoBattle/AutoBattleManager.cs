using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class AutoBattleManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerHealthText;
        [SerializeField] private TextMeshProUGUI enemyHealthText;
        
        [SerializeField] private Transform playerBoardItemsContainer;
        [SerializeField] private GameObject battleItemPrefab;
        
        
        private const int basePlayerHealth = 150;
        
        private AutoBattlePlayer Player = new AutoBattlePlayer(basePlayerHealth);

        private AutoBattlePlayer Enemy = new AutoBattlePlayer(basePlayerHealth);
        
        internal EventHandler OnGameStarted;
        
        internal EventHandler OnGameEnded;

        internal bool IsAutoBattleRunning { get; private set; }= false;
        
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
            InitializePlayerStats();
            InitializePlayerBoardItems();
            playerHealthText.text = Player.CurrentHealth.ToString();
            enemyHealthText.text = Player.MaxHealth.ToString();
            Player.OnHealthChanged += OnChangingPlayerHealth;
            Enemy.OnHealthChanged += OnChangingEnemyHealth;
            StartGame();
        }

        private void InitializePlayerBoardItems()
        {
            if (InventoryManager.Instance == null)
                return;
            foreach (BoardItem boardItem in InventoryManager.Instance.BoardItemList)
            {
                Instantiate(battleItemPrefab, playerBoardItemsContainer).GetComponent<BattleBoardItem>().Initialize(boardItem);
            }
            
        }

        private void InitializePlayerStats()
        {
            if (InventoryManager.Instance == null)
                return;
            int playerBaseHealth = basePlayerHealth;
            Debug.Log($"BonusItemList {InventoryManager.Instance.BonusItemList.Count}");
            foreach (BonusItem bonusItem in InventoryManager.Instance.BonusItemList)
            {
                playerBaseHealth += bonusItem.BonusHealth;
            }
            Player = new AutoBattlePlayer(playerBaseHealth);
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

        internal void DealDamage(int damage, Team targetTeam)
        {
            if (targetTeam == Team.Enemy)
                Enemy.TakeDamage(damage);
            else if (targetTeam == Team.Player)
                Player.TakeDamage(damage);
            if(Player.IsDead || Enemy.IsDead)
                EndGame();
                
        }
        
        internal void EndGame()
        {
            IsAutoBattleRunning = false;
            Team winner = Player.IsDead?Team.Enemy:Team.Player;
            Debug.Log($"{winner} won !");
            OnGameEnded?.Invoke(this, EventArgs.Empty);
        }
    }
    internal enum Team { Player, Enemy };
}