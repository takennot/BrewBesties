using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{

    [SerializeField] Canvas pauseMenuCanvas;
    [SerializeField] Canvas optionsCanvas;
    [SerializeField] GameObject options;
    [SerializeField] Button resumeButton;
    [SerializeField] Button restartButton;
    public bool isPaused = false;
    private bool isArcade;
    [SerializeField] GameManagerScript gameManager;
    [SerializeField] Slider volumeSlider;
    [SerializeField] private AudioClip buttonSelectAudio;
    [SerializeField] private AudioClip buttonClickAudio;
    [SerializeField] private AudioSource audioSource;

    public bool canPressPause = true;
    public bool isInOptions = false;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuCanvas.enabled = false;
        //optionsCanvas.enabled = false;
        options.SetActive(false);
        Time.timeScale = 1f;
        if (FindAnyObjectByType<PlayerScript>().consoleType == PlayerScript.ConsoleType.ArcadeMachine)
        {
            isArcade = true;
        }
        else
        {
            isArcade = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        StartAndEnd startAndEnd = FindAnyObjectByType<StartAndEnd>();

        if(!isPaused )
        {
            resumeButton.enabled = false;
        }
        else
        {
            resumeButton.enabled = true;
        }

        if (isPaused && isInOptions)
        {
            pauseMenuCanvas.enabled = false;
        }
        else if(isPaused)
        {
            pauseMenuCanvas.enabled = true;
        }


        if (startAndEnd == null)
        {
            if((Input.GetButtonDown("Pause") && !isArcade) || (Input.GetKeyDown(KeyCode.UpArrow) && !isArcade))
            {
                if (canPressPause)
                {
                    Pause();
                }
            }
        } 
        else if (FindAnyObjectByType<StartAndEnd>().hasStarted && ((Input.GetButtonDown("Pause") && !isArcade) || (Input.GetKeyDown(KeyCode.UpArrow) && !isArcade)))
        {
            if (canPressPause)
            {
                Pause();
            }
        }

        // if options canvas enabled 
        // if input is B pressed
        // disable options canvas, focus on resume?
        //pauseMenuCanvas.enabled = true;
        //optionsCanvas.enabled = false;
        //resumeButton.Select();
    }

    public void Pause()
    {
        Debug.Log("Pause!!!");

        gameManager.PauseGame();

        switch (isPaused)
        {
            case true:
                OnResume();
                isPaused = false;
                break;
            case false:
                pauseMenuCanvas.enabled = true;
                Time.timeScale = 0f;
                resumeButton.Select();
                isPaused = true;

                AudioController audioController = FindAnyObjectByType<AudioController>();
                if(audioController != null)
                {
                    audioController.song_source.Pause();
                }

                break;
        }
    }

    public void OnResume() 
    {
        Debug.Log("OnResume");
        Debug.Log("Resume");

        switch (isPaused)
        {
            case true:
                pauseMenuCanvas.enabled = false;
                Time.timeScale = 1f;
                isPaused = false;
                
                AudioController audioController = FindAnyObjectByType<AudioController>();
                if (audioController != null)
                {
                    Debug.Log("Audio found!");
                    audioController.song_source.UnPause();
                }

                break;
            case false: return;
        }
    }

    public void OnRestart()
    {
        switch (isPaused)
        {
            case true:
                foreach (PlayerScript player in gameManager.GetComponent<GameManagerScript>().GetPlayersList())
                {
                    player.GetCharacterController().enabled = true;
                }
                pauseMenuCanvas.enabled = false;
                isPaused = false;
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                restartButton.enabled = false;

                break;
            case false: return;
        }
    }

    public void OnOptions()
    {
        Debug.Log("OnOptions");
        canPressPause = false;
        isInOptions = true;
        
        //optionsCanvas.enabled = true;
        options.SetActive(true);
        volumeSlider.Select();
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.1f);
    }

    public void OnBack()
    {
        Debug.Log("OnBack");
        canPressPause = true;
        isInOptions = false;
        pauseMenuCanvas.enabled = true;
        //optionsCanvas.enabled = false;
        options.SetActive(false);
        //resumeButton.Select();
    }

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

    public void OnMainMenu()
    {
        switch (isPaused)
        {
            case true:
                Time.timeScale = 1f;
                SceneManager.LoadScene(1);
                break;
            case false: return;
        }

    }

    public void OnQuit()
    {
        if (isPaused == true)
        {
            Application.Quit();
        } 
    }
    public void OnSelect()
    {
        audioSource.PlayOneShot(buttonSelectAudio);
    }
    public void OnClickSound()
    {
        audioSource.PlayOneShot(buttonClickAudio);
    }
}
