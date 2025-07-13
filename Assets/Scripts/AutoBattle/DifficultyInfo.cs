using Shared;
using UnityEditor.Animations;
using UnityEngine;

namespace AutoBattle
{
    [CreateAssetMenu(fileName = "Inventory Set", menuName = "AutoBattle/Difficulty Info")]
    public class DifficultyInfo : ScriptableObject
    {
        [field: SerializeField] internal Difficulty Difficulty { get; set; }
        [field : SerializeField] internal RuntimeAnimatorController AnimatorController { get; set; }
        [field : SerializeField] internal bool FlipX { get; set; }
    }
}