using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
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
            if (isBreaking)
                return;
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log($"Player {other.gameObject.name} collided with {gameObject.name}");
                if (other.transform.position.y < transform.position.y)
                    return;
                Debug.Log($"Player is landing on the platform");
                Functions.ChangeAlpha(spriteRenderer,breakingPlatformAlpha);
                DisableBreakingPlatformAsync().Forget();
            }
        }

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