using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine.Profiling;

namespace Elanetic.Tools
{

	public class SpriteAnimator : MonoBehaviour
	{
		private class SpriteAnimatorJobExecutor : MonoBehaviour
		{
			public int batchSize = 64;
			public SpriteAnimatorJob job;
			private SpriteAnimator[] m_Animators = new SpriteAnimator[512];
			private JobHandle m_JobHandle;
			private int m_AnimatorCount = 0;
			private bool m_JobIsScheduled = false;
			

			[BurstCompile]
			public struct SpriteAnimatorJob : IJobFor
			{
				public struct AnimatorInfo
				{
					public float playbackSpeed;
					public bool loop;
					public int frameCount;
				}

				public struct AnimatorResult
                {
					public float timer;
					public int frame;
					public bool isPlaying;
                }
				[ReadOnly]
				public NativeArray<AnimatorInfo> infos;
				public NativeArray<AnimatorResult> results;
				public float deltaTime;

				public void Execute(int i)
				{
					AnimatorInfo animatorInfo = infos[i];
					float currentTimer = results[i].timer;
					int currentFrame = results[i].frame;
					bool continuePlaying = results[i].isPlaying;
					currentTimer += animatorInfo.playbackSpeed * deltaTime;

					int count = (int)math.floor(currentTimer);
					if (currentTimer >= 1.0f)
					{
						//AdvanceFrames()
						if(animatorInfo.frameCount > 1)
							count %= animatorInfo.frameCount;

						if(currentFrame + count > animatorInfo.frameCount - 1)
                        {
							if(animatorInfo.loop)
                            {
								currentFrame = animatorInfo.frameCount - 1 - currentFrame;
                            }
							else
                            {
								currentFrame = animatorInfo.frameCount - 1;
								continuePlaying = false;
                            }
                        }
						else
                        {
							currentFrame += count;
                        }
						//End AdvanceFrames()

						currentTimer %= 1.0f;
					}
					else if (currentTimer < 0.0f)
					{
						//GoBackFrames()
						count = (-count) % animatorInfo.frameCount;

						if(currentFrame - count < 0)
                        {
							if(animatorInfo.loop)
                            {
								currentFrame = animatorInfo.frameCount - 1 - currentFrame;
                            }
							else
                            {
								currentFrame = 0;
								continuePlaying = false;
                            }
                        }
						else
                        {
							currentFrame -= count;
							if(currentFrame == 0 && !animatorInfo.loop)
							{
								continuePlaying = false;
                            }
                        }
						//End GoBackFrames()

						currentTimer = 1.0f + (currentTimer % 1.0f);
					}

					//Apply result
					results[i] = new AnimatorResult()
					{
						timer = currentTimer,
						frame = currentFrame,
						isPlaying = continuePlaying
					};
				}
			}

			void Awake()
			{
				job = new SpriteAnimatorJob()
				{
					infos = new NativeArray<SpriteAnimatorJob.AnimatorInfo>(512, Allocator.Persistent),
					results = new NativeArray<SpriteAnimatorJob.AnimatorResult>(512, Allocator.Persistent),
					deltaTime = Time.deltaTime
				};

				DontDestroyOnLoad(this);
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				StartCoroutine(EarlyLateUpdate());
			}

			void OnDestroy()
			{
				if (m_AnimatorCount > 0)
					Debug.LogWarning("Destroying the Sprite Animator Job Executor will probably cause issues while there are active Sprite Animators.");

				job.infos.Dispose();
				job.results.Dispose();
			}
			//Having Update as a coroutine ensures that it is called after all other objects have Updated so we can schedule the job between now and LateUpdate
			private IEnumerator EarlyLateUpdate()
            {
				while(true)
                {
					if (m_AnimatorCount > 0)
					{
						m_JobIsScheduled = true;
						job.deltaTime = Time.deltaTime;
						m_JobHandle = job.ScheduleParallel(m_AnimatorCount, batchSize, new JobHandle());
					}
					yield return null;
                }
            }

