using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPopUp : MonoBehaviour
{
    public Vector2 offset = new Vector2(1, 0);
    private IInteractuableObject currentObject;
    private Player player;

    public static InteractPopUp instance;

    private void Start()
    {
        instance = this;
        player = FindObjectOfType<Player>();
        Hide();
    }

    private void Update()
    {

        if (!GameManager.instance.gamePaused)
        {
            transform.position = (Vector2)player.transform.position + offset;
            if (Input.GetButtonDown("Interact"))
            {
                currentObject.Interact();
                gameObject.SetActive(false);
            }
        }
        
    }

    public void Show(IInteractuableObject interactuableObject)
    {
        gameObject.SetActive(true);
        transform.position = (Vector2)player.transform.position + offset;
        currentObject = interactuableObject;
        Debug.Log("hey");
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        currentObject = null;
    }
}
