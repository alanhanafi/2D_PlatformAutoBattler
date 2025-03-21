using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class PickupItemPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemDescription;
        [SerializeField] private float popupDuration = 3f;
        [SerializeField] private Image itemImage;
        
        private float currentTimer;

        private void Update()
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= popupDuration)
                gameObject.SetActive(false);
        }

        internal void DisplayItemPopup(PickupItem pickupItem)
        {
            itemName.text = pickupItem.ItemName;
            itemDescription.text = pickupItem.ItemDescription;
            itemImage.sprite = pickupItem.ItemSprite;
            currentTimer = 0;
            gameObject.SetActive(true);
        }
    }
}