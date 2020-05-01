using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public int currentArea;
    public CameraLimits[] cameraLimits;
    // Start is called before the first frame update
    void Start()
    {
        //cameraLimits = new CameraLimits[10];
        //foreach(CameraLimits cl in FindObjectsOfType<CameraLimits>())
        //{
        //    cameraLimits[cl.area] = cl;
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AreaChange(int area)
    {
        currentArea = area;
    }

    public Vector3 ClampPositionOnArea(Vector3 desiredPosition)
    {
        if (cameraLimits[currentArea])
        {
            if (cameraLimits[currentArea].box)
            {
                float clampedX = Mathf.Clamp(desiredPosition.x, cameraLimits[currentArea].limits[0].x, cameraLimits[currentArea].limits[1].x);
                float clampedY = Mathf.Clamp(desiredPosition.y, cameraLimits[currentArea].limits[1].y, cameraLimits[currentArea].limits[0].y);

                desiredPosition = new Vector3(clampedX, clampedY, desiredPosition.z);
            }
        }
        return desiredPosition;
    }

}
