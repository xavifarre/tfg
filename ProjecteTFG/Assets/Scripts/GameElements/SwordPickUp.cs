using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordPickUp : MonoBehaviour, IInteractuableObject
{
    private Player player;
    private void Start()
    {
        player = FindObjectOfType<Player>();
    }
    public void Interact()
    {
        player.PickSword();
        ToriiLevelController.instance.PickSword();
        TutorialManager.instance.StartTutorial();
        Destroy(gameObject);
    }
}
