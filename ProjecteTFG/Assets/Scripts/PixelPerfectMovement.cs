using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelPerfectMovement : MonoBehaviour
{

    public bool isActive = true;
    public int pixelsUnit = 16;

    public static bool active;
    public static int pixelsPerUnit;

    private void Update()
    {
        active = isActive;
        pixelsPerUnit = pixelsUnit;
    }

    public static Vector3 PixelPerfectClamp(Vector3 moveVector)
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

    //Move to position (rb)
    public static void Move(Vector3 position, Rigidbody2D rb)
    {
        if (active)
        {
            rb.MovePosition(PixelPerfectClamp(position));
        }
        else
        {
            rb.MovePosition(position);
        }
    }

    //Move to position (transform)
    public static void Move(Vector3 position, Transform transform)
    {
        if (active)
        {
            transform.position = PixelPerfectClamp(position);
        }
        else
        {
            transform.position = position;
        }

    }
}
