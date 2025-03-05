using System;

namespace DefaultNamespace
{
    public class AutoBattlePlayer
    {
        internal int MaxHealth { get; set; }
        internal int CurrentHealth { get; set; }
        
        internal EventHandler<int> OnHealthChanged;
        
        internal EventHandler OnDeath;
        
        internal bool IsDead => CurrentHealth <= 0;

        internal void TakeDamage(int damage)
        {
            UpdateHealth(CurrentHealth - damage);
        }
        
        private void UpdateHealth(int newHealth)
        {
            CurrentHealth = newHealth;
            OnHealthChanged?.Invoke(this, CurrentHealth);
            if(CurrentHealth <= 0)
                OnDeath?.Invoke(this, null);
        }

        internal AutoBattlePlayer(int maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        } 
    }
}