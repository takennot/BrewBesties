using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Analytics;
using System.IO;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManagerScript : MonoBehaviour
{
    // fuck this shit too
    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;
    [SerializeField] GameObject player3;
    [SerializeField] GameObject player4;
    public Transform spawnpoint1;
    public Transform spawnpoint2;
    public Transform spawnpoint3;
    public Transform spawnpoint4;
    List<PlayerScript> players = new List<PlayerScript>();
    //ReadOnlyArray<Gamepad> gamepads;

    [SerializeField] private int playerAmount;

    [SerializeField] private bool isTutorial;

    [SerializeField] private Goal goal;

    [SerializeField] private AnimationScale animScale;

    public bool runElementsInLevel = true;

    private int saveSlot;

    [Header("GarbageCheck")]
    [SerializeField] private int ingredientMax = 25;
    [SerializeField] private int bottleMax = 15;
    [SerializeField] private int firewoodMax = 15;


    private void Awake()
    {
        if(MainMenuData.playerAmount > 0)
        {
            playerAmount = MainMenuData.playerAmount;
        }

        Debug.Log("player amount: " + playerAmount);
        for (int i = 1; i <= playerAmount; i++)
        {
            Debug.Log("Spawn player: " + i);
            SpawnPlayer(i);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("GarbageCheck", 5.0f, 0.1f);

    }


    // fuck all of this
    // Update is called once per frame
    void Update()
    {
        // IS THIS NEEDED??? VVV
        if (!runElementsInLevel)
        {
            foreach (PlayerScript player in players)
            {
                if (player != null)
                {
                    player.enabled = false;
                }
            }

            if(goal == null)
                goal = FindAnyObjectByType<Goal>();

            goal.SetActivated(false);
        }
        else
        {
            foreach (PlayerScript player in players)
            {
                if (player != null)
                {
                    player.enabled = true;
                }
            }

            goal.SetActivated(true);
        }

        {
            //InputSystem.onDeviceChange += (device, change) =>
            //{
            //    switch (change)
            //    {
            //        case InputDeviceChange.Added:
            //            // check if there are less than 4 players, so when player joins, player amount cant exceed 4
            //            //if (FindObjectsOfType<PlayerScript>().Length < 4)
            //            //{
            //            //    Instantiate(player, new Vector3(1, 1, 1), Quaternion.identity);
            //            //}
            //            string[] gamepadNames = Input.GetJoystickNames();

            //            for (int i = 0; i < gamepadNames.Length; i++)
            //            {
            //                if (gamepadNames[i] != "" && !PlayerExists(i))
            //                {
            //                    SpawnPlayer(i);
            //                }
            //            }
            //            break;
            //        case InputDeviceChange.Removed:
            //            break;
            //        case InputDeviceChange.Disconnected:
            //            //if (FindObjectsOfType<PlayerScript>().Length < 2)
            //            //{
            //            //    // Pause game here
            //            //    Debug.Log("Cant continue game with only 1 player!");
            //            //    Application.Quit();
            //            //    UnityEditor.EditorApplication.isPlaying = false;
            //            //}
            //            foreach (PlayerScript player in players)
            //            {
            //                if (!Input.GetJoystickNames()[player.playerIndex].Equals(""))
            //                {
            //                    // Gamepad disconnected, destroy player
            //                    Destroy(player.gameObject);
            //                    players.Remove(player);
            //                    break;
            //                }
            //            }
            //            break;
            //        case InputDeviceChange.Reconnected:
            //            break;
            //        default:
            //            break;
            //    }
            //};
        }
    }

    private bool isPaused = false;
    public void PauseGame()
    {
        
        switch (isPaused)
        {
            case true:
                //Debug.Log("UnPause gm");
                isPaused = false;
                goal.SetActivated(!isPaused);
                runElementsInLevel = true;

                // handle players
                foreach (PlayerScript player in GetPlayersList())
                {
                    player.GetCharacterController().enabled = true;
                }

                break;
            case false:
                //Debug.Log("Pause gm");
                isPaused = true;
                goal.SetActivated(!isPaused);
                runElementsInLevel = false;

                foreach (PlayerScript player in GetPlayersList())
                {
                    player.GetCharacterController().enabled = false;
                }

                break;
        }

        // handle cauldrons
        foreach (CauldronState cauldronState in FindObjectsOfType<CauldronState>())
        {
            cauldronState.Pause();
        }
    }

    void SpawnPlayer(int playerIndex)
    {
        GameObject newPlayer;
        PlayerScript playerController;

        switch (isTutorial)
        {
            case true:
                switch (playerIndex)
                {
                    case 1:
                        //newPlayer = Instantiate(player1, spawnpoint1.position, Quaternion.identity);
                        playerController = player1.GetComponent<PlayerScript>();
                        playerController.InitializePlayer(playerIndex);
                        if (MainMenuData.isArcade)
                        {
                            playerController.consoleType = PlayerScript.ConsoleType.ArcadeMachine;
                        }
                        else
                        {
                            playerController.consoleType = PlayerScript.ConsoleType.Xbox;
                        }
                        players.Add(playerController);

                        player1.GetComponent<PlayerScript>().Respawn(spawnpoint1);

                        break;
                    case 2:
                        //newPlayer = Instantiate(player2, spawnpoint2.position, Quaternion.identity);
                        playerController = player2.GetComponent<PlayerScript>();
                        playerController.InitializePlayer(playerIndex);
                        if (MainMenuData.isArcade)
                        {
                            playerController.consoleType = PlayerScript.ConsoleType.ArcadeMachine;
                        }
                        else
                        {
                            playerController.consoleType = PlayerScript.ConsoleType.Xbox;
                        }
                        players.Add(playerController);

                        player2.GetComponent<PlayerScript>().Respawn(spawnpoint2);

                        break;
                    case 3:
                        //newPlayer = Instantiate(player3, spawnpoint3.position, Quaternion.identity);
                        playerController = player3.GetComponent<PlayerScript>();
                        playerController.InitializePlayer(playerIndex);
                        if (MainMenuData.isArcade)
                        {
                            playerController.consoleType = PlayerScript.ConsoleType.ArcadeMachine;
                        }
                        else
                        {
                            playerController.consoleType = PlayerScript.ConsoleType.Xbox;
                        }
                        players.Add(playerController);

                        player3.GetComponent<PlayerScript>().Respawn(spawnpoint3);

                        break;
                    case 4:
                        //newPlayer = Instantiate(player4, spawnpoint4.position, Quaternion.identity);
                        playerController = player4.GetComponent<PlayerScript>();
                        playerController.InitializePlayer(playerIndex);
                        if (MainMenuData.isArcade)
                        {
                            playerController.consoleType = PlayerScript.ConsoleType.ArcadeMachine;
                        }
                        else
                        {
                            playerController.consoleType = PlayerScript.ConsoleType.Xbox;
                        }
                        players.Add(playerController);

                        player4.GetComponent<PlayerScript>().Respawn(spawnpoint4);

                        break;

                    default:
                        break;
                }
                break;
            case false:
                switch (playerIndex)
                {
                    case 1:
                        newPlayer = Instantiate(player1, spawnpoint1.position, Quaternion.identity);
                        newPlayer.transform.rotation = spawnpoint1.rotation;
                        playerController = newPlayer.GetComponent<PlayerScript>();
                        playerController.InitializePlayer(playerIndex);
                        if (MainMenuData.isArcade)
                        {
                            playerController.consoleType = PlayerScript.ConsoleType.ArcadeMachine;
                        }
                        else
                        {
                            playerController.consoleType = PlayerScript.ConsoleType.Xbox;
                        }
                        players.Add(playerController);
                        break;
                    case 2:
                        newPlayer = Instantiate(player2, spawnpoint2.position, Quaternion.identity);
                        newPlayer.transform.rotation = spawnpoint2.rotation;
                        playerController = newPlayer.GetComponent<PlayerScript>();
                        playerController.InitializePlayer(playerIndex);
                        if (MainMenuData.isArcade)
                        {
                            playerController.consoleType = PlayerScript.ConsoleType.ArcadeMachine;
                        }
                        else
                        {
                            playerController.consoleType = PlayerScript.ConsoleType.Xbox;
                        }
                        players.Add(playerController);
                        break;
                    case 3:
                        newPlayer = Instantiate(player3, spawnpoint3.position, Quaternion.identity);
                        newPlayer.transform.rotation = spawnpoint3.rotation;
                        playerController = newPlayer.GetComponent<PlayerScript>();
                        playerController.InitializePlayer(playerIndex);
                        if (MainMenuData.isArcade)
                        {
                            playerController.consoleType = PlayerScript.ConsoleType.ArcadeMachine;
                        }
                        else
                        {
                            playerController.consoleType = PlayerScript.ConsoleType.Xbox;
                        }
                        players.Add(playerController);
                        break;
                    case 4:
                        newPlayer = Instantiate(player4, spawnpoint4.position, Quaternion.identity);
                        newPlayer.transform.rotation = spawnpoint4.rotation;
                        playerController = newPlayer.GetComponent<PlayerScript>();
                        playerController.InitializePlayer(playerIndex);
                        if (MainMenuData.isArcade)
                        {
                            playerController.consoleType = PlayerScript.ConsoleType.ArcadeMachine;
                        }
                        else
                        {
                            playerController.consoleType = PlayerScript.ConsoleType.Xbox;
                        }
                        players.Add(playerController);
                        break;

                    default:
                        break;
                }
                
                break;
        }
    }

    public List<PlayerScript> GetPlayersList()
    {
        return players;
    }

    //bool PlayerExists(int playerIndex)
    //{
    //    foreach (PlayerScript player in players)
    //    {
    //        if (player.playerIndex == playerIndex)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    /// <summary>
    /// Checks for ingredients, bottles and wood logs.
    /// Whenever there are more than X amount of ingredients/bottles/logs.
    /// It will remove one of items.
    /// </summary>
    void GarbageCheck()
    {
        Ingredient[] ingredients = FindObjectsByType<Ingredient>(FindObjectsSortMode.InstanceID);
        Bottle[] bottles = FindObjectsByType<Bottle>(FindObjectsSortMode.InstanceID);
        Firewood[] firewoods = FindObjectsByType<Firewood>(FindObjectsSortMode.InstanceID);

        if(ingredients != null && ingredients.Length > ingredientMax)
        {
            if (!ingredients.Last().gameObject.GetComponent<Item>().IsPickedUp() && !ingredients.Last().gameObject.GetComponent<Ingredient>().GetIsMagic())
            {
                animScale.ScaleDownAndDestroy(ingredients.Last().gameObject);
            }
        }
        if (bottles != null && bottles.Length > bottleMax)
        {
            if (!bottles.Last().gameObject.GetComponent<Item>().IsPickedUp() && bottles.Last().gameObject.GetComponent<Bottle>().IsEmpty())
            {
                animScale.ScaleDownAndDestroy(bottles.Last().gameObject);
            }
        }
        if (firewoods != null && firewoods.Length > firewoodMax)
        {
            if (!firewoods.Last().gameObject.GetComponent<Item>().IsPickedUp())
            {
                animScale.ScaleDownAndDestroy(firewoods.Last().gameObject);
            }
        }
    }

    public int GetPlayerAmount()
    {
        return playerAmount;
    }

    public void SaveLog()
    {
        if (isTutorial == false)
        {
            // %userprofile%\AppData\LocalLow\<companyname>\<productname>
            //string logPath = Application.persistentDataPath + "\\SessionLogs\\" + AnalyticsSessionInfo.sessionId + ".txt";
            string logPath = Path.Combine(Application.persistentDataPath, "SessionLogs", AnalyticsSessionInfo.sessionId + ".txt");


            // Ensure the directory exists
            string directoryPath = Path.GetDirectoryName(logPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Check if the file already exists
            if (!File.Exists(logPath))
            {

                //FileStream fs = File.Create(logPath);

                // If the file doesn't exist, create it and write the data
                using (StreamWriter sw = File.CreateText(logPath))
                {
                    sw.WriteLine("----------------------------------------------");
                    sw.WriteLine("Session ID: " + AnalyticsSessionInfo.sessionId);
                    sw.WriteLine("Level: " + SceneManager.GetActiveScene().name);
                    sw.WriteLine("Score in level " + SceneManager.GetActiveScene().name + ": " + goal.GetScore());
                    sw.WriteLine("----------------------------------------------");
                }

                Console.WriteLine($"Data saved to {logPath}");
            }
            else
            {
                // If the file exists, append the data
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine("Session ID: " + AnalyticsSessionInfo.sessionId);
                    sw.WriteLine("Level: " + SceneManager.GetActiveScene().name);
                    sw.WriteLine("Score in level " + SceneManager.GetActiveScene().name + ": " + goal.GetScore());
                    sw.WriteLine("----------------------------------------------");
                }

                Console.WriteLine($"Data appended to {logPath}");
            }
        }
    }
}
