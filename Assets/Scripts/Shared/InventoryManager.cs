using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class InventoryManager : MonoBehaviour
    {
        internal static Difficulty CurrentDifficulty = Difficulty.Easy;
        
        [SerializeField] private PickupItemPopup pickupItemPopup;
        
        [SerializeField] private GameObject mainItemUIGameObject;
        [SerializeField] private Transform mainItemsParent;
        
        [SerializeField] private TextMeshProUGUI healthCounter;
        [SerializeField] private TextMeshProUGUI damageCounter;
        [SerializeField] private TextMeshProUGUI attackSpeedCounter;
        
        
        internal List<MainItem> EnemyMainItemList { get; private set; }
        internal List<BonusItem> EnemyBonusItemList { get; private set; }
        [SerializeField] private BonusItem BaseHealthBonusItem;
        [SerializeField] private BonusItem BaseDamageBonusItem;
        [SerializeField] private BonusItem BaseAttackSpeedItem;

        [SerializeField] private InventorySet[] EnemyInventorySet;
        
        internal int EnemyHealthBonusCount => EnemyBonusItemList.Count(item => item.BonusType == BonusType.Health);
        internal int EnemyDamageBonusCount => EnemyBonusItemList.Count(item => item.BonusType == BonusType.Damage);
        internal int EnemyAttackSpeedBonusCount => EnemyBonusItemList.Count(item => item.BonusType == BonusType.AttackSpeed);

        internal int PlayerHealthBonusCount{ get; private set; }
        internal int PlayerDamageBonusCount{ get; private set; }
        internal int PlayerAttackSpeedBonusCount { get; private set; }
        
        #region Singleton

        internal static InventoryManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null) 
                return;
            DontDestroyOnLoad(this);
            Instance = this;
        }
        

        #endregion

        private void Start()
        {
            InitializeEnemyItems(CurrentDifficulty);
        }

        private void InitializeEnemyItems(Difficulty difficulty)
        {
            EnemyBonusItemList = new List<BonusItem>();
            for (int i = 0; i < EnemyInventorySet[(int)difficulty].DamageBonusCount; i++)
            {
                EnemyBonusItemList.Add(BaseDamageBonusItem);
            }
            for (int i = 0; i < EnemyInventorySet[(int)difficulty].HealthBonusCount; i++)
            {
                EnemyBonusItemList.Add(BaseHealthBonusItem);
            }
            for (int i = 0; i < EnemyInventorySet[(int)difficulty].AttackSpeedBonusCount; i++)
            {
                EnemyBonusItemList.Add(BaseAttackSpeedItem);
            }
            EnemyMainItemList = EnemyInventorySet[(int)difficulty].MainItems;
        }

        internal List<MainItem> PlayerMainItemList { get; } = new();
        
        internal List<BonusItem> PlayerBonusItemList { get; }= new();

        internal void AddMainItem(PickupBoardItem pickupMainItem)
        {
            pickupItemPopup.DisplayItemPopup(pickupMainItem);
            PlayerMainItemList.Add(pickupMainItem.MainItem);
            Instantiate(mainItemUIGameObject, mainItemsParent).GetComponent<MainItemUI>().Initialize(pickupMainItem.MainItem);
        }
        
        internal void AddBonusItem(PickupBonusItem pickupBonusItem)
        {
            PlayerBonusItemList.Add(pickupBonusItem.BonusItem);
            Debug.Log($"BonusItemList {PlayerBonusItemList.Count}");
            switch (pickupBonusItem.BonusItem.BonusType)
            {
                case BonusType.Health:
                    PlayerHealthBonusCount++;
                    healthCounter.text = $"x{PlayerHealthBonusCount}";
                    break;
                case BonusType.Damage:
                    PlayerDamageBonusCount++;
                    damageCounter.text = $"x{PlayerDamageBonusCount}";
                    break;
                case BonusType.AttackSpeed:
                    PlayerAttackSpeedBonusCount++;
                    attackSpeedCounter.text = $"x{PlayerAttackSpeedBonusCount}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    public enum Difficulty { Easy, Medium, Hard }
}