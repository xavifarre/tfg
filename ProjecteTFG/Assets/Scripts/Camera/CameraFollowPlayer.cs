using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    private Player player;

    public bool followingPlayer = true;

    public float defaultSmoothTime = 0.125f;
    private float smoothTime;
    public float facingOffset = 3;

    private Vector3 velocity = Vector3.zero;
    public Vector3 offset = new Vector3(0,0,-10);
    private CameraManager cm;

    private Vector3 desiredPosition;
    

    private void Start()
    {
        cm = FindObjectOfType<CameraManager>();
        player = FindObjectOfType<Player>();

        smoothTime = defaultSmoothTime;
    }

    private void FixedUpdate()
    {
        if (followingPlayer)
        {
            desiredPosition = player.transform.position + offset + (Vector3)player.lastDir * facingOffset;

            desiredPosition = cm.ClampPositionOnArea(desiredPosition);
        }

        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        PixelPerfectMovement.Move(smoothedPosition, transform);
    }

    public void SetDestination(Vector3 dest, float smoothDuration = -1)
    {
        desiredPosition = dest + offset;
        
        //Stop following player
        followingPlayer = false;

        //Update smooth time
        if(smoothDuration == -1)
        {
            smoothTime = defaultSmoothTime;
        }
        else
        {
            smoothTime = smoothDuration;
        }
    }

    public void FollowPlayer(float smoothDuration = -1)
    {
        followingPlayer = true;

        //Update smooth time
        if (smoothDuration == -1)
        {
            smoothTime = defaultSmoothTime;
        }
        else
        {
            smoothTime = smoothDuration;
            
        }
        StartCoroutine(ResetSmoothTime(smoothTime));
    }

    private IEnumerator ResetSmoothTime(float t)
    {
        yield return new WaitForSeconds(t);
        smoothTime = defaultSmoothTime;
    }
}
