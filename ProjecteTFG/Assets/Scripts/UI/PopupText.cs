using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupText : MonoBehaviour
{
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length);
    }

    public void SetText(string s)
    {
        transform.GetChild(0).GetComponent<Text>().text = s;
    }
}
