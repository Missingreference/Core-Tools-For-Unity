using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Elanetic.Tools
{
    static public class Texture2DExtensions
    {
        //Do not use these extension functions if you plan to pass VERY large textures in as the color pool will stay in memory.
        static private Color32[] m_ColorPool = new Color32[1000];
        static private Color32 m_LastColor = Color.clear;
        static private int m_LastAmount = 1000;

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
            int totalPixelCount = blockWidth * blockHeight;
            //Check if the color pool needs to be resized
            if(totalPixelCount > m_ColorPool.Length)
            {
                //Resize color pool
                //250 is an arbitrary size to try to minimize color pool resizes(garbage allocation) and not waste unused memory. Change to personal preference.
                m_ColorPool = new Color32[Mathf.Min(totalPixelCount + 250, SystemInfo.maxTextureSize * SystemInfo.maxTextureSize)];
                m_LastColor = Color.clear;
                m_LastAmount = m_ColorPool.Length;
            }
            else if(totalPixelCount <= 0)
            {
                //Let Unity's SetPixels deal with that error
                targetTexture.SetPixels32(x, y, blockWidth, blockHeight, m_ColorPool, miplevel);
                return;
            }

            //Check if the last color is equivalent
            if(color.r == m_LastColor.r && color.g == m_LastColor.g && color.b == m_LastColor.b && color.a == m_LastColor.a)
            {
                //Reuse color array and fill if needed
                for(int i = m_LastAmount; i < totalPixelCount; i++)
                {
                    m_ColorPool[i] = color;
                }
                m_LastAmount = Mathf.Max(m_LastAmount, totalPixelCount);
            }
            else
            {
                //New color, no performance gain
                for(int i = 0; i < totalPixelCount; i++)
                {
                    m_ColorPool[i] = color;
                }
                m_LastColor = color;
                m_LastAmount = totalPixelCount;
            }

            //Do SetPixels
            targetTexture.SetPixels32(x, y, blockWidth, blockHeight, m_ColorPool, miplevel);
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