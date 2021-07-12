using System;

using UnityEngine;

namespace Elanetic.Tools
{
    static public class UnityObjectExtensions
    {
        /// <summary>
        /// Check whether the UnityEngine.Object is actually null rather than using equals(==) operator to check for null. A better alternative for checking if the object exists and better performance.
        /// </summary>
        static public bool IsNull(this UnityEngine.Object unityObject)
        {
            return ReferenceEquals(unityObject, null);
        }
    }
}