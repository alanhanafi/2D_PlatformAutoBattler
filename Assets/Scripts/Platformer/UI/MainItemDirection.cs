using System;
using System.Collections.Generic;
using Shared.Main_Items;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer
{
    public class MainItemDirection : MonoBehaviour
    {
        [SerializeField] private Image soloImage;
        [SerializeField] private GameObject soloImageDirection;
        [SerializeField] private GameObject duoImageDirection;
        [SerializeField] private Image duoFirstImage;
        [SerializeField] private Image duoSecondImage;
        
        
        private Vector3 playerPosition; 
        private Vector3? targetItemPosition; 
        private RectTransform arrowUI;
        private Vector3 screenCenter;

        private void Start()
        {
            screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            arrowUI = GetComponent<RectTransform>();
        }

        void Update()
        {
            if (targetItemPosition == null) return;

            // Get direction from player to target
            Vector2 direction = (targetItemPosition.Value - PlatformerManager.Instance.PlayerPosition).normalized;

            // Convert to angle (Unity uses a different rotation system, so adjust accordingly)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Rotate the UI arrow to point towards the target
            arrowUI.rotation = Quaternion.Euler(0, 0, angle);

            // Keep Arrow Inside Screen (Optional)
            KeepArrowOnScreen(direction);
        }

        void KeepArrowOnScreen(Vector2 direction)
        {
            Vector3 screenPos = screenCenter + (Vector3)(direction * 2000f); // Adjust 200f to position arrow near screen edge

            // Clamp arrow within the screen bounds
            screenPos.x = Mathf.Clamp(screenPos.x, 50, Screen.width - 50);
            screenPos.y = Mathf.Clamp(screenPos.y, 50, Screen.height - 50);

            arrowUI.position = screenPos;
        }
        
        internal void UpdateDirection(Vector3 position, List<MainItem> mainItems)
        {
            Debug.Log($"UpdateDirection: {position}");
            targetItemPosition = position;
            if (mainItems.Count == 1)
            {
                duoImageDirection.SetActive(false);
                soloImage.sprite = mainItems[0].Sprite;
                soloImageDirection.SetActive(true);
            }
            else
            {
                soloImageDirection.SetActive(false);
                duoFirstImage.sprite = mainItems[0].Sprite;
                duoSecondImage.sprite = mainItems[1].Sprite;
                duoImageDirection.SetActive(true);
            }
            gameObject.SetActive(true);
        }
    }
}