			void LateUpdate()
			{
				if (m_JobIsScheduled)
				{
					m_JobIsScheduled = false;
					Profiler.BeginSample("JOB COMPLETE");
					m_JobHandle.Complete();
					Profiler.EndSample();
					Profiler.BeginSample("JOB COMPLETE EVENT");
					int knownCount = m_AnimatorCount;
                    for (int i = 0; i < m_AnimatorCount; i++)
                    {
						m_Animators[i].OnJobComplete();
						if(knownCount != m_AnimatorCount)
                        {
							i--;
							knownCount = m_AnimatorCount;
                        }
                    }
					Profiler.EndSample();
				}
			}

			public void EnableAnimator(SpriteAnimator animator)
			{
				if (m_JobIsScheduled) Debug.LogWarning("Should not add Sprite Animator to job while it is executing.");
				animator.m_JobIndex = m_AnimatorCount;
				m_AnimatorCount++;
				if (m_AnimatorCount > job.infos.Length)
				{
					//Resize
					NativeArray<SpriteAnimatorJob.AnimatorInfo> oldArray = job.infos;
					job.infos = new NativeArray<SpriteAnimatorJob.AnimatorInfo>(job.infos.Length + 512, Allocator.Persistent);
					for (int i = 0; i < oldArray.Length; i++)
					{
						job.infos[i] = oldArray[i];
					}
					oldArray.Dispose();

					NativeArray<SpriteAnimatorJob.AnimatorResult> oldResultArray = job.results;
					job.results = new NativeArray<SpriteAnimatorJob.AnimatorResult>(job.results.Length + 512, Allocator.Persistent);
					for (int i = 0; i < oldResultArray.Length; i++)
					{
						job.results[i] = oldResultArray[i];
					}
					oldResultArray.Dispose();

					SpriteAnimator[] oldAnimatorArray = m_Animators;
					m_Animators = new SpriteAnimator[m_Animators.Length + 512];
					for (int i = 0; i < oldAnimatorArray.Length; i++)
					{
						m_Animators[i] = oldAnimatorArray[i];
					}
				}
				try
				{
					m_Animators[m_AnimatorCount - 1] = animator;
				}
				catch
                {
					Debug.LogError(m_Animators.Length + " | " + m_AnimatorCount);
					throw;
                }
				job.infos[m_AnimatorCount - 1] = new SpriteAnimatorJob.AnimatorInfo()
				{
					playbackSpeed = animator.playbackSpeed,
					loop = animator.loop,
					frameCount = animator.animation.frameCount
				};
				job.results[m_AnimatorCount - 1] = new SpriteAnimatorJob.AnimatorResult()
				{
					timer = animator.m_Timer,
					frame = animator.currentFrame,
					isPlaying = true
				};
			}

			public void DisableAnimator(SpriteAnimator animator)
			{
				if (m_JobIsScheduled) Debug.LogWarning("Should not remove Sprite Animator to job while it is executing.");
				m_AnimatorCount--;
				if (animator.m_JobIndex < m_AnimatorCount)
				{
					m_Animators[animator.m_JobIndex] = m_Animators[m_AnimatorCount];
					job.infos[animator.m_JobIndex] = job.infos[m_AnimatorCount];
					job.results[animator.m_JobIndex] = job.results[m_AnimatorCount];

					m_Animators[animator.m_JobIndex].m_JobIndex = animator.m_JobIndex;
				}
				animator.m_JobIndex = -1;
			}
		}

		static private SpriteAnimatorJobExecutor m_JobExecutor;

