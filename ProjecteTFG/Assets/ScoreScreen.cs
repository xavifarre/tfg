using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreScreen : MonoBehaviour
{
    public Image selector;
    public List<RectTransform> items;
    public List<RectTransform> confirmItems;
    private int currentIndex = 1;
    private int previousIndex = 1;
    public float offsetX = 50;
    public float sizeY = 30;
    private float timeOffset = 0.2f;
    private float tPrevious = -1;
    public float audioIncrementValue = 0.5f;
    public float tBlink = 0.2f;
    private float t = 0;

    public GameObject mainMenu;
    public GameObject confirmMenu;
    public TextMeshProUGUI confirmText;

    private Action confirmAction;

    [Header("Stats")]
    public TextMeshProUGUI damageDealt;
    public TextMeshProUGUI kills;
    public TextMeshProUGUI damageReceived;
    public TextMeshProUGUI deaths;
    public TextMeshProUGUI crystals;
    public TextMeshProUGUI heal;
    public TextMeshProUGUI timeElapsed;

    private void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {

            if (Input.GetAxisRaw("Vertical") >= 0.8f && tPrevious >= timeOffset)
            {
                MoveUp();
            }
            else if (Input.GetAxisRaw("Vertical") <= -0.8f && tPrevious >= timeOffset)
            {
                MoveDown();
            }

            if (Input.GetAxisRaw("Horizontal") >= 0.8f && tPrevious >= timeOffset)
            {
                MoveDown();
            }
            else if (Input.GetAxisRaw("Horizontal") <= -0.8f && tPrevious >= timeOffset)
            {
                MoveUp();
            }


            if (mainMenu.activeSelf)
            {
                ControlMainMenu();
            }
            else if (confirmMenu.activeSelf)
            {
                ControlConfirm();
            }



            if (t >= tBlink && tBlink != -1)
            {
                t -= tBlink;
                selector.enabled = !selector.enabled;
            }

            t += Time.unscaledDeltaTime;

            tPrevious += Time.unscaledDeltaTime;
        }
    }

    private void Init()
    {
        currentIndex = 1;
        ShowMain();
       
        damageDealt.text = Globals.damageDealtCount.ToString();
        kills.text = Globals.killCount.ToString();
        damageReceived.text = Globals.damageReceivedCount.ToString();
        deaths.text = Globals.deathCount.ToString();
        heal.text = Globals.healCount.ToString();
        crystals.text = Globals.crystalCount.ToString();
        int minutes = Mathf.FloorToInt(Globals.totalTime / 1000 / 60);
        int seconds = Mathf.FloorToInt(Globals.totalTime / 1000) % 60;
        string sSeconds = seconds < 10 ? "0" + seconds : seconds.ToString();
        int mili = Mathf.FloorToInt(Globals.totalTime) % 1000;
        string sMili = mili.ToString();
        if(mili < 10)
        {
            sMili = "00" + mili;
        }
        else if(mili < 100)
        {
            sMili = "0" + mili;
        }
        timeElapsed.text = minutes + ":" + sSeconds + "." + sMili;
    }

    public void ShowMain()
    {
        tPrevious = timeOffset;
        currentIndex = previousIndex;
        mainMenu.SetActive(true);
        confirmMenu.SetActive(false);
        UpdateSelector();
    }

    public void ShowConfirm()
    {
        tPrevious = timeOffset;
        mainMenu.SetActive(false);
        confirmMenu.SetActive(true);
        confirmText.color = Color.white;
        confirmItems[0].GetComponent<TextMeshProUGUI>().color = Color.white;
        UpdateSelector();
    }

    private void MoveUp()
    {
        tPrevious = 0;
        List<RectTransform> currentList = mainMenu.activeSelf ? items : confirmItems;
        currentIndex = MathFunctions.Mod(currentIndex - 1, currentList.Count);
        if (mainMenu.activeSelf) previousIndex = currentIndex;
        UpdateSelector();
    }

    private void MoveDown()
    {
        tPrevious = 0;
        List<RectTransform> currentList = mainMenu.activeSelf ? items : confirmItems;
        currentIndex = MathFunctions.Mod(currentIndex + 1, currentList.Count);
        if (mainMenu.activeSelf) previousIndex = currentIndex;
        UpdateSelector();
    }

    private void UpdateSelector()
    {
        List<RectTransform> currentList = mainMenu.activeSelf ? items : confirmItems;
        GameObject currentObject = mainMenu.activeSelf ? mainMenu : confirmMenu;
        RectTransform rect = currentList[currentIndex];
        selector.rectTransform.anchoredPosition = rect.anchoredPosition + (Vector2)currentObject.transform.localPosition;
        selector.rectTransform.sizeDelta = new Vector2(rect.sizeDelta.x + offsetX, sizeY);
    }

    public void ControlMainMenu()
    {
        if (Input.GetButtonDown("Interact"))
        {
            ClickOptionMainMenu();
        }
    }

    public void ControlConfirm()
    {
        if (Input.GetButtonDown("Interact"))
        {
            ClickOptionConfirmMenu();
        }
    }

    private void ClickOptionMainMenu()
    {
        if (currentIndex == 0)
        {
            confirmAction = ConfirmRestartGame;
            confirmText.text = "Do you want to restart the game?";
            ShowConfirm();
        }
        else if (currentIndex == 1)
        {
            confirmAction = QuitGame;
            confirmText.text = "Do you want to close the game?";
            ShowConfirm();
        }
    }

    private void ClickOptionConfirmMenu()
    {
        if (currentIndex == 0)
        {
            confirmAction();
        }
        else if (currentIndex == 1)
        {
            Init();
        }
    }

    private void QuitGame()
    {
        Application.Quit();
    }


    private void ConfirmRestartGame()
    {
        SaveSystem.DeleteGame();
        Resume();
        SceneManager.LoadScene("GameLoader");
    }

    private void Resume()
    {
        tPrevious = 0;
        //GameManager.instance.ResumeGame();
    }
}
