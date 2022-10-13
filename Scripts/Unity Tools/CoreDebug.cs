using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using UnityEngine;
using Random = UnityEngine.Random;

namespace Elanetic.Tools.Unity
{
    /// <summary>
    /// Collection of debugging tools for Unity.
    /// </summary>
    static public class CoreDebug
    {
        static private Texture2D m_ErrorTexture;
        static private Sprite m_ErrorSprite;

        /// <summary>
        /// Retrieve the shared error texture. Useful for cases where creating a new texture is not necessary. However modification of this texture means it will affect other code using this shared texture.
        /// </summary>
        static public Texture2D GetSharedErrorTexture()
        {
            if(m_ErrorTexture == null)
            {
                m_ErrorTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                m_ErrorTexture.filterMode = FilterMode.Point;
                m_ErrorTexture.wrapMode = TextureWrapMode.Repeat;
                m_ErrorTexture.SetPixels(new Color[] { Color.black, Color.magenta, Color.magenta, Color.black });
                m_ErrorTexture.Apply();
            }
            return m_ErrorTexture;
        }

        /// <summary>
        /// Create an error texture with the pattern of pink and black.
        /// </summary>
        static public Texture2D CreateErrorTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Repeat;
            Color32[] pixels = new Color32[width * height];
            for(int i = 0; i < pixels.Length; i++)
            {
                if(((i % width) + ((i / width) % 2)) % 2 == 0)
                    pixels[i] = Color.black;
                else
                    pixels[i] = Color.magenta;
            }
            texture.SetPixels32(pixels);
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// Retrieve the shared error texture as a sprite. Useful for cases where creating a new texture and sprite is not necessary. However modification of this texture/sprite means it will affect other code using this shared texture/sprite.
        /// </summary>
        static public Sprite GetSharedErrorSprite()
        {
            if(m_ErrorSprite == null)
            {
                m_ErrorSprite = Sprite.Create(GetSharedErrorTexture(), new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 4.0f);
            }

            return m_ErrorSprite;
        }

        /// <summary>
        /// Create an error sprite with a texture pattern of pink and black.
        /// </summary>
        static public Sprite CreateErrorSprite(int width, int height)
        {
            return Sprite.Create(CreateErrorTexture(width, height), new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 4.0f);
        }


        static public void DrawBounds(BoundsInt bounds) => DrawBounds(new Bounds(bounds.center, bounds.size));

        static public void DrawBounds(BoundsInt bounds, float centerMarkerSize) => DrawBounds(new Bounds(bounds.center, bounds.size), centerMarkerSize, Color.red, 0.0f, true);

        static public void DrawBounds(BoundsInt bounds, float centerMarkerSize, Color color) => DrawBounds(new Bounds(bounds.center, bounds.size), centerMarkerSize, color, 0.0f, true);

        static public void DrawBounds(BoundsInt bounds, float centerMarkerSize, Color color, float duration) => DrawBounds(new Bounds(bounds.center, bounds.size), centerMarkerSize, color, duration, true);

        static public void DrawBounds(BoundsInt bounds, float centerMarkerSize, Color color, float duration, bool depthTest) => DrawBounds(new Bounds(bounds.center, bounds.size), centerMarkerSize, color, duration, depthTest);

        static public void DrawBounds(BoundsInt bounds, Color color) => DrawBounds(new Bounds(bounds.center, bounds.size), color);

        static public void DrawBounds(Bounds bounds) => DrawBounds(bounds, 0.0f, Color.red, 0.0f, true);

        static public void DrawBounds(Bounds bounds, float centerMarkerSize) => DrawBounds(bounds, centerMarkerSize, Color.red, 0.0f, true);

        static public void DrawBounds(Bounds bounds, float centerMarkerSize, Color color) => DrawBounds(bounds, centerMarkerSize, color, 0.0f, true);

        static public void DrawBounds(Bounds bounds, float centerMarkerSize, Color color, float duration) => DrawBounds(bounds, centerMarkerSize, color, duration, true);

        static public void DrawBounds(Bounds bounds, Color color) => DrawBounds(bounds, 0.0f, color, 0.0f, true);

