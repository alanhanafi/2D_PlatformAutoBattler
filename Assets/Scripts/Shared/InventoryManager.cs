using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private PickupItemPopup pickupItemPopup;
        
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

        internal List<MainItem> BoardItemList { get; } = new();
        
        internal List<BonusItem> BonusItemList { get; }= new();

        internal void AddBoardItem(PickupBoardItem pickupBoardItem)
        {
            pickupItemPopup.DisplayItemPopup(pickupBoardItem);
            BoardItemList.Add(pickupBoardItem.MainItem);
        }
        
        internal void AddBonusItem(PickupBonusItem pickupBonusItem)
        {
            // Do not show popup for bonus item, too intrusive
            //pickupItemPopup.DisplayItemPopup(pickupBonusItem);
            BonusItemList.Add(pickupBonusItem.BonusItem);
            Debug.Log($"BonusItemList {BonusItemList.Count}");
        }
    }
}