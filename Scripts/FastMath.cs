///Some of these functions are sourced from:
///http://graphics.stanford.edu/~seander/bithacks.html
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elanetic.Tools
{
    /// <summary>
    /// The purpose of this math library is to use variations of math functions to prioritize performance above everything else with minimal branching as possible.
    /// This may come at a cost of a range of valid input which will be specified by function summaries and have SAFE_EXECUTION preprocessor to be a safety check for valid input.
    /// </summary>
    static public class FastMath
    {
        /// <summary>
        /// Get the absolute value of the inputted value.
        /// </summary>
        /// Notes:
        /// UnityEngine.Mathf.Abs(int) calls System.Math.Abs(int).
        /// System.Math has branching and safety checks in build.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public int Abs(int value)
        {
#if SAFE_EXECUTION
            if(value == int.MinValue)
                throw new OverflowException("The minimum integer -2147483648 is not capable of beings positive due to the maximum integer being 2147483647. A byproduct of two's complement.");
#endif
            //63 comes from: (sizeof(int) * CHAR_BIT) - 1;
            //CHAR_BIT is from C/C++ which represents the number of bits in a byte (8) in C#
            int mask = value >> 63;
            return (value + mask) ^ mask;
        }

        /// <summary>
        /// Get the smaller of the two numbers.
        /// </summary>
        /// Notes:
        /// Just as fast as System.Min and UnityEngine.Mathf.Min.
        /// Unable to use the above link's bit twiddling for branchless min and max due to C#'s booleans are not easily convertable to 0 or 1 integers.
        /// Using unsafe pointers to convert from bool to int does not convert to just 1 but rather to any number not zero including negative numbers.
        /// This method is the best I've found for maximum compatibility. Use FastMin if you are able to follow the rule specified in it's summary.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public int Min(int value0, int value1)
        {
            if(value0 > value1)
                return value1;
            return value0;
        }

        /// <summary>
        /// Get the smaller of the two numbers.
        /// Use this if you are sure that the result of A subtracted by B is within int.MinValue and int.MaxValue.
        /// It is faster than normal Min but will return incorrect data if the above rule is not followed.
        /// A good rule of thumb is if you think you are potentially using big numbers, use the regular FastMath.Min function.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public int MinFast(int a, int b)
        {
#if SAFE_EXECUTION
            int dif;
            try
            {
                dif = checked(a - b);
            }
            catch(OverflowException ex)
            {
                throw new OverflowException("FastMath.MinFast has been used incorrectly. A - B must not overflow past int.MinValue or int.MaxValue otherwise it will result in a bad return. Use FastMath.Min instead for cases like these.", ex);
            }
#else
            int dif = a - b;
#endif
            return b + (dif & (dif >> 63));
        }

        /// <summary>
        /// Get the larger of the two numbers.
        /// </summary>
        /// Notes:
        /// Just as fast as System.Max and UnityEngine.Mathf.Max.
        /// Unable to use the above link's bit twiddling for branchless min and max due to C#'s booleans are not easily convertable to 0 or 1 integers.
        /// Using unsafe pointers to convert from bool to int does not convert to just 1 but rather to any number not zero including negative numbers.
        /// This method is the best I've found for maximum compatibility. Use FastMax if you are able to follow the rule specified in it's summary.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public int Max(int value0, int value1)
        {
            if(value1 < value0)
                return value0;
            return value1;
        }

        /// <summary>
        /// Get the larger of the two numbers.
        /// Use this if you are sure that the result of A subtracted by B is within int.MinValue and int.MaxValue.
        /// It is faster than normal Min but will return incorrect data if the above rule is not followed.
        /// A good rule of thumb is if you think you are potentially using big numbers, use the regular FastMath.Max function.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public int MaxFast(int a, int b)
        {
#if SAFE_EXECUTION
            int dif;
            try
            {
                dif = checked(a - b);
            }
            catch(OverflowException ex)
            {
                throw new OverflowException("FastMath.MaxFast has been used incorrectly. A - B must not overflow past int.MinValue or int.MaxValue otherwise it will result in a bad return. Use FastMath.Max instead for cases like these.", ex);
            }
#else
            int dif = a - b;
#endif
            return a - (dif & (dif >> 63));
        }

        /// <summary>
        /// Get the value clamped between a minimum and maximum value. If within the bounds it will return the inputted value. Has branching statements.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public int Clamp(int value, int min, int max)
        {
            if(value < min) return min;
            else if(value > max) return max;
            return value;
        }

        /// <summary>
        /// Get the value clamped between a minimum and maximum value. If within the bounds it will return the inputted value.
        /// Use this if you are sure that the result of Value subtracted by Min or Value subtracted by Max is within int.MinValue and int.MaxValue.
        /// It is faster than normal Clamp but will return incorrect data if the above rule is not followed.
        /// A good rule of thumb is if you think you are potentially using big numbers, use the regular FastMath.Clamp function.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public int ClampFast(int value, int min, int max)
        {
#if SAFE_EXECUTION
            int dif;
            try
            {
                dif = checked(value - min);
                dif = checked((value - (dif & (dif >> 63))) - max);
            }
            catch(OverflowException ex)
            {
                throw new OverflowException("FastMath.FastClamp has been used incorrectly. Value - Min or Value - Max must not overflow past int.MinValue or int.MaxValue otherwise it will result in a bad return. Use FastMath.Clamp instead for cases like these.", ex);
            }
#else
            int dif = value - min;
            dif = (value - (dif & (dif >> 63))) - max;
#endif
            return max + (dif & (dif >> 63));
        }

        /// <summary>
        /// Get the logarithmic value at base 2. The result is of type integer so expect floor rounding and no infinities.
        /// Source: https://stackoverflow.com/a/30643928
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public int Log2(int value)
        {
            int r = 0xFFFF - value >> 31 & 0x10;
            value >>= r;
            int shift = 0xFF - value >> 31 & 0x8;
            value >>= shift;
            r |= shift;
            shift = 0xF - value >> 31 & 0x4;
            value >>= shift;
            r |= shift;
            shift = 0x3 - value >> 31 & 0x2;
            value >>= shift;
            r |= shift;
            r |= (value >> 1);
            return r;
        }
        
        /// <summary>
        /// Get the signed modulos.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public int Mod(int dividend, int divisor)
        {
            return ((dividend % divisor) + divisor) % divisor;
        }

        /// <summary>
        /// Get the signed modulos.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static public float Mod(float dividend, float divisor)
        {
            return ((dividend % divisor) + divisor) % divisor;
        }
    }
}