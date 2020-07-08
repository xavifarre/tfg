using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public static HealthBar instance;

    private int maxHealth;
    public Slider bar;
    public TextMeshProUGUI text;

    private void Start()
    {
        instance = this;
    }

    public static void Initialize(int maxHp, int hp)
    {
        instance.maxHealth = maxHp;
        instance.bar.maxValue = maxHp;
        instance.bar.value = hp;
        instance.text.text = hp + " / " + instance.maxHealth;
    }

    public static void UpdateBar(int hp)
    {
        instance.bar.value = hp;
        instance.text.text = hp + " / " + instance.maxHealth;
    }

    public static void Hide()
    {
        instance.gameObject.SetActive(false);
    }

}
