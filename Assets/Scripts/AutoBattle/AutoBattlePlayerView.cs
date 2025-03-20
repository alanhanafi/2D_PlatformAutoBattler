using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class AutoBattlePlayerView : MonoBehaviour
    {
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private Transform damageTextParent;
        
        private List<DamageText> damageTextList = new();
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private Animator animator;
        
        private SpriteRenderer spriteRenderer;
        
        private void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer  = GetComponent<SpriteRenderer>();
        }

        internal void PlayAttackAnimation()
        {
            animator.SetTrigger(AttackTrigger);
        }

        internal void PlayHitFx()
        {
            Functions.ChangeAlpha(spriteRenderer, .5f);
            WaitThenChangeAlphaAsync().Forget();
        }

        private async UniTask WaitThenChangeAlphaAsync()
        {
            await UniTask.WaitForSeconds(0.2f);
            Functions.ChangeAlpha(spriteRenderer, 1);
        }

        internal void ShowDamageText(int damageToDisplay)
        {
            if (damageTextList.Count == 0 || damageTextList.All(obj => obj.gameObject.activeSelf))
            {
                DamageText newDamageText = Instantiate(damageTextPrefab, damageTextParent).GetComponent<DamageText>();
                newDamageText.ShowDamageText(damageToDisplay);
                damageTextList.Add(newDamageText);
                return;
            }
            damageTextList.First(obj => !obj.gameObject.activeSelf).ShowDamageText(damageToDisplay);
        }
    }
}