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

        private List<Item> itemList = new();

        internal void AddItem(Item item)
        {
            itemList.Add(item);
        }
    }
}