        static public void DrawBounds(Bounds bounds, float centerMarkerSize, Color color, float duration, bool depthTest)
        {
            Vector3 frontTopLeft = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z);
            Vector3 frontTopRight = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z);
            Vector3 frontBottomLeft = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z);
            Vector3 frontBottomRight = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z);
            Vector3 backTopLeft = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z);
            Vector3 backTopRight = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z);
            Vector3 backBottomLeft = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z);
            Vector3 backBottomRight = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z);

            //Cube
            Debug.DrawLine(frontTopLeft, frontTopRight, color, duration, depthTest);
            Debug.DrawLine(frontTopRight, frontBottomRight, color, duration, depthTest);
            Debug.DrawLine(frontBottomRight, frontBottomLeft, color, duration, depthTest);
            Debug.DrawLine(frontBottomLeft, frontTopLeft, color, duration, depthTest);

            Debug.DrawLine(backTopLeft, backTopRight, color, duration, depthTest);
            Debug.DrawLine(backTopRight, backBottomRight, color, duration, depthTest);
            Debug.DrawLine(backBottomRight, backBottomLeft, color, duration, depthTest);
            Debug.DrawLine(backBottomLeft, backTopLeft, color, duration, depthTest);

            Debug.DrawLine(frontTopLeft, backTopLeft, color, duration, depthTest);
            Debug.DrawLine(frontTopRight, backTopRight, color, duration, depthTest);
            Debug.DrawLine(frontBottomRight, backBottomRight, color, duration, depthTest);
            Debug.DrawLine(frontBottomLeft, backBottomLeft, color, duration, depthTest);

            //Center marker
            centerMarkerSize *= 0.5f;
            //X
            Debug.DrawLine(new Vector3(bounds.center.x - centerMarkerSize, bounds.center.y, bounds.center.z), new Vector3(bounds.center.x + centerMarkerSize, bounds.center.y, bounds.center.z), color, duration, depthTest);
            //Y
            Debug.DrawLine(new Vector3(bounds.center.x, bounds.center.y - centerMarkerSize, bounds.center.z), new Vector3(bounds.center.x, bounds.center.y + centerMarkerSize, bounds.center.z), color, duration, depthTest);
            //Z
            Debug.DrawLine(new Vector3(bounds.center.x, bounds.center.y, bounds.center.z - centerMarkerSize), new Vector3(bounds.center.x, bounds.center.y, bounds.center.z + centerMarkerSize), color, duration, depthTest);
        }

        static public void DrawRect(Rect rect) => DrawRect(rect, 0.0f, Color.red, 0.0f, true);

        static public void DrawRect(Rect rect, float centerMarkerSize) => DrawRect(rect, centerMarkerSize, Color.red, 0.0f, true);

        static public void DrawRect(Rect rect, float centerMarkerSize, Color color) => DrawRect(rect, centerMarkerSize, color, 0.0f, true);

        static public void DrawRect(Rect rect, float centerMarkerSize, Color color, float duration) => DrawRect(rect, centerMarkerSize, color, duration, true);

        static public void DrawRect(Rect rect, Color color) => DrawRect(rect, 0.0f, color, 0.0f, true);

        //Remember a rect's position is bottom left. Not center.
        static public void DrawRect(Rect rect, float centerMarkerSize, Color color, float duration, bool depthTest)
        {
            DrawBounds(new Bounds(rect.center, rect.size), centerMarkerSize, color, duration, depthTest);
        }

        static public void DrawRect(Vector2 center, float size) => DrawRect(center, size, 0.0f, Color.red, 0.0f, true);

        static public void DrawRect(Vector2 center, float size, float centerMarkerSize) => DrawRect(center, size, centerMarkerSize, Color.red, 0.0f, true);

        static public void DrawRect(Vector2 center, float size, float centerMarkerSize, Color color) => DrawRect(center, size, centerMarkerSize, color, 0.0f, true);

        static public void DrawRect(Vector2 center, float size, float centerMarkerSize, Color color, float duration) => DrawRect(center, size, centerMarkerSize, color, duration, true);

        static public void DrawRect(Vector2 center, float size, Color color) => DrawRect(center, size, 0.0f, color, 0.0f, true);

        static public void DrawRect(Vector2 center, float size, float centerMarkerSize, Color color, float duration, bool depthTest)
        {
            DrawRect(new Rect(new Vector2(center.x - (size * 0.5f), center.y - (size * 0.5f)), new Vector2(size, size)), centerMarkerSize, color, duration, depthTest);
        }

        static public void DrawPoint(Vector3 point) => DrawPoint(point, 1.0f, Color.red, 0.0f, true);

        static public void DrawPoint(Vector3 point, float size) => DrawPoint(point, size, Color.red, 0.0f, true);

        static public void DrawPoint(Vector3 point, float size, Color color) => DrawPoint(point, size, color, 0.0f, true);

        static public void DrawPoint(Vector3 point, float size, Color color, float duration) => DrawPoint(point, size, color, duration, true);

        static public void DrawPoint(Vector3 point, Color color) => DrawPoint(point, 1.0f, color, 0.0f, true);

        static public void DrawPoint(Vector3 point, float size, Color color, float duration, bool depthTest)
        {
            DrawBounds(new Bounds(point, new Vector3(size * 0.5f, size * 0.5f, size * 0.5f)), size, color, duration, depthTest);
        }
    }
}
