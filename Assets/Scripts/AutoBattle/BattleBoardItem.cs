using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class BattleBoardItem : MonoBehaviour
    {
        [SerializeField] private Image objectImage;
        [SerializeField] private Image maskImage;
        [SerializeField] private Image backgroundImage;
        
        [FormerlySerializedAs("tempBoardItem")] [FormerlySerializedAs("tempItem")] [SerializeField] private MainItem tempMainItem;

        [SerializeField] private Team team;

        private Team targetTeam => team == Team.Enemy ? Team.Player : Team.Enemy;
        
        private float baseCooldownInSeconds;
        
        private float timerInSeconds = 0;

        private MainItem mainItem;

        private bool isActive;

        private void Start()
        {
            if(AutoBattleGameManager.Instance.IsAutoBattleRunning)
                StartFight();
            else
                AutoBattleGameManager.Instance.OnGameStarted += OnStartingGame;
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

        internal void Initialize(MainItem mainItem)
        {
            this.mainItem = mainItem;
            AutoBattleGameManager.Instance.OnGameStarted += OnStartingGame;
            AutoBattleGameManager.Instance.OnGameEnded += OnEndingGame;
        }

        private void StartFight()
        {
            if(mainItem is null)
                Initialize(tempMainItem);
            isActive = true;
        }

        private void TriggerItem()
        {
            Debug.Log($"Trigger item: {mainItem.name}");
            //AutoBattleManager.Instance.DealDamage(boardItem.Damage, targetTeam);
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