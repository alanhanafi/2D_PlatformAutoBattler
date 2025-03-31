using Shared.Main_Items;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer
{
    public class MainItemUI : MonoBehaviour
    {
        [SerializeField] private Image image;

        internal void Initialize(MainItem mainItem)
        {
            image.sprite = mainItem.Sprite;
        }
    }
}