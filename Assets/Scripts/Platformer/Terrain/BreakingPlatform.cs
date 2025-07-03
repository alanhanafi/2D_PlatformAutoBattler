using Cysharp.Threading.Tasks;
using Shared;
using UnityEngine;

namespace Platformer.Terrain
{
    public class BreakingPlatform : MonoBehaviour
    {
        [SerializeField] private float breakSpeedInSeconds = 2f;
        
        [SerializeField] private float rebuildSpeedInSeconds = 5f;
        
        [SerializeField] private float breakingPlatformAlpha = .5f;
        
        [SerializeField] private float brokenPlatformAlpha = .1f;
        
        private bool isBreaking = false;
        
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }


        private void OnCollisionEnter2D(Collision2D other)
        {
            if (isBreaking || !other.gameObject.CompareTag("Player") 
                           || other.transform.position.y < transform.position.y)
                return;
            Functions.ChangeAlpha(spriteRenderer,breakingPlatformAlpha);
            DisableBreakingPlatformAsync().Forget();
        }

        /// <summary>
        /// Disable the breaking platform making it unusable after a delay.
        /// Re enables the platform after a delay.
        /// </summary>
        private async UniTask DisableBreakingPlatformAsync()
        {
            isBreaking = true;
            await UniTask.WaitForSeconds(breakSpeedInSeconds);
            Functions.ChangeAlpha(spriteRenderer,brokenPlatformAlpha);
            GetComponent<Collider2D>().enabled = false;
            isBreaking = false;
            await UniTask.WaitForSeconds(rebuildSpeedInSeconds);
            Functions.ChangeAlpha(spriteRenderer,1);
            GetComponent<Collider2D>().enabled = true;
        } 
        
    }
}