using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class BattleItem : MonoBehaviour
    {
        [SerializeField] private Image objectImage;
        [SerializeField] private Image maskImage;
        [SerializeField] private Image backgroundImage;
        
        [SerializeField] private Item tempItem;

        [SerializeField] private Team team;

        private Team targetTeam => team == Team.Enemy ? Team.Player : Team.Enemy;
        
        private float baseCooldownInSeconds;
        
        private float timerInSeconds = 0;

        private Item item;

        private bool isActive;

        private void Start()
        {
            if(AutoBattleManager.Instance.IsAutoBattleRunning)
                StartFight();
            else
                AutoBattleManager.Instance.OnGameStarted += OnStartingGame;
        }

        // TODO : Replace this by an event in autobattle manager that handles all the items during each frame,
        // or find another way to handle multiple item triggers during the same frame to allow for a draw
        private void Update()
        {
            if (!isActive)
                return;
            timerInSeconds+=Time.deltaTime;
            if (timerInSeconds >= baseCooldownInSeconds)
            {
                TriggerItem();
                timerInSeconds -= baseCooldownInSeconds;
            }
            maskImage.fillAmount = timerInSeconds / baseCooldownInSeconds;
                
        }

        internal void Initialize(Item item)
        {
            this.item = item;
            //objectImage.sprite = item.Sprite;
            backgroundImage.color = item.Color;
            baseCooldownInSeconds = item.CooldownInSeconds;
            AutoBattleManager.Instance.OnGameStarted += OnStartingGame;
            AutoBattleManager.Instance.OnGameEnded += OnEndingGame;
        }

        private void StartFight()
        {
            if(item is null)
                Initialize(tempItem);
            isActive = true;
        }

        private void TriggerItem()
        {
            Debug.Log($"Trigger item: {item.name}");
            AutoBattleManager.Instance.DealDamage(item.Damage, targetTeam);
        }

        private void OnStartingGame(object sender, EventArgs e)
        {
            StartFight();
        }

        private void OnEndingGame(object sender, EventArgs e)
        {
            isActive = false;
        }
        
        
    }
}