		public bool playOnEnable = false;
		public float playbackSpeed 
		{
			get => m_PlaybackSpeed;
			set
            {
				m_PlaybackSpeed = value;
				if (m_JobIndex > -1)
				{
					m_JobExecutor.job.infos[m_JobIndex] = new SpriteAnimatorJobExecutor.SpriteAnimatorJob.AnimatorInfo()
					{
						frameCount = m_SpriteAnimation.frames.Length,
						playbackSpeed = m_PlaybackSpeed,
						loop = m_Loop
					};
				}
            }
        }
		public bool loop 
		{
			get => m_Loop;
			set
			{
				m_Loop = value;
				if (m_JobIndex > -1)
				{
					m_JobExecutor.job.infos[m_JobIndex] = new SpriteAnimatorJobExecutor.SpriteAnimatorJob.AnimatorInfo()
					{
						frameCount = m_SpriteAnimation.frames.Length,
						playbackSpeed = m_PlaybackSpeed,
						loop = m_Loop
					};
				}
			}
        }
		public bool destroyOnFinish = false;
		public bool isPlaying { get; private set; } = false;
		public int currentFrame { get; private set; } = 0;
		public Sprite currentSprite
        {
			get
            {
				if(m_AnimationDirty)
                {
					m_CurrentSprite = m_SpriteAnimation.sprites[m_SpriteAnimation.frames[currentFrame]];
				}
				return m_CurrentSprite;
            }
        }

		public SpriteRenderer targetSpriteRenderer
        {
			get => m_SpriteRenderer;
			set
            {
				m_SpriteRenderer = value;
				m_HasSpriteRenderer = m_SpriteRenderer != null;
            }
        }
		public Image targetImage
        {
			get => m_Image;
			set
            {
				m_Image = value;
				m_HasImage = m_Image != null;
            }
        }

		public new SpriteAnimation animation
		{
			get
			{
				return m_SpriteAnimation;
			}
			set
			{
				m_SpriteAnimation = value;
				OnAnimationSet();
			}
		}

		[HideInInspector]
		public List<SpriteAnimator> shots = new List<SpriteAnimator>();

		public Action onFinishedAnimation;
		public Action onFrameChanged;

		[SerializeField]
		private SpriteAnimation m_SpriteAnimation;

		private float m_Timer = 0.0f;

		private int m_JobIndex = -1;

		[SerializeField]
		private float m_PlaybackSpeed = 1.0f;
		[SerializeField]
		private bool m_Loop = false;
		private bool m_HasSpriteRenderer = false;
		private bool m_HasImage = false;
		private SpriteRenderer m_SpriteRenderer;
		private Image m_Image;
		private Sprite m_CurrentSprite = null;

		private bool m_AnimationDirty = false;

        void Awake()
        {
            if(m_JobExecutor == null)
            {
				m_JobExecutor = new GameObject("Sprite Animator Job Object").AddComponent<SpriteAnimatorJobExecutor>();
			}
		}

        void OnEnable()
		{
			if (m_SpriteAnimation != null && m_SpriteAnimation.IsValid(false))
			{
				if (currentFrame > m_SpriteAnimation.frames.Length - 1)
				{
					currentFrame = 0;
				}
				SetFrame(currentFrame);
			}
			if (playOnEnable) Play();
		}

		void OnJobComplete()
        {
			Profiler.BeginSample("RETRIEVE RESULTS");
			m_Timer = m_JobExecutor.job.results[m_JobIndex].timer;
			int currentFrame = m_JobExecutor.job.results[m_JobIndex].frame;
			bool continuePlaying = m_JobExecutor.job.results[m_JobIndex].isPlaying;

			Profiler.EndSample();

#if UNITY_EDITOR
			//Make sure the inspector's version of playbackSpeed and loop is up to date on the next frame
			m_JobExecutor.job.infos[m_JobIndex] = new SpriteAnimatorJobExecutor.SpriteAnimatorJob.AnimatorInfo()
			{
				frameCount = m_SpriteAnimation.frames.Length,
				playbackSpeed = m_PlaybackSpeed,
				loop = m_Loop
			};
#endif

			Profiler.BeginSample("SPRITE CHANGE");
			if (currentFrame != this.currentFrame)
			{

				Profiler.BeginSample("Visibility Check");
				if (m_IsVisible)
				{
					Profiler.BeginSample("Retrieve Sprite");

					Sprite sprite = m_SpriteAnimation.sprites[m_SpriteAnimation.frames[currentFrame]];

					Profiler.EndSample();
					Profiler.BeginSample("Apply To SpriteRenderer");
					if (m_HasImage)
					{
						if (sprite == null)
							targetImage.enabled = false;
						else
							targetImage.enabled = true;
						targetImage.sprite = sprite;
					}
					if (m_HasSpriteRenderer)
						targetSpriteRenderer.sprite = sprite;

					m_AnimationDirty = false;
					Profiler.EndSample();
				}
				else
				{
					m_AnimationDirty = true;
				}
				Profiler.EndSample();

				this.currentFrame = currentFrame;
				onFrameChanged?.Invoke();
			}

			Profiler.EndSample();
			if (!continuePlaying)
			{
				Pause();
				OnFinishedAnimation();
			}
		}

