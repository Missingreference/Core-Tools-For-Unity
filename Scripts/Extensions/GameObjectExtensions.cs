using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elanetic.Tools
{
    public static class GameObjectExtensions
    {

        public static T RequireComponent<T>(this GameObject gameObject) where T : UnityEngine.Component
        {
            T component = gameObject.GetComponent<T>();
            if(component == null) return gameObject.AddComponent<T>();
            return component;
        }

    }
}