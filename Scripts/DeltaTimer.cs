using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Elanetic.Tools
{
    public class DeltaTimer
    {
        static private UnityRuntimeListener m_RuntimeListener = null;
        static private DeltaTimer[] m_Timers = new DeltaTimer[64];
        static private int m_TimerCount = 0;

        static private readonly int RESIZE_AMOUNT = 64;

        public bool isRunning { get; private set; } = false;
        public float currentTime { get; private set; } = 0;
        public float percent => currentTime / endTime;
        public float endTime { get; private set; } = 0;
        public Action<float> listener { get; private set; } = null;

        public Action onComplete;

        private int m_Index = -1;

        public DeltaTimer(float time, Action<float> listener)
        {
#if SAFE_EXECUTION
            if(time <= 0.0f)
                throw new ArgumentException(nameof(time), "Inputted time must be more than zero.");
            if(listener == null)
                throw new ArgumentNullException(nameof(listener), "Inputted listener cannot be null.");
#endif

            if(m_RuntimeListener.IsNull())
                m_RuntimeListener = new GameObject("Unity Runtime Listener").AddComponent<UnityRuntimeListener>();

            endTime = time;
            this.listener = listener;
        }

        public void Start()
        {
            if(isRunning) return;

            if(currentTime >= endTime)
            {
                currentTime = 0.0f;
            }
            isRunning = true;

            if(m_TimerCount == m_Timers.Length)
            {
                DeltaTimer[] newTimerArray = new DeltaTimer[m_Timers.Length + RESIZE_AMOUNT];
                for(int i = 0; i < m_Timers.Length; i++)
                {
                    newTimerArray[i] = m_Timers[i];
                }
                m_Timers = newTimerArray;
            }

            m_Index = m_TimerCount;
            m_Timers[m_Index] = this;
            m_TimerCount++;

            m_RuntimeListener.enabled = true;
        }

        public void Stop()
        {
            if(!isRunning) return;

            isRunning = false;

            m_TimerCount--;
            if(m_TimerCount > 0)
            {
                DeltaTimer timerToMove = m_Timers[m_TimerCount];
                m_Timers[m_Index] = timerToMove;
                timerToMove.m_Index = m_Index;
            }
            else
            {
                m_RuntimeListener.enabled = false;
            }
        }

        public void Reset()
        {
            Stop();
            currentTime = 0;
        }

        private void Execute()
        {
            currentTime += Time.deltaTime;
            if(currentTime > endTime)
            {
                currentTime = endTime;

                listener(currentTime);
                Stop();
                onComplete?.Invoke();
            }
            else
            {
                listener(currentTime);
            }
        }

        [DefaultExecutionOrder(0)]
        private class UnityRuntimeListener : MonoBehaviour
        {
            private bool m_IsQuitting = false;

            void Awake()
            {
                DontDestroyOnLoad(gameObject);
                gameObject.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
            }

            void Update()
            {
                for(int i = 0; i < m_TimerCount; i++)
                {
                    DeltaTimer timer = m_Timers[i];
                    timer.Execute();
                    if(!timer.isRunning)
                    {
                        i--;
                    }
                }
            }

            void OnDisable()
            {
                if(m_IsQuitting || m_TimerCount == 0) return;

                throw new InvalidOperationException("UnityRuntimeListener should never be disabled or destroyed during runtime. This will interrupt every listener that uses this update method.");
            }

            void OnDestroy()
            {
                if(m_IsQuitting) return;

                throw new InvalidOperationException("UnityRuntimeListener should never be destroyed during runtime. This will interrupt every listener that uses this update method.");
            }

            void OnApplicationQuit()
            {
                m_IsQuitting = true;
            }
        }
    }

}