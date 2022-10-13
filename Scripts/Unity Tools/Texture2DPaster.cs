#if UNITY_COLLECTIONS //Defined in Elanetic.Tools assembly definition file
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Profiling;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Elanetic.Tools.Unity
{
    public class Texture2DPaster
    {
        public Texture2D resultTexture { get; private set; }

        private BoundsInt2D m_BaseRect;
        private int m_JobCount = 0;
        private JobHandle[] m_JobHandles = new JobHandle[8];
        private BoundsInt2D[] m_Rects = new BoundsInt2D[8];

        [BurstCompile]
        private struct TexturePasterJob : IJobFor
        {
            [NativeDisableContainerSafetyRestriction]
            [NativeDisableParallelForRestriction]
            [WriteOnly]
            public NativeArray<int> targetTexture;
            [ReadOnly]
            public int2 targetTextureSize;
            [NativeDisableContainerSafetyRestriction]
            [NativeDisableParallelForRestriction]
            [ReadOnly]
            public NativeArray<int> pastingTexture;
            [ReadOnly]
            public int2 pastingTextureSize;
            [ReadOnly]
            public int2 targetPosition;
            [ReadOnly]
            public int targetWidth;
            [ReadOnly]
            public int3 clampedSides;
            [ReadOnly]
            public int receiveWidth;

            [SkipLocalsInit]
            public void Execute(int i)
            {
                int2 targetCoord = IndexToCoord(i, targetWidth) + targetPosition;
                int2 receiveCoord = IndexToCoord(i, receiveWidth);
                receiveCoord = new int2(receiveCoord.x + clampedSides.x, receiveCoord.y + clampedSides.z);

                int targetIndex = CoordToIndex(targetCoord, targetTextureSize.x);
                int receiveIndex = CoordToIndex(receiveCoord, pastingTextureSize.x);

                //RGBA bytes
                targetTexture[targetIndex] = pastingTexture[receiveIndex];
            }

            [return: AssumeRange(0, 16384)]
            static int CoordToIndex(int2 coord, int width)
            {
                return (coord.y * width) + coord.x;
            }

            private int2 IndexToCoord(int index, int width)
            {
                return new int2(index % width, index / width);
            }
        }

        [BurstCompile]
        private struct TexturePasterJobRotation1 : IJobFor
        {
            [NativeDisableContainerSafetyRestriction]
            [NativeDisableParallelForRestriction]
            [WriteOnly]
            public NativeArray<byte> targetTexture;
            [ReadOnly]
            public int2 targetTextureSize;
            [ReadOnly]
            public NativeArray<byte> pastingTexture;
            [ReadOnly]
            public int2 pastingTextureSize;
            [ReadOnly]
            public int2 targetPosition;
            [ReadOnly]
            public int targetWidth;
            [ReadOnly]
            public int3 clampedSides;

            public void Execute(int i)
            {
                int2 targetCoord = IndexToCoord(i, targetWidth) + targetPosition;

                int2 receiveCoord = IndexToCoord90(i, pastingTextureSize.y - clampedSides.x - clampedSides.y);
                receiveCoord.x += clampedSides.z;
                receiveCoord.y += clampedSides.y;


                int targetIndex = CoordToIndex(targetCoord, targetTextureSize.x) * 4;
                int receiveIndex = CoordToIndex(receiveCoord, pastingTextureSize.x) * 4;

                //RGBA bytes
                targetTexture[targetIndex] = pastingTexture[receiveIndex];
                targetTexture[targetIndex + 1] = pastingTexture[receiveIndex + 1];
                targetTexture[targetIndex + 2] = pastingTexture[receiveIndex + 2];
                targetTexture[targetIndex + 3] = pastingTexture[receiveIndex + 3];
            }

            private int CoordToIndex(int2 coord, int width)
            {
                return (coord.y * width) + coord.x;
            }

            private int2 IndexToCoord90(int index, int height)
            {
                return new int2(index / height, (height - 1) - (index % height));
            }

            private int2 IndexToCoord(int index, int width)
            {
                return new int2(index % width, index / width);
            }
        }

        [BurstCompile]
        private struct TexturePasterJobRotation2 : IJobFor
        {
            [NativeDisableContainerSafetyRestriction]
            [NativeDisableParallelForRestriction]
            [WriteOnly]
            public NativeArray<byte> targetTexture;
            [ReadOnly]
            public int2 targetTextureSize;
            [ReadOnly]
            public NativeArray<byte> pastingTexture;
            [ReadOnly]
            public int2 pastingTextureSize;
            [ReadOnly]
            public int2 targetPosition;
            [ReadOnly]
            public int targetWidth;
            [ReadOnly]
            public int3 clampedSides;
            public void Execute(int i)
            {
                int2 targetCoord = IndexToCoord(i, targetWidth) + targetPosition;

                int2 receiveCoord = IndexToCoord180(i, pastingTextureSize.x - clampedSides.x - clampedSides.y, pastingTextureSize.y);
                receiveCoord.x += clampedSides.y;
                receiveCoord.y -= clampedSides.z;

                int targetIndex = CoordToIndex(targetCoord, targetTextureSize.x) * 4;
                int receiveIndex = CoordToIndex(receiveCoord, pastingTextureSize.x) * 4;

                //RGBA bytes
                targetTexture[targetIndex] = pastingTexture[receiveIndex];
                targetTexture[targetIndex + 1] = pastingTexture[receiveIndex + 1];
                targetTexture[targetIndex + 2] = pastingTexture[receiveIndex + 2];
                targetTexture[targetIndex + 3] = pastingTexture[receiveIndex + 3];
            }

            private int CoordToIndex(int2 coord, int width)
            {
                return (coord.y * width) + coord.x;
            }

            private int2 IndexToCoord180(int index, int width, int height)
            {
                return new int2((width - 1) - (index % width), (height - 1) - (index / width));
            }

            private int2 IndexToCoord(int index, int width)
            {
                return new int2(index % width, index / width);
            }
        }

        [BurstCompile]
        private struct TexturePasterJobRotation3 : IJobFor
        {
            [NativeDisableContainerSafetyRestriction]
            [NativeDisableParallelForRestriction]
            [WriteOnly]
            public NativeArray<byte> targetTexture;
            [ReadOnly]
            public int2 targetTextureSize;
            [ReadOnly]
            public NativeArray<byte> pastingTexture;
            [ReadOnly]
            public int2 pastingTextureSize;
            [ReadOnly]
            public int2 targetPosition;
            [ReadOnly]
            public int targetWidth;
            [ReadOnly]
            public int3 clampedSides;

            public void Execute(int i)
            {
                int2 targetCoord = IndexToCoord(i, targetWidth) + targetPosition;
                int2 receiveCoord = IndexToCoord270(i, pastingTextureSize.x, pastingTextureSize.y - clampedSides.x - clampedSides.y);
                receiveCoord.x -= clampedSides.z;
                receiveCoord.y += clampedSides.x;


                int targetIndex = CoordToIndex(targetCoord, targetTextureSize.x) * 4;
                int receiveIndex = CoordToIndex(receiveCoord, pastingTextureSize.x) * 4;

                //RGBA bytes
                targetTexture[targetIndex] = pastingTexture[receiveIndex];
                targetTexture[targetIndex + 1] = pastingTexture[receiveIndex + 1];
                targetTexture[targetIndex + 2] = pastingTexture[receiveIndex + 2];
                targetTexture[targetIndex + 3] = pastingTexture[receiveIndex + 3];
            }

            private int CoordToIndex(int2 coord, int width)
            {
                return (coord.y * width) + coord.x;
            }

            private int2 IndexToCoord270(int index, int width, int height)
            {
                return new int2((width - 1) - (index / height), index % height);
            }

            private int2 IndexToCoord(int index, int width)
            {
                return new int2(index % width, index / width);
            }
        }

        [BurstCompile]
        private struct TexturePasterJobColor : IJobFor
        {
            [NativeDisableContainerSafetyRestriction]
            [NativeDisableParallelForRestriction]
            [WriteOnly]
            public NativeArray<int> targetTexture;
            [ReadOnly]
            public int targetTextureWidth;
            [ReadOnly]
            public int2 targetPosition;
            [ReadOnly]
            public int targetWidth;
            [ReadOnly]
            public int color;

            [SkipLocalsInit]
            public void Execute(int i)
            {
                int2 targetCoord = IndexToCoord(i, targetWidth) + targetPosition;
                int targetIndex = CoordToIndex(targetCoord, targetTextureWidth);

                //RGBA bytes
                targetTexture[targetIndex] = color;
            }

            [return: AssumeRange(0, 16384)]
            static int CoordToIndex(int2 coord, int width)
            {
                return (coord.y * width) + coord.x;
            }

            private int2 IndexToCoord(int index, int width)
            {
                return new int2(index % width, index / width);
            }
        }

        public Texture2DPaster(Texture2D baseTexture)
        {
#if SAFE_EXECUTION
            if(baseTexture == null)
                throw new ArgumentNullException("Inputted base texture is null.");
            if(baseTexture.format != TextureFormat.RGBA32)
                Debug.LogWarning("Inputted texture format is '" + baseTexture.format.ToString() + "'. Any format other than RGBA32 will most likely result in exceptions or corrupt data.");
#endif
            resultTexture = baseTexture;
            m_BaseRect = new BoundsInt2D(0, 0, resultTexture.width, resultTexture.height);
        }

        //Size of blank(alpha) texture
        public Texture2DPaster(Vector2Int textureSize)
        {
#if SAFE_EXECUTION
            if(textureSize.x <= 0 || textureSize.y <= 0)
                throw new ArgumentException("Inputted texture size must have a width and height of more than zero.");
#endif
            resultTexture = new Texture2D(textureSize.x, textureSize.y);
            m_BaseRect = new BoundsInt2D(0, 0, resultTexture.width, resultTexture.height);
        }

        public void AddColor(Color color, Vector2Int position, Vector2Int size, bool checkForOverlap = true)
        {
            if(m_JobCount == m_JobHandles.Length)
            {
                for(int i = 0; i < m_JobCount; i++)
                {
                    if(m_JobHandles[i].IsCompleted)
                    {
                        m_JobCount--;
                        continue;
                    }
                    else
                    {
                        int dif = m_JobHandles.Length - m_JobCount;
                        for(int h = 0; h < m_JobCount; h++)
                        {
                            int oldIndex = dif + h;
                            m_JobHandles[h] = m_JobHandles[oldIndex];
                            m_Rects[h] = m_Rects[oldIndex];
                        }
                        break;
                    }
                }
                if(m_JobCount == m_JobHandles.Length)
                {
                    Array.Resize<JobHandle>(ref m_JobHandles, m_JobHandles.Length * 2);
                    Array.Resize<BoundsInt2D>(ref m_Rects, m_JobHandles.Length);
                }
            }

            BoundsInt2D rect = new BoundsInt2D(position.x, position.y, size.x, size.y);

#if SAFE_EXECUTION
            if(!rect.Overlaps(m_BaseRect))
                throw new ArgumentException("Inputted texture position does not overlap with the base texture.");
#endif

            int targetWidth = Mathf.Max(0, Mathf.Min(rect.max.x, m_BaseRect.max.x) - Mathf.Max(rect.min.x, m_BaseRect.min.x) + 1);

            JobHandle jobHandle = new JobHandle();
            if(checkForOverlap)
            {
                for(int h = m_JobHandles.Length - 1; h >= 0; h--)
                {
                    if(rect.Overlaps(m_Rects[h]))
                    {
                        jobHandle = m_JobHandles[h];
                        break;
                    }
                }
            }
            Color32 color32 = (Color32)color;
            TexturePasterJobColor job = new TexturePasterJobColor()
            {
                targetTexture = resultTexture.GetRawTextureData<int>(),
                targetTextureWidth = resultTexture.width,
                targetPosition = new int2(Mathf.Max(0, position.x), Mathf.Max(0, position.y)),
                targetWidth = targetWidth,
                color = (int)((color32.r << 0) | (color32.g << 8) | (color32.b << 16) | (color32.a << 24))
            };
            m_JobHandles[m_JobCount] = job.ScheduleParallel(m_BaseRect.GetOverlapCount(rect), 64, jobHandle);

            m_Rects[m_JobCount] = rect;
            m_JobCount++;
        }

        public void AddColor(Color color, bool checkForOverlap=true) => AddColor(color, Vector2Int.zero, new Vector2Int(resultTexture.width,resultTexture.height), checkForOverlap);

        //Rotation: 0 = 0 degrees, 1 = 90 degrees, 2 = 180 degrees 3 = 270 degrees, etc
        //checkForOverlap: If you are sure that any textures added to this texture paster don't overlap eachother, save on performance by setting to false.
        //If incorrectly set to false errors/race conditions will occur!
        public void AddTexture(Texture2D texture, Vector2Int texturePosition, int rotation=0, bool checkForOverlap=true)
        {
            Profiler.BeginSample("AddTexture");
            Profiler.BeginSample("Setup");
#if SAFE_EXECUTION
            if(texture == null)
                throw new ArgumentNullException("Inputted texture is null.");
            if(texture.format != TextureFormat.RGBA32)
                Debug.LogWarning("Inputted texture format is '" + texture.format.ToString() + "'. Any format other than RGBA32 will most likely result in exceptions or corrupt data.");
#endif
            Profiler.BeginSample("Resize Arrays");
            if(m_JobCount == m_JobHandles.Length)
            {
                for(int i = 0; i < m_JobCount; i++)
                {
                    if(m_JobHandles[i].IsCompleted)
                    {
                        m_JobCount--;
                        continue;
                    }
                    else
                    {
                        int dif = m_JobHandles.Length - m_JobCount;
                        for(int h = 0; h < m_JobCount; h++)
                        {
                            int oldIndex = dif + h;
                            m_JobHandles[h] = m_JobHandles[oldIndex];
                            m_Rects[h] = m_Rects[oldIndex];
                        }
                        break;
                    }
                }
                if(m_JobCount == m_JobHandles.Length)
                {
                    Array.Resize<JobHandle>(ref m_JobHandles, m_JobHandles.Length * 2);
                    Array.Resize<BoundsInt2D>(ref m_Rects, m_JobHandles.Length);
                }
            }
            Profiler.EndSample();
            Profiler.BeginSample("Rect and Rotation Setup");

            BoundsInt2D rect = new BoundsInt2D(texturePosition.x, texturePosition.y, texture.width, texture.height);

            rotation %= 4;

            if(rotation < 0)
                rotation += 4;

            if(rotation % 2 != 0)
            {
                rect = new BoundsInt2D(texturePosition.x, texturePosition.y, texture.height, texture.width);
            }

#if SAFE_EXECUTION
            if(!rect.Overlaps(m_BaseRect))
                throw new ArgumentException("Inputted texture position does not overlap with the base texture.");
#endif

            int targetWidth = Mathf.Max(0, Mathf.Min(rect.max.x, m_BaseRect.max.x) - Mathf.Max(rect.min.x, m_BaseRect.min.x) + 1);

            int3 clampedSides = int3.zero;

            if(rect.min.x < m_BaseRect.min.x)
            {
                clampedSides.x = m_BaseRect.min.x - rect.min.x;
            }

            if(rect.max.x > m_BaseRect.max.x)
            {
                clampedSides.y = rect.max.x - m_BaseRect.max.x;
            }

            if(rect.min.y < m_BaseRect.min.y)
            {
                clampedSides.z = m_BaseRect.min.y - rect.min.y;
            }
            Profiler.EndSample();
            Profiler.BeginSample("Check For Overlap");
            JobHandle jobHandle = new JobHandle();
            if(checkForOverlap)
            {
                for(int h = m_JobHandles.Length - 1; h >= 0; h--)
                {
                    if(rect.Overlaps(m_Rects[h]))
                    {
                        jobHandle = m_JobHandles[h];
                        break;
                    }
                }
            }
            Profiler.EndSample();

            Profiler.EndSample();
            Profiler.BeginSample("Create Job and Schedule");
            switch(rotation)
            {
                case 0:
                    TexturePasterJob job0 = new TexturePasterJob()
                    {
                        targetTexture = resultTexture.GetRawTextureData<int>(),
                        targetTextureSize = new int2(resultTexture.width, resultTexture.height),
                        pastingTexture = texture.GetRawTextureData<int>(),
                        pastingTextureSize = new int2(texture.width, texture.height),
                        targetPosition = new int2(Mathf.Max(0, texturePosition.x), Mathf.Max(0, texturePosition.y)),
                        targetWidth = targetWidth,
                        clampedSides = clampedSides,
                        receiveWidth = texture.width - clampedSides.x - clampedSides.y
                    };
                    m_JobHandles[m_JobCount] = job0.ScheduleParallel(m_BaseRect.GetOverlapCount(rect), 32, jobHandle);
                    break;
                case 1:
                    TexturePasterJobRotation1 job1 = new TexturePasterJobRotation1()
                    {
                        targetTexture = resultTexture.GetRawTextureData<byte>(),
                        targetTextureSize = new int2(resultTexture.width, resultTexture.height),
                        pastingTexture = texture.GetRawTextureData<byte>(),
                        pastingTextureSize = new int2(texture.width, texture.height),
                        targetPosition = new int2(Mathf.Max(0, texturePosition.x), Mathf.Max(0, texturePosition.y)),
                        targetWidth = targetWidth,
                        clampedSides = clampedSides,
                    };
                    m_JobHandles[m_JobCount] = job1.ScheduleParallel(m_BaseRect.GetOverlapCount(rect), 32, jobHandle);
                    break;
                case 2:
                    TexturePasterJobRotation2 job2 = new TexturePasterJobRotation2()
                    {
                        targetTexture = resultTexture.GetRawTextureData<byte>(),
                        targetTextureSize = new int2(resultTexture.width, resultTexture.height),
                        pastingTexture = texture.GetRawTextureData<byte>(),
                        pastingTextureSize = new int2(texture.width, texture.height),
                        targetPosition = new int2(Mathf.Max(0, texturePosition.x), Mathf.Max(0, texturePosition.y)),
                        targetWidth = targetWidth,
                        clampedSides = clampedSides,
                    };
                    m_JobHandles[m_JobCount] = job2.ScheduleParallel(m_BaseRect.GetOverlapCount(rect), 32, jobHandle);
                    break;
                case 3:
                    TexturePasterJobRotation3 job3 = new TexturePasterJobRotation3()
                    {
                        targetTexture = resultTexture.GetRawTextureData<byte>(),
                        targetTextureSize = new int2(resultTexture.width, resultTexture.height),
                        pastingTexture = texture.GetRawTextureData<byte>(),
                        pastingTextureSize = new int2(texture.width, texture.height),
                        targetPosition = new int2(Mathf.Max(0, texturePosition.x), Mathf.Max(0, texturePosition.y)),
                        targetWidth = targetWidth,
                        clampedSides = clampedSides,
                    };
                    m_JobHandles[m_JobCount] = job3.ScheduleParallel(m_BaseRect.GetOverlapCount(rect), 32, jobHandle);
                    break;
            }
            m_Rects[m_JobCount] = rect;
            m_JobCount++;
            Profiler.EndSample();
            Profiler.EndSample();
        }

        public void AddTexture(Texture2D texture, Vector2Int texturePosition, bool checkForOverlap=true) => AddTexture(texture, texturePosition, 0, checkForOverlap);

        public void AddTexture(Texture2D texture, int rotation=0, bool checkForOverlap=true) => AddTexture(texture, Vector2Int.zero, rotation, checkForOverlap);

        public void ForceComplete()
        {
#if SAFE_EXECUTION
            if(m_JobCount == 0)
                throw new InvalidOperationException("Tried to force Texture2D Paster but there is no pasting textures. Make sure to add textures using AddTexture() and then ForceComplete.");
#endif
            for(int h = 0; h < m_JobCount; h++)
            {
                m_JobHandles[h].Complete();
            }

            m_JobCount = 0;
        }
    }
}
#endif