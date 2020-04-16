using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowTintEffect : MonoBehaviour
{
    public SpriteRenderer target;
    public int colorFrames = 1;
    [Range(0, 1)]
    public float alpha = 1;
    public List<Color> colors = new List<Color> { new Color32(238, 130, 238, 255), new Color32(75, 0, 130, 255), new Color32(0, 0, 255, 255), new Color32(0, 255, 0, 255), new Color32(255, 255, 0, 255), new Color32(255, 127, 0, 255), new Color32(255, 0, 0, 255)};


    private int i,t;
    // Start is called before the first frame update
    void Start()
    {
        if (!target)
        {
            target = GetComponent<SpriteRenderer>();
        }
        i = Random.Range(0,colors.Count);
        t = 0;
    }

    // Update is called once per frame
    void Update()
    {
        t++;
  
        if(t >= colorFrames)
        {
            t = 0;
            i++;
            i %= colors.Count;
            target.color = new Color(colors[i].r, colors[i].g, colors[i].b, alpha);
        }
    }
}
