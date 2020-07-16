using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Image selector;
    public List<RectTransform> items;
    private int currentIndex = 0;
    public int defaultIndex = 0;
    public float offsetX = 50;
    public float sizeY = 30;
    private float timeOffset = 0.2f;
    private float tPrevious = -1;
    public float audioIncrementValue = 0.5f;
    public float tBlink = 0.2f;
    private float t = 0;

    public AudioMixer audioMixer;

    private List<Resolution> resolutions;

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
                ClickOption();
            }

            

            if(t >= tBlink && tBlink != -1)
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
        currentIndex = defaultIndex;
        tPrevious = timeOffset;
        UpdateSelector();
        UpdateTextFullScreen(Screen.fullScreen);
    }

    private void MoveUp()
    {
        tPrevious = 0;
        currentIndex = MathFunctions.Mod(currentIndex - 1, items.Count);
        UpdateSelector();
    }

    private void MoveDown()
    {
        tPrevious = 0;
        currentIndex = MathFunctions.Mod(currentIndex + 1, items.Count);
        UpdateSelector();
    }

    private void UpdateSelector()
    {
        RectTransform rect = items[currentIndex];
        selector.rectTransform.anchoredPosition = rect.anchoredPosition;
        selector.rectTransform.sizeDelta = new Vector2(rect.sizeDelta.x + offsetX, sizeY);
        if(currentIndex == 3)
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

    private void ClickOption()
    {
        if(currentIndex == 0)
        {
            ResolutionUp();
        }
        else if (currentIndex == 1)
        {
            ToggleFullScreen();
        }
        else if(currentIndex == 3)
        {
            Resume();
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
