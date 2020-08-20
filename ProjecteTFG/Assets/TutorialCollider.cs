using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(Globals.gameState == GameState.SwordPicked)
        {
            TutorialManager.instance.StartTutorial();
        }
    }
}
