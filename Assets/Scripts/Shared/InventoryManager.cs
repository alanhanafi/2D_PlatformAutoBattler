using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class InventoryManager : MonoBehaviour
    {
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

        internal void AddBoardItem(MainItem mainItem)
        {
            BoardItemList.Add(mainItem);
        }
        
        internal void AddBonusItem(BonusItem bonusItem)
        {
            BonusItemList.Add(bonusItem);
            Debug.Log($"BonusItemList {BonusItemList.Count}");
        }
    }
}