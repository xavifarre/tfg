using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Image selector;
    public List<RectTransform> items;
    public List<RectTransform> confirmItems;
    private int currentIndex = 0;
    private int previousIndex = 0;
    public float offsetX = 50;
    public float sizeY = 30;
    private float timeOffset = 0.2f;
    private float tPrevious = -1;
    public float audioIncrementValue = 0.5f;
    public float tBlink = 0.2f;
    private float t = 0;

    public AudioMixer audioMixer;

    private List<Resolution> resolutions;

    public GameObject mainMenu;
    public GameObject confirmMenu;
    public TextMeshProUGUI confirmText;

    private Action confirmAction;

    private Slider sliderVolume;
    private TMP_Dropdown resolutionDropdown;
    // Start is called before the first frame update
    void Start()
    {
        InitResolutions();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {

            if (Input.GetAxis("Vertical") >= 0.8f && tPrevious >= timeOffset)
            {
                MoveUp();
            }
            else if (Input.GetAxis("Vertical") <= -0.8f && tPrevious >= timeOffset)
            {
                MoveDown();
            }

            if (confirmMenu.activeSelf)
            {
                if (Input.GetAxis("Horizontal") >= 0.8f && tPrevious >= timeOffset)
                {
                    MoveDown();
                }
                else if (Input.GetAxis("Horizontal") <= -0.8f && tPrevious >= timeOffset)
                {
                    MoveUp();
                }
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

    public void Show()
    {
        Init();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Init()
    {
        ShowMain();
        UpdateTextFullScreen(Screen.fullScreen);
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
        currentIndex = 1;
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
        if(mainMenu.activeSelf) previousIndex = currentIndex;
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

        if(currentIndex >= 3 || confirmMenu.activeSelf)
        {
            selector.transform.GetChild(0).gameObject.SetActive(false);
            selector.transform.GetChild(1).gameObject.SetActive(false);
        }
        else 
        {
            selector.transform.GetChild(0).gameObject.SetActive(true);
            selector.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
    
    public void ControlMainMenu()
    {
        if (currentIndex == 0)
        {
            if (Input.GetAxis("Horizontal") >= 0.8f && tPrevious >= timeOffset)
            {
                ResolutionUp();
            }
            else if (Input.GetAxis("Horizontal") <= -0.8f && tPrevious >= timeOffset)
            {
                ResolutionDown();
            }
        }
        else if (currentIndex == 1)
        {
            if (Input.GetAxis("Horizontal") >= 0.8f && tPrevious >= timeOffset)
            {
                ToggleFullScreen();
            }
            else if (Input.GetAxis("Horizontal") <= -0.8f && tPrevious >= timeOffset)
            {
                ToggleFullScreen();
            }
        }
        else if (currentIndex == 2)
        {
            if (Input.GetAxis("Horizontal") >= 0.8f)
            {
                VolumeUp();
            }
            else if (Input.GetAxis("Horizontal") <= -0.8f)
            {
                VolumeDown();
            }
        }

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
        if(currentIndex == 0)
        {
            ResolutionUp();
        }
        else if (currentIndex == 1)
        {
            ToggleFullScreen();
        }
        else if (currentIndex == 3)
        {
            confirmAction = ReturnLobby;
            confirmText.text = "Do you want to return to lobby?";
            ShowConfirm();
        }
        else if (currentIndex == 4)
        {
            confirmAction = QuestionRestartGame;
            confirmText.text = "Do you want to restart the save file?";
            ShowConfirm();
        }
        else if(currentIndex == 5)
        {
            Resume();
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

    private void ResolutionUp()
    {
        tPrevious = 0;
        resolutionDropdown.value = MathFunctions.Mod(resolutionDropdown.value + 1, resolutionDropdown.options.Count);
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
    }

    private void ResolutionDown()
    {
        tPrevious = 0;
        resolutionDropdown.value = MathFunctions.Mod(resolutionDropdown.value - 1, resolutionDropdown.options.Count);
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
    }

    private void ToggleFullScreen()
    {
        tPrevious = 0;
        Screen.fullScreen = !Screen.fullScreen;
        UpdateTextFullScreen(!Screen.fullScreen);
    }

    private void VolumeUp()
    {
        tPrevious = 0;
        if (sliderVolume == null)
        {
            sliderVolume = items[2].GetComponent<Slider>();
        }

        sliderVolume.value += audioIncrementValue;
        audioMixer.SetFloat("Volume", sliderVolume.value);
    }

    private void VolumeDown()
    {
        tPrevious = 0;
        if (sliderVolume == null)
        {
            sliderVolume = items[2].GetComponent<Slider>();
        }
        sliderVolume.value -= audioIncrementValue;
        audioMixer.SetFloat("Volume", sliderVolume.value);
    }

    private void ReturnLobby()
    {
        Debug.Log("Return lobby");
        Resume();
    }

    private void QuestionRestartGame()
    {
        confirmAction = ConfirmRestartGame;
        confirmText.text = "Are you sure you want to delete all data? You cannot revert this action";
        ShowConfirm();
        confirmText.color = Color.red;
        confirmItems[0].GetComponent<TextMeshProUGUI>().color = Color.red;
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
        GameManager.instance.ResumeGame();
    }
    private void UpdateTextFullScreen(bool isFull)
    {
        items[1].GetComponent<TextMeshProUGUI>().text = isFull ? "Yes" : "No";
    }

    private void InitResolutions()
    {
        resolutionDropdown = items[0].GetComponent<TMP_Dropdown>();
        resolutionDropdown.ClearOptions();
        Resolution[] resolutionArray = Screen.resolutions;
        resolutions = new List<Resolution>();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutionArray.Length; i++)
        {
            string option = resolutionArray[i].width + "x" + resolutionArray[i].height;
            if (!options.Contains(option))
            {
                options.Add(option);
                resolutions.Add(resolutionArray[i]);

                if (resolutionArray[i].width == Screen.currentResolution.width && resolutionArray[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = resolutions.Count - 1;
                }
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
}
