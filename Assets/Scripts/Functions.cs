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
    }
}