using System;
using Platformer;
using Shared;
using Shared.Main_Items;
using TMPro;
using UnityEngine;

namespace AutoBattle
{
    public class AutoBattleUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject mainItemUIPrefab;
        
        [SerializeField] private TextMeshProUGUI playerBonusHealthText;
        [SerializeField] private TextMeshProUGUI playerBonusDamageText;
        [SerializeField] private TextMeshProUGUI playerBonusAttackSpeedText;
        [SerializeField] private TextMeshProUGUI enemyBonusHealthText;
        [SerializeField] private TextMeshProUGUI enemyBonusDamageText;
        [SerializeField] private TextMeshProUGUI enemyBonusAttackSpeedText;

        [SerializeField] private GameObject replayButtonsParent;
        [SerializeField] private GameObject replayTextGameObject;
        [SerializeField] private TextMeshProUGUI endGameText;
        [SerializeField] private TextMeshProUGUI playerHealthText;
        [SerializeField] private TextMeshProUGUI enemyHealthText;
        
        [SerializeField] private string drawText = "DRAW";
        [SerializeField] private string victoryText = "YOU WON !";
        [SerializeField] private string defeatText = "YOU LOST !";
        
        [SerializeField] private Transform playerMainItemsUIContainer;
        [SerializeField] private Transform enemyMainItemsUIContainer;

        #region Singleton
        
        internal static AutoBattleUIManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
                return;
            Instance = this;
        }

        #endregion

        internal void Initialize(AutoBattlePlayerState playerState, AutoBattlePlayerState enemyState)
        {
            playerHealthText.text = playerState.CurrentHealth.ToString();
            enemyHealthText.text = enemyState.MaxHealth.ToString();
            playerState.OnHealthChanged += OnChangingPlayerHealth;
            enemyState.OnHealthChanged += OnChangingEnemyHealth;
            AutoBattleGameManager.Instance.OnGameEnded += OnGameEnding;

            var inventoryManager = InventoryManager.Instance;
            if (inventoryManager is null)
                return;
            foreach (MainItem allyMainItem in inventoryManager.PlayerMainItemList)
            {
                Instantiate(mainItemUIPrefab, playerMainItemsUIContainer).GetComponent<MainItemUI>().Initialize(allyMainItem);
            }
            foreach (MainItem allyMainItem in inventoryManager.EnemyMainItemList)
            {
                Instantiate(mainItemUIPrefab, enemyMainItemsUIContainer).GetComponent<MainItemUI>().Initialize(allyMainItem);
            }
            playerBonusHealthText.text = $"x{inventoryManager.PlayerHealthBonusCount}";
            playerBonusDamageText.text = $"x{inventoryManager.PlayerDamageBonusCount}";
            playerBonusAttackSpeedText.text = $"x{inventoryManager.PlayerAttackSpeedBonusCount}";
            enemyBonusHealthText.text = $"x{inventoryManager.EnemyHealthBonusCount}";
            enemyBonusDamageText.text = $"x{inventoryManager.EnemyDamageBonusCount}";
            enemyBonusAttackSpeedText.text = $"x{inventoryManager.EnemyAttackSpeedBonusCount}";
        }

        private void OnGameEnding(object sender, EventArgs e)
        {
            AutoBattleGameManager gameManager = AutoBattleGameManager.Instance;
            AutoBattlePlayerState playerState = gameManager.GetPlayerState(Team.Player);
            AutoBattlePlayerState enemyState = gameManager.GetPlayerState(Team.Enemy);
            
            switch (playerState.IsDead)
            {
                case true when enemyState.IsDead:
                    endGameText.text = drawText;
                    break;
                case true:
                    endGameText.text = defeatText;
                    break;
                default:
                {
                    if(enemyState.IsDead)
                        endGameText.text = victoryText;
                    break;
                }
            }
            endGameText.gameObject.SetActive(true);
            replayButtonsParent.gameObject.SetActive(true);
            replayTextGameObject.gameObject.SetActive(true);
        }


        private void OnChangingEnemyHealth(object sender, int newHealth)
        {
            enemyHealthText.text = newHealth.ToString();
        }

        private void OnChangingPlayerHealth(object sender, int newHealth)
        {
            playerHealthText.text = newHealth.ToString();
        }
    }
}