using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.Collections;

namespace Elanetic.Tools.Unity
{
    static public class Texture2DExtensions
    {
        static public void SetPixels(this Texture2D targetTexture, int x, int y, int blockWidth, int blockHeight, Color color)
        {
            SetPixels32(targetTexture, x, y, blockWidth, blockHeight, color, 0);
        }

        static public void SetPixels(this Texture2D targetTexture, int x, int y, int blockWidth, int blockHeight, Color color, int miplevel)
        {
            SetPixels32(targetTexture, x, y, blockWidth, blockHeight, color, miplevel);
        }

        static public void SetPixels32(this Texture2D targetTexture, int x, int y, int blockWidth, int blockHeight, Color32 color)
        {
            SetPixels32(targetTexture, x, y, blockWidth, blockHeight, color, 0);
        }

        static public void SetPixels32(this Texture2D targetTexture, int x, int y, int blockWidth, int blockHeight, Color32 color, int miplevel)
        {
#if SAFE_EXECUTION
            if(blockWidth * blockHeight <= 0)
                throw new ArgumentException("Inputted block size must be more than zero.");
            if(x < 0 || y < 0 || x >= targetTexture.width || y >= targetTexture.height)
                throw new ArgumentException("Inputted position is not within the texture's coordinates");
            if(x + blockWidth > targetTexture.width || y + blockHeight > targetTexture.height)
                throw new ArgumentException("Inputted block size goes outside the bounds of the texture.");
            if(targetTexture.format != TextureFormat.RGBA32)
                Debug.LogWarning("This SetPixels overload requires an 8 bits per channel RGBA format. Any other format other than RGBA32 may cause errors or unexpected behaviour.");
#endif
            //REVIEW: These next few lines are optional to ensure to sanitize input but also more performant without them.
            /*
            if(x > targetTexture.width || y > targetTexture.height) return;

            x = Mathf.Max(0, x);
            y = Mathf.Max(0, y);
            blockWidth = Mathf.Min(blockWidth, targetTexture.width - x);
            blockHeight = Mathf.Min(blockHeight, targetTexture.height - y);
            */
            //By converting color32 to an int it means that there are less interations of the for loop. REVIEW: Is this faster?
            int colorInt = (int)((color.r << 0) | (color.g << 8) | (color.b << 16) | (color.a << 24));

            NativeArray<int> pixelData = targetTexture.GetPixelData<int>(miplevel);
            int count = blockWidth * blockHeight;

            for(int i = 0; i < count; i++)
            {
                pixelData[CoreUtilities.CoordToIndex(new Vector2Int(x,y) + CoreUtilities.IndexToCoord(i, blockWidth), targetTexture.width)] = colorInt;
            }
        }

        static public void SetPixels(this Texture2D targetTexture, Color color)
        {
            SetPixels32(targetTexture, 0, 0, targetTexture.width, targetTexture.height, color, 0);
        }

        static public void SetPixels32(this Texture2D targetTexture, Color32 color)
        {
            SetPixels32(targetTexture, 0, 0, targetTexture.width, targetTexture.height, color, 0);
        }
    }
}