using UnityEngine;

namespace Platformer
{
    public class FadeOutUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup fadeOutCanvasGroup;
        
        [SerializeField] private float fadeOutDuration = .5f;

        private bool isFading;

        private float fadeOutTimer;

        private void Update()
        {
            if (!isFading)
                return;
            if (fadeOutTimer<fadeOutDuration)
            {
                fadeOutTimer += Time.deltaTime;
                fadeOutCanvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeOutTimer / fadeOutDuration);
            }
            else
            {
                fadeOutCanvasGroup.alpha = 0f;
                isFading = false;
                fadeOutTimer = 0f;
            }
        }

        public void FadeOut()
        {
            isFading = true;
        }
    }
}