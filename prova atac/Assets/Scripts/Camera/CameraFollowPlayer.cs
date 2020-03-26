using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Player player;

    public float smoothTime = 0.125f;
    public float facingOffset = 3;

    private Vector3 velocity = Vector3.zero;
    public Vector3 offset = new Vector3(0,0,-10);

   
    private void FixedUpdate()
    {
        Vector3 desiredPosition = player.transform.position + offset + (Vector3)player.lastDir * facingOffset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        transform.position = smoothedPosition;

        if (PixelPerfectMovement.active)
        {
            transform.position = PixelPerfectMovement.PixelPerfectClamp(smoothedPosition, 16);
        }
    }
}
