using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopupTextController : MonoBehaviour
{
    private static PopupText popupText;
    private static GameObject worldCanvas;

    public static void Initialize()
    {
        if (!worldCanvas)
        {
            worldCanvas = GameObject.Find("WorldCanvas");
        }
        
        if (!popupText)
        {
            popupText = Resources.Load<PopupText>("UI/PopupDamage");
        }
    }

    public static void CreatePopupText(string s, Vector3 position)
    {
        PopupText instance = Instantiate(popupText, worldCanvas.transform);

        instance.transform.position = position;
        instance.SetText(s);
    }
}
