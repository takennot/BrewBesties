using Collections.Shaders.CircleTransition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.Rendering;

public class TutorialManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameManagerScript gameManager;
    [SerializeField] private SliderManager sliderManager;
    [SerializeField] private KillboxManager killboxManager;
    [SerializeField] private StartAndEnd startAndEnd;

    [Header("Tutorial Conditions")]


    [Header("GameObjects")]
    [SerializeField] private CounterState[] counterGhosts;
    [SerializeField] private GameObject[] resourceBoxes;
    [SerializeField] private GameObject[] workstationsPrompts;
    [SerializeField] private GameObject cauldron;
    [SerializeField] private GameObject goal;
    private Goal goalState;
    [SerializeField] private GameObject potionBox;
    [SerializeField] private AudioController audioController;


    [Header("Animations")]
    [SerializeField] private AnimationScale animScale;
    [SerializeField] private Animator animWipe;

    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource sourceSuccess;
    [SerializeField] private AudioClip sucessClip;
    [SerializeField] private AudioClip smallSucessClip; // used when placed on counter with ghost ingredient.
    [SerializeField] private AudioClip smallFailClip; // used when picked up from counter with ghost ingredient.
    [SerializeField] private AudioSource sourceScale;

    [Header("Missions")]
    [SerializeField] private List<Mission> missions;

    [Header("UI")]
    [SerializeField] private CameraUIManager cameraUI;
    [SerializeField] private TextTypewriter typewriter;

    [Header("Players")]
    [SerializeField] private List<PlayerScript> players;

    [Header("Customers")]
    private bool hasSpawnedCustomer = false;
    private bool shouldSpawnCustomers = false;
    [SerializeField] private float customerDelayTime = 3.0f;

    [Header("Ending")]
    [SerializeField] private CircleTransition circleTransition;

    //Original Spawnpoints
    private Transform sp1;
    private Transform sp2;
    private Transform sp3;
    private Transform sp4;

    public class Mission
    {
        public string missionName;
        public Func<bool> missionCondition;
        public bool isCompleted;
        public List<bool> playerFulfillment;
        public IEnumerator completionAction;
    }

    void Start()
    {
        sp1 = gameManager.spawnpoint1;
        sp2 = gameManager.spawnpoint2;
        sp3 = gameManager.spawnpoint3;
        sp4 = gameManager.spawnpoint4;

        int playerCount = gameManager.GetPlayerAmount();
        if (playerCount == 0) playerCount = 2;

        InitializePlayers(playerCount);
        InitializeMissions();

        //Scale 0,0,0
        foreach (var workstationPrompt in workstationsPrompts)
        {
            workstationPrompt.transform.localScale = new Vector3(0, 0, 0);
        }
        cauldron.transform.localScale = new Vector3(0, 0, 0);
        cauldron.SetActive(false);
        goal.transform.localScale = new Vector3(0, 0, 0);
        goalState = goal.GetComponentInChildren<Goal>();
        goal.GetComponentInChildren<CollidingTriggerCounting>().SetEnableTrigger(false);
        goalState.magicMushroomPercent = 100f;
        potionBox.transform.localScale = new Vector3(0, 0, 0);

    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(Mission5CompletionAction());
        }

        foreach (Mission mission in missions)
        {
            if (!mission.isCompleted && mission.missionCondition())
            {
                StartCoroutine(CompleteMission(mission));
            }
        }

        if(shouldSpawnCustomers)
        {
            if (goalState.amountOfCustomers == 0 && !hasSpawnedCustomer)
            {
                StartCoroutine(DelayedNewCustomer());
                hasSpawnedCustomer = true; // Set the flag to prevent continuous calls
            }
        }
    }

    private void InitializePlayers(int playerCount)
    {
        int excessPlayers = players.Count - playerCount;

        if (excessPlayers > 0)
        {
            for (int i = playerCount; i < players.Count; i++)
            {
                players[i].gameObject.SetActive(false);
            }
            players.RemoveRange(playerCount, excessPlayers);
        }

        for (int i = 0; i < playerCount; i++)
        {
            Transform spawnPoint = null;

            switch (i)
            {
                case 0:
                spawnPoint = gameManager.spawnpoint1;
                break;
                case 1:
                spawnPoint = gameManager.spawnpoint2;
                break;
                case 2:
                spawnPoint = gameManager.spawnpoint3;
                break;
                case 3:
                spawnPoint = gameManager.spawnpoint4;
                break;
                default:
                Debug.LogError("Not enough spawn points for all players!");
                break;
            }

            if (spawnPoint != null)
            {
                players[i].transform.position = spawnPoint.position;
            }
        }
    }

    // ******** Initialize
    private void InitializeMissions()
    {
        // Initialize missions with their conditions, header, and subtexts
        missions = new List<Mission>
    {
        new Mission
        {
            missionName = "Place mushrooms on ghost mushrooms",
            missionCondition = () => CheckPlaceOnGhostMushrooms(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            completionAction = Mission1CompletionAction(),
        },
        /*
        new Mission
        {
            missionName = "Make x magic mushrooms",
            missionCondition = () => CheckMagicMushrooms(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            completionAction = Mission2CompletionAction(),
        },
        new Mission
        {
            missionName = "Make x magic potions",
            missionCondition = () => CheckMagicPotions(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            completionAction = Mission3CompletionAction(),
        },
        new Mission
        {
            missionName = "Serve x magic potions",
            missionCondition = () => CheckServeMagicPotions(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            completionAction = Mission4CompletionAction(),
        },
        new Mission
        {
            missionName = "Serve x potions",
            missionCondition = () => CheckServePotions(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            completionAction = Mission5CompletionAction(),
        }
        */
    };

        int playerCount = gameManager.GetPlayerAmount();
        if (playerCount == 0) playerCount = 2;

    }

    // ************** COMPLETE MISSION GENERIC ***********************

    private IEnumerator CompleteMission(Mission mission)
    {
        Debug.Log("<color=green>Completed mission: " + mission.missionName + "</color>");
        mission.isCompleted = true;
        sourceSuccess.PlayOneShot(sourceSuccess.clip);
        yield return StartCoroutine(mission.completionAction); // Use StartCoroutine here
    }


    // ************** MISSION CONDITIONS **********************

    private bool CheckPlaceOnGhostMushrooms()
    {
        bool success = true;
        foreach (CounterState counter in counterGhosts)
        {
            if(counter.storedItem == null)
            {
                success = false;
            }
        }

        if(success)
        {
            Debug.Log("<color=green>Success on placing ingredients</color>");
        }

        return success;
    }


    // ************** COMPLETE MISSION SPECIFIC ***********************

    IEnumerator Mission1CompletionAction()
    {
        yield return null;
    }

    IEnumerator Mission2CompletionAction()
    {
        yield return null;
    }


    IEnumerator Mission3CompletionAction()
    {

        yield return null;
    }

    IEnumerator Mission4CompletionAction()
    {
        yield return null;
    }

    IEnumerator Mission5CompletionAction()
    {

       
        startAndEnd.End();  
        //LoadScene();
        yield return null;
    }

    private IEnumerator DelayedNewCustomer()
    {
        yield return new WaitForSeconds(customerDelayTime);

        goalState.NewCustomer();
        hasSpawnedCustomer = false; // Reset the flag to allow for the next customer spawn
    }

    private void LoadScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        } else
        {
            Debug.LogWarning("There is no next scene available. Loading scene index 1");
            SceneManager.LoadScene(1);
        }
    }
    private void ScaleUpArray(GameObject[] gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            animScale.ScaleUp(gameObject);
        }
        sourceScale.PlayOneShot(source.clip);
    }
}

