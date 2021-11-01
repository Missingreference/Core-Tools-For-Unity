using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.Scripting;

namespace Elanetic.Tools
{
    /// <summary>
    /// Execute code at the beginning of Unity startup. Specifically after assemblies loaded. Order execution by an integer.
    /// Unconfirmed: It may be more reliable than static constructor due to Unity potentially having an IL2CPP bug where it sometimes does not call static constructors as well as static constructors are not called unless they are referenced in some way according to MSDN or something.
    /// </summary>
    static public class RuntimeInitialization
    {
        static RuntimeInitialization() => Init();

        static private bool m_Initialized = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static private void Init()
        {
            if(m_Initialized) return;
            m_Initialized = true;

            List<Tuple<MethodInfo, int>> methodsToExecute = new List<Tuple<MethodInfo, int>>(64);

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for(int i = 0; i < assemblies.Length; i++)
            {
                Assembly assembly = assemblies[i];
                Type[] types = assembly.GetTypes();
                for(int h = 0; h < types.Length; h++)
                {
                    MethodInfo[] methods = types[h].GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    for(int j = 0; j < methods.Length; j++)
                    {
                        MethodInfo methodInfo = methods[j];
                        RunOnLoadAttribute attribute = methodInfo.GetCustomAttribute<RunOnLoadAttribute>(true);
                        if(attribute != null)
                        {
                            bool inserted = false;
                            for(int m = 0; m < methodsToExecute.Count; m++)
                            {
                                if(attribute.executionOrder >= methodsToExecute[m].Item2) continue;

                                methodsToExecute.Insert(m, new Tuple<MethodInfo, int>(methodInfo, attribute.executionOrder));
                                inserted = true;
                                break;
                            }

                            if(!inserted)
                            {
                                methodsToExecute.Add(new Tuple<MethodInfo, int>(methodInfo, attribute.executionOrder));
                            }
                        }
                    }
                }
            }

            //Execute
            for(int i = 0; i < methodsToExecute.Count; i++)
            {
                methodsToExecute[i].Item1.Invoke(null, null);
            }

            GC.Collect();
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RunOnLoadAttribute : PreserveAttribute
    {
        public int executionOrder { get; private set; } = 0;

        public RunOnLoadAttribute() { }

        public RunOnLoadAttribute(int executionOrder)
        {
            this.executionOrder = executionOrder;
        }
    }
}