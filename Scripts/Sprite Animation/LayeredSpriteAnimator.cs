using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Elanetic.Tools
{
	public class LayeredSpriteAnimator : MonoBehaviour
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
				if (m_SpriteRenderer == null)
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
				if (m_Image == null)
				{
					m_Image = GetComponent<Image>();
				}
				return m_Image;
			}
		}

		[HideInInspector]
		public List<SpriteAnimator> shots = new List<SpriteAnimator>();

		public Action onFrameChanged;
		public Action onFinishedAnimation;

		private Shader m_RequiredShader;
		private Texture2D m_ClearTexture;
		private SpriteAnimation[] m_Animations = new SpriteAnimation[10];
		private SpriteRenderer m_SpriteRenderer = null;
		private Image m_Image = null;
		private int m_FirstNonNullIndex = -1;

		private float m_Timer = 0.0f;

		void Awake()
		{
			m_RequiredShader = Shader.Find("Sprites/Layered");
			m_ClearTexture = new Texture2D(2, 2);
			m_ClearTexture.SetPixels(Color.clear);
			m_ClearTexture.Apply();
		}

		void OnEnable()
		{
			if (m_FirstNonNullIndex == -1)
			{
				//Nothing to play
				currentFrame = 0;
			}
			else
			{
				if (currentFrame > m_Animations[m_FirstNonNullIndex].frames.Length - 1)
				{
					currentFrame = 0;
				}
				if (m_Animations[m_FirstNonNullIndex].frames.Length > 0)
					SetFrame(currentFrame);
				if (playOnEnable) Play();
			}

			SetMaterial();
		}

		void Update()
		{
			if (!isPlaying) return;

			if(spriteRenderer == null && image == null)
			{
				enabled = false;
				Debug.LogError("No Sprite Renderer or Image component is attached.");
				return;
			}

			if((m_SpriteRenderer != null && m_SpriteRenderer.material.shader != m_RequiredShader) ||
				(image != null && m_Image.material.shader != m_RequiredShader))
			{
				Debug.LogWarning("Required shader 'Sprites/Layered' was changed on the attached SpriteRenderer or Image. Fixing...");
				SetMaterial();
			}


			m_Timer += playbackSpeed * Time.deltaTime;

			if (m_Timer >= 1.0f)
			{
				AdvanceFrames(Mathf.FloorToInt(m_Timer));
				m_Timer %= 1.0f;
			}

			if (m_Timer < 0.0f)
			{
				GoBackFrames(-Mathf.FloorToInt(m_Timer));
				m_Timer = 1.0f + (m_Timer % 1.0f);
			}
		}

		#region Public Methods

		public void SetAnimation(SpriteAnimation animation, int layer)
		{
			if (layer < 0 || layer > m_Animations.Length-1) throw new ArgumentOutOfRangeException("Argument 'layer' must be from the range of 0 to 9 of an index.");

			m_Animations[layer] = animation;

			//Get new first non null index
			m_FirstNonNullIndex = -1;
			for (int i = 0; i < m_Animations.Length; i++)
			{
				if (m_Animations[i] != null)
				{
					m_FirstNonNullIndex = i;
					break;
				}
			}

			if (animation != null)
			{
				if (!animation.IsValid(false))
				{
					throw new ArgumentException("Inputted animation is not valid and will cause errors when executing.");
				}

				if (m_FirstNonNullIndex != layer)
				{
					if (animation.frames.Length != m_Animations[m_FirstNonNullIndex].frames.Length)
					{
						Debug.LogWarning("The length of the inputted animation(" + animation.frames.Length + ") will not match the lowest layer number of frames( " + m_Animations[m_FirstNonNullIndex].frames.Length + "). Some frames will be truncated or replaced with clear textures upon animating.");
					}
				}
			}

			OnSpritesSet();
		}

		public void Play()
		{
			if (m_FirstNonNullIndex == -1)
			{
				Debug.LogWarning("Cannot play Layered Sprite Animator. No Animations are set.");
				return;
			}

			isPlaying = true;
			if (spriteRenderer != null)
				m_SpriteRenderer.enabled = true;
			if (image != null)
				m_Image.enabled = true;

			if (m_Timer == 0.0f && playbackSpeed < 0.0f)
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

			//Cannot advance frames without animations
			if (m_FirstNonNullIndex == -1) return;

			if (m_Animations[m_FirstNonNullIndex].frames.Length > 1)
				count %= m_Animations[m_FirstNonNullIndex].frames.Length;

			if (currentFrame + count > m_Animations[m_FirstNonNullIndex].frames.Length - 1)
			{
				if (loop)
				{
					count = m_Animations[m_FirstNonNullIndex].frames.Length - 1 - currentFrame;
					SetFrame(count);
				}
				else
				{
					SetFrame(m_Animations[m_FirstNonNullIndex].frames.Length - 1);
					Pause();
					OnFinishedAnimation();
				}
			}
			else
			{
				SetFrame(currentFrame + count);
			}
		}

		public void GoBackFrames(int count)
		{
			if (count < 0)
			{
				Debug.LogError("Paramater count cannot be negative.");
				return;
			}

			//Cannot go back frames without animations
			if (m_FirstNonNullIndex == -1) return;

			count %= m_Animations[m_FirstNonNullIndex].frames.Length;

			if (currentFrame - count < 0)
			{
				if (loop)
				{
					count = m_Animations[m_FirstNonNullIndex].frames.Length - 1 - currentFrame;
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
			if (m_FirstNonNullIndex == -1)
			{
				Debug.LogError("Cannot animate to the set frame. There are not animations in the animation list.");
				return;
			}

			if (frameIndex < 0 || frameIndex > m_Animations[m_FirstNonNullIndex].frames.Length - 1)
			{
				Debug.LogWarning("Parameter frameIndex is out of range. Min: 0  Max: " + (m_Animations[m_FirstNonNullIndex].frames.Length - 1) + " Inputted parameter: " + frameIndex);
				return;
			}

			if (image == null && spriteRenderer == null)
			{
				enabled = false;
				Debug.LogError("No Sprite Renderer or Image component is attached.");
			}

			Sprite sprite = m_Animations[m_FirstNonNullIndex].GetFrame(frameIndex);

			if (m_Image != null)
			{
				if (sprite == null)
					m_Image.enabled = false;
				else
					m_Image.enabled = true;
				m_Image.sprite = sprite;
			}

			if (m_SpriteRenderer != null)
			{
				m_SpriteRenderer.sprite = sprite;
				string s = "";
                for(int i = 0; i < sprite.uv.Length; i++)
                {
					s += "[" + sprite.uv[i].x + "," + sprite.uv[i].y + "] ";
                }
				//Debug.Log(sprite.name + ": Length: " + sprite.uv.Length + ": " + s);
			}

			//TODO Use MaterialPropertyBlock for extra performance. http://thomasmountainborn.com/2016/05/25/materialpropertyblocks/
			for(int i = Mathf.Max(1,m_FirstNonNullIndex); i < 10; i++)
			{
				if(m_Animations[i] != null)
                {
					if(frameIndex < m_Animations[i].frameCount)
					{
						sprite = m_Animations[i]?.GetFrame(frameIndex);
					}
					else
                    {
						sprite = null;
                    }
                }
				else
                {
					sprite = null;
                }

				if(i > m_FirstNonNullIndex && sprite != null)
				{
					if (m_SpriteRenderer != null)
					{
						m_SpriteRenderer.material.SetTexture("_MainTex" + (i + 1).ToString(), sprite.texture);
					}
					if(m_Image != null)
					{
						m_Image.material.SetTexture("_MainTex" + (i + 1).ToString(), sprite.texture);
					}
				}
				else
				{
					if (m_SpriteRenderer != null)
					{
						m_SpriteRenderer.material.SetTexture("_MainTex" + (i + 1).ToString(), m_ClearTexture);
					}
					if (m_Image != null)
					{
						m_Image.material.SetTexture("_MainTex" + (i + 1).ToString(), m_ClearTexture);
					}
				}
			}

			currentFrame = frameIndex;
			onFrameChanged?.Invoke();
		}

		public void SetAnimationTimeNormalized(float normalizedTime)
		{
			if (m_FirstNonNullIndex == -1)
				return;

			normalizedTime = Mathf.Clamp01(normalizedTime);

			SetFrame(Mathf.FloorToInt(m_Animations[m_FirstNonNullIndex].frames.Length * normalizedTime));
			m_Timer = (m_Animations[m_FirstNonNullIndex].frames.Length * normalizedTime) % 1.0f;

		}

		//"Shoots" an instance of this gameobject with custom settings for the SpriteAnimator to be destroyed upon the animation being complete
		public void Shoot()
		{
			while (shots.Count > 0 && shots[0] == null)
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
			while (shots.Count > 0)
			{
				if (shots[0] != null)
					Destroy(shots[0].gameObject);
				shots.RemoveAt(0);
			}
		}

		#endregion Public Methods

		#region Private Methods

		private void OnFinishedAnimation()
		{
			onFinishedAnimation?.Invoke();
			if (destroyOnFinish)
				Destroy(gameObject);
		}

		private void OnSpritesSet()
		{

			if (m_FirstNonNullIndex == -1)
			{
				Stop();

				if (image != null)
				{
					m_Image.enabled = false;
					m_Image.sprite = null;
				}

				if (spriteRenderer != null)
				{
					m_SpriteRenderer.sprite = null;
				}
			}
			else
			{
				if (currentFrame > m_Animations[m_FirstNonNullIndex].frames.Length - 1)
				{
					SetFrame(m_Animations[m_FirstNonNullIndex].frames.Length - 1);
				}
				else
				{
					SetFrame(currentFrame);
				}
			}
		}

		private void SetMaterial()
		{
			if(spriteRenderer != null)
			{
				m_SpriteRenderer.material = new Material(m_RequiredShader);
			}
			if(image != null)
			{
				m_Image.material = new Material(m_RequiredShader);
			}
		}

		#endregion Private Methods
	}
}