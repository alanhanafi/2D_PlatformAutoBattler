using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DefaultNamespace
{
    public static class Functions
    {
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
    }
}