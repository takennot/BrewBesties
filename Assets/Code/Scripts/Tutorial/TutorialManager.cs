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
    [SerializeField] private int requiredServePotion = 1;
    [SerializeField] private int requiredServePotionsAll = 3;

    [Space(10)]
    [Header("GameObjects")]
    [SerializeField] private CounterState[] counterGhosts;
    [SerializeField] private GameObject[] countersMiddle;
    [SerializeField] private GameObject[] ghostMushrooms;

    [SerializeField] private GameObject resourceBoxLeft;
    [SerializeField] private GameObject resourceBoxRight;

    [SerializeField] private GameObject cauldronLeft;
    [SerializeField] private GameObject cauldronRight;

    [SerializeField] private GameObject[] workstations;
    [SerializeField] private GameObject[] promptUI;

    [SerializeField] private GameObject goal;
    private Goal goalState;
    [SerializeField] private GameObject potionBoxLeft;
    [SerializeField] private GameObject potionBoxRight;
    [SerializeField] private AudioController audioController;



    [Space(10)]
    [Header("Animations")]
    [SerializeField] private AnimationScale animScale;
    [SerializeField] private Animator animWipe;

    [Space(10)]
    [Header("Missions")]
    [SerializeField] private List<Mission> missions;


    [Space(10)]
    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource sourceSuccess;
    [SerializeField] private AudioClip sucessClip;
    [SerializeField] private AudioClip smallSucessClip; // used when placed on counter with ghost ingredient.
    [SerializeField] private AudioClip smallFailClip; // used when picked up from counter with ghost ingredient.
    [SerializeField] private AudioSource sourceScale;



    [Header("UI")]
    [SerializeField] private CameraUIManager cameraUI;
    [SerializeField] private TextTypewriter typewriter;
    [SerializeField] private TMP_Text textObjective;

    [Header("Players")]
    [SerializeField] private List<PlayerScript> players;

    [Header("Customers")]
    private bool hasSpawnedCustomer = false;
    private bool shouldSpawnCustomers = false;
    [SerializeField] private float customerDelayTime = 3.0f;

    [Header("Ending")]
    [SerializeField] private CircleTransition circleTransition;

    private float timerUI = 0;

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

        cauldronLeft.transform.localScale = new Vector3(0, 0, 0);
        cauldronLeft.SetActive(false);
        cauldronRight.transform.localScale = new Vector3(0, 0, 0);
        cauldronRight.SetActive(false);

        goal.transform.localScale = new Vector3(0, 0, 0);
        goalState = goal.GetComponentInChildren<Goal>();
        goal.GetComponentInChildren<CollidingTriggerCounting>().SetEnableTrigger(false);
        goalState.magicMushroomPercent = 0f;

        potionBoxLeft.transform.localScale = new Vector3(0, 0, 0);
        potionBoxLeft.SetActive(false);
        potionBoxRight.transform.localScale = new Vector3(0, 0, 0);
        potionBoxRight.SetActive(false);

        workstations[0].transform.localScale = new Vector3(0, 0, 0);
        workstations[0].SetActive(false);
        workstations[1].transform.localScale = new Vector3(0, 0, 0);
        workstations[1].SetActive(false);
        promptUI[0].transform.localScale = new Vector3(0, 0, 0);
        promptUI[1].transform.localScale = new Vector3(0, 0, 0);
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
        timerUI += Time.deltaTime;
        if(timerUI > 0.075f)
        {
            timerUI = 0;
            cameraUI.Initilize();
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

        if (shouldSpawnCustomers)
        {
            if (goalState.amountOfCustomers == 0 && !hasSpawnedCustomer)
            {
                StartCoroutine(DelayedNewCustomer());
                hasSpawnedCustomer = true; // Set the flag to prevent continuous calls
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
        
        new Mission
        {
            missionName = "Fill cauldron (right)",
            missionCondition = () => CheckFillCauldronRight(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            completionAction = Mission2CompletionAction(),
        },
        new Mission
        {
            missionName = "Fill cauldron (left)",
            missionCondition = () => CheckFillCauldronLeft(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            completionAction = Mission3CompletionAction(),
        },
        new Mission
        {
            missionName = "Fill bottle",
            missionCondition = () => CheckFillPotion(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            completionAction = Mission4CompletionAction(),
        },
        
        new Mission
        {
            missionName = "Serve mushroom potion",
            missionCondition = () => CheckServePotions(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            completionAction = Mission5CompletionAction(),
        },
        new Mission
        {
            missionName = "Serve all potions",
            missionCondition = () => CheckServeAll(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            completionAction = Mission6CompletionAction(),
        }

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
        cameraUI.Initilize();
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
        return success;
    }

    private bool CheckFillCauldronRight()
    {
        return cauldronRight.GetComponent<CauldronState>().GetIngredientCount() >= 3;
    }

    private bool CheckFillCauldronLeft()
    {
        return cauldronLeft.GetComponent<CauldronState>().GetIngredientCount() >= 3;
    }
    private bool CheckFillPotion()
    {
        if (!missions[2].isCompleted) return false;
        
        int potionCount = 0;

        foreach(PlayerScript player in players)
        {
            if (player.GetObjectInHands() != null && player.GetObjectInHands().GetComponent<Bottle>() != null && 
                player.GetObjectInHands().GetComponent<Bottle>().GetPotion() != null 
                && player.GetObjectInHands().GetComponent<Bottle>().GetPotion().isPotionDone)
            {
                potionCount++;
            }
        }
        return potionCount >= 1;
    }

    private bool CheckServePotions()
    {
        return goalState.GetCompletedRecipesCount() >= requiredServePotion;
    }

    private bool CheckServeAll()
    {
        return goalState.GetCompletedRecipesCount() >= requiredServePotion + requiredServePotionsAll;
    }

    // ************** COMPLETE MISSION SPECIFIC ***********************

    IEnumerator Mission1CompletionAction()
    {
        yield return new WaitForSeconds(0.3f);
        sourceScale.PlayOneShot(sourceScale.clip);
        foreach(GameObject ghostMushroom in ghostMushrooms)
        {
            animScale.ScaleDownAndDestroy(ghostMushroom);
        }
        List<Item> items = FindObjectsOfType<Item>().ToList();
        foreach (Item item in items)
        {
            animScale.ScaleDownAndDestroy(item.gameObject);
        }

        yield return new WaitForSeconds(0.5f);
        sourceScale.PlayOneShot(sourceScale.clip);
        cauldronRight.SetActive(true);
        animScale.ScaleUp(cauldronRight, new(2,2,2));
        resourceBoxRight.SetActive(false);
        animScale.ScaleDown(resourceBoxRight);

        textObjective.text = "• Fill <sprite name=\"Interact\"> the cauldron (large cooking pot) \n   with 3 mushrooms.";

        yield return null;
    }

    IEnumerator Mission2CompletionAction()
    {
        cauldronRight.GetComponentInChildren<ParticleSystem>().Stop();

        yield return new WaitForSeconds(1.75f);
        sourceScale.PlayOneShot(sourceScale.clip);
        animScale.ScaleDown(cauldronRight);
        animScale.ScaleDown(resourceBoxLeft);

        cauldronLeft.SetActive(true);
        resourceBoxRight.SetActive(true);
        animScale.ScaleUp(cauldronLeft, new(2,2,2));
        animScale.ScaleUp(resourceBoxRight);
        List<Item> items = FindObjectsOfType<Item>().ToList();
        foreach (Item item in items)
        {
            animScale.ScaleDownAndDestroy(item.gameObject);
        }

        textObjective.text = "• Fill <sprite name=\"Interact\"> the new cauldron \n   with 3 mushrooms.";

        yield return new WaitForSeconds(1f);

        cauldronRight.SetActive(false);
        resourceBoxLeft.SetActive(false);
        yield return null;
    }


    IEnumerator Mission3CompletionAction()
    {
        yield return new WaitForSeconds(0.75f);
        sourceScale.PlayOneShot(sourceScale.clip);
        animScale.ScaleUp(potionBoxRight);
        potionBoxRight.SetActive(true);
        textObjective.text = "• Fill <sprite name=\"Interact\"> a bottle from the cauldron. \n Make sure the cauldron is done brewing!";

        yield return null;
    }

    IEnumerator Mission4CompletionAction()
    {
        animScale.ScaleUp(goal);
        goal.SetActive(true);
        goal.GetComponentInChildren<Goal>().SetActivated(true);
        goal.GetComponentInChildren<CollidingTriggerCounting>().SetEnableTrigger(true);
        shouldSpawnCustomers = true;
        textObjective.text = "• Give <sprite name=\"Interact\"> the potion (brew) to the customer.";

        yield return null;
    }

    IEnumerator Mission5CompletionAction()
    {
        goalState.magicMushroomPercent = 0.6f;
        yield return new WaitForSeconds(0.75f);
        ScaleDownArray(countersMiddle);

        ScaleUpArray(workstations);
        workstations[0].SetActive(true);
        workstations[1].SetActive(true);

        ScaleUpArray(promptUI);
        yield return new WaitForSeconds(0.75f);

        textObjective.text = "• Keep serving the customers. Customers \n" +
            "will tell you their order when standing nearby. \n" +
            "It is displayed in the top left corner.";

        yield return null;

    }

    IEnumerator Mission6CompletionAction()
    {
        yield return new WaitForSeconds(0.75f);

        startAndEnd.End();
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

    private void ScaleDownArray(GameObject[] gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            animScale.ScaleDown(gameObject);
        }
        sourceScale.PlayOneShot(source.clip);
    }
}

