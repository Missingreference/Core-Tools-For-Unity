///
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Random = UnityEngine.Random;

namespace Elanetic.Tools
{
    static public class Utils
    {
        static private Texture2D m_ErrorTexture;
        static private Sprite m_ErrorSprite;

        /// <summary>
        /// Retrieve a random color. Alpha is always 100%.
        /// </summary>
        static public Color GetRandomColor()
        {
            return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
        }

        /// <summary>
        /// Converts a Color or Color32 to hexadecimal in the format of RRGGBBAA. No # symbol is included at the beginning.
        /// </summary>
        static public string ColorToHex(Color32 color)
        {
            return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
        }

        /// <summary>
        /// Convert a hex string (in the format of RRGGBB, RRGGBBAA, #RRGGBB or #RRGGBBAA) to UnityEngine.Color. If no alpha channel inputted, it is set to 100%.
        /// </summary>
        static public Color HexToColor(string hex)
        {
            if(hex[0] == '#')
            {
                if(hex.Length > 7)
                {
                    return new Color32(
                        byte.Parse(hex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber),
                        byte.Parse(hex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber),
                        byte.Parse(hex.Substring(5, 2), System.Globalization.NumberStyles.HexNumber),
                        byte.Parse(hex.Substring(7, 2), System.Globalization.NumberStyles.HexNumber)
                        );
                }
                else
                {
                    return new Color32(
                        byte.Parse(hex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber),
                        byte.Parse(hex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber),
                        byte.Parse(hex.Substring(5, 2), System.Globalization.NumberStyles.HexNumber),
                        255);
                }
            }
            else
            {
                if (hex.Length > 6)
                {
                    return new Color32(
                        byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                        byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                        byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                        byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)
                        );
                }
                else
                {
                    return new Color32(
                        byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                        byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                        byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                        255);
                }
            }
        }

        static public int CoordToIndex(Vector2Int coord, BoundsInt2D bounds)
        {
            return ((coord.y - bounds.min.y) * bounds.size.x) + (coord.x - bounds.min.x);
        }

        static public int CoordToIndex(Vector2Int coord, int width)
        {
            return (coord.y * width) + coord.x;
        }

        static public Vector2Int IndexToCoord(int index, BoundsInt2D bounds)
        {
#if SAFE_EXECUTION
            if(index < 0 || index > bounds.count) 
                throw new IndexOutOfRangeException("Index '" + index + "' is not within the specified bounds. Index would end up being at the coord (" + ((index % bounds.size.x) + bounds.min.x).ToString() + "," + ((index / bounds.size.x) + bounds.min.y).ToString() + ") (Min: " + bounds.min + " Max: " + bounds.max + ") (Max index: " + bounds.count + ")");
#endif
            return new Vector2Int((index % bounds.size.x) + bounds.min.x, (index / bounds.size.x) + bounds.min.y);
        }

        static public Vector2Int IndexToCoord(int index, int width)
        {
            return new Vector2Int(index % width, index / width);
        }

        static public float RoundToNearestMultiple(float numberToRound, float multipleOf)
        {
            return ((int)Math.Round(numberToRound / multipleOf)) * multipleOf;
        }

        //Get the smallest number being a mulitpleOf that can fit the numberToAlign.
        //Example: n = 30, m = 4, result = 32  32 is the smallest number that is a multiple of 4 that can fit 30, 28 cannot fit 30 so the next multiple of 4 would be 32
        static public int AlignToMultipleOf(int numberToAlign, int multipleOf)
        {
            //(n+(m-1))/m)*m
            return ((numberToAlign + (multipleOf - 1)) / multipleOf) * multipleOf;
        }

        static public uint AlignToMultipleOf(uint numberToAlign, uint multipleOf)
        {
            //(n+(m-1))/m)*m
            return ((numberToAlign + (multipleOf - 1u)) / multipleOf) * multipleOf;
        }

        static public long AlignToMultipleOf(long numberToAlign, long multipleOf)
        {
            //(n+(m-1))/m)*m
            return ((numberToAlign + (multipleOf - 1L)) / multipleOf) * multipleOf;
        }

        static public Vector2 Normalized(Vector2 start, Vector2 end)
        {
            Vector2 heading = end - start;
            return heading / heading.magnitude;
        }

        /// <summary>
        /// Checks if the mask has only a single flag set.
        /// Also: A mask of zero or less returns false.
        /// https://stackoverflow.com/a/37795876
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        static public bool MaskHasOnlyOneFlag(int mask) //Faster than GetFlagCount?
        {
            return ((mask - 1) & mask) == 0 && mask > 0;
        }

        /// <summary>
        /// Get count of flags set on a mask.
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        static public int GetFlagCount(int mask)
        {
            int count = 0;
            while(mask > 0)
            {
                mask &= (mask - 1);
                count++;
            }
            return count;
        }

        static public Texture2D GetErrorTexture()
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

