using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elanetic.Tools
{

    [RequireComponent(typeof(LayeredSpriteAnimator))]
    public class LayeredSpriteDirector : MonoBehaviour
    {
        public LayeredSpriteAnimator spriteAnimator { get; private set; }
        public string currentAnimation { get; private set; } = null;
        /// <summary>
        /// If true, calling play for the same animation as the 'currentAnimation' will restart the animation. Setting to false would be good for if calling play constantly.
        /// </summary>
        public bool resetOnSamePlayingAnimation { get; set; }

        private readonly Dictionary<string, SpriteAnimation>[] m_Animations = new Dictionary<string, SpriteAnimation>[10];
        private string m_NextAnimation = "";

        void Awake()
        {
            for(int i = 0; i < m_Animations.Length; i++)
            {
                m_Animations[i] = new Dictionary<string, SpriteAnimation>();
            }
        }

        void Start()
        {

        }

        void OnEnable()
        {
            spriteAnimator = GetComponent<LayeredSpriteAnimator>();
            spriteAnimator.onFinishedAnimation += OnAnimationFinished;
        }

        private void OnDisable()
        {
            spriteAnimator.onFinishedAnimation -= OnAnimationFinished;
        }

        void Update()
        {

        }

        #region Public Functions

        public void AddAnimation(SpriteAnimation animation, int layer)
        {
            if(animation == null) throw new ArgumentNullException("Argument 'animation' cannot be null.");
            if(layer < 0 || layer > m_Animations.Length - 1) throw new ArgumentOutOfRangeException("Argument 'layer' must be from the range of 0 to 9 of an index.");

            if(m_Animations[layer].ContainsKey(animation.animationName))
            {
                Debug.LogError("Cannot add animation. Animation with the name '" + animation.animationName + "' already exists as an animation.");
                return;
            }

            m_Animations[layer].Add(animation.animationName, animation);
        }

        public void AddAnimations(SpriteAnimation[] animations, int layer)
        {
            for(int i = 0; i < animations.Length; i++)
            {
                AddAnimation(animations[i], layer);
            }
        }

        //Checks all layers for at least one existence of the specified animation name
        public bool HasAnimationAny(string animationName)
        {
            if(string.IsNullOrWhiteSpace(animationName)) throw new ArgumentNullException("Argument 'animationName' cannot be null or whitespace.");

            for(int i = 0; i < m_Animations.Length; i++)
            {
                if(m_Animations[i].ContainsKey(animationName)) return true;
            }

            return false;
        }

        public bool HasAnimation(string animationName, int layer)
        {
            if(string.IsNullOrWhiteSpace(animationName)) throw new ArgumentNullException("Argument 'animationName' cannot be null or whitespace.");
            if(layer < 0 || layer > m_Animations.Length - 1) throw new ArgumentOutOfRangeException("Argument 'layer' must be from the range of 0 to 9 of an index.");

            return m_Animations[layer].ContainsKey(animationName);
        }

        public void RemoveAnimation(string animationName, int layer)
        {
            if(string.IsNullOrWhiteSpace(animationName)) throw new ArgumentNullException("Argument 'animationName' cannot be null or whitespace.");
            if(layer < 0 || layer > m_Animations.Length - 1) throw new ArgumentOutOfRangeException("Argument 'layer' must be from the range of 0 to 9 of an index.");

            if(currentAnimation == animationName)
            {
                //The animation we want to remove is currently playing. Stop the animation and remove the sprites from the SpriteAnimator. 
                spriteAnimator.Stop();
                spriteAnimator.SetAnimation(null, layer);
                //Should the next animation be played if it exists? //OnAnimationFinished();
            }

            if(animationName == m_NextAnimation)
            {
                m_NextAnimation = "";
            }

            if(!m_Animations[layer].Remove(animationName))
            {
                Debug.LogError("Cannot remove animation. Animation with the name '" + animationName + "' does not exist.");
                return;
            }
        }

        public void RemoveAnimationOnAllLayers(string animationName)
        {
            if(string.IsNullOrWhiteSpace(animationName)) throw new ArgumentNullException("Argument 'animationName' cannot be null or whitespace.");
            for(int i = 0; i < m_Animations.Length; i++)
            {
                if(m_Animations[i].ContainsKey(animationName))
                {
                    RemoveAnimation(animationName, i);
                }
            }
        }

        public void RemoveAllLayerAnimations(int layer)
        {
            if(layer < 0 || layer > m_Animations.Length - 1) throw new ArgumentOutOfRangeException("Argument 'layer' must be from the range of 0 to 9 of an index.");

            string[] animationNames = GetAnimationNames(layer);
            for(int i = 0; i < animationNames.Length; i++)
            {
                RemoveAnimation(animationNames[i], layer);
            }
        }

        public void RemoveAllAnimations()
        {
            //TODO Currently allocates garbage. Fix.
            for(int i = 0; i < m_Animations.Length; i++)
            {
                string[] animations = GetAnimationNames(i);
                for(int h = 0; h < animations.Length; h++)
                {
                    RemoveAnimation(animations[h], i);
                }
            }
        }

        public string[] GetAnimationNames(int layer)
        {
            return m_Animations[layer].Keys.ToArray();
        }

        public SpriteAnimation GetAnimation(string animationName, int layer)
        {
            if(layer < 0 || layer > m_Animations.Length - 1) throw new ArgumentOutOfRangeException("Argument 'layer' must be from the range of 0 to 9 of an index.");
            SpriteAnimation animation;
            if(m_Animations[layer].TryGetValue(animationName, out animation))
            {
                return animation;
            }
            return null;
        }

        /*
        public Tuple<string, Sprite[]>[] GetAnimations()
        {
            Tuple<string, Sprite[]>[] animations = new Tuple<string, Sprite[]>[m_Animations.Count];
            string[] animationNames = GetAnimationNames();
            for(int i = 0; i < m_Animations.Count; i++)
            {
                animations[i] = new Tuple<string, Sprite[]>(animationNames[i], m_Animations[animationNames[i]]);
            }
            return animations;
        }*/

        //Use this one if you want to use whatever settings that are already set on the Sprite Animator or if you want to set it yourself somewhere else
        public void Play(string animationName)
        {
            Play(animationName, spriteAnimator.loop, spriteAnimator.playbackSpeed, 0);
        }

        public void Play(string animationName, bool loop)
        {
            Play(animationName, loop, spriteAnimator.playbackSpeed, 0);
        }

        public void Play(string animationName, float playbackSpeed)
        {
            Play(animationName, spriteAnimator.loop, playbackSpeed, 0);
        }

        public void Play(string animationName, int startFrame)
        {
            Play(animationName, spriteAnimator.loop, spriteAnimator.playbackSpeed, startFrame);
        }

        public void Play(string animationName, bool loop, float playbackSpeed)
        {
            Play(animationName, loop, playbackSpeed, 0);
        }

        public void Play(string animationName, bool loop, int startFrame)
        {
            Play(animationName, loop, spriteAnimator.playbackSpeed, startFrame);
        }

        public void Play(string animationName, float playbackSpeed, int startFrame)
        {
            Play(animationName, spriteAnimator.loop, playbackSpeed, startFrame);
        }

        public void Play(string animationName, bool loop, float playbackSpeed, int startFrame)
        {
            if(string.IsNullOrWhiteSpace(animationName)) throw new ArgumentNullException("Argument 'animationName' cannot be null or whitespace.");
            
            if(!resetOnSamePlayingAnimation && animationName == currentAnimation)
            {
                //Animation won't reset if the animations are the same.
                spriteAnimator.loop = loop;
                spriteAnimator.playbackSpeed = playbackSpeed;
                return;
            }
            SpriteAnimation animation;
            for(int i = 0; i < m_Animations.Length; i++)
            {
                if(m_Animations[i].TryGetValue(animationName, out animation))
                {
                    spriteAnimator.SetAnimation(animation, i);
                }
                else
                {
                    spriteAnimator.SetAnimation(null, i);
                }
            }
            spriteAnimator.Stop();
            spriteAnimator.loop = loop;
            spriteAnimator.playbackSpeed = playbackSpeed;
            spriteAnimator.SetFrame(0);
            spriteAnimator.Play();
            currentAnimation = animationName;
        }

        /*
        public void PlayOnceThenLoop(string firstAnimationName, string secondLoopingAnimation)
        {
            if(String.IsNullOrEmpty(firstAnimationName))
            {
                Debug.LogError("Cannot play animation. First Animation parameter cannot be null or empty.");
                return;
            }

            if(!m_Animations.ContainsKey(firstAnimationName))
            {
                Debug.LogError("Cannot play animation. Animation with the name '" + firstAnimationName + "' does not exist.");
                return;
            }

            Play(firstAnimationName, false);


            if(String.IsNullOrEmpty(secondLoopingAnimation))
            {
                Debug.LogError("Cannot play animation. Second Animation parameter cannot be null or empty.");
                return;
            }

            if(!m_Animations.ContainsKey(secondLoopingAnimation))
            {
                Debug.LogError("Cannot play animation. Animation with the name '" + secondLoopingAnimation + "' does not exist.");
                return;
            }
            m_NextAnimation = secondLoopingAnimation;
        }
        */

        #endregion

        #region Private Functions

        private void OnAnimationFinished()
        {
            if(m_NextAnimation != "")
            {
                Play(m_NextAnimation, true);
                m_NextAnimation = "";
            }
        }

        #endregion
    }
}