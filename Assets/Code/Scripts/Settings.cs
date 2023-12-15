using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Canvas optionsCanvas;
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
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (optionsCanvas.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                optionsCanvas.gameObject.SetActive(false);
                settingsButton.Select();
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
}
