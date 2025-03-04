using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class InventoryManager : MonoBehaviour
    {
        private List<Item> itemList = new();

        internal void AddItem(Item item)
        {
            itemList.Add(item);
        }
    }
}