		private bool m_IsVisible = false;

        private void OnBecameVisible()
        {
			m_IsVisible = true;

			if (m_AnimationDirty)
			{
				Sprite sprite = m_SpriteAnimation.sprites[m_SpriteAnimation.frames[currentFrame]];
				if (m_HasImage)
				{
					if (sprite == null)
						targetImage.enabled = false;
					else
						targetImage.enabled = true;
					targetImage.sprite = sprite;
				}
				if (m_HasSpriteRenderer)
					targetSpriteRenderer.sprite = sprite;

				m_AnimationDirty = false;
			}
		}
        private void OnBecameInvisible()
        {
			m_IsVisible = false;
        }

        #region Public Methods

        public void Play()
		{
            if (m_SpriteAnimation == null)
            {
                Debug.LogWarning("Cannot play Sprite Animator. No Sprite Animation is set.");
                return;
            }
            else if (!m_SpriteAnimation.IsValid(false))
            {
                Debug.LogWarning("Cannot play Sprite Animator. The Sprite Animation is not valid.");
                return;
            }

			if (!isPlaying)
			{
				m_JobExecutor.EnableAnimator(this);
			}

			isPlaying = true;

			if (m_Timer == 0.0f && playbackSpeed < 0.0f)
				m_Timer = 1.0f;
		}

		/// <summary>
		/// Toggles the isPlaying boolean to false which pauses the internal playback timer. Does not call OnFinishedAnimation event.
		/// </summary>
		public void Pause()
		{
			if(isPlaying)
			{
				m_JobExecutor.DisableAnimator(this);
			}
			isPlaying = false;
		}

		/// <summary>
		/// Same as SpriteAnimator.Pause but sets the current frame to the first frame, resets playback timer and calls the OnFinishedAnimation event.
		/// </summary>
		public void Stop()
		{
			if (isPlaying || (m_Timer != 0.0f && playbackSpeed >= 0.0f) || (m_Timer != 1.0f && playbackSpeed < 0.0f) || currentFrame != 0)
			{
				Pause();
				if (playbackSpeed < 0.0f)
				{
					m_Timer = 1.0f;
				}
				else
				{
					m_Timer = 0.0f;
				}
				SetFrame(0);
				OnFinishedAnimation();
			}
		}

		public void NextFrame()
		{
			AdvanceFrames(1);
		}

		public void PreviousFrame()
		{
			GoBackFrames(1);
		}

		public void AdvanceFrames(int count)
		{
			if (count < 0)
			{
				Debug.LogError("Paramater count cannot be negative.");
				return;
			}

			if (m_SpriteAnimation.frames.Length > 1)
				count %= m_SpriteAnimation.frames.Length;

			if (currentFrame + count > m_SpriteAnimation.frames.Length - 1)
			{
				if (loop)
				{
					count = m_SpriteAnimation.frames.Length - 1 - currentFrame;
					SetFrame(count);
				}
				else
				{
					SetFrame(m_SpriteAnimation.frames.Length - 1);
					Pause();
					OnFinishedAnimation();
				}
			}
			else
			{
				SetFrame(currentFrame + count);
				/*if(currentFrame == sprites.Length - 1 && !loop)
				{
					Pause();
					OnFinishedAnimation();
				}*/
			}
		}

