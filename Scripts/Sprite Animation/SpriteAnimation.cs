using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Elanetic.Tools
{
    public class SpriteAnimation
    {
        public string animationName { get; private set; }
        public Sprite[] sprites { get; private set; }
        public int[] frames { get; private set; }

        public SpriteAnimation(string animationName, Sprite[] sprites, params int[] animationFrames)
        {
            if(string.IsNullOrWhiteSpace(animationName)) throw new ArgumentException("Argument 'animationName' cannot be null or whitespace.");
            if (sprites == null || sprites.Length == 0) throw new ArgumentException("Argument 'sprites' cannot be null or an empty array.");
            if (animationFrames == null || animationFrames.Length == 0) throw new ArgumentException("Argument 'animationFrames' cannot be null or an empty array.");

            this.animationName = animationName;
            this.sprites = sprites;
            frames = animationFrames;
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
                if(frames[i] < 0 || frames[i] > sprites.Length-1) return false;
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
            if (frameIndex < 0 || frameIndex > sprites.Length - 1) throw new IndexOutOfRangeException("Argument 'frameIndex' must be more than zero and a valid index within the size of the sprites array.");
            return sprites[frames[frameIndex]];
        }
    }
}