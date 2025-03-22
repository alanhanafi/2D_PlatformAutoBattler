using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(AutoBattlePlayerView))]
    public class AutoBattlePlayerState : MonoBehaviour
    {
        private const float critMultiplier = 1.5f;
        private const int basePlayerHealth = 500;
        private const float baseAttackSpeed = .5f;
        private const int baseAttackDamage = 20;
        private const float attackTimerTriggerInSeconds = 1;

        [SerializeField] private Team team;
        
        // SerializeField for test purposes
        [SerializeField] 
        internal List<MainItem> mainItemsList = new();
        
        internal int MaxHealth { get; set; }
        internal int AttackDamage { get; set; }
        // Attacks per second, if 0.5 => 2 Attacks per second
        internal float AttackSpeed { get; set; }
        internal int CurrentHealth { get; set; }

        internal PoisonState CurrentPoisonState { get; set; } = new();
        
        internal EventHandler<int> OnHealthChanged;
        
        internal EventHandler OnDeath;
        
        internal EventHandler<(AutoBattlePlayerState target, int damage, bool isDirect)> OnDamageDealt;
        internal EventHandler<(AutoBattlePlayerState source, int damage, bool isDirect)> OnDamageReceived;
        
        internal bool IsDead => CurrentHealth <= 0;

        internal bool IsPoisoned => CurrentPoisonState.IsPoisoned;

        private AutoBattlePlayerView playerView;

        private float currentAttackTimer = 0;

        private Team enemyTeam => team == Team.Enemy ? Team.Player : Team.Enemy;

        private void Awake()
        {
            playerView = GetComponent<AutoBattlePlayerView>();
        }

        private void Start()
        {
            AutoBattleGameManager.Instance.OnTimePassed += OnTimePassing;
        }

        private void OnTimePassing(object sender, float timePassed)
        {
            if (IsPoisoned)
            {
                Debug.Log($"Poisoned {timePassed}");
                CurrentPoisonState.PassTime(timePassed,this);
            }
            currentAttackTimer += timePassed*AttackSpeed;
            if (currentAttackTimer >= attackTimerTriggerInSeconds)
            {
                TriggerAttack();
                currentAttackTimer -= attackTimerTriggerInSeconds;
            }
        }

        private void TriggerAttack()
        {
            DealDamage(AutoBattleGameManager.Instance.GetPlayerState(enemyTeam),AttackDamage,true);
            playerView.PlayAttackAnimation();
        }

        internal void ShowDamageText(int damageToDisplay)
        {
            playerView.ShowDamageText(damageToDisplay);
        }

        internal void TakeDamage(int damage)
        {
            UpdateHealth(CurrentHealth - damage);
            playerView.PlayHitFx();
        }
        
        private void UpdateHealth(int newHealth)
        {
            CurrentHealth = newHealth;
            OnHealthChanged?.Invoke(this, CurrentHealth);
            if(CurrentHealth <= 0)
                OnDeath?.Invoke(this, null);
        }

        internal void InitializeStats(int additionalHealth, int additionalAttackDamage, float additionalAttackSpeed)
        {
            MaxHealth = basePlayerHealth + additionalHealth;
            CurrentHealth = MaxHealth;
            AttackDamage = baseAttackDamage + additionalAttackDamage;
            AttackSpeed = baseAttackSpeed + additionalAttackSpeed;
            foreach (MainItem mainItem in mainItemsList)
            {
                mainItem.SubscribeToEvents(this);
            }
        }
        
        
        internal void DealDamage(AutoBattlePlayerState target, int damage, bool isDirect)
        {
            if (mainItemsList.Any(item => item is CritMainItem))
                damage = (int)Math.Round(damage*critMultiplier, MidpointRounding.AwayFromZero);
            target.TakeDamage(damage);
            target.ShowDamageText(damage);
            OnDamageDealt?.Invoke(this, (target, damage, isDirect));
            OnDamageReceived?.Invoke(target, (this, damage, isDirect));
        }
        
        internal void SelfHeal(int healValue, bool isDirect)
        {
            UpdateHealth(CurrentHealth + healValue);
        }

        internal void TriggerPoison(int poisonValue)
        {
            // Poison is self-inflicted damage for now
            DealDamage(this, poisonValue, false);
        }

        internal void ApplyPoison(int poisonValue)
        {
            Debug.Log($"Applying poison {poisonValue}");
            CurrentPoisonState.AddPoisonValue(poisonValue);
        }

        internal void IncreaseAttackDamage(int addedDamage)
        {
            // TODO : Add a visual effect
            AttackDamage += addedDamage;
        }
    }
}