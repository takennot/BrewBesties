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
    public void SetFullscreen()
    {
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
    }

    public void SetBorderless()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    public void SetWindowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
    }
    // Resolutions with current fullscreen mode & refresh rate
    public void Set720Resolution()
    {
        Screen.SetResolution(1280,720, Screen.fullScreenMode, Screen.currentResolution.refreshRateRatio);
    }
    public void Set1080Resolution()
    {
        Screen.SetResolution(1920, 1080, Screen.fullScreenMode, Screen.currentResolution.refreshRateRatio);
    }
    public void Set1440Resolution()
    {
        Screen.SetResolution(2560, 1440, Screen.fullScreenMode, Screen.currentResolution.refreshRateRatio);
    }
    public void Set2160Resolution()
    {
        Screen.SetResolution(3840, 2160, Screen.fullScreenMode, Screen.currentResolution.refreshRateRatio);
    }
    public void SetMaxSupportedResolution()
    {
        Screen.SetResolution(Screen.resolutions.Last().width, Screen.resolutions.Last().height, true);
    }
    // refresh rates
    public void Set30Hz()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen, new RefreshRate() { numerator = 30, denominator = 1});
    }
    public void Set60Hz()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen, new RefreshRate() { numerator = 60, denominator = 1 });
    }
    public void Set120Hz()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen, new RefreshRate() { numerator = 120, denominator = 1 });
    }
    public void Set144Hz()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen, new RefreshRate() { numerator = 144, denominator = 1 });
    }
    public void SetMaxScreenHz()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen, new RefreshRate() { numerator = 0, denominator = 1});
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
