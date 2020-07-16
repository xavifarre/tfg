﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPopUp : MonoBehaviour
{
    public static ButtonPopUp instance;

    public Vector2 offset = new Vector2(1, 1);
    public List<GameObject> buttonObjList;
    public List<string> buttonNamesList;
    private Player player;
    private int currentId;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        instance = this;
    }

    private void Update()
    {
        if (!GameManager.instance.gamePaused)
        {
            if (currentId != -1)
            {
                transform.position = (Vector2)player.transform.position + offset;
                if (Input.GetButtonDown(buttonNamesList[currentId]))
                {
                    Hide();
                }
            }
        }
    }

    public void Show(string action)
    {
        Debug.Log(action);
        currentId = buttonNamesList.IndexOf(action);
        buttonObjList[currentId].SetActive(true);
        transform.position = (Vector2)player.transform.position + offset;
        
    }

    public void Hide()
    {
        buttonObjList[currentId].SetActive(false);
        currentId = -1;
    }

    public bool IsShowing()
    {
        return currentId != -1;
    }
}
