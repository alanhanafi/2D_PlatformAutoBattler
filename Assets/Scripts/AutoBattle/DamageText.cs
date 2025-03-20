using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField]
        private float animationDuration = 0.5f;
        
        [SerializeField]
        private float verticalSpeed = 50;

        [SerializeField] private TextMeshProUGUI damageText;
        
        internal void ShowDamageText(int damage)
        {
            damageText.text = $"-{damage}";
            gameObject.SetActive(true);
            PlayTextAnimationAsync().Forget();
        }

        private async UniTask PlayTextAnimationAsync()
        {
            float timer = 0;
            Vector3 startPos = transform.position;
            while (timer < animationDuration)
            {
                timer += Time.deltaTime;
                transform.position += Vector3.up * (verticalSpeed * Time.deltaTime);
                Functions.ChangeAlpha(damageText,1 - timer / animationDuration);
                await UniTask.Yield();
            }
            gameObject.SetActive(false);
            transform.position = startPos;
            Functions.ChangeAlpha(damageText,1);
            
        }
    }
}