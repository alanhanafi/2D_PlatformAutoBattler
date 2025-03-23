using System.Collections.Generic;
using Shared.Main_Items;
using UnityEngine;

namespace Platformer
{
    public interface PickupItem
    {
        public Sprite ItemSprite { get;  }
        public string ItemName { get;  }
        public string ItemDescription { get;  }
        
        public void Initialize(List<MainItem> spawnedItems);
        
        public void OnTriggerEnter2D(Collider2D other);
    }
}