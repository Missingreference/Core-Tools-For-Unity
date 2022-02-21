using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;

using Elanetic.Graphics;
using Elanetic.Tools;

namespace Elanetic.Tilemaps
{
    public class TextureAtlas
    {
        /// <summary>
        /// Size of each individual texture in atlas
        /// </summary>
        public Vector2Int textureSize { get; private set; }

        /// <summary>
        /// The full atlas as a texture.
        /// Can be modified and destroyed but may cause errors.
        /// </summary>
        public Texture2D fullTexture { get; private set; }

        /// <summary>
        /// How textures that have been added to this atlas.
        /// </summary>
        public int textureCount { get; private set; }

        /// <summary>
        /// The maximum amount of textures that fit in this texture atlas.
        /// </summary>
        public Vector2Int maxTextureCount => m_MaxTextureCount;

        public TextureFormat format { get; private set; }

        public IntPtr nativeTexturePointer => m_DirectTexture.nativePointer;

        private DirectTexture2D m_DirectTexture;
        private Vector2Int m_MaxTextureCount;


        public TextureAtlas(Vector2Int textureSize, TextureFormat textureFormat=TextureFormat.RGBA32) : this(textureSize, new Vector2Int(8, 8), textureFormat) { }

        public TextureAtlas(Vector2Int textureSize, Vector2Int initialTextureCount, TextureFormat textureFormat=TextureFormat.RGBA32)
        {
#if SAFE_EXECUTION
            if(textureSize.x <= 0 || textureSize.y <= 0)
                throw new ArgumentException("Inputted texture size must be a positive number.");
            if(textureSize.x >= SystemInfo.maxTextureSize / 2 && textureSize.y >= SystemInfo.maxTextureSize / 2)
                throw new ArgumentException("Texture is too large to get any benefit from a texture atlas. Texture Size: " + textureSize.ToString());
            if(SystemInfo.copyTextureSupport == CopyTextureSupport.None)
                throw new NotSupportedException("SystemInfo reports copyTextureSupport is not supported.");
#endif
            this.textureSize = textureSize;


            format = textureFormat;
            m_MaxTextureCount = initialTextureCount;
            if(textureSize.x * m_MaxTextureCount.x > SystemInfo.maxTextureSize)
            {
                m_MaxTextureCount = new Vector2Int(SystemInfo.maxTextureSize / textureSize.x, m_MaxTextureCount.y);
            }
            if(textureSize.y * m_MaxTextureCount.y > SystemInfo.maxTextureSize)
            {
                m_MaxTextureCount = new Vector2Int(m_MaxTextureCount.x, SystemInfo.maxTextureSize / textureSize.y);
            }

            /*fullTexture = new Texture2D(m_MaxTextureCount.x * textureSize.x, m_MaxTextureCount.y * textureSize.y, textureFormat, false);
            fullTexture.filterMode = FilterMode.Point;
            NativeArray<int> dataArray = fullTexture.GetRawTextureData<int>();
            for(int i = 0; i < dataArray.Length; i++)
            {
                dataArray[i] = 0;
            }
            //We make the atlas not readable due to the spamming of Graphics.CopyTexture's false warnings.
            fullTexture.Apply(false, true);*/
            m_DirectTexture = DirectGraphics.CreateTexture(m_MaxTextureCount.x * textureSize.x, m_MaxTextureCount.y * textureSize.y, textureFormat);
            fullTexture = m_DirectTexture.texture;
            DirectGraphics.ClearTexture(fullTexture);
        }

        public int AddTexture(Texture2D texture)
        {
#if SAFE_EXECUTION
            if(texture == null)
                throw new ArgumentNullException("Inputted texture cannot be null.");
            if(texture.width != textureSize.x || texture.height != textureSize.y)
                throw new ArgumentException("Size of texture does not match the size of the atlas' specified texture size of '" + textureSize + "'. Inputted '" + texture.width + ", " + texture.height + "'.");
            if(texture.format != format)
                Debug.LogWarning("Inputted texture does not match texture atlas format. This may cause exceptions or unexpected behaviour. Input: " + texture.format.ToString() + ", Atlas format: " + format.ToString());
            if(m_MaxTextureCount.x * m_MaxTextureCount.y == textureCount)
                throw new InvalidOperationException("Texture atlas limit of '" + (m_MaxTextureCount.x * m_MaxTextureCount.y) + "' reached. Either use ReplaceTexture or create another texture atlas.");
#endif

            Vector2Int targetPixelCoordinate = AtlasIndexToPixelCoord(textureCount);
            //Graphics.CopyTexture(texture, 0, 0, 0, 0, textureSize.x, textureSize.y, fullTexture, 0, 0, targetPixelCoordinate.x, targetPixelCoordinate.y);
            
            DirectGraphics.CopyTexture(texture.GetNativeTexturePtr(), 0, 0, textureSize.x, textureSize.y, m_DirectTexture.nativePointer, targetPixelCoordinate.x, targetPixelCoordinate.y);

            int atlasIndex = textureCount;
            textureCount++;
            return atlasIndex;
        }

