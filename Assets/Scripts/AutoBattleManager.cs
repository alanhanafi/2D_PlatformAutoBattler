using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class AutoBattleManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerHealthText;
        [SerializeField] private TextMeshProUGUI enemyHealthText;
        
        private AutoBattlePlayer Player = new AutoBattlePlayer(150);

        private AutoBattlePlayer Enemy = new AutoBattlePlayer(150);
        
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
            StartGame();
            playerHealthText.text = Player.CurrentHealth.ToString();
            enemyHealthText.text = Player.MaxHealth.ToString();
            Player.OnHealthChanged += OnChangingPlayerHealth;
            Enemy.OnHealthChanged += OnChangingEnemyHealth;
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