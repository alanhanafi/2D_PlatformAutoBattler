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
        
        [FormerlySerializedAs("tempItem")] [SerializeField] private BoardItem tempBoardItem;

        [SerializeField] private Team team;

        private Team targetTeam => team == Team.Enemy ? Team.Player : Team.Enemy;
        
        private float baseCooldownInSeconds;
        
        private float timerInSeconds = 0;

        private BoardItem boardItem;

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

        internal void Initialize(BoardItem boardItem)
        {
            this.boardItem = boardItem;
            //objectImage.sprite = item.Sprite;
            backgroundImage.color = boardItem.Color;
            baseCooldownInSeconds = boardItem.CooldownInSeconds;
            AutoBattleManager.Instance.OnGameStarted += OnStartingGame;
            AutoBattleManager.Instance.OnGameEnded += OnEndingGame;
        }

        private void StartFight()
        {
            if(boardItem is null)
                Initialize(tempBoardItem);
            isActive = true;
        }

        private void TriggerItem()
        {
            Debug.Log($"Trigger item: {boardItem.name}");
            AutoBattleManager.Instance.DealDamage(boardItem.Damage, targetTeam);
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