///
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.Tilemaps;

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

        //Get the normalized direction from a start point towards an end point
        static public Vector2 NormalizedDirection(Vector2 start, Vector2 end)
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

        static public void GridLineOverlap(Vector2 worldStartPoint, Vector2 worldEndPoint, Grid grid, List<Vector2Int> results)
        {
            worldStartPoint = grid.WorldToLocal(worldStartPoint);
            worldEndPoint = grid.WorldToLocal(worldEndPoint);

            GridLineOverlap(worldStartPoint, worldEndPoint, results);
        }

        /// <summary>
        /// Get all tile coordinates(assuming the size of the tile is 1.0f with no spacing in world space) that a line from start to end points overlaps.
        /// </summary>
        //Source: http://playtechs.blogspot.com/2007/03/raytracing-on-grid.html
        static public void GridLineOverlap(Vector2 localStartPoint, Vector2 localEndPoint, List<Vector2Int> results)
        {
            Vector2 deltaPoint = new Vector2(Mathf.Abs(localEndPoint.x - localStartPoint.x), Mathf.Abs(localEndPoint.y - localStartPoint.y));

            int posX = Mathf.FloorToInt(localStartPoint.x);
            int posY = Mathf.FloorToInt(localStartPoint.y);

            int n = 1;
            int x_Inc, y_Inc;
            float error;

            if(deltaPoint.x == 0)
            {
                x_Inc = 0;
                error = float.PositiveInfinity;
            }
            else if(localEndPoint.x > localStartPoint.x)
            {
                x_Inc = 1;
                n += Mathf.FloorToInt(localEndPoint.x) - posX;
                error = (Mathf.Floor(localStartPoint.x) + 1 - localStartPoint.x) * deltaPoint.y;
            }
            else
            {
                x_Inc = -1;
                n += posX - Mathf.FloorToInt(localEndPoint.x);
                error = (localStartPoint.x - Mathf.Floor(localStartPoint.x)) * deltaPoint.y;
            }

            if(deltaPoint.y == 0)
            {
                y_Inc = 0;
                error -= float.PositiveInfinity;
            }
            else if(localEndPoint.y > localStartPoint.y)
            {
                y_Inc = 1;
                n += Mathf.FloorToInt(localEndPoint.y) - posY;
                error -= (Mathf.Floor(localStartPoint.y) + 1 - localStartPoint.y) * deltaPoint.x;
            }
            else
            {
                y_Inc = -1;
                n += posY - Mathf.FloorToInt(localEndPoint.y);
                error -= (localStartPoint.y - Mathf.Floor(localStartPoint.y)) * deltaPoint.x;
            }

            results.Clear();
            if(results.Capacity < n)
            {
                //An extra 10 just for fun
                results.Capacity = n + 10;
            }

            for(; n > 0; --n)
            {
                results.Add(new Vector2Int(posX, posY));

                if(error > 0)
                {
                    posY += y_Inc;
                    error -= deltaPoint.x;
                }
                else
                {
                    posX += x_Inc;
                    error += deltaPoint.y;
                }
            }
        }

        /// <summary>
        /// Will be deleted due to being slower than other implementations. Use LineClip instead.
        /// </summary>
        //Source: https://en.wikipedia.org/wiki/Cohen%E2%80%93Sutherland_algorithm
        /*
        static public bool LineClipCohenSutherland(Vector2 startPoint, Vector2 endPoint, Rect rect, out ValueTuple<Vector2,Vector2> result)
        {
            const int INSIDE = 0; // 0000
            const int LEFT = 1;   // 0001
            const int RIGHT = 2;  // 0010
            const int BOTTOM = 4; // 0100
            const int TOP = 8;    // 1000

            Vector2 min = rect.min;
            Vector2 max = rect.max;

            int ComputeOutCode(Vector2 point)
            {
                int code = INSIDE; // initialised as being inside of [[clip window]]

                if(point.x < min.x)          // to the left of clip window
                    code |= LEFT;
                else if(point.x > max.x)      // to the right of clip window
                    code |= RIGHT;
                if(point.y < min.y)           // below the clip window
                    code |= BOTTOM;
                else if(point.y > max.y)      // above the clip window
                    code |= TOP;

                return code;
            }

            // compute outcodes for P0, P1, and whatever point lies outside the clip rectangle
            int outCode0 = ComputeOutCode(startPoint);
            int outCode1 = ComputeOutCode(endPoint);


            bool accept = false;

            while(true)
            {
                if((outCode0 | outCode1) == 0)
                {
                    // bitwise OR is 0: both points inside window; trivially accept and exit loop
                    accept = true;
                    break;
                }
                else if((outCode0 & outCode1) != 0)
                {
                    // bitwise AND is not 0: both points share an outside zone (LEFT, RIGHT, TOP,
                    // or BOTTOM), so both must be outside window; exit loop (accept is false)
                    break;
                }
                else
                {
                    // failed both tests, so calculate the line segment to clip
                    // from an outside point to an intersection with clip edge
                    float x, y;

                    int outcodeOut = outCode1 > outCode0 ? outCode1 : outCode0;

                    // Now find the intersection point;
                    // use formulas:
                    //   slope = (y1 - y0) / (x1 - x0)
                    //   x = x0 + (1 / slope) * (ym - y0), where ym is ymin or ymax
                    //   y = y0 + slope * (xm - x0), where xm is xmin or xmax
                    // No need to worry about divide-by-zero because, in each case, the
                    // outcode bit being tested guarantees the denominator is non-zero
                    if((outcodeOut & TOP) != 0)
                    {
                        x = startPoint.x + (endPoint.x - startPoint.x) * (max.y - startPoint.y) / (endPoint.y - startPoint.y);
                        y = max.y;
                    }
                    else if((outcodeOut & BOTTOM) != 0)
                    {
                        // point is below the clip window
                        x = startPoint.x + (endPoint.x - startPoint.x) * (min.y - startPoint.y) / (endPoint.y - startPoint.y);
                        y = min.y;
                    }
                    else if((outcodeOut & RIGHT) != 0)
                    {
                        // point is to the right of clip window
                        y = startPoint.y + (endPoint.y - startPoint.y) * (max.x - startPoint.x) / (endPoint.x - startPoint.x);
                        x = max.x;
                    }
                    else if((outcodeOut & LEFT) != 0)
                    {
                        // point is to the left of clip window
                        y = startPoint.y + (endPoint.y - startPoint.y) * (min.x - startPoint.x) / (endPoint.x - startPoint.x);
                        x = min.x;
                    }
                    else
                    {
                        x = float.NaN;
                        y = float.NaN;
                    }

                    // Now we move outside point to intersection point to clip
                    // and get ready for next pass.
                    if(outcodeOut == outCode0)
                    {
                        startPoint = new Vector2(x, y);
                        outCode0 = ComputeOutCode(startPoint);
                    }
                    else
                    {
                        endPoint = new Vector2(x, y);
                        outCode1 = ComputeOutCode(endPoint);
                    }
                }
            }

            result = new ValueTuple<Vector2, Vector2>(startPoint, endPoint);
            return accept;
        }
        */

        /// <summary>
        /// Cast a line with a rect and trim the line to be contained within the rect. Returns a bool with whether the line overlaps the rect or not. Output variable is the resulting line that is contained within the rect.
        /// A modified version of the proposed algorithm from https://arxiv.org/abs/1908.01350.
        /// I've verified this proposed white paper(or at least my implementation) is faster than the Cohen-Sutherland algorithm on Wikipedia(https://en.wikipedia.org/wiki/Cohen%E2%80%93Sutherland_algorithm).
        /// Differences is that there is no loop or temporary array since I believe an array lookup would be slower(even using a stackalloc pointer or span for a stack allocated array) than a direct access to a local variable and the loop is definitely not needed(and potentially slower but not 100%).
        /// </summary>
        static public bool LineClip(Vector2 startPoint, Vector2 endPoint, Rect rect, out ValueTuple<Vector2, Vector2> result)
        {
            Vector2 min = rect.min;
            Vector2 max = rect.max;
            
            if(!(startPoint.x < min.x && endPoint.x < min.x)  && !(startPoint.x > max.x && endPoint.x > max.x))
            {
                if(!(startPoint.y < min.y && endPoint.y < min.y) && !(startPoint.y > max.y && endPoint.y < max.y))
                {
                    Vector2 resultStart = startPoint;
                    Vector2 resultEnd = endPoint;

                    //Determine result start
                    if(resultStart.x < min.x)
                    {
                        resultStart = new Vector2(min.x, ((endPoint.y - startPoint.y) / (endPoint.x - startPoint.x)) * (min.x - startPoint.x) + startPoint.y);
                    }
                    else if(resultStart.x > max.x)
                    {
                        resultStart = new Vector2(max.x, ((endPoint.y - startPoint.y) / (endPoint.x - startPoint.x)) * (max.x - startPoint.x) + startPoint.y);
                    }

                    if(resultStart.y < min.y)
                    {
                        resultStart = new Vector2(((endPoint.x - startPoint.x) / (endPoint.y - startPoint.y)) * (min.y - startPoint.y) + startPoint.x, min.y);
                    }
                    else if(resultStart.y > max.y)
                    {
                        resultStart = new Vector2(((endPoint.x - startPoint.x) / (endPoint.y - startPoint.y)) * (max.y - startPoint.y) + startPoint.x, max.y);
                    }

                    //Determine result end
                    if(resultEnd.x < min.x)
                    {
                        resultEnd = new Vector2(min.x, ((endPoint.y - startPoint.y) / (endPoint.x - startPoint.x)) * (min.x - startPoint.x) + startPoint.y);
                    }
                    else if(resultEnd.x > max.x)
                    {
                        resultEnd = new Vector2(max.x, ((endPoint.y - startPoint.y) / (endPoint.x - startPoint.x)) * (max.x - startPoint.x) + startPoint.y);
                    }

                    if(resultEnd.y < min.y)
                    {
                        resultEnd = new Vector2(((endPoint.x - startPoint.x) / (endPoint.y - startPoint.y)) * (min.y - startPoint.y) + startPoint.x, min.y);
                    }
                    else if(resultEnd.y > max.y)
                    {
                        resultEnd = new Vector2(((endPoint.x - startPoint.x) / (endPoint.y - startPoint.y)) * (max.y - startPoint.y) + startPoint.x, max.y);
                    }

                    if(!(resultStart.x < min.x && resultEnd.x < min.x) && !(resultStart.x > max.x && resultEnd.x > max.x))
                    {
                        result = new ValueTuple<Vector2, Vector2>(resultStart, resultEnd);
                        return true;
                    }
                }
            }

            //The line does not overlap with the rect
            result = new ValueTuple<Vector2, Vector2>(startPoint, endPoint);
            return false;
        }

        /// <summary>
        /// Cast a ray towards a rect. Returns true if the ray intersects with rect. The hitpoint is outputted as a result. 
        /// </summary>
        static public bool RaycastRect(Ray2D ray, Rect rect, out Vector2 hitPoint)
        {
            Vector2 min = rect.min;
            Vector2 max = rect.max;

            Vector2 endPoint = ray.direction * float.MaxValue;

            if(!(ray.origin.x < min.x && endPoint.x < min.x) && !(ray.origin.x > max.x && endPoint.x > max.x))
            {
                if(!(ray.origin.y < min.y && endPoint.y < min.y) && !(ray.origin.y > max.y && endPoint.y < max.y))
                {
                    Vector2 resultStart = ray.origin;

                    //Determine result start
                    if(resultStart.x < min.x)
                    {
                        resultStart = new Vector2(min.x, ((endPoint.y - ray.origin.y) / (endPoint.x - ray.origin.x)) * (min.x - ray.origin.x) + ray.origin.y);
                    }
                    else if(resultStart.x > max.x)
                    {
                        resultStart = new Vector2(max.x, ((endPoint.y - ray.origin.y) / (endPoint.x - ray.origin.x)) * (max.x - ray.origin.x) + ray.origin.y);
                    }

                    if(resultStart.y < min.y)
                    {
                        resultStart = new Vector2(((endPoint.x - ray.origin.x) / (endPoint.y - ray.origin.y)) * (min.y - ray.origin.y) + ray.origin.x, min.y);
                    }
                    else if(resultStart.y > max.y)
                    {
                        resultStart = new Vector2(((endPoint.x - ray.origin.x) / (endPoint.y - ray.origin.y)) * (max.y - ray.origin.y) + ray.origin.x, max.y);
                    }

                    if(!(resultStart.x < min.x) && !(resultStart.x > max.x))
                    {
                        hitPoint = resultStart;
                        return true;
                    }
                }
            }

            //The ray does not intersect with the rect
            hitPoint = new Vector2(float.NaN, float.NaN);
            return false;
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