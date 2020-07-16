using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCanvas : MonoBehaviour
{
    public static PauseMenu menu;

    // Start is called before the first frame update
    void Start()
    {
        menu = transform.GetChild(0).GetComponent<PauseMenu>();
    }

    public static void Show()
    {
        menu.Show();
    }

    public static void Hide()
    {
        menu.Hide();
    }
}
