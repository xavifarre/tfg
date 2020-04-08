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

    //Accelerar float exponencialment
    public static float EaseInExp(float t, float vStart, float vEnd, float duration, int e)
    {
        t /= duration;
        return (vEnd - vStart) * Mathf.Pow(t, e) + vStart;
    }

    //Moviment desaccelerat exponencialment
    public static Vector3 EaseOutExp(float t, Vector3 origin, Vector3 dest, float duration, int e)
    {
        t /= duration;
        t--;
        return (dest - origin) * (Mathf.Pow(t, e) + 1) + origin;
    }

    //Desaccelerar float exponencialment
    public static float EaseOutExp(float t, float vStart, float vEnd, float duration, int e)
    {
        t /= duration;
        t--;
        return (vEnd - vStart) * (Mathf.Pow(t, e) + 1) + vStart;
    }

    //Retorna un index aleatori dins d'una llista de probabilitats
    //Retorna -1 en cas d'error
    public static int RandomProbability(List<float> weightList)
    {
        int index = -1;
        float rand = Random.Range(0, 10000) / 10000f;
        float sum = 0;

        for (int i = 0; i < weightList.Count; i++)
        {
            sum += weightList[i];
            if (sum >= rand)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    /*
    * Aproxima la direcció "dir" a la direcció en 90º més propera, en forma d'enter 0-3
    * 0 -> Dreta
    * 1 -> Amunt
    * 2 -> Esquerra
    * 3 -> Abaix
    */
    public static int GetDirection(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
        {
            if (dir.x >= 0)
            {
                return 0;
            }
            else
            {
                return 2;
            }
        }
        else
        {
            if (dir.y >= 0)
            {
                return 1;
            }
            else
            {
                return 3;
            }
        }
    }
}
