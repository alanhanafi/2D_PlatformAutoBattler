﻿using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Platformer.Terrain
{
    public class Bumper : MonoBehaviour
    {
        [SerializeField] private float triggeredMaxHeight = .5f;
        
        [SerializeField] private float maxHeightDuration = .2f;
        
        [SerializeField] private float lerpSpeed = .5f;
        
        [SerializeField] private bool isSandbox;
        
        private SpriteRenderer spriteRenderer;

        private float baseBumperHeight;

        private UniTask currentBumperUpdate;
        
        private CancellationTokenSource cancellationTokenSource;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            baseBumperHeight = spriteRenderer.size.y;
            cancellationTokenSource = new CancellationTokenSource();
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log($"Player {other.gameObject.name} collided with {gameObject.name}");
                if(isSandbox)
                    SandboxManager.Instance.BumpPlayer();
                else
                    PlatformerManager.Instance.BumpPlayer();
                if(currentBumperUpdate.Status == UniTaskStatus.Pending)
                    cancellationTokenSource.Cancel();
                cancellationTokenSource = new CancellationTokenSource();
                // TODO : Use an animation
                currentBumperUpdate = UpdateBumperHeight(cancellationTokenSource.Token);
            }
        }

        private async UniTask UpdateBumperHeight(CancellationToken cancellationToken)
        {
            float t = 0f;
            
            float currentBumperHeight = spriteRenderer.size.y;
            
            // Bump
            while (t < 1)
            {
                t += Time.deltaTime*lerpSpeed;
                if (t > 1)
                    t = 1;
                spriteRenderer.size = new Vector2(spriteRenderer.size.x, Mathf.Lerp(currentBumperHeight, triggeredMaxHeight, t));
                await UniTask.Yield(cancellationToken);
            }

            // Wait while up
            await UniTask.WaitForSeconds(maxHeightDuration, cancellationToken: cancellationToken);
            
            // Fall
            t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime*lerpSpeed;
                if (t > 1)
                    t = 1;
                spriteRenderer.size = new Vector2(spriteRenderer.size.x, Mathf.Lerp(triggeredMaxHeight, baseBumperHeight, t));
                await UniTask.Yield(cancellationToken);
            }
        }
        
    }
}