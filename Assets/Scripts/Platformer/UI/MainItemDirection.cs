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
        [SerializeField] private int borderPadding = 10;
        [SerializeField] private RectTransform arrowToRotate;


        private Vector3 playerPosition => PlatformerManager.Instance.PlayerPosition; 
        private Vector3? targetItemPosition; 
        private RectTransform rectTransform;
        
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            rectTransform = GetComponent<RectTransform>();
        }

        void Update()
        {
            if (targetItemPosition == null) return;
            Vector3 targetPosition = mainCamera.WorldToScreenPoint(targetItemPosition.Value);
            Vector3 fromPosition = mainCamera.WorldToScreenPoint(playerPosition);
            fromPosition.z = 0;
            targetPosition.z = 0;
            Vector3 direction = (targetPosition - fromPosition).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0)
                angle += 360;
            arrowToRotate.localEulerAngles = new Vector3(0, 0, angle);

            float borderSize = borderPadding;
            Vector3 cappedTargetScreenPosition = targetPosition;

            if (cappedTargetScreenPosition.x <= borderSize)
                cappedTargetScreenPosition.x = borderSize;
            if (cappedTargetScreenPosition.y <= borderSize)
                cappedTargetScreenPosition.y = borderSize;
            if (cappedTargetScreenPosition.x >= Screen.width - borderSize)
                cappedTargetScreenPosition.x = Screen.width - borderSize;
            if (cappedTargetScreenPosition.y >= Screen.height - borderSize)
                cappedTargetScreenPosition.y = Screen.height - borderSize;

            rectTransform.position = cappedTargetScreenPosition;
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0f);
        }
        
        
        
        
        internal void UpdateRoomTargetPosition(Vector3 position, List<MainItem> mainItems)
        {
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