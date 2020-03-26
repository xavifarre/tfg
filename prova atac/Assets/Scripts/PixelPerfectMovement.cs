using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelPerfectMovement
{
    public static bool active = true;
    public static Vector3 PixelPerfectClamp(Vector3 moveVector, float pixelsPerUnit)
    {
        Vector3 vectorInPixels = new Vector3(
            Mathf.RoundToInt(moveVector.x * pixelsPerUnit),
            Mathf.RoundToInt(moveVector.y * pixelsPerUnit),
            moveVector.z);

        //Debug.Log("Clamp X: " + vectorInPixels.x + " into " + vectorInPixels.x / pixelsPerUnit);
        //Debug.Log("Clamp Y: " + vectorInPixels.y + " into " + vectorInPixels.y / pixelsPerUnit);

        Vector3 vectorClamped = new Vector3(vectorInPixels.x / pixelsPerUnit, vectorInPixels.y / pixelsPerUnit, moveVector.z);

        return vectorClamped;
    }
}
