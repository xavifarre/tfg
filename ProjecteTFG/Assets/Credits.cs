﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            EndCredits();
        }
    }

    public void EndCredits()
    {
        SceneManager.LoadScene("StatsScreen");
    }
}