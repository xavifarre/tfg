using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopupTextController : MonoBehaviour
{
    private static PopupText popupDamage;
    private static PopupText popupDamageSelf;
    private static PopupText popupHeal;
    private static GameObject worldCanvas;

    public static void Initialize()
    {
        if (!worldCanvas)
        {
            worldCanvas = GameObject.Find("DamageNumbersCanvas");
        }
        
        if (!popupDamage)
        {
            popupDamage = Resources.Load<PopupText>("UI/PopupDamage");
        }
        if (!popupDamageSelf)
        {
            popupDamageSelf = Resources.Load<PopupText>("UI/PopupDamageSelf");
        }

        if (!popupHeal)
        {
            popupHeal = Resources.Load<PopupText>("UI/PopupHeal");
        }
    }

    public static void CreatePopupTextDamage(string s, Vector3 position)
    {
        PopupText instance = Instantiate(popupDamage, worldCanvas.transform);

        instance.transform.position = position;
        instance.SetText(s);
    }

    public static void CreatePopupTextDamageSelf(string s, Vector3 position)
    {
        PopupText instance = Instantiate(popupDamageSelf, worldCanvas.transform);

        instance.transform.position = position;
        instance.SetText(s);
    }

    public static void CreatePopupTextHeal(string s, Vector3 position)
    {
        PopupText instance = Instantiate(popupHeal, worldCanvas.transform);

        instance.transform.position = position;
        instance.SetText("+" + s);
    }
}
