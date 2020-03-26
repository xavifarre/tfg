using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathFunctions : MonoBehaviour
{
    //Moviment accelerat exponencialment
    public static Vector3 EaseInExp(float t, Vector3 origin, Vector3 dest, float duration, int e)
    {
        t /= duration;
        return (dest - origin) * Mathf.Pow(t, e) + origin;
    }

    //Moviment desaccelerat exponencialment
    public static Vector3 EaseOutExp(float t, Vector3 origin, Vector3 dest, float duration, int e)
    {
        t /= duration;
        t--;
        return (dest - origin) * (Mathf.Pow(t, e) + 1) + origin;
    }

    //Desaccelerar velocitat exponencialment
    public static float EaseOutVelExp(float t, float vStart, float vEnd, float duration, int e)
    {
        t /= duration;
        t--;
        return (vEnd - vStart) * (Mathf.Pow(t, e) + 1) + vStart;
    }
}
