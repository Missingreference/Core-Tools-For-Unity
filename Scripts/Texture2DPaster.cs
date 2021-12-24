#if UNITY_COLLECTIONS //Defined in Elanetic.Tools assembly definition file
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Elanetic.Tools
{
    public class Texture2DPaster
    {
        public bool isExecuting { get; private set; } = false;
        public Texture2D resultTexture { get; private set; }

        private BoundsInt2D m_BaseRect;
        private List<BoundsInt2D> m_Rects = new List<BoundsInt2D>();
        private List<IJobFor> m_Jobs = new List<IJobFor>(5);
        private List<int> m_JobRotations = new List<int>();
        private JobHandle[] m_JobHandles;
        private bool m_Completed = false;

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

        //Rotation: 0 = 0 degrees, 1 = 90 degrees, 2 = 180 degrees 3 = 270 degrees, etc
        public void AddTexture(Texture2D texture, Vector2Int texturePosition, int rotation=0)
        {
#if SAFE_EXECUTION
            if(texture == null)
                throw new ArgumentNullException("Inputted texture is null.");
            if(texture.format != TextureFormat.RGBA32)
                Debug.LogWarning("Inputted texture format is '" + texture.format.ToString() + "'. Any format other than RGBA32 will most likely result in exceptions or corrupt data.");
#endif
            BoundsInt2D rect = new BoundsInt2D(texturePosition.x, texturePosition.y, texture.width, texture.height);;

            rotation %= 4;

            if(rotation < 0)
                rotation += 4;

            if(rotation % 2 != 0)
            {
                rect = new BoundsInt2D(texturePosition.x, texturePosition.y, texture.width, texture.height);
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
                    m_Jobs.Add(job0);
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
                    m_Jobs.Add(job1);
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
                    m_Jobs.Add(job2);
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
                    m_Jobs.Add(job3);
                    break;
            }


            m_Rects.Add(rect);
            m_JobRotations.Add(rotation);
        }

        public void AddTexture(Texture2D texture, int rotation=0) => AddTexture(texture, Vector2Int.zero, rotation);

        //Since this relies on the Job System. Make sure to call this as early as possible such as Update for maximum performance gain. Completion is scheduled for LateUpdate.
        public void Execute()
        {
#if SAFE_EXECUTION
            if(m_Jobs.Count == 0)
                throw new InvalidOperationException("Tried to execute Texture2D Paster but there is no pasting textures. Make sure to add textures using AddTexture().");
#endif
            m_JobHandles = new JobHandle[m_Jobs.Count];

            for(int i = 0; i < m_Jobs.Count; i++)
            {
                JobHandle jobHandle = new JobHandle();
                for(int h = i - 1; h >= 0; h--)
                {
                    if(m_Rects[i].Overlaps(m_Rects[h]))
                    {
                        jobHandle = m_JobHandles[h];
                        break;
                    }
                }

                int rotation = m_JobRotations[i];
                switch(rotation)
                {
                    case 0:
                        m_JobHandles[i] = ((TexturePasterJob)m_Jobs[i]).ScheduleParallel(m_BaseRect.GetOverlapCount(m_Rects[i]), 32, jobHandle);
                        break;
                    case 1:
                        m_JobHandles[i] = ((TexturePasterJobRotation1)m_Jobs[i]).ScheduleParallel(m_BaseRect.GetOverlapCount(m_Rects[i]), 32, jobHandle);
                        break;
                    case 2:
                        m_JobHandles[i] = ((TexturePasterJobRotation2)m_Jobs[i]).ScheduleParallel(m_BaseRect.GetOverlapCount(m_Rects[i]), 32, jobHandle);
                        break;
                    case 3:
                        m_JobHandles[i] = ((TexturePasterJobRotation3)m_Jobs[i]).ScheduleParallel(m_BaseRect.GetOverlapCount(m_Rects[i]), 32, jobHandle);
                        break;
                }
            }

            isExecuting = true;
        }

        public void ForceComplete()
        {
#if SAFE_EXECUTION
            if(m_Jobs.Count == 0)
                throw new InvalidOperationException("Tried to force Texture2D Paster but there is no pasting textures. Make sure to add textures using AddTexture() and then Execute().");
            if(m_JobHandles == null)
                throw new InvalidOperationException("Tried to force Texture2D Paster but it has not been executed. Make sure to call Execute() before trying to finish the operation.");
#endif

            if(m_Completed) return;
            m_Completed = true;
            
            for(int h = 0; h < m_JobHandles.Length; h++)
            {
                m_JobHandles[h].Complete();
            }

            isExecuting = false;
        }
    }
}
#endif