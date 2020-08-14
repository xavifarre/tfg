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
    private Vector3 realPos;

    private void Start()
    {
        cm = FindObjectOfType<CameraManager>();
        player = FindObjectOfType<Player>();
        if (followingPlayer)
        {
            transform.position = player.transform.position + offset;
        }
        realPos = transform.position;
        smoothTime = defaultSmoothTime;
    }

    private void FixedUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (followingPlayer)
        {
            desiredPosition = player.transform.position + offset + (Vector3)player.lastDir * facingOffset;

            desiredPosition = cm.ClampPositionOnArea(desiredPosition);
        }

        realPos = Vector3.SmoothDamp(realPos, desiredPosition, ref velocity, smoothTime);

        PixelPerfectMovement.Move(realPos, transform);
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
