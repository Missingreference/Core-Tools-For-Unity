using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;

namespace Elanetic.Tools
{
    public class Texture2DPaster
    {
        static private Texture2DPasterCompleter completer;

        private class Texture2DPasterCompleter : MonoBehaviour
        {
            public Queue<Texture2DPaster> pasters = new Queue<Texture2DPaster>();

            void Awake()
            {
                DontDestroyOnLoad(this);
                gameObject.hideFlags = HideFlags.HideAndDontSave;
            }

            void OnDestroy()
            {
                Complete();
            }

            void LateUpdate()
            {
                Complete();
            }

            void Complete()
            {
                while(pasters.Count > 0)
                {
                    Texture2DPaster paster = pasters.Dequeue();
                    paster.ForceComplete();
                }
            }
        }

        public Action onTextureReady;
        public Texture2D resultTexture { get; private set; }
        private BoundsInt2D m_BaseRect;
        private List<BoundsInt2D> m_Rects = new List<BoundsInt2D>();
        private List<TexturePasterJob> m_Jobs = new List<TexturePasterJob>(5);
        private JobHandle[] m_JobHandles;
        private bool m_Completed = false;

        [BurstCompile]
        private struct TexturePasterJob : IJobFor
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
            public int targetRotation;
            [ReadOnly]
            public int targetWidth;
            [ReadOnly]
            public int3 clampedSides;

            public void Execute(int i)
            {
                int2 targetCoord = IndexToCoord(i, targetWidth) + targetPosition;
                int2 receiveCoord = new int2();

                if(targetRotation == 0)
                {
                    receiveCoord = IndexToCoord(i, pastingTextureSize.x - clampedSides.x - clampedSides.y);
                    receiveCoord.x += clampedSides.x;
                    receiveCoord.y += clampedSides.z;
                }
                if(targetRotation == 1)
                {
                    receiveCoord = IndexToCoord90(i, pastingTextureSize.y - clampedSides.x - clampedSides.y);
                    receiveCoord.x += clampedSides.z;
                    receiveCoord.y += clampedSides.y;
                }
                else if(targetRotation == 2)
                {
                    receiveCoord = IndexToCoord180(i, pastingTextureSize.x - clampedSides.x - clampedSides.y, pastingTextureSize.y);
                    receiveCoord.x += clampedSides.y;
                    receiveCoord.y -= clampedSides.z;
                }
                else if(targetRotation == 3)
                {
                    receiveCoord = IndexToCoord270(i, pastingTextureSize.x, pastingTextureSize.y - clampedSides.x - clampedSides.y);
                    receiveCoord.x -= clampedSides.z;
                    receiveCoord.y += clampedSides.x;
                }


                int targetIndex = CoordToIndex(targetCoord, targetTextureSize.x) * 4;
                int receiveIndex = CoordToIndex(receiveCoord, pastingTextureSize.x) * 4;

                //RGBA bytes
                targetTexture[targetIndex] = pastingTexture[receiveIndex];
                targetTexture[targetIndex+1] = pastingTexture[receiveIndex+1];
                targetTexture[targetIndex+2] = pastingTexture[receiveIndex+2];
                targetTexture[targetIndex+3] = pastingTexture[receiveIndex+3];
            }

            private int CoordToIndex(int2 coord, int width)
            {
                return (coord.y * width) + coord.x;
            }

            private int2 IndexToCoord270(int index, int width, int height)
            {
                return new int2((width - 1) - (index / height), index % height);
            }

            private int2 IndexToCoord180(int index, int width, int height)
            {
                return new int2((width - 1) - (index % width), (height - 1) - (index / width));
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
#endif
            BoundsInt2D rect = new BoundsInt2D(texturePosition.x, texturePosition.y, texture.width, texture.height);;

            TexturePasterJob job = new TexturePasterJob()
            {
                targetTexture = resultTexture.GetRawTextureData<byte>(),
                targetTextureSize = new int2(resultTexture.width, resultTexture.height),
                pastingTexture = texture.GetRawTextureData<byte>(),
                pastingTextureSize = new int2(texture.width, texture.height),
                targetPosition = new int2(Mathf.Max(0,texturePosition.x), Mathf.Max(0, texturePosition.y)),
                targetRotation = rotation % 4
            };

            if(job.targetRotation < 0)
                job.targetRotation += 4;

            if(job.targetRotation % 2 != 0)
            {
                rect = new BoundsInt2D(texturePosition.x, texturePosition.y, job.pastingTextureSize.y, job.pastingTextureSize.x);
            }

#if SAFE_EXECUTION
            if(!rect.Overlaps(m_BaseRect))
                throw new ArgumentException("Inputted texture position does not overlap with the base texture.");
#endif

            job.targetWidth = Mathf.Max(0, Mathf.Min(rect.max.x, m_BaseRect.max.x) - Mathf.Max(rect.min.x, m_BaseRect.min.x) + 1);

            if(rect.min.x < m_BaseRect.min.x)
            {
                job.clampedSides.x = m_BaseRect.min.x - rect.min.x;
            }

            if(rect.max.x > m_BaseRect.max.x)
            {
                job.clampedSides.y = rect.max.x - m_BaseRect.max.x;
            }

            if(rect.min.y < m_BaseRect.min.y)
            {
                job.clampedSides.z = m_BaseRect.min.y - rect.min.y;
            }

            m_Rects.Add(rect);
            m_Jobs.Add(job);
        }

        public void AddTexture(Texture2D texture, int rotation=0) => AddTexture(texture, Vector2Int.zero, rotation);

        //Since this relies on the Job System. Make sure to call this as early as possible such as Update for maximum performance gain. Completion is scheduled for LateUpdate.
        public void Execute()
        {
#if SAFE_EXECUTION
            if(m_Jobs.Count == 0)
                throw new InvalidOperationException("Tried to execute Texture2D Paster but there is no pasting textures. Make sure to add textures using AddTexture().");
#endif
            if(completer == null)
            {
                completer = new GameObject("Texture2D Paster Completer").AddComponent<Texture2DPasterCompleter>();
            }
            completer.pasters.Enqueue(this);

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

                m_JobHandles[i] = m_Jobs[i].ScheduleParallel(m_BaseRect.GetOverlapCount(m_Rects[i]), 32, jobHandle);
            }
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
            onTextureReady?.Invoke();
        }
    }
}