using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
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