        public void ReplaceTexture(int atlasIndex, Texture2D texture)
        {
#if SAFE_EXECUTION
            if(atlasIndex >= textureCount)
                throw new ArgumentException("Specified atlas index '" + atlasIndex + "' has not been set as a texture. Use AddTexture instead.");
            if(texture == null)
                throw new ArgumentNullException("Inputted texture cannot be null.");
#endif

            Vector2Int targetPixelCoordinate = AtlasIndexToPixelCoord(atlasIndex);
            //Graphics.CopyTexture(texture, 0, 0, 0, 0, textureSize.x, textureSize.y, fullTexture, 0, 0, targetPixelCoordinate.x, targetPixelCoordinate.y);
            DirectGraphics.CopyTexture(texture.GetNativeTexturePtr(), 0, 0, textureSize.x, textureSize.y, m_DirectTexture.nativePointer, targetPixelCoordinate.x, targetPixelCoordinate.y);
        }

        public Sprite CreateSprite(int atlasIndex, Vector2 pivot, float pixelsPerUnit)
        {
#if SAFE_EXECUTION
            if(atlasIndex < 0 || atlasIndex >= textureCount)
                throw new ArgumentOutOfRangeException(nameof(atlasIndex), "Inputted atlas index is out of range.");
#endif
            return Sprite.Create(fullTexture, new Rect(AtlasIndexToPixelCoord(atlasIndex), new Vector2(textureSize.x, textureSize.y)), pivot, pixelsPerUnit, 0, SpriteMeshType.FullRect, Vector4.zero, false);
        }

        public Vector2Int AtlasIndexToPixelCoord(int atlasIndex)
        {
            return Utils.IndexToCoord(atlasIndex, m_MaxTextureCount.x) * textureSize;
        }

        /*
        private void ResizeAtlas()
        {
            Vector2Int newMaxTextureCount = new Vector2Int(m_MaxTextureCount.x + 64, m_MaxTextureCount.y + 64);

            Vector2Int oldTextureSize = m_MaxTextureCount * textureSize;
            Vector2Int newTextureSize = newMaxTextureCount * textureSize;

            if(newTextureSize.x > SystemInfo.maxTextureSize)
            {
                newMaxTextureCount = new Vector2Int(SystemInfo.maxTextureSize / textureSize.x, newMaxTextureCount.y);
                newTextureSize = newMaxTextureCount * textureSize;
            }
            if(newTextureSize.y > SystemInfo.maxTextureSize)
            {
                newMaxTextureCount = new Vector2Int(newMaxTextureCount.x, SystemInfo.maxTextureSize / textureSize.y);
                newTextureSize = newMaxTextureCount * textureSize;
            }

#if SAFE_EXECUTION
            if(m_MaxTextureCount == newMaxTextureCount)
                throw new InvalidOperationException("Cannot resize atlas to a larger size. Reached system max texture size.");
#endif


            Color32[] pixels = fullTexture.GetPixels32();

            if(!fullTexture.Resize(newTextureSize.x, newTextureSize.y))
            {
                throw new InvalidOperationException("Texture2D resize failed.");
            }

            NativeArray<Color32> textureColorArray = fullTexture.GetPixelData<Color32>(0);
            int texturePixelCount = textureSize.x * textureSize.y;
            for(int i = 0; i < textureCount; i++)
            {
                for(int h = 0; h < texturePixelCount; h++)
                {
                    textureColo
                }
            }
            for(int i = 0; i < pixels.Length; i++)
            {
                int newIndex = Utils.CoordToIndex(Utils.IndexToCoord(i, oldTextureSize.x), newTextureSize.x);
                textureColorArray[newIndex] = pixels[i];
            }

            /*
            Color32 clearColor = new Color32(255, 0, 0, 255);

            int topDifference = newTextureSize.x * (newTextureSize.y - oldTextureSize.y);
            int startIndex = newTextureSize.x * oldTextureSize.y; 
            for(int i = 0; i < topDifference; i++)
            {
                textureColorArray[startIndex + i] = clearColor;
            }

            int sideWidth = newTextureSize.x - oldTextureSize.x;
            int sideDifference = sideWidth * oldTextureSize.y;
            for(int i = 0; i < sideDifference; i++)
            {
                textureColorArray[Utils.CoordToIndex(Utils.IndexToCoord(i, sideWidth) + new Vector2Int(oldTextureSize.x, 0), newTextureSize.x)] = clearColor;
            }
            */
            /*
            m_MaxTextureCount = newMaxTextureCount;
            fullTexture.Apply();
        }
    */
    }
}
