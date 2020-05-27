///This script will 'direct' a SpriteAnimator script on what animations to play.
/// Features:
/// -Have a dictionary of animations to choose from.
/// -Play another animation once one animation has finished.
/// -
///
/// Use the attached SpriteAnimator component to change the playback speed, loop or current frame setting. Use that component to pause/stop as well.
///TODO: What if AddAnimation and RemoveAnimation are called while the animation is playing(currentAnimation)?

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elanetic.Tools
{

    [RequireComponent(typeof(SpriteAnimator))]
    public class SpriteDirector : MonoBehaviour
    {
        public SpriteAnimator spriteAnimator { get; private set; }
        public string currentAnimationName { get; private set; } = null;
        /// <summary>
        /// If true, calling play for the same animation as the 'currentAnimation' will restart the animation. Setting to false would be good for if calling play constantly.
        /// </summary>
        public bool resetOnSamePlayingAnimation { get; set; }

        private Dictionary<string, SpriteAnimation> m_Animations = new Dictionary<string, SpriteAnimation>();
        private string m_NextAnimation = "";

        void Awake()
        {

        }

        void Start()
        {

        }

        void OnEnable()
        {
            spriteAnimator = GetComponent<SpriteAnimator>();
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

        public void AddAnimation(SpriteAnimation animation)
        {
            if(string.IsNullOrEmpty(animation.animationName))
            {
                throw new ArgumentException("Cannot add animation. The name of the Sprite Animation cannot be null or empty.", nameof(animation));
            }

            if(animation == null)
            {
                throw new ArgumentNullException(nameof(animation), "The inputted Sprite Animation cannot be null.");
            }

            if(m_Animations.ContainsKey(animation.animationName))
            {
                Debug.LogError("Cannot add animation. Animation with the name '" + animation.animationName + "' already exists as an animation.");
                return;
            }

            m_Animations.Add(animation.animationName, animation);
        }

        public void AddAnimations(SpriteAnimation[] animations)
        {
            for(int i = 0; i < animations.Length; i++)
            {
                AddAnimation(animations[i]);
            }
        }

        public void RemoveAnimation(string animationName)
        {
            if(String.IsNullOrEmpty(animationName))
            {
                Debug.LogError("Cannot remove animation. Animation Name parameter cannot be null or empty.");
                return;
            }

            if(!m_Animations.ContainsKey(animationName))
            {
                Debug.LogError("Cannot remove animation. Animation with the name '" + animationName + "' does not exist.");
                return;
            }

            if(currentAnimationName == animationName)
            {
                //The animation we want to remove is currently playing. Stop the animation and remove the sprites from the SpriteAnimator. 
                spriteAnimator.Stop();
                spriteAnimator.animation = null;
                //Should the next animation be played if it exists? //OnAnimationFinished();
            }

            if(animationName == m_NextAnimation)
            {
                m_NextAnimation = "";
            }

            m_Animations.Remove(animationName);
        }

        public void RemoveAllAnimations()
        {
            string[] animations = GetAnimationNames();
            for(int i = 0; i < animations.Length; i++)
            {
                RemoveAnimation(animations[i]);
            }
        }

        public string[] GetAnimationNames()
        {
            return m_Animations.Keys.ToArray();
        }

        public SpriteAnimation GetAnimation(string animationName)
        {
            if(m_Animations.TryGetValue(animationName, out SpriteAnimation animation))
            {
                return animation;
            }
            return null;
        }

        public SpriteAnimation[] GetAnimations()
        {
            SpriteAnimation[] animations = new SpriteAnimation[m_Animations.Count];
            string[] animationNames = GetAnimationNames();
            for(int i = 0; i < m_Animations.Count; i++)
            {
                animations[i] = m_Animations[animationNames[i]];
            }
            return animations;
        }

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
            if(String.IsNullOrEmpty(animationName))
            {
                Debug.LogError("Cannot play animation. Animation Name parameter cannot be null or empty.");
                return;
            }

            if(!m_Animations.ContainsKey(animationName))
            {
                Debug.LogError("Cannot play animation. Animation with the name '" + animationName + "' does not exist.");
                return;
            }

            if(!resetOnSamePlayingAnimation && animationName == currentAnimationName)
            {
                //Animation won't reset if the animations are the same.
                spriteAnimator.loop = loop;
                spriteAnimator.playbackSpeed = playbackSpeed;
                return;
            }

            spriteAnimator.animation = m_Animations[animationName];
            spriteAnimator.Stop();
            spriteAnimator.loop = loop;
            spriteAnimator.playbackSpeed = playbackSpeed;
            spriteAnimator.SetFrame(startFrame);
            spriteAnimator.Play();
            currentAnimationName = animationName;
        }

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