using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Elanetic.Tools.Unity
{
    public class SpriteAnimation
    {
        //The name of this animation
        public string animationName { get; private set; }
        //The reference to the sprite assets
        public Sprite[] sprites { get; private set; }
        //The individual frames that reference the index of the sprite in the sprites array
        public int[] frames { get; private set; }
        //The default animation speed setting. Can be overidden by the Sprite Animator.
        public float animationSpeed { get; private set; }
        //The default loop setting. Can be overidden by the Sprite Animator.
        public bool loop { get; private set; }

        public int frameCount => frames.Length;

        public SpriteAnimation(string animationName, Sprite[] sprites) : this(animationName, sprites, 1.0f, false) { }

        //Assumes that the inputted sprites are the frames
        public SpriteAnimation(string animationName, Sprite[] sprites, float animationSpeed, bool loop)
        {
#if DEBUG
            if(string.IsNullOrWhiteSpace(animationName)) 
                throw new ArgumentException("Argument 'animationName' cannot be null or whitespace.");
            if (sprites == null || sprites.Length == 0) 
                throw new ArgumentException("Argument 'sprites' cannot be null or an empty array.");
#endif

            this.animationName = animationName;
            this.sprites = sprites;

            //Fill in the frame indices with the sprite indices
            frames = new int[sprites.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i] = i;
            }
            this.animationSpeed = animationSpeed;
            this.loop = loop;
        }

        public SpriteAnimation(string animationName, Sprite[] sprites, params int[] animationFrames) : this(animationName, sprites, 1.0f, false, animationFrames) { }

        public SpriteAnimation(string animationName, Sprite[] sprites, float animationSpeed, bool loop, params int[] animationFrames)
        {
#if DEBUG
            if(string.IsNullOrWhiteSpace(animationName)) 
                throw new ArgumentException("Argument 'animationName' cannot be null or whitespace.");
            if (sprites == null || sprites.Length == 0) 
                throw new ArgumentException("Argument 'sprites' cannot be null or an empty array.");
            if (animationFrames == null || animationFrames.Length == 0) 
                throw new ArgumentException("Argument 'animationFrames' cannot be null or an empty array.");
#endif

            this.animationName = animationName;
            this.sprites = sprites;
            frames = animationFrames;
            this.animationSpeed = animationSpeed;
            this.loop = loop;
        }

        /// <summary>
        /// Checks if this animation is playable and will not throw an OutOfRange exception when playing this animation.
        /// </summary>
        /// <param name="requireNonNullSprites"></param>
        /// <returns></returns>
        public bool IsValid(bool requireNonNullSprites=false)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                if(frames[i] < 0 || frames[i] > sprites.Length - 1) return false;
                if (requireNonNullSprites && sprites[frames[i]] == null) return false;
            }

            return true;
        }

        /// <summary>
        /// Get the sprite(animation frame) at the specified index of the animation.
        /// </summary>
        /// <param name="frameIndex"></param>
        /// <returns></returns>
        public Sprite GetFrame(int frameIndex)
        {
#if DEBUG
            if(frameIndex < 0 || frameIndex > frames.Length - 1) 
                throw new IndexOutOfRangeException("Argument 'frameIndex' must be more than or equal to zero and a valid index within the size of the sprites array.");
#endif
            return sprites[frames[frameIndex]];
        }
    }
}