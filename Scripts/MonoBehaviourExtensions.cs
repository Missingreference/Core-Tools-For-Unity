using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elanetic.Tools
{
    public static class MonoBehaviourExtensions
    {

        public static T RequireComponent<T>(this MonoBehaviour behaviour) where T : UnityEngine.Component
        {
            T component = behaviour.GetComponent<T>();
            if(component == null) return behaviour.gameObject.AddComponent<T>();
            return component;
        }

    }
}