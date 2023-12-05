using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class MainMenuScript : MonoBehaviour
{
    public static int playerAmount = 2;
    [SerializeField] private Toggle toggle2p;
    [SerializeField] private Toggle toggle3p;
    [SerializeField] private Toggle toggle4p;
    [SerializeField] private Color toggleColorSelected;
    [SerializeField] private bool isArcade = false;
    [SerializeField] private Canvas credits;
    [SerializeField] private Canvas settings;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button fullscreenButton;

    private ColorBlock colorSelected;
    private ColorBlock colorUnSelected;

    private void Awake()
    {
        credits.gameObject.SetActive(false);
        settings.gameObject.SetActive(false);

        colorSelected = toggle2p.colors;
        colorSelected.normalColor = toggleColorSelected;
        colorUnSelected = toggle2p.colors;

        toggle2p.Select();

        AudioController audioController = FindAnyObjectByType<AudioController>();
        if (audioController != null)
        {
            audioController.song_source.volume = PlayerPrefs.GetFloat("Volume");       
        }
    }
    private void Update()
    {
        if (toggle2p.isOn)
        {
/*            toggle2p.colors = colorSelected;
            toggle3p.colors = colorUnSelected;
            toggle4p.colors = colorUnSelected;*/

            playerAmount = 2;
            MainMenuData.playerAmount = playerAmount;
        }
        else if (toggle3p.isOn)
        {
/*            toggle2p.colors = colorUnSelected;
            toggle3p.colors = colorSelected;
            toggle4p.colors = colorUnSelected;*/

            playerAmount = 3;
            MainMenuData.playerAmount = playerAmount;
        }
        else if (toggle4p.isOn)
        {
            //toggle2p.colors = colorUnSelected;
            //toggle3p.colors = colorUnSelected;
            //toggle4p.colors = colorSelected;

            playerAmount = 4;
            MainMenuData.playerAmount = playerAmount;
        }
        // if player 1 presses A & B on controller
        // switch arcade mode on or off
        if (Input.GetKeyDown(KeyCode.Joystick1Button2) && Input.GetKeyDown(KeyCode.Joystick1Button3))
        {
            switch (isArcade)
            {
                case true:
                    isArcade = false; 
                    MainMenuData.isArcade = false;
                    toggle3p.gameObject.SetActive(true);
                    toggle4p.gameObject.SetActive(true);
                    break;
                case false:
                    isArcade = true;
                    MainMenuData.isArcade = true;
                    toggle3p.gameObject.SetActive(false);
                    toggle4p.gameObject.SetActive(false);
                    break;
            }
        }

        if (Input.GetButtonDown("PickUpOne"))
        {
            if (EventSystem.current.currentSelectedGameObject.GetComponent<Button>())
            {
                EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
            }
            if (EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>())
            {
                EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn = true;
            }
        }
        if (credits.gameObject.activeSelf)
        {
            creditsButton.Select();
            if (Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                credits.gameObject.SetActive(false);
            }
        }
    }

    public void OnStartGame()
    {
        SceneManager.LoadScene(3);
    }

    public void OnStartTutorial()
    {
        SceneManager.LoadScene(2);
    }

    public void OnCredits()
    {
        credits.gameObject.SetActive(true);
    }

    public void OnSettings()
    {
        settings.gameObject.SetActive(true);
        fullscreenButton.Select();
    }



    public void OnQuitGame()
    {
        Application.Quit();
        // if teachers complain about Quitting speed, takes too long yada yada bla bla - use this xDDDDDDDDDDDDD :letscook: babyyyyyyyy
        // be careful - THIS WILL SHUTDOWN EDITOR TOO
        //System.Diagnostics.Process.GetCurrentProcess().Kill();
    }
}
