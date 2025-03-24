using System;
using TMPro;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace Shared
{
    public static class Functions
    {

        internal static int GetUpRoundedValue(float floatValue)
        {
            return (int)Math.Round(floatValue,MidpointRounding.AwayFromZero);
        }
        
        /// <summary>
        /// Checks if the layer is in the layerMask.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="layerMask"></param>
        /// <returns>true if the layer is in the layerMask, false else.</returns>
        internal static bool IsInLayerMask(int layer, LayerMask layerMask)
        {
            return layerMask == (layerMask | (1 << layer));
        }

        /// <summary>
        /// Changes the alpha of the color of a sprite renderer.
        /// </summary>
        /// <param name="spriteRenderer"></param>
        /// <param name="newAlpha"></param>
        public static void ChangeAlpha(SpriteRenderer spriteRenderer, float newAlpha)
        {
            var newColor = spriteRenderer.color;
            newColor.a = newAlpha;
            spriteRenderer.color = newColor;
        }
        
        public static void ChangeAlpha(TextMeshProUGUI textMeshProUGUI, float newAlpha)
        {
            var newColor = textMeshProUGUI.color;
            newColor.a = newAlpha;
            textMeshProUGUI.color = newColor;
        }
        
        #if UNITY_EDITOR
        public static void SetObjectDirty(Object o) {
            EditorUtility.SetDirty(o);
        }

        public static void SetObjectDirty(GameObject go) {
            EditorUtility.SetDirty(go);
            EditorSceneManager.MarkSceneDirty(go.scene); //This used to happen automatically from SetDirty
        }

        public static void SetObjectDirty(Component comp) {
            EditorUtility.SetDirty(comp);
            EditorSceneManager.MarkSceneDirty(comp.gameObject.scene); //This used to happen automatically from SetDirty
        }
        #endif
    }
}