using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Isaac.Tools
{

	public class SpriteAnimator : MonoBehaviour
	{

		public bool playOnEnable = false;
		public float playbackSpeed = 1.0f;
		public bool loop = false;
		public bool destroyOnFinish = false;
		public bool isPlaying { get; private set; } = false;
		public int currentFrame { get; private set; } = 0;

		public SpriteRenderer spriteRenderer
		{
			get
			{
				if(m_SpriteRenderer == null)
				{
					m_SpriteRenderer = GetComponent<SpriteRenderer>();
				}
				return m_SpriteRenderer;
			}
		}

		public Image image
		{
			get
			{
				if(m_Image == null)
				{
					m_Image = GetComponent<Image>();
				}
				return m_Image;
			}
		}

		public Sprite[] sprites
		{
			get
			{
				return m_Sprites;
			}
			set
			{
				m_Sprites = value;
				OnSpritesSet();
			}
		}

		[HideInInspector]
		public List<SpriteAnimator> shots = new List<SpriteAnimator>();

		public Action onFinishedAnimation;

		[SerializeField]
		private Sprite[] m_Sprites = new Sprite[0];
		private SpriteRenderer m_SpriteRenderer = null;
		private Image m_Image = null;

		private float m_Timer = 0.0f;

		void OnEnable()
		{
			m_Image = GetComponent<Image>();
			m_SpriteRenderer = GetComponent<SpriteRenderer>();
			if(currentFrame > sprites.Length - 1)
			{
				currentFrame = 0;
			}
			if(sprites.Length > 0)
				SetFrame(currentFrame);
			if(playOnEnable) Play();
		}

		void Update()
		{
			if(!isPlaying) return;

			if(m_Image == null && m_SpriteRenderer == null)
			{
				m_Image = GetComponent<Image>();
				m_SpriteRenderer = GetComponent<SpriteRenderer>();
				if(m_Image == null && m_SpriteRenderer == null)
				{
					enabled = false;
					Debug.LogError("No Sprite Renderer or Image component is attached.");
					return;
				}
			}

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
			if(sprites.Length == 0)
			{
				Debug.LogWarning("Cannot play Sprite Animator. No Sprites are set.");
				return;
			}

			isPlaying = true;
			if(m_SpriteRenderer != null)
				m_SpriteRenderer.enabled = true;

			if(m_Timer == 0.0f && playbackSpeed < 0.0f)
				m_Timer = 1.0f;
			if(m_Image != null)
				m_Image.enabled = true;
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

			if(sprites.Length > 1)
				count %= sprites.Length;

			if(currentFrame + count > sprites.Length - 1)
			{
				if(loop)
				{
					count = sprites.Length - 1 - currentFrame;
					SetFrame(count);
				}
				else
				{
					SetFrame(sprites.Length - 1);
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

			count %= sprites.Length;

			if(currentFrame - count < 0)
			{
				if(loop)
				{
					count = sprites.Length - 1 - currentFrame;
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
			if(sprites.Length == 0)
			{
				Debug.LogError("Cannot animate to the set frame. There are not sprites in the sprite list.");
				return;
			}

			if(frameIndex < 0 || frameIndex > sprites.Length - 1)
			{
				Debug.LogWarning("Parameter frameIndex is out of range. Min: 0  Max: " + (sprites.Length - 1) + " Inputted parameter: " + frameIndex);
				return;
			}

			if(m_Image == null && m_SpriteRenderer == null)
			{
				m_Image = GetComponent<Image>();
				m_SpriteRenderer = GetComponent<SpriteRenderer>();
				if(m_Image == null && m_SpriteRenderer == null)
				{
					enabled = false;
					Debug.LogError("No Sprite Renderer or Image component is attached.");
				}
			}

			Sprite sprite = sprites[frameIndex];

			if(m_Image != null)
			{
				if(sprite == null)
					m_Image.enabled = false;
				else
					m_Image.enabled = true;
				m_Image.sprite = sprite;
			}

			if(m_SpriteRenderer != null)
			{
				m_SpriteRenderer.sprite = sprite;
			}

			currentFrame = frameIndex;
		}

		public void SetAnimationTimeNormalized(float normalizedTime)
		{
			if(sprites.Length == 0)
				return;

			normalizedTime = Mathf.Clamp01(normalizedTime);

			SetFrame(Mathf.FloorToInt(sprites.Length * normalizedTime));
			m_Timer = (sprites.Length * normalizedTime) % 1.0f;

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

		private void OnSpritesSet()
		{
			if(m_Sprites == null)
			{
				m_Sprites = new Sprite[0];
			}

			if(m_Sprites.Length == 0)
			{
				Stop();
				if(m_Image == null && m_SpriteRenderer == null)
				{
					m_Image = GetComponent<Image>();
					m_SpriteRenderer = GetComponent<SpriteRenderer>();
				}

				if(m_Image != null)
				{
					m_Image.enabled = false;
					m_Image.sprite = null;
				}

				if(m_SpriteRenderer != null)
				{
					m_SpriteRenderer.sprite = null;
				}
			}
			else
			{
				if(currentFrame > m_Sprites.Length - 1)
				{
					SetFrame(m_Sprites.Length - 1);
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