		public void GoBackFrames(int count)
		{
			if (count < 0)
			{
				Debug.LogError("Paramater count cannot be negative.");
				return;
			}

			count %= m_SpriteAnimation.frames.Length;

			if (currentFrame - count < 0)
			{
				if (loop)
				{
					count = m_SpriteAnimation.frames.Length - 1 - currentFrame;
					SetFrame(count);
				}
				else
				{
					SetFrame(0);
					Pause();
					OnFinishedAnimation();
				}
			}
			else
			{
				SetFrame(currentFrame - count);
				if (currentFrame == 0 && !loop)
				{
					Pause();
					OnFinishedAnimation();
				}
			}
		}

		public void SetFrame(int frameIndex)
		{
			if (m_SpriteAnimation.frames.Length == 0)
			{
				Debug.LogError("Cannot animate to the set frame. There are not sprites in the sprite list.");
				return;
			}
			else if (m_SpriteAnimation == null)
			{
				Debug.LogError("Cannot animate to the set frame. No Sprite Animation is set.");
				return;
			}
			//Too much of a performance overhead to check SpriteAnimation.IsValid() every time we set the frame.

			if (frameIndex < 0 || frameIndex > m_SpriteAnimation.frames.Length - 1)
			{
				Debug.LogWarning("Parameter frameIndex is out of range. Min: 0  Max: " + (m_SpriteAnimation.frames.Length - 1) + " Inputted parameter: " + frameIndex);
				return;
			}

			Sprite sprite = m_SpriteAnimation.sprites[m_SpriteAnimation.frames[frameIndex]];

			if (m_HasImage)
			{
				if (sprite == null)
					targetImage.enabled = false;
				else
					targetImage.enabled = true;
				targetImage.sprite = sprite;
			}

			if (m_HasSpriteRenderer)
			{
				targetSpriteRenderer.sprite = sprite;
			}

			currentFrame = frameIndex;
			onFrameChanged?.Invoke();
		}

		public void SetAnimationTimeNormalized(float normalizedTime)
		{
			if (m_SpriteAnimation == null)
				return;

			normalizedTime = Mathf.Clamp01(normalizedTime);

			SetFrame(Mathf.FloorToInt(m_SpriteAnimation.frames.Length * normalizedTime));
			m_Timer = (m_SpriteAnimation.frames.Length * normalizedTime) % 1.0f;
			if(m_JobIndex > -1)
            {
				m_JobExecutor.job.results[m_JobIndex] = new SpriteAnimatorJobExecutor.SpriteAnimatorJob.AnimatorResult()
				{
					timer = m_Timer,
					frame = currentFrame,
					isPlaying = this.isPlaying
				};
            }
		}

		public float GetAnimationTimeNormalized()
		{
			float timePerFrame = 1.0f / m_SpriteAnimation.frameCount;
			return (currentFrame * timePerFrame) + (m_Timer * timePerFrame);
		}

		#endregion Public Methods

		#region Private Methods

		private void OnFinishedAnimation()
		{
			onFinishedAnimation?.Invoke();
			if (destroyOnFinish)
				Destroy(gameObject);
		}

		private void OnAnimationSet()
		{
			if (m_SpriteAnimation == null || !m_SpriteAnimation.IsValid(false))
			{
				Stop();
				if (m_HasImage)
				{
					targetImage.enabled = false;
					targetImage.sprite = null;
				}

				if (m_HasSpriteRenderer)
				{
					targetSpriteRenderer.sprite = null;
				}
			}
			else
			{

				if (m_JobIndex > -1)
				{
					m_JobExecutor.job.infos[m_JobIndex] = new SpriteAnimatorJobExecutor.SpriteAnimatorJob.AnimatorInfo()
					{
						frameCount = m_SpriteAnimation.frames.Length,
						playbackSpeed = m_PlaybackSpeed,
						loop = m_Loop
					};
				}
				if (currentFrame > m_SpriteAnimation.frames.Length - 1)
				{
					SetFrame(m_SpriteAnimation.frames.Length - 1);
				}
				else
				{
					SetFrame(currentFrame);
				}
			}
		}

		#endregion Private Methods
	}
}