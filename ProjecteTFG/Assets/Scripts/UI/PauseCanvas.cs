using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCanvas : MonoBehaviour
{
    public static GameObject menu;

    // Start is called before the first frame update
    void Start()
    {
        menu = transform.GetChild(0).gameObject;
    }

    public static void Show()
    {
        menu.SetActive(true);
    }

    public static void Hide()
    {
        menu.SetActive(false);
    }
}
