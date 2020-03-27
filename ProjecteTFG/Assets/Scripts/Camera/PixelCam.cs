using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
[AddComponentMenu("Image Effects/PixelBoy")]
public class PixelCam : MonoBehaviour
{
    public int w = 720;
    private int h;

    private Camera cam;

    protected void Start()
    {
        cam = FindObjectOfType<Camera>();
    }
    void Update()
    {

        float ratio = ((float)cam.pixelHeight / (float)cam.pixelWidth);
        h = Mathf.RoundToInt(w * ratio);
        Debug.Log(h);
    }
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        source.filterMode = FilterMode.Point;
        RenderTexture buffer = RenderTexture.GetTemporary(w, h, -1);
        buffer.filterMode = FilterMode.Point;
        Graphics.Blit(source, buffer);
        Graphics.Blit(buffer, destination);
        RenderTexture.ReleaseTemporary(buffer);
    }
}