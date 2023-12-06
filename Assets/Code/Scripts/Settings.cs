using System.Collections;
using System.Collections.Generic;
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
    public void SetFullscreen()
    {
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        PlayerPrefs.SetInt("FullscreenMode", 0);
        PlayerPrefs.Save();
    }

    public void SetBorderless()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        PlayerPrefs.SetInt("FullscreenMode", 1);
        PlayerPrefs.Save();
    }

    public void SetWindowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        PlayerPrefs.SetInt("FullscreenMode", 2);
        PlayerPrefs.Save();
    }
    // Resolutions with current fullscreen mode & refresh rate
    public void Set720Resolution()
    {
        Screen.SetResolution(1280,720, Screen.fullScreenMode, Screen.currentResolution.refreshRateRatio);
        PlayerPrefs.SetInt("Resolution Height", 720);
        PlayerPrefs.SetInt("Resolution Width", 1280);
        PlayerPrefs.Save();
    }
    public void Set1080Resolution()
    {
        Screen.SetResolution(1920, 1080, Screen.fullScreenMode, Screen.currentResolution.refreshRateRatio);
        PlayerPrefs.SetInt("Resolution Height", 1080);
        PlayerPrefs.SetInt("Resolution Width", 1920);
        PlayerPrefs.Save();
    }
    public void Set1440Resolution()
    {
        Screen.SetResolution(2560, 1440, Screen.fullScreenMode, Screen.currentResolution.refreshRateRatio);
        PlayerPrefs.SetInt("Resolution Height", 1440);
        PlayerPrefs.SetInt("Resolution Width", 2560);
        PlayerPrefs.Save();
    }
    public void Set2160Resolution()
    {
        Screen.SetResolution(3840, 2160, Screen.fullScreenMode, Screen.currentResolution.refreshRateRatio);
        PlayerPrefs.SetInt("Resolution Height", 2160);
        PlayerPrefs.SetInt("Resolution Width", 3840);
        PlayerPrefs.Save();
    }
    public void SetMaxSupportedResolution()
    {
        Screen.SetResolution(Screen.resolutions.Last().width, Screen.resolutions.Last().height, true);
        PlayerPrefs.SetInt("Resolution Height", 720);
        PlayerPrefs.SetInt("Resolution Width", 1280);
        PlayerPrefs.Save();
    }
    // refresh rates
    public void Set30Hz()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen, new RefreshRate() { numerator = 30, denominator = 1});
        PlayerPrefs.SetInt("RefreshRate", 30);
        PlayerPrefs.Save();
    }
    public void Set60Hz()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen, new RefreshRate() { numerator = 60, denominator = 1 });
        PlayerPrefs.SetInt("RefreshRate", 60);
        PlayerPrefs.Save();
    }
    public void Set120Hz()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen, new RefreshRate() { numerator = 120, denominator = 1 });
        PlayerPrefs.SetInt("RefreshRate", 120);
        PlayerPrefs.Save();
    }
    public void Set144Hz()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen, new RefreshRate() { numerator = 144, denominator = 1 });
        PlayerPrefs.SetInt("RefreshRate", 144);
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
