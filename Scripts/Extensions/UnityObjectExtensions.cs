using System;

using UnityEngine;

namespace Elanetic.Tools
{
    static public class UnityObjectExtensions
    {
        /// <summary>
        /// Check whether the UnityEngine.Object is actually null rather than using equals(==) operator to check for null. A better alternative for checking if the object exists and better performance.
        /// Be sure that if you destroy an unity object that you set it to null to get expected results like any other class in existence.
        /// </summary>
        static public bool IsNull(this UnityEngine.Object unityObject)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            //Some psycho at Unity made it so that GetComponent<T>() returns a non-null fake object in the editor when the component does not exist
            //Thus we have to use Unity's == overload to ensure expected results losing out performance in the editor
            return unityObject == null;
#else
            return ReferenceEquals(unityObject, null);
#endif
        }
    }
}