        static public Texture2D GetErrorTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Repeat;
            Color32[] pixels = new Color32[width * height];
            for(int i = 0; i < pixels.Length; i++)
            {
                if(((i%width) + ((i/width) % 2)) % 2 == 0)
                    pixels[i] = Color.black;
                else
                    pixels[i] = Color.magenta;
            }
            texture.SetPixels32(pixels);
            texture.Apply();
            return texture;
        }

        static public Sprite GetErrorSprite()
        {
            if(m_ErrorSprite == null)
            {
                m_ErrorSprite = Sprite.Create(GetErrorTexture(), new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 4.0f);
            }

            return m_ErrorSprite;
        }

        static public Sprite GetErrorSprite(int width, int height)
        {
            return Sprite.Create(GetErrorTexture(width, height), new Rect(0,0,width,height), new Vector2(0.5f, 0.5f), 4.0f);
        }

        static public Texture2D PadTexture2D(Texture2D targetTexture, int amount)
        {
            return PadTexture2D(targetTexture, amount, Color.clear);
        }

        static public Texture2D PadTexture2D(Texture2D targetTexture, int amount, Color color)
        {
#if SAFE_EXECUTION
            if (targetTexture == null) 
                throw new ArgumentNullException(nameof(targetTexture), "Inputted texture is null.");
            if(amount <= 0) 
                throw new ArgumentException("Argument 'amount' must be more than zero.", nameof(amount));
#endif

            Texture2D finalTexture = new Texture2D(targetTexture.width+(amount*2), targetTexture.height+(amount*2), targetTexture.format, false);

            finalTexture.filterMode = targetTexture.filterMode;

            //Set padded pixels to color
            finalTexture.SetPixels(0, 0, finalTexture.width, amount, color);
            finalTexture.SetPixels(0, finalTexture.height - amount, finalTexture.width, amount, color);
            finalTexture.SetPixels(0, amount, amount, finalTexture.height-(amount*2), color);
            finalTexture.SetPixels(finalTexture.width - amount, amount, amount, finalTexture.height - (amount * 2), color);

            finalTexture.SetPixels32(amount, amount, targetTexture.width, targetTexture.height, targetTexture.GetPixels32());

            finalTexture.Apply();

            return finalTexture;
        }

        /// <summary>
        /// Split texture using sprite information. Make sure each inputted sprite uses the same source texture.
        /// </summary>
        /// <param name="sprites"></param>
        /// <returns></returns>
        static public Texture2D[] SeperateTexturesFromSprites(Sprite[] sprites)
        {
            Texture2D[] resultTextures = new Texture2D[sprites.Length];

            Texture2D sourceTexture = sprites[0].texture;
            Color32[] sourcePixels = sourceTexture.GetPixels32();

            for(int i = 0; i < sprites.Length; i++)
            {
                Sprite sprite = sprites[i];
#if SAFE_EXECUTION
                if(sprite.texture != sourceTexture) 
                    throw new InvalidOperationException("Each inputted sprite must have the same source texture.");
                if(sprite == null) 
                    throw new NullReferenceException("Inputted sprite is null at index " + i.ToString() + ".");
#endif

                resultTextures[i] = new Texture2D((int)sprite.rect.size.x, (int)sprite.rect.size.y, sourceTexture.format, false);
                
                int pixelCount = (int)(sprite.rect.size.x * sprite.rect.size.y);
                Color32[] pixels = new Color32[pixelCount];
                for(int h = 0; h < pixelCount; h++)
                {
                    Vector2Int pixel = Utils.IndexToCoord(h, (int)sprite.rect.size.x);
                    //TODO Use SetPixelData instead for performance
                    int index = Utils.CoordToIndex(pixel + new Vector2Int((int)sprite.rect.position.x, (int)(sprite.rect.position.y)), sourceTexture.width);
                    pixels[h] = sourcePixels[index];
                }
                resultTextures[i].SetPixels32(pixels);
                resultTextures[i].Apply();
            }

            return resultTextures;
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

        static public Vector2 DegreesToVector2(float degrees)
        {
            return new Vector2(Mathf.Cos(degrees * Mathf.Deg2Rad), Mathf.Sin(degrees * Mathf.Deg2Rad));
        }

        static public float Vector2ToDegrees(Vector2 direction)
        {
            if (direction.y < 0.0f)
                return -(Mathf.Acos(direction.x) * Mathf.Rad2Deg);
            return Mathf.Acos(direction.x) * Mathf.Rad2Deg;
        }

        static public Vector2 RotateVector2(Vector2 direction, float degrees)
        {
            return DegreesToVector2(Vector2ToDegrees(direction) + degrees);
        }

        static public Vector2 RotateAround(Vector2 point, Vector2 center, float degrees)
        {
            float radians = Mathf.Deg2Rad * degrees;
            float cosTheta = Mathf.Cos(radians);
            float sinTheta = Mathf.Sin(radians);

            return new Vector2((cosTheta * (point.x - center.x) - sinTheta * (point.y - center.y) + center.x),
                          (sinTheta * (point.x - center.x) + cosTheta * (point.y - center.y) + center.y));
        }
    }
}