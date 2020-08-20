using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Elanetic.Tools
{

	public class SpriteAnimator : MonoBehaviour
	{

		public bool playOnEnable = false;
		public float playbackSpeed = 1.0f;
		public bool loop = false;
		public bool destroyOnFinish = false;
		public bool isPlaying { get; private set; } = false;
		public int currentFrame { get; private set; } = 0;
		public Sprite currentSprite { get; private set; } = null;

		public SpriteRenderer targetSpriteRenderer { get; set; }
		public Image targetImage { get; set; }

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

		void OnEnable()
		{
			if(m_SpriteAnimation != null && m_SpriteAnimation.IsValid(false))
			{
				if (currentFrame > m_SpriteAnimation.frames.Length - 1)
				{
					currentFrame = 0;
				}
				SetFrame(currentFrame);
			}
			if(playOnEnable) Play();
		}

		void Update()
		{
			if(!isPlaying) return;

			m_Timer += playbackSpeed * Time.deltaTime;

			if(m_Timer >= 1.0f)
			{
				AdvanceFrames(Mathf.FloorToInt(m_Timer));
				m_Timer %= 1.0f;
			}

			if(m_Timer < 0.0f)
			{
				GoBackFrames(-Mathf.FloorToInt(m_Timer));
				m_Timer = 1.0f + (m_Timer % 1.0f);
			}
		}

		#region Public Methods

		public void Play()
		{
			if(m_SpriteAnimation == null)
			{
				Debug.LogWarning("Cannot play Sprite Animator. No Sprite Animation is set.");
				return;
			}
			else if(!m_SpriteAnimation.IsValid(false))
			{
				Debug.LogWarning("Cannot play Sprite Animator. The Sprite Animation is not valid.");
				return;
			}

			isPlaying = true;

			if(m_Timer == 0.0f && playbackSpeed < 0.0f)
				m_Timer = 1.0f;
		}

		/// <summary>
		/// Toggles the isPlaying boolean to false which pauses the internal playback timer. Does not call OnFinishedAnimation event.
		/// </summary>
		public void Pause()
		{
			isPlaying = false;
		}

		/// <summary>
		/// Same as SpriteAnimator.Pause but sets the current frame to the first frame, resets playback timer and calls the OnFinishedAnimation event.
		/// </summary>
		public void Stop()
		{
			if(isPlaying || (m_Timer != 0.0f && playbackSpeed >= 0.0f) || (m_Timer != 1.0f && playbackSpeed < 0.0f) || currentFrame != 0)
			{
				Pause();
				if(playbackSpeed < 0.0f)
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
			if(count < 0)
			{
				Debug.LogError("Paramater count cannot be negative.");
				return;
			}

			if(m_SpriteAnimation.frames.Length > 1)
				count %= m_SpriteAnimation.frames.Length;

			if (currentFrame + count > m_SpriteAnimation.frames.Length - 1)
			{
				if(loop)
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
			if(count < 0)
			{
				Debug.LogError("Paramater count cannot be negative.");
				return;
			}

			count %= m_SpriteAnimation.frames.Length;

			if(currentFrame - count < 0)
			{
				if(loop)
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
				if(currentFrame == 0 && !loop)
				{
					Pause();
					OnFinishedAnimation();
				}
			}
		}

		public void SetFrame(int frameIndex)
		{
			if(m_SpriteAnimation.frames.Length == 0)
			{
				Debug.LogError("Cannot animate to the set frame. There are not sprites in the sprite list.");
				return;
			}
			else if(m_SpriteAnimation == null)
            {
				Debug.LogError("Cannot animate to the set frame. No Sprite Animation is set.");
				return;
            }
			//Too much of a performance overhead to check SpriteAnimation.IsValid() every time we set the frame.

			if(frameIndex < 0 || frameIndex > m_SpriteAnimation.frames.Length - 1)
			{
				Debug.LogWarning("Parameter frameIndex is out of range. Min: 0  Max: " + (m_SpriteAnimation.frames.Length - 1) + " Inputted parameter: " + frameIndex);
				return;
			}

			Sprite sprite = m_SpriteAnimation.sprites[m_SpriteAnimation.frames[frameIndex]];

			if(targetImage != null)
			{
				if(sprite == null)
					targetImage.enabled = false;
				else
					targetImage.enabled = true;
				targetImage.sprite = sprite;
			}

			if(targetSpriteRenderer != null)
			{
				targetSpriteRenderer.sprite = sprite;
			}

			currentFrame = frameIndex;
			currentSprite = sprite;
			onFrameChanged?.Invoke();
		}

		public void SetAnimationTimeNormalized(float normalizedTime)
		{
			if(m_SpriteAnimation == null)
				return;

			normalizedTime = Mathf.Clamp01(normalizedTime);

			SetFrame(Mathf.FloorToInt(m_SpriteAnimation.frames.Length * normalizedTime));
			m_Timer = (m_SpriteAnimation.frames.Length * normalizedTime) % 1.0f;

		}

		//"Shoots" an instance of this gameobject with custom settings for the SpriteAnimator to be destroyed upon the animation being complete
		public void Shoot()
		{
			while(shots.Count > 0 && shots[0] == null)
			{
				shots.RemoveAt(0);
			}

			SpriteAnimator sAnimator = Instantiate(gameObject).GetComponent<SpriteAnimator>();
			shots.Add(sAnimator);
			sAnimator.transform.SetParent(transform.parent);
			sAnimator.transform.position = transform.position;
			sAnimator.transform.localScale = transform.localScale;
			sAnimator.transform.rotation = transform.rotation;
			sAnimator.playOnEnable = true;
			sAnimator.destroyOnFinish = true;
			sAnimator.gameObject.SetActive(true);
		}

		public void ClearShots()
		{
			while(shots.Count > 0)
			{
				if(shots[0] != null)
					Destroy(shots[0].gameObject);
				shots.RemoveAt(0);
			}
		}

		#endregion Public Methods

		#region Private Methods

		private void OnFinishedAnimation()
		{
			onFinishedAnimation?.Invoke();
			if(destroyOnFinish)
				Destroy(gameObject);
		}

		private void OnAnimationSet()
		{
			if(m_SpriteAnimation == null || !m_SpriteAnimation.IsValid(false))
			{
				Stop();
				if(targetImage != null)
				{
					targetImage.enabled = false;
					targetImage.sprite = null;
				}

				if(targetSpriteRenderer != null)
				{
					targetSpriteRenderer.sprite = null;
				}
			}
			else
			{
				if(currentFrame > m_SpriteAnimation.frames.Length - 1)
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