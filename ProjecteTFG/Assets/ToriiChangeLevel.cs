using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ToriiChangeLevel : MonoBehaviour
{

    public float changeLevelDistance = 10f;
    public bool xAxis = true;
    public Image blackScreen;

    private Vector3 enterPoint; 

    private void Start()
    {

    }


    public void TransitionToLevel()
    {
        SceneManager.LoadScene("SummonerLevel");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        enterPoint = collision.transform.position;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Color c = blackScreen.color;
            float distance = Mathf.Abs(xAxis ? collision.transform.position.x - enterPoint.x : collision.transform.position.y - enterPoint.y);
            blackScreen.color = new Color(c.r, c.g, c.b, Mathf.Lerp(0, 1, distance/ changeLevelDistance));
            if(distance > changeLevelDistance)
            {
                TransitionToLevel();
            }
        }
    }
}
