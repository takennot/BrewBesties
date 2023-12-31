using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Canvas optionsCanvas;
    [SerializeField] private AudioClip buttonSelectAudio;
    [SerializeField] private AudioClip buttonClickAudio;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private PauseMenuScript pauseMenu;
    [SerializeField] public bool isInPause;
    [SerializeField] public List<Button> framerateButtons;
    // Start is called before the first frame update
    void Start()
    {
        switch (PlayerPrefs.GetInt("FullscreenMode"))
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
        Screen.SetResolution(PlayerPrefs.GetInt("Resolution Width", 1920), PlayerPrefs.GetInt("Resolution Height", 1080), Screen.fullScreenMode, Screen.currentResolution.refreshRateRatio);
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen, new RefreshRate() { numerator = Convert.ToUInt32(PlayerPrefs.GetInt("Refresh Rate", 60)), denominator = 1 });
        Invoke("SetStartVolume", 0.3f);
    }
    private void SetStartVolume()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.1f);

        // You can add additional actions here if needed
    }

    // Update is called once per frame
    void Update()
    {
        if (optionsCanvas.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                switch (isInPause)
                {
                    case true:
                        pauseMenu.OnBack();
                        settingsButton.Select();
                        break;
                    case false:
                        optionsCanvas.gameObject.SetActive(false);
                        settingsButton.Select();
                        break;
                }
                Debug.Log("OptionsBack");
            }
        }
    }
    // fullscreen modes
    // 0 - fullscreen
    // 1 - borderless
    // 2 - windowed
    public void SetFullscreenMode(int mode)
    {
        switch (mode)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                PlayerPrefs.SetInt("FullscreenMode", mode);
                PlayerPrefs.Save();
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                PlayerPrefs.SetInt("FullscreenMode", mode);
                PlayerPrefs.Save();
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                PlayerPrefs.SetInt("FullscreenMode", mode);
                PlayerPrefs.Save();
                break;
            default:
                goto case 2;
        }
    }
    // Resolutions with current fullscreen mode & refresh rate
    public void SetResolution(string resolution)
    {
        string[] split = resolution.Split("x"[0]);
        int width = int.Parse(split[0]);
        int height = int.Parse(split[1]);
        Screen.SetResolution(width, height, Screen.fullScreenMode, Screen.currentResolution.refreshRateRatio);
        PlayerPrefs.SetInt("Resolution Height", height);
        PlayerPrefs.SetInt("Resolution Width", width);
        PlayerPrefs.Save();
    }
    public void SetMaxSupportedResolution()
    {
        Screen.SetResolution(Screen.resolutions.Last().width, Screen.resolutions.Last().height, true);
        PlayerPrefs.SetInt("Resolution Height", Screen.resolutions.Last().height);
        PlayerPrefs.SetInt("Resolution Width", Screen.resolutions.Last().width);
        PlayerPrefs.Save();
    }
    // refresh rates
    public void SetRefreshRate(int hz)
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen, new RefreshRate() { numerator = Convert.ToUInt32(hz), denominator = 1 });
        PlayerPrefs.SetInt("RefreshRate", hz);
        PlayerPrefs.Save();
    }
    public void SetMaxScreenHz()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen, new RefreshRate() { numerator = 0, denominator = 1});
        PlayerPrefs.SetInt("RefreshRate", 0);
        PlayerPrefs.Save();
    }
    // Music Volume
    public void OnVolumeChange()
    {
        AudioController audioController = FindAnyObjectByType<AudioController>();
        if (audioController != null)
        {
            Debug.Log("Audio found!");
            audioController.song_source.volume = volumeSlider.value;
        }
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.Save();
    }
    public void OnSelect()
    {
        audioSource.PlayOneShot(buttonSelectAudio);
    }
    public void OnClickSound()
    {
        audioSource.PlayOneShot(buttonClickAudio);
    }
    public void CheckScreenState()
    {
        if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
        {
            foreach (Button button in framerateButtons)
            {
                button.interactable = false;
                button.enabled = false;
            }
        }
        else if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
        {
            foreach (Button button in framerateButtons)
            {
                button.interactable = true;
                button.enabled = true;
            }
        }
    }
}