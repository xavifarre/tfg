using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Player player;
    private int currentDir;
    private Collider2D col;

    public List<Vector2> offsetList;
    private List<IInteractuableObject> interactuableList;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.GetComponent<Player>();
        col = GetComponent<Collider2D>();
        interactuableList = new List<IInteractuableObject>();
    }

    // Update is called once per frame
    void Update()
    {
        currentDir = MathFunctions.GetDirection(player.lastDir);
        col.offset = offsetList[currentDir];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        interactuableList.Add(collision.GetComponent<IInteractuableObject>());
        InteractPopUp.instance.Show(interactuableList[interactuableList.Count-1]);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        interactuableList.Remove(collision.GetComponent<IInteractuableObject>());
        if (interactuableList.Count == 0)
        {
            InteractPopUp.instance.Hide();
        }
        else
        {
            InteractPopUp.instance.Show(interactuableList[0]);
        }
    }
}
