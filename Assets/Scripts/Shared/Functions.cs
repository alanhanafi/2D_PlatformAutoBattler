using System;
using Platformer;
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
        internal static void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        internal static void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        internal static int GetUpRoundedValue(float floatValue)
        {
            return (int)Math.Round(floatValue,MidpointRounding.AwayFromZero);
        }

        internal static int GetEnumMemberCount(Type enumType)
        {
            return Enum.GetNames(enumType).Length;
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