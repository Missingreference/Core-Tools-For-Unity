///This script will 'direct' a SpriteAnimator script on what animations to play.
/// Features:
/// -Have a dictionary of animations to choose from.
/// -
/// Use the attached SpriteAnimator component to change the playback speed, loop or current frame setting. Use that component to pause/stop as well.
///TODO: What if AddAnimation and RemoveAnimation are called while the animation is playing(currentAnimation)?
///TODO: Be able to play animation by simply calling Play(int) for better performance. Can be useful for cases such as a character uses an enum for tracking various animations and thus can call Play((int)enum);

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elanetic.Tools
{

    [RequireComponent(typeof(SpriteAnimator))]
    public class SpriteDirector : MonoBehaviour
    {
        public SpriteAnimator spriteAnimator { get; private set; }
        public int animationCount { get; private set; }

        private Dictionary<string, SpriteAnimation> m_Animations = new Dictionary<string, SpriteAnimation>();

        void Awake()
        {
            spriteAnimator = GetComponent<SpriteAnimator>();
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
            animationCount = m_Animations.Count;
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

            if(spriteAnimator.animation.animationName == animationName)
            {
                //The animation we want to remove is currently playing. Stop the animation and remove the sprites from the SpriteAnimator. 
                spriteAnimator.Stop();
                spriteAnimator.animation = null;
            }

            m_Animations.Remove(animationName);
            animationCount = m_Animations.Count;
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
            //TODO: Bad performance
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

        //Use this one if you want to use whatever settings that are already set in the Sprite Animation.
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

            if(m_Animations.TryGetValue(animationName, out SpriteAnimation animation))
            {
                //Play(animation, loop, playbackSpeed, startFrame);
                PlayAnimation(animation, loop, playbackSpeed, startFrame);
            }
            else
            {
                //When this error occurs, make sure to call AddAnimation with the animation you want SpriteDirector to remember.
                Debug.LogError("Cannot play animation. Animation with the name '" + animationName + "' does not exist.");
            }
        }

        public void Play(SpriteAnimation animation)
        {
            VerifyAnimation(animation);

            PlayAnimation(animation, animation.loop, animation.animationSpeed, 0);
        }

        public void Play(SpriteAnimation animation, bool loop)
        {
            VerifyAnimation(animation);

            PlayAnimation(animation, loop, animation.animationSpeed, 0);
        }

        public void Play(SpriteAnimation animation, float playbackSpeed)
        {
            VerifyAnimation(animation);

            PlayAnimation(animation, animation.loop, playbackSpeed, 0);
        }

        public void Play(SpriteAnimation animation, int startFrame)
        {
            VerifyAnimation(animation);

            PlayAnimation(animation, animation.loop, animation.animationSpeed, startFrame);
        }

        public void Play(SpriteAnimation animation, bool loop, float playbackSpeed)
        {
            VerifyAnimation(animation);

            PlayAnimation(animation, loop, playbackSpeed, 0);
        }

        public void Play(SpriteAnimation animation, bool loop, int startFrame)
        {

            VerifyAnimation(animation);

            PlayAnimation(animation, loop, animation.animationSpeed, startFrame);
        }

        public void Play(SpriteAnimation animation, float playbackSpeed, int startFrame)
        {
            VerifyAnimation(animation);

            PlayAnimation(animation, animation.loop, playbackSpeed, startFrame);
        }

        public void Play(SpriteAnimation animation, bool loop, float playbackSpeed, int startFrame)
        {
            VerifyAnimation(animation);

            PlayAnimation(animation, loop, playbackSpeed, startFrame);
        }

        //Called when attempting to play a SpriteAnimation.
        private void VerifyAnimation(SpriteAnimation animation)
        {
            if(animation == null) throw new NullReferenceException("Sprite Animation cannot be null.");

            if(!animation.IsValid(false)) throw new InvalidOperationException("Cannot play invalid Sprite Animation.");

            if(!m_Animations.ContainsValue(animation)) throw new InvalidOperationException("Cannot play animation. The animation has not been added to the Sprite Director.");
        }

        private void PlayAnimation(SpriteAnimation animation, bool loop, float playbackSpeed, int startFrame)
        {
            spriteAnimator.animation = animation;
            spriteAnimator.Stop();
            spriteAnimator.loop = loop;
            spriteAnimator.playbackSpeed = playbackSpeed;
            spriteAnimator.SetFrame(startFrame);
            spriteAnimator.Play();
        }

        #endregion
    }
}