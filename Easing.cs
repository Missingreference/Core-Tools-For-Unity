using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

/* 
 * Functions taken from Tween.js - Licensed under the MIT license
 * at https://github.com/sole/tween.js
 */
static public class Easing
{
    static public class Linear
    {
        static public float InOut(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * percentage;
        }

        static public Vector2 InOut(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage));
        }

        static public Vector3 InOut(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage),
                InOut(start.z, finish.z, percentage));
        }

        static public Color InOut(Color start, Color finish, float percentage)
        {
            return new Color(
                InOut(start.r, finish.r, percentage),
                InOut(start.g, finish.g, percentage),
                InOut(start.b, finish.b, percentage),
                InOut(start.a, finish.a, percentage));
        }
        
        static public Automator InOutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
    }

    static public class Quadratic
    {
        static public float In(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * (percentage * percentage);
        }

        static public float Out(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * (percentage * (2f - percentage));
        }

        static public float InOut(float start, float finish, float percentage)
        {
            float difference = finish - start;
            if ((percentage *= 2f) < 1f) return start + difference * (0.5f * percentage * percentage);
            return start + difference * (-0.5f * ((percentage -= 1f) * (percentage - 2f) - 1f));
        }

        static public Vector2 In(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage));
        }

        static public Vector2 Out(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage));
        }

        static public Vector2 InOut(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage));
        }

        static public Vector3 In(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage),
                In(start.z, finish.z, percentage));
        }

        static public Vector3 Out(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage),
                Out(start.z, finish.z, percentage));
        }

        static public Vector3 InOut(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage),
                InOut(start.z, finish.z, percentage));
        }

        static public Color In(Color start, Color finish, float percentage)
        {
            return new Color(
                In(start.r, finish.r, percentage),
                In(start.g, finish.g, percentage),
                In(start.b, finish.b, percentage),
                In(start.a, finish.a, percentage)
                );
        }

        static public Color Out(Color start, Color finish, float percentage)
        {
            return new Color(
                Out(start.r, finish.r, percentage),
                Out(start.g, finish.g, percentage),
                Out(start.b, finish.b, percentage),
                Out(start.a, finish.a, percentage)
                );
        }

        static public Color InOut(Color start, Color finish, float percentage)
        {
            return new Color(
                InOut(start.r, finish.r, percentage),
                InOut(start.g, finish.g, percentage),
                InOut(start.b, finish.b, percentage),
                InOut(start.a, finish.a, percentage)
                );
        }
        
        static public Automator InAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
    };

    static public class Cubic
    {
        static public float In(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * (percentage * percentage * percentage);
        }

        static public float Out(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * (1f + ((percentage -= 1f) * percentage * percentage));
        }

        static public float InOut(float start, float finish, float percentage)
        {
            float difference = finish - start;
            if ((percentage *= 2f) < 1f) return start + difference * (0.5f * percentage * percentage * percentage);
            return start + difference * (0.5f * ((percentage -= 2f) * percentage * percentage + 2f));
        }

        static public Vector2 In(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage));
        }

        static public Vector2 Out(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage));
        }

        static public Vector2 InOut(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage));
        }

        static public Vector3 In(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage),
                In(start.z, finish.z, percentage));
        }

        static public Vector3 Out(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage),
                Out(start.z, finish.z, percentage));
        }

        static public Vector3 InOut(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage),
                InOut(start.z, finish.z, percentage));
        }

        static public Color In(Color start, Color finish, float percentage)
        {
            return new Color(
                In(start.r, finish.r, percentage),
                In(start.g, finish.g, percentage),
                In(start.b, finish.b, percentage),
                In(start.a, finish.a, percentage)
                );
        }

        static public Color Out(Color start, Color finish, float percentage)
        {
            return new Color(
                Out(start.r, finish.r, percentage),
                Out(start.g, finish.g, percentage),
                Out(start.b, finish.b, percentage),
                Out(start.a, finish.a, percentage)
                );
        }

        static public Color InOut(Color start, Color finish, float percentage)
        {
            return new Color(
                InOut(start.r, finish.r, percentage),
                InOut(start.g, finish.g, percentage),
                InOut(start.b, finish.b, percentage),
                InOut(start.a, finish.a, percentage)
                );
        }
        
        static public Automator InAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
    };

    static public class Quartic
    {
        static public float In(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * (percentage * percentage * percentage * percentage);
        }

        static public float Out(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * (1f - ((percentage -= 1f) * percentage * percentage * percentage));
        }

        static public float InOut(float start, float finish, float percentage)
        {
            float difference = finish - start;
            if ((percentage *= 2f) < 1f) return start + difference * (0.5f * percentage * percentage * percentage * percentage);
            return start + difference * (-0.5f * ((percentage -= 2f) * percentage * percentage * percentage - 2f));
        }

        static public Vector2 In(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage));
        }

        static public Vector2 Out(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage));
        }

        static public Vector2 InOut(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage));
        }

        static public Vector3 In(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage),
                In(start.z, finish.z, percentage));
        }

        static public Vector3 Out(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage),
                Out(start.z, finish.z, percentage));
        }

        static public Vector3 InOut(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage),
                InOut(start.z, finish.z, percentage));
        }

        static public Color In(Color start, Color finish, float percentage)
        {
            return new Color(
                In(start.r, finish.r, percentage),
                In(start.g, finish.g, percentage),
                In(start.b, finish.b, percentage),
                In(start.a, finish.a, percentage)
                );
        }

        static public Color Out(Color start, Color finish, float percentage)
        {
            return new Color(
                Out(start.r, finish.r, percentage),
                Out(start.g, finish.g, percentage),
                Out(start.b, finish.b, percentage),
                Out(start.a, finish.a, percentage)
                );
        }

        static public Color InOut(Color start, Color finish, float percentage)
        {
            return new Color(
                InOut(start.r, finish.r, percentage),
                InOut(start.g, finish.g, percentage),
                InOut(start.b, finish.b, percentage),
                InOut(start.a, finish.a, percentage)
                );
        }
        
        static public Automator InAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
    };

    static public class Quintic
    {
        static public float In(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * (percentage * percentage * percentage * percentage * percentage);
        }

        static public float Out(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * (1f + ((percentage -= 1f) * percentage * percentage * percentage * percentage));
        }

        static public float InOut(float start, float finish, float percentage)
        {
            float difference = finish - start;
            if ((percentage *= 2f) < 1f) return start + difference * (0.5f * percentage * percentage * percentage * percentage * percentage);
            return start + difference * (0.5f * ((percentage -= 2f) * percentage * percentage * percentage * percentage + 2f));
        }

        static public Vector2 In(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage));
        }

        static public Vector2 Out(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage));
        }

        static public Vector2 InOut(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage));
        }

        static public Vector3 In(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage),
                In(start.z, finish.z, percentage));
        }

        static public Vector3 Out(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage),
                Out(start.z, finish.z, percentage));
        }

        static public Vector3 InOut(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage),
                InOut(start.z, finish.z, percentage));
        }

        static public Color In(Color start, Color finish, float percentage)
        {
            return new Color(
                In(start.r, finish.r, percentage),
                In(start.g, finish.g, percentage),
                In(start.b, finish.b, percentage),
                In(start.a, finish.a, percentage)
                );
        }

        static public Color Out(Color start, Color finish, float percentage)
        {
            return new Color(
                Out(start.r, finish.r, percentage),
                Out(start.g, finish.g, percentage),
                Out(start.b, finish.b, percentage),
                Out(start.a, finish.a, percentage)
                );
        }

        static public Color InOut(Color start, Color finish, float percentage)
        {
            return new Color(
                InOut(start.r, finish.r, percentage),
                InOut(start.g, finish.g, percentage),
                InOut(start.b, finish.b, percentage),
                InOut(start.a, finish.a, percentage)
                );
        }
        
        static public Automator InAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
    };

    static public class Sinusoidal
    {
        static public float In(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * (1f - Mathf.Cos(percentage * Mathf.PI / 2f));
        }

        static public float Out(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * (Mathf.Sin(percentage * Mathf.PI / 2f));
        }

        static public float InOut(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * (0.5f * (1f - Mathf.Cos(Mathf.PI * percentage)));
        }

        static public Vector2 In(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage));
        }

        static public Vector2 Out(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage));
        }

        static public Vector2 InOut(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage));
        }

        static public Vector3 In(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage),
                In(start.z, finish.z, percentage));
        }

        static public Vector3 Out(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage),
                Out(start.z, finish.z, percentage));
        }

        static public Vector3 InOut(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage),
                InOut(start.z, finish.z, percentage));
        }

        static public Color In(Color start, Color finish, float percentage)
        {
            return new Color(
                In(start.r, finish.r, percentage),
                In(start.g, finish.g, percentage),
                In(start.b, finish.b, percentage),
                In(start.a, finish.a, percentage)
                );
        }

        static public Color Out(Color start, Color finish, float percentage)
        {
            return new Color(
                Out(start.r, finish.r, percentage),
                Out(start.g, finish.g, percentage),
                Out(start.b, finish.b, percentage),
                Out(start.a, finish.a, percentage)
                );
        }

        static public Color InOut(Color start, Color finish, float percentage)
        {
            return new Color(
                InOut(start.r, finish.r, percentage),
                InOut(start.g, finish.g, percentage),
                InOut(start.b, finish.b, percentage),
                InOut(start.a, finish.a, percentage)
                );
        }
        
        static public Automator InAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
    };

    static public class Exponential
    {
        static public float In(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * (percentage == 0f ? 0f : Mathf.Pow(1024f, percentage - 1f));
        }

        static public float Out(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * (percentage == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * percentage));
        }

        static public float InOut(float start, float finish, float percentage)
        {
            float difference = finish - start;
            if(percentage == 0f) return start;
            if(percentage == 1f) return finish;
            if ((percentage *= 2f) < 1f) return start + difference * (0.5f * Mathf.Pow(1024f, percentage - 1f));
            return start + difference * (0.5f * (-Mathf.Pow(2f, -10f * (percentage - 1f)) + 2f));
        }

        static public Vector2 In(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage));
        }

        static public Vector2 Out(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage));
        }

        static public Vector2 InOut(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage));
        }

        static public Vector3 In(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage),
                In(start.z, finish.z, percentage));
        }

        static public Vector3 Out(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage),
                Out(start.z, finish.z, percentage));
        }

        static public Vector3 InOut(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage),
                InOut(start.z, finish.z, percentage));
        }

        static public Color In(Color start, Color finish, float percentage)
        {
            return new Color(
                In(start.r, finish.r, percentage),
                In(start.g, finish.g, percentage),
                In(start.b, finish.b, percentage),
                In(start.a, finish.a, percentage)
                );
        }

        static public Color Out(Color start, Color finish, float percentage)
        {
            return new Color(
                Out(start.r, finish.r, percentage),
                Out(start.g, finish.g, percentage),
                Out(start.b, finish.b, percentage),
                Out(start.a, finish.a, percentage)
                );
        }

        static public Color InOut(Color start, Color finish, float percentage)
        {
            return new Color(
                InOut(start.r, finish.r, percentage),
                InOut(start.g, finish.g, percentage),
                InOut(start.b, finish.b, percentage),
                InOut(start.a, finish.a, percentage)
                );
        }
        
        static public Automator InAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
    };

    static public class Circular
    {
        static public float In(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * (1f - Mathf.Sqrt(1f - percentage * percentage));
        }

        static public float Out(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * (Mathf.Sqrt(1f - ((percentage -= 1f) * percentage)));
        }

        static public float InOut(float start, float finish, float percentage)
        {
            float difference = finish - start;
            if ((percentage *= 2f) < 1f) return start + difference * (-0.5f * (Mathf.Sqrt(1f - percentage * percentage) - 1));
            return start + difference * (0.5f * (Mathf.Sqrt(1f - (percentage -= 2f) * percentage) + 1f));
        }

        static public Vector2 In(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage));
        }

        static public Vector2 Out(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage));
        }

        static public Vector2 InOut(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage));
        }

        static public Vector3 In(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage),
                In(start.z, finish.z, percentage));
        }

        static public Vector3 Out(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage),
                Out(start.z, finish.z, percentage));
        }

        static public Vector3 InOut(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage),
                InOut(start.z, finish.z, percentage));
        }

        static public Color In(Color start, Color finish, float percentage)
        {
            return new Color(
                In(start.r, finish.r, percentage),
                In(start.g, finish.g, percentage),
                In(start.b, finish.b, percentage),
                In(start.a, finish.a, percentage)
                );
        }

        static public Color Out(Color start, Color finish, float percentage)
        {
            return new Color(
                Out(start.r, finish.r, percentage),
                Out(start.g, finish.g, percentage),
                Out(start.b, finish.b, percentage),
                Out(start.a, finish.a, percentage)
                );
        }

        static public Color InOut(Color start, Color finish, float percentage)
        {
            return new Color(
                InOut(start.r, finish.r, percentage),
                InOut(start.g, finish.g, percentage),
                InOut(start.b, finish.b, percentage),
                InOut(start.a, finish.a, percentage)
                );
        }
        
        static public Automator InAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
    };

    static public class Elastic
    {
        static public float In(float start, float finish, float percentage)
        {
            if (percentage == 0) return 0;
            if (percentage == 1) return 1;
            float difference = finish - start;
            return start + difference * (-Mathf.Pow(2f, 10f * (percentage -= 1f)) * Mathf.Sin((percentage - 0.1f) * (2f * Mathf.PI) / 0.4f));
        }

        static public float Out(float start, float finish, float percentage)
        {
            if (percentage == 0) return 0;
            if (percentage == 1) return 1;
            float difference = finish - start;
            return start + difference * (Mathf.Pow(2f, -10f * percentage) * Mathf.Sin((percentage - 0.1f) * (2f * Mathf.PI) / 0.4f) + 1f);
        }

        static public float InOut(float start, float finish, float percentage)
        {
            float difference = finish - start;
            if ((percentage *= 2f) < 1f) return start + difference * (-0.5f * Mathf.Pow(2f, 10f * (percentage -= 1f)) * Mathf.Sin((percentage - 0.1f) * (2f * Mathf.PI) / 0.4f));
            return start + difference * (Mathf.Pow(2f, -10f * (percentage -= 1f)) * Mathf.Sin((percentage - 0.1f) * (2f * Mathf.PI) / 0.4f) * 0.5f + 1f);
        }

        static public Vector2 In(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage));
        }

        static public Vector2 Out(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage));
        }

        static public Vector2 InOut(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage));
        }

        static public Vector3 In(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage),
                In(start.z, finish.z, percentage));
        }

        static public Vector3 Out(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage),
                Out(start.z, finish.z, percentage));
        }

        static public Vector3 InOut(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage),
                InOut(start.z, finish.z, percentage));
        }

        static public Color In(Color start, Color finish, float percentage)
        {
            return new Color(
                In(start.r, finish.r, percentage),
                In(start.g, finish.g, percentage),
                In(start.b, finish.b, percentage),
                In(start.a, finish.a, percentage)
                );
        }

        static public Color Out(Color start, Color finish, float percentage)
        {
            return new Color(
                Out(start.r, finish.r, percentage),
                Out(start.g, finish.g, percentage),
                Out(start.b, finish.b, percentage),
                Out(start.a, finish.a, percentage)
                );
        }

        static public Color InOut(Color start, Color finish, float percentage)
        {
            return new Color(
                InOut(start.r, finish.r, percentage),
                InOut(start.g, finish.g, percentage),
                InOut(start.b, finish.b, percentage),
                InOut(start.a, finish.a, percentage)
                );
        }
        
        static public Automator InAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
    };

    public class Back
    {
        static float s = 1.70158f;
        static float s2 = 2.5949095f;

        static public float In(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * (percentage * percentage * ((s + 1f) * percentage - s));
        }

        static public float Out(float start, float finish, float percentage)
        {
            float difference = finish - start;
            return start + difference * ((percentage -= 1f) * percentage * ((s + 1f) * percentage + s) + 1f);
        }

        static public float InOut(float start, float finish, float percentage)
        {
            float difference = finish - start;
            if ((percentage *= 2f) < 1f) return start + difference * (0.5f * (percentage * percentage * ((s2 + 1f) * percentage - s2)));
            return start + difference * (0.5f * ((percentage -= 2f) * percentage * ((s2 + 1f) * percentage + s2) + 2f));
        }

        static public Vector2 In(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage));
        }

        static public Vector2 Out(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage));
        }

        static public Vector2 InOut(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage));
        }

        static public Vector3 In(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage),
                In(start.z, finish.z, percentage));
        }

        static public Vector3 Out(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage),
                Out(start.z, finish.z, percentage));
        }

        static public Vector3 InOut(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage),
                InOut(start.z, finish.z, percentage));
        }

        static public Color In(Color start, Color finish, float percentage)
        {
            return new Color(
                In(start.r, finish.r, percentage),
                In(start.g, finish.g, percentage),
                In(start.b, finish.b, percentage),
                In(start.a, finish.a, percentage)
                );
        }

        static public Color Out(Color start, Color finish, float percentage)
        {
            return new Color(
                Out(start.r, finish.r, percentage),
                Out(start.g, finish.g, percentage),
                Out(start.b, finish.b, percentage),
                Out(start.a, finish.a, percentage)
                );
        }

        static public Color InOut(Color start, Color finish, float percentage)
        {
            return new Color(
                InOut(start.r, finish.r, percentage),
                InOut(start.g, finish.g, percentage),
                InOut(start.b, finish.b, percentage),
                InOut(start.a, finish.a, percentage)
                );
        }
        
        static public Automator InAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
    };

    public class Bounce
    {
		
        static public float In(float start, float finish, float percentage)
        {
			return Out(finish, start, 1.0f - percentage);
        }

        static public float Out(float start, float finish, float percentage)
        {
            float difference = finish - start;
            if (percentage < (1f / 2.75f))
            {
                return start + difference * (7.5625f * percentage * percentage);
            }
            else if (percentage < (2f / 2.75f))
            {
                return start + difference * (7.5625f * (percentage -= (1.5f / 2.75f)) * percentage + 0.75f);
            }
            else if (percentage < (2.5f / 2.75f))
            {
                return start + difference * (7.5625f * (percentage -= (2.25f / 2.75f)) * percentage + 0.9375f);
            }
            else {
                return start + difference * (7.5625f * (percentage -= (2.625f / 2.75f)) * percentage + 0.984375f);
            }
        }


        static public float InOut(float start, float finish, float percentage)
        {
            float difference = finish - start;
			if (percentage < 0.5f) return (In(start, finish-(difference/2.0f), percentage * 2f));
			return (Out(start+(difference/2.0f), finish, percentage * 2f - 1f));
        }

        static public Vector2 In(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage));
        }

        static public Vector2 Out(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage));
        }

        static public Vector2 InOut(Vector2 start, Vector2 finish, float percentage)
        {
            return new Vector2(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage));
        }

        static public Vector3 In(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                In(start.x, finish.x, percentage),
                In(start.y, finish.y, percentage),
                In(start.z, finish.z, percentage));
        }

        static public Vector3 Out(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                Out(start.x, finish.x, percentage),
                Out(start.y, finish.y, percentage),
                Out(start.z, finish.z, percentage));
        }

        static public Vector3 InOut(Vector3 start, Vector3 finish, float percentage)
        {
            return new Vector3(
                InOut(start.x, finish.x, percentage),
                InOut(start.y, finish.y, percentage),
                InOut(start.z, finish.z, percentage));
        }

        static public Color In(Color start, Color finish, float percentage)
        {
            return new Color(
                In(start.r, finish.r, percentage),
                In(start.g, finish.g, percentage),
                In(start.b, finish.b, percentage),
                In(start.a, finish.a, percentage)
                );
        }

        static public Color Out(Color start, Color finish, float percentage)
        {
            return new Color(
                Out(start.r, finish.r, percentage),
                Out(start.g, finish.g, percentage),
                Out(start.b, finish.b, percentage),
                Out(start.a, finish.a, percentage)
                );
        }

        static public Color InOut(Color start, Color finish, float percentage)
        {
            return new Color(
                InOut(start.r, finish.r, percentage),
                InOut(start.g, finish.g, percentage),
                InOut(start.b, finish.b, percentage),
                InOut(start.a, finish.a, percentage)
                );
        }
        
        static public Automator InAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(float start, float finish, float time, Action<float> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
        
        static public Automator InAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(In, start, finish, time, setValueFunction);
        }
        
        static public Automator OutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(Out, start, finish, time, setValueFunction);
        }
        
        static public Automator InOutAuto(Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            return new Automator(InOut, start, finish, time, setValueFunction);
        }
    };

    public enum EasingFunction
    {
        Linear,
        QuadraticIn,
        QuadraticOut,
        QuadraticInOut,
        CubicIn,
        CubicOut,
        CubicInOut,
        QuarticIn,
        QuarticOut,
        QuarticInOut,
        QuinticIn,
        QuinticOut,
        QuinticInOut,
        SinusoidalIn,
        SinusoidalOut,
        SinusoidalInOut,
        ExponentialIn,
        ExponentialOut,
        ExponentialInOut,
        CircularIn,
        CircularOut,
        CircularInOut,
        ElasticIn,
        ElasticOut,
        ElasticInOut,
        BackIn,
        BackOut,
        BackInOut,
        BounceIn,
        BounceOut,
        BounceInOut
    }

    static public float Ease(float start, float finish, float percentage, EasingFunction function)
    {
        return GetAssociatedEaseFunctionFloat(function).Invoke(start, finish, percentage);
    }

    static public Vector2 Ease(Vector2 start, Vector2 finish, float percentage, EasingFunction function)
    {
        return GetAssociatedEaseFunctionVector2(function).Invoke(start, finish, percentage);
    }

    static public Vector3 Ease(Vector3 start, Vector3 finish, float percentage, EasingFunction function)
    {
        return GetAssociatedEaseFunctionVector3(function).Invoke(start, finish, percentage);
    }

    static public Color Ease(Color start, Color finish, float percentage, EasingFunction function)
    {
        return GetAssociatedEaseFunctionColor(function).Invoke(start, finish, percentage);
    }

    static public float Ease(float start, float finish, float timeElapsed, float finalTime, EasingFunction function)
    {
        return Ease(start, finish, timeElapsed / finalTime, function);
    }

    static public Vector2 Ease(Vector2 start, Vector2 finish, float timeElapsed, float finalTime, EasingFunction function)
    {
        return Ease(start, finish, timeElapsed / finalTime, function);
    }

    static public Vector3 Ease(Vector3 start, Vector3 finish, float timeElapsed, float finalTime, EasingFunction function)
    {
        return Ease(start, finish, timeElapsed / finalTime, function);
    }

    static public Color Ease(Color start, Color finish, float timeElapsed, float finalTime, EasingFunction function)
    {
        return Ease(start, finish, timeElapsed / finalTime, function);
    }

    static private Func<float,float,float,float> GetAssociatedEaseFunctionFloat(EasingFunction function)
    {
        switch(function)
        {
            case EasingFunction.Linear:
                return Linear.InOut;
            case EasingFunction.QuadraticIn:
                return Quadratic.In;
            case EasingFunction.QuadraticOut:
                return Quadratic.Out;
            case EasingFunction.QuadraticInOut:
                return Quadratic.InOut;
            case EasingFunction.CubicIn:
                return Cubic.In;
            case EasingFunction.CubicOut:
                return Cubic.Out;
            case EasingFunction.CubicInOut:
                return Cubic.InOut;
            case EasingFunction.QuarticIn:
                return Quartic.In;
            case EasingFunction.QuarticOut:
                return Quartic.Out;
            case EasingFunction.QuarticInOut:
                return Quartic.InOut;
            case EasingFunction.QuinticIn:
                return Quintic.In;
            case EasingFunction.QuinticOut:
                return Quintic.Out;
            case EasingFunction.QuinticInOut:
                return Quintic.InOut;
            case EasingFunction.SinusoidalIn:
                return Sinusoidal.In;
            case EasingFunction.SinusoidalOut:
                return Sinusoidal.Out;
            case EasingFunction.SinusoidalInOut:
                return Sinusoidal.InOut;
            case EasingFunction.ExponentialIn:
                return Exponential.In;
            case EasingFunction.ExponentialOut:
                return Exponential.Out;
            case EasingFunction.ExponentialInOut:
                return Exponential.InOut;
            case EasingFunction.CircularIn:
                return Circular.In;
            case EasingFunction.CircularOut:
                return Circular.Out;
            case EasingFunction.CircularInOut:
                return Circular.InOut;
            case EasingFunction.ElasticIn:
                return Elastic.In;
            case EasingFunction.ElasticOut:
                return Elastic.Out;
            case EasingFunction.ElasticInOut:
                return Elastic.InOut;
            case EasingFunction.BackIn:
                return Back.In;
            case EasingFunction.BackOut:
                return Back.Out;
            case EasingFunction.BackInOut:
                return Back.InOut;
            case EasingFunction.BounceIn:
                return Bounce.In;
            case EasingFunction.BounceOut:
                return Bounce.Out;
            case EasingFunction.BounceInOut:
                return Bounce.InOut;
            default:
                throw new ArgumentOutOfRangeException(nameof(function), function, null);
        }
    }
    
    static private Func<Vector2,Vector2,float,Vector2> GetAssociatedEaseFunctionVector2(EasingFunction function)
    {
        switch(function)
        {
            case EasingFunction.Linear:
                return Linear.InOut;
            case EasingFunction.QuadraticIn:
                return Quadratic.In;
            case EasingFunction.QuadraticOut:
                return Quadratic.Out;
            case EasingFunction.QuadraticInOut:
                return Quadratic.InOut;
            case EasingFunction.CubicIn:
                return Cubic.In;
            case EasingFunction.CubicOut:
                return Cubic.Out;
            case EasingFunction.CubicInOut:
                return Cubic.InOut;
            case EasingFunction.QuarticIn:
                return Quartic.In;
            case EasingFunction.QuarticOut:
                return Quartic.Out;
            case EasingFunction.QuarticInOut:
                return Quartic.InOut;
            case EasingFunction.QuinticIn:
                return Quintic.In;
            case EasingFunction.QuinticOut:
                return Quintic.Out;
            case EasingFunction.QuinticInOut:
                return Quintic.InOut;
            case EasingFunction.SinusoidalIn:
                return Sinusoidal.In;
            case EasingFunction.SinusoidalOut:
                return Sinusoidal.Out;
            case EasingFunction.SinusoidalInOut:
                return Sinusoidal.InOut;
            case EasingFunction.ExponentialIn:
                return Exponential.In;
            case EasingFunction.ExponentialOut:
                return Exponential.Out;
            case EasingFunction.ExponentialInOut:
                return Exponential.InOut;
            case EasingFunction.CircularIn:
                return Circular.In;
            case EasingFunction.CircularOut:
                return Circular.Out;
            case EasingFunction.CircularInOut:
                return Circular.InOut;
            case EasingFunction.ElasticIn:
                return Elastic.In;
            case EasingFunction.ElasticOut:
                return Elastic.Out;
            case EasingFunction.ElasticInOut:
                return Elastic.InOut;
            case EasingFunction.BackIn:
                return Back.In;
            case EasingFunction.BackOut:
                return Back.Out;
            case EasingFunction.BackInOut:
                return Back.InOut;
            case EasingFunction.BounceIn:
                return Bounce.In;
            case EasingFunction.BounceOut:
                return Bounce.Out;
            case EasingFunction.BounceInOut:
                return Bounce.InOut;
            default:
                throw new ArgumentOutOfRangeException(nameof(function), function, null);
        }
    }
    
    static private Func<Vector3,Vector3,float,Vector3> GetAssociatedEaseFunctionVector3(EasingFunction function)
    {
        switch(function)
        {
            case EasingFunction.Linear:
                return Linear.InOut;
            case EasingFunction.QuadraticIn:
                return Quadratic.In;
            case EasingFunction.QuadraticOut:
                return Quadratic.Out;
            case EasingFunction.QuadraticInOut:
                return Quadratic.InOut;
            case EasingFunction.CubicIn:
                return Cubic.In;
            case EasingFunction.CubicOut:
                return Cubic.Out;
            case EasingFunction.CubicInOut:
                return Cubic.InOut;
            case EasingFunction.QuarticIn:
                return Quartic.In;
            case EasingFunction.QuarticOut:
                return Quartic.Out;
            case EasingFunction.QuarticInOut:
                return Quartic.InOut;
            case EasingFunction.QuinticIn:
                return Quintic.In;
            case EasingFunction.QuinticOut:
                return Quintic.Out;
            case EasingFunction.QuinticInOut:
                return Quintic.InOut;
            case EasingFunction.SinusoidalIn:
                return Sinusoidal.In;
            case EasingFunction.SinusoidalOut:
                return Sinusoidal.Out;
            case EasingFunction.SinusoidalInOut:
                return Sinusoidal.InOut;
            case EasingFunction.ExponentialIn:
                return Exponential.In;
            case EasingFunction.ExponentialOut:
                return Exponential.Out;
            case EasingFunction.ExponentialInOut:
                return Exponential.InOut;
            case EasingFunction.CircularIn:
                return Circular.In;
            case EasingFunction.CircularOut:
                return Circular.Out;
            case EasingFunction.CircularInOut:
                return Circular.InOut;
            case EasingFunction.ElasticIn:
                return Elastic.In;
            case EasingFunction.ElasticOut:
                return Elastic.Out;
            case EasingFunction.ElasticInOut:
                return Elastic.InOut;
            case EasingFunction.BackIn:
                return Back.In;
            case EasingFunction.BackOut:
                return Back.Out;
            case EasingFunction.BackInOut:
                return Back.InOut;
            case EasingFunction.BounceIn:
                return Bounce.In;
            case EasingFunction.BounceOut:
                return Bounce.Out;
            case EasingFunction.BounceInOut:
                return Bounce.InOut;
            default:
                throw new ArgumentOutOfRangeException(nameof(function), function, null);
        }
    }
    
    static private Func<Color,Color,float,Color> GetAssociatedEaseFunctionColor(EasingFunction function)
    {
        switch(function)
        {
            case EasingFunction.Linear:
                return Linear.InOut;
            case EasingFunction.QuadraticIn:
                return Quadratic.In;
            case EasingFunction.QuadraticOut:
                return Quadratic.Out;
            case EasingFunction.QuadraticInOut:
                return Quadratic.InOut;
            case EasingFunction.CubicIn:
                return Cubic.In;
            case EasingFunction.CubicOut:
                return Cubic.Out;
            case EasingFunction.CubicInOut:
                return Cubic.InOut;
            case EasingFunction.QuarticIn:
                return Quartic.In;
            case EasingFunction.QuarticOut:
                return Quartic.Out;
            case EasingFunction.QuarticInOut:
                return Quartic.InOut;
            case EasingFunction.QuinticIn:
                return Quintic.In;
            case EasingFunction.QuinticOut:
                return Quintic.Out;
            case EasingFunction.QuinticInOut:
                return Quintic.InOut;
            case EasingFunction.SinusoidalIn:
                return Sinusoidal.In;
            case EasingFunction.SinusoidalOut:
                return Sinusoidal.Out;
            case EasingFunction.SinusoidalInOut:
                return Sinusoidal.InOut;
            case EasingFunction.ExponentialIn:
                return Exponential.In;
            case EasingFunction.ExponentialOut:
                return Exponential.Out;
            case EasingFunction.ExponentialInOut:
                return Exponential.InOut;
            case EasingFunction.CircularIn:
                return Circular.In;
            case EasingFunction.CircularOut:
                return Circular.Out;
            case EasingFunction.CircularInOut:
                return Circular.InOut;
            case EasingFunction.ElasticIn:
                return Elastic.In;
            case EasingFunction.ElasticOut:
                return Elastic.Out;
            case EasingFunction.ElasticInOut:
                return Elastic.InOut;
            case EasingFunction.BackIn:
                return Back.In;
            case EasingFunction.BackOut:
                return Back.Out;
            case EasingFunction.BackInOut:
                return Back.InOut;
            case EasingFunction.BounceIn:
                return Bounce.In;
            case EasingFunction.BounceOut:
                return Bounce.Out;
            case EasingFunction.BounceInOut:
                return Bounce.InOut;
            default:
                throw new ArgumentOutOfRangeException(nameof(function), function, null);
        }
    }
    

    public class Automator
    {
        
        public Coroutine coroutine { get; private set; }
        public bool isRunning { get; private set; } = true;
        public float time { get; private set; }
        public float elapsedTime { get; private set; } = 0.0f;
        public float progress => Mathf.Clamp01(elapsedTime/time);
        public Action onEasingFinished;
        
        
        private bool m_IsSet = false;
        
        //Float Constructor
        public Automator(Func<float, float, float, float> easingFunction, float start, float finish, float time, Action<float> setValueFunction)
        {
            if(m_IsSet) return;
            
            this.time = Mathf.Max(0.0f, time);
            
            //Null Check
            if(easingFunction == null)
            {
                Debug.LogError("easingFunction parameter cannot be null.");
                isRunning = false;
                return;
            }
            if(setValueFunction == null)
            {
                Debug.LogError("setValueFunction parameter cannot be null.");
                isRunning = false;
                return;
            }

            coroutine = Co.Run(DoEasing(easingFunction, start, finish, setValueFunction));

            m_IsSet = true;
        }
        
        //Vector2 Constructor
        public Automator(Func<Vector2, Vector2, float, Vector2> easingFunction, Vector2 start, Vector2 finish, float time, Action<Vector2> setValueFunction)
        {
            if(m_IsSet) return;
            
            this.time = Mathf.Max(0.0f, time);
            
            //Null Check
            if(easingFunction == null)
            {
                Debug.LogError("easingFunction parameter cannot be null.");
                isRunning = false;
                return;
            }
            if(setValueFunction == null)
            {
                Debug.LogError("setValueFunction parameter cannot be null.");
                isRunning = false;
                return;
            }

            coroutine = Co.Run(DoEasing(easingFunction, start, finish, setValueFunction));

            m_IsSet = true;
        }
        
        //Vector3 Constructor
        public Automator(Func<Vector3, Vector3, float, Vector3> easingFunction, Vector3 start, Vector3 finish, float time, Action<Vector3> setValueFunction)
        {
            if(m_IsSet) return;
            
            this.time = Mathf.Max(0.0f, time);
            
            //Null Check
            if(easingFunction == null)
            {
                Debug.LogError("easingFunction parameter cannot be null.");
                isRunning = false;
                return;
            }
            if(setValueFunction == null)
            {
                Debug.LogError("setValueFunction parameter cannot be null.");
                isRunning = false;
                return;
            }

            coroutine = Co.Run(DoEasing(easingFunction, start, finish, setValueFunction));

            m_IsSet = true;
        }
        
        //Color Constructor
        public Automator(Func<Color, Color, float, Color> easingFunction, Color start, Color finish, float time, Action<Color> setValueFunction)
        {
            if(m_IsSet) return;
            
            this.time = Mathf.Max(0.0f, time);
            
            //Null Check
            if(easingFunction == null)
            {
                Debug.LogError("easingFunction parameter cannot be null.");
                isRunning = false;
                return;
            }
            if(setValueFunction == null)
            {
                Debug.LogError("setValueFunction parameter cannot be null.");
                isRunning = false;
                return;
            }

            coroutine = Co.Run(DoEasing(easingFunction, start, finish, setValueFunction));

            m_IsSet = true;
        }
        
        //Enum Float Constructor
        public Automator(EasingFunction easingFunction, float start, float finish, float time, Action<float> setValueFunction)
        {
            if(m_IsSet) return;

            this.time = Mathf.Max(0.0f, time);
            
            //Null Check
            if(setValueFunction == null)
            {
                Debug.LogError("setValueFunction parameter cannot be null.");
                isRunning = false;
                return;
            }
            
            coroutine = Co.Run(DoEasing(GetAssociatedEaseFunctionFloat(easingFunction), start, finish, setValueFunction));
            
            m_IsSet = true;
        }

        public void Stop()
        {
            if(!isRunning) return;
            
            Co.Stop(coroutine);
            isRunning = false;
            onEasingFinished?.Invoke();
            CleanUp();
        }

        private void CleanUp()
        {
            onEasingFinished = null;
            coroutine = null;
        }
        
        //Float iterator
        private IEnumerator DoEasing(Func<float, float, float, float> func, float start, float finish, Action<float> setValueFunction)
        {
            if(Mathf.Approximately(time,0.0f))
            {
                setValueFunction.Invoke(finish);
            }
            
            while(elapsedTime < time)
            {
                elapsedTime = Mathf.Min(time, elapsedTime + Time.deltaTime);
                setValueFunction.Invoke(func.Invoke(start, finish, Mathf.Clamp01(elapsedTime/time)));
                yield return null;
            }

            isRunning = false;
            onEasingFinished?.Invoke();
            CleanUp();
        }
        
        //Vector2 iterator
        private IEnumerator DoEasing(Func<Vector2, Vector2, float, Vector2> func, Vector2 start, Vector2 finish, Action<Vector2> setValueFunction)
        {
            if(Mathf.Approximately(time,0.0f))
            {
                setValueFunction.Invoke(finish);
            }
            
            while(elapsedTime < time)
            {
                elapsedTime = Mathf.Min(time, elapsedTime + Time.deltaTime);
                setValueFunction.Invoke(func.Invoke(start, finish, Mathf.Clamp01(elapsedTime/time)));
                yield return null;
            }

            isRunning = false;
            onEasingFinished?.Invoke();
            CleanUp();
        }
        
        //Vector3 iterator
        private IEnumerator DoEasing(Func<Vector3, Vector3, float, Vector3> func, Vector3 start, Vector3 finish, Action<Vector3> setValueFunction)
        {
            if(Mathf.Approximately(time,0.0f))
            {
                setValueFunction.Invoke(finish);
            }
            
            while(elapsedTime < time)
            {
                elapsedTime = Mathf.Min(time, elapsedTime + Time.deltaTime);
                setValueFunction.Invoke(func.Invoke(start, finish, Mathf.Clamp01(elapsedTime/time)));
                yield return null;
            }

            isRunning = false;
            onEasingFinished?.Invoke();
            CleanUp();
        }
        
        //Color iterator
        private IEnumerator DoEasing(Func<Color, Color, float, Color> func, Color start, Color finish, Action<Color> setValueFunction)
        {
            if(Mathf.Approximately(time,0.0f))
            {
                setValueFunction.Invoke(finish);
            }
            
            while(elapsedTime < time)
            {
                elapsedTime = Mathf.Min(time, elapsedTime + Time.deltaTime);
                setValueFunction.Invoke(func.Invoke(start, finish, Mathf.Clamp01(elapsedTime/time)));
                yield return null;
            }

            isRunning = false;
            onEasingFinished?.Invoke();
            CleanUp();
        }
    }
}