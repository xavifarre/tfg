using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericInteractuableObject : MonoBehaviour, IInteractuableObject
{
    public Text text;
    public string dialogue;
    private Animator animator;

    private void Start()
    {
        animator = text.GetComponent<Animator>();
    }
    public void Interact()
    {
        Debug.Log("Interacted!");
        animator.SetTrigger("Show");
        text.text = dialogue;
    }

    public Vector2 GetPos()
    {
        return transform.position;
    }
}
