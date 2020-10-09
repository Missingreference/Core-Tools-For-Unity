using System;
using UnityEngine;

namespace Elanetic.Tools
{
    /// <summary>
    /// Functions taken from Tween.js - Licensed under the MIT license at https://github.com/sole/tween.js
    ///
    /// TODO: Test if GetAssociatedEaseFunctions are more performant using an array lookup than a switch.
    /// TODO: Implement basic function for each class that only takes a percentage as a paremeter like the source js code.
    /// TODO: Implement alternative function for each class that takes timeElapsed and finalTime similar to Ease functions.
    /// </summary>
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
                if((percentage *= 2f) < 1f) return start + difference * (0.5f * percentage * percentage);
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
                if((percentage *= 2f) < 1f) return start + difference * (0.5f * percentage * percentage * percentage);
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
                if((percentage *= 2f) < 1f) return start + difference * (0.5f * percentage * percentage * percentage * percentage);
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
                if((percentage *= 2f) < 1f) return start + difference * (0.5f * percentage * percentage * percentage * percentage * percentage);
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
                if((percentage *= 2f) < 1f) return start + difference * (0.5f * Mathf.Pow(1024f, percentage - 1f));
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
                if((percentage *= 2f) < 1f) return start + difference * (-0.5f * (Mathf.Sqrt(1f - percentage * percentage) - 1));
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
        };

        static public class Elastic
        {
            static public float In(float start, float finish, float percentage)
            {
                if(percentage == 0) return start;
                if(percentage == 1) return finish;
                float difference = finish - start;
                return start + difference * (-Mathf.Pow(2f, 10f * (percentage -= 1f)) * Mathf.Sin((percentage - 0.1f) * (2f * Mathf.PI) / 0.4f));
            }

            static public float Out(float start, float finish, float percentage)
            {
                if(percentage == 0) return start;
                if(percentage == 1) return finish;
                float difference = finish - start;
                return start + difference * (Mathf.Pow(2f, -10f * percentage) * Mathf.Sin((percentage - 0.1f) * (2f * Mathf.PI) / 0.4f) + 1f);
            }

            static public float InOut(float start, float finish, float percentage)
            {
                float difference = finish - start;
                if((percentage *= 2f) < 1f) return start + difference * (-0.5f * Mathf.Pow(2f, 10f * (percentage -= 1f)) * Mathf.Sin((percentage - 0.1f) * (2f * Mathf.PI) / 0.4f));
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
                if((percentage *= 2f) < 1f) return start + difference * (0.5f * (percentage * percentage * ((s2 + 1f) * percentage - s2)));
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
                if(percentage < (1f / 2.75f))
                {
                    return start + difference * (7.5625f * percentage * percentage);
                }
                else if(percentage < (2f / 2.75f))
                {
                    return start + difference * (7.5625f * (percentage -= (1.5f / 2.75f)) * percentage + 0.75f);
                }
                else if(percentage < (2.5f / 2.75f))
                {
                    return start + difference * (7.5625f * (percentage -= (2.25f / 2.75f)) * percentage + 0.9375f);
                }
                else
                {
                    return start + difference * (7.5625f * (percentage -= (2.625f / 2.75f)) * percentage + 0.984375f);
                }
            }


            static public float InOut(float start, float finish, float percentage)
            {
                float difference = finish - start;
                if(percentage < 0.5f) return (In(start, finish - (difference / 2.0f), percentage * 2f));
                return (Out(start + (difference / 2.0f), finish, percentage * 2f - 1f));
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
            return Ease(start, finish, Mathf.Clamp01(timeElapsed / finalTime), function);
        }

        static public Vector2 Ease(Vector2 start, Vector2 finish, float timeElapsed, float finalTime, EasingFunction function)
        {
            return Ease(start, finish, Mathf.Clamp01(timeElapsed / finalTime), function);
        }

        static public Vector3 Ease(Vector3 start, Vector3 finish, float timeElapsed, float finalTime, EasingFunction function)
        {
            return Ease(start, finish, Mathf.Clamp01(timeElapsed / finalTime), function);
        }

        static public Color Ease(Color start, Color finish, float timeElapsed, float finalTime, EasingFunction function)
        {
            return Ease(start, finish, Mathf.Clamp01(timeElapsed / finalTime), function);
        }

        static private Func<float, float, float, float> GetAssociatedEaseFunctionFloat(EasingFunction function)
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

        static private Func<Vector2, Vector2, float, Vector2> GetAssociatedEaseFunctionVector2(EasingFunction function)
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

        static private Func<Vector3, Vector3, float, Vector3> GetAssociatedEaseFunctionVector3(EasingFunction function)
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

        static private Func<Color, Color, float, Color> GetAssociatedEaseFunctionColor(EasingFunction function)
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
    }
}