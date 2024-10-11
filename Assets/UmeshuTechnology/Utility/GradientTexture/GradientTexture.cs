using System;
using Umeshu.Uf;
using UnityEngine;

namespace Umeshu.Utility
{
    [System.Serializable]
    public class GradientTexture
    {
        [SerializeField] internal Gradient gradient;
        [SerializeField] internal int width = 256;
        [SerializeField] internal Texture2D texture;

#if UNITY_EDITOR
        internal void UpdateTextureFromGradient()
        {
            if (texture == null)
                CreateTextureFromGradient();

            width = texture.width;

            for (int _x = 0; _x < width; _x++)
                texture.SetPixel(_x, 0, gradient.Evaluate((float)_x / width));

            texture.Apply();
            UnityEditor.EditorUtility.SetDirty(texture);

        }

        internal void CreateTextureFromGradient()
        {
            texture = UfEditor.CreateTexture2DInProject(width, 1);
            texture.filterMode = FilterMode.Point;
            texture.alphaIsTransparency = true;
        }
#endif

        public Texture2D GetTexture() => texture;
        public Gradient GetGradient() => gradient;
    }
}
