using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static CustomerOrder;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameManagerScript gameManager;
    private List<int> successfulMagicPotionsIDs = new List<int>();
    private List<int> successfulMagicMushroomsIDs = new List<int>();
    private int potionCount = 0;
    private int magicMushroomCount = 0;

    [Header("Tutorial Conditions")]
    [SerializeField] private float requiredMagicMushrooms = 3f;
    [SerializeField] private float requiredMagicPotions = 3f;
    [Tooltip("Player swap will be at 1/3 of requiredServedPotions")]
    [SerializeField] private float requiredServedPotions = 6f; 

    [Header("GameObjects")]
    [SerializeField] private GameObject[] resourceBoxes;
    [SerializeField] private GameObject[] workstationsPrompts;
    [SerializeField] private GameObject cauldron;
    [SerializeField] private GameObject goal;
    private Goal goalState;
    [SerializeField] private GameObject potionBox;

    [Header("Animations")]
    [SerializeField] private AnimationScale animScale;

    [Header("Missions")]
    [SerializeField] private List<Mission> missions;

    [Header("UI")]
    [SerializeField] private Color colorCompleted = Color.green;
    [SerializeField] private Slider sliderMagicMushrooms;
    [SerializeField] private Slider sliderMagicPotions;
    [SerializeField] private Slider sliderServePotions;

    [Header("Players")]
    [SerializeField] private List<PlayerScript> players;

    [SerializeField] private TMP_Text text;

    private int currentMissionIndex = 0;

    [Header("Customers")]
    private bool hasSpawnedCustomer = false;
    private bool shouldSpawnCustomers = false;
    [SerializeField] private float customerDelayTime = 3.0f;

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
        public Action onCompletionAction;
    }

    void Start()
    {
        sp1 = gameManager.spawnpoint1;
        sp2 = gameManager.spawnpoint2;
        sp3 = gameManager.spawnpoint3;
        sp4 = gameManager.spawnpoint4;

        int playerCount = gameManager.GetPlayerAmount();
        if (playerCount == 0) playerCount = 2;

        sliderMagicMushrooms.maxValue = requiredMagicMushrooms;
        sliderMagicPotions.maxValue = requiredMagicPotions;
        sliderServePotions.maxValue = requiredServedPotions;

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
        goalState.magicMushroomPercent = 100f;
        potionBox.transform.localScale = new Vector3(0, 0, 0);
    }

    void Update()
    {
        foreach (Mission mission in missions)
        {
            if (!mission.isCompleted && mission.missionCondition())
            {
                CompleteMission(mission);
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
            missionName = "Pickup",
            missionCondition = () => CheckPickupCondition(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            onCompletionAction = () => Mission1CompletionAction(),
        },
        new Mission
        {
            missionName = "Make x magic mushrooms",
            missionCondition = () => CheckMagicMushrooms(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            onCompletionAction = () => Mission2CompletionAction(),
        },
        new Mission
        {
            missionName = "Make x magic potions",
            missionCondition = () => CheckMagicPotions(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            onCompletionAction = () => Mission3CompletionAction(),
        },
        new Mission
        {
            missionName = "Make x magic potions",
            missionCondition = () => CheckServeMagicPotions(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            onCompletionAction = () => Mission4CompletionAction(),
        }
    };

        int playerCount = gameManager.GetPlayerAmount();
        if (playerCount == 0) playerCount = 2;

    }

    private void ScaleUpArray(GameObject[] gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            animScale.ScaleUp(gameObject);
        }
    }


    private void CompleteMission(Mission mission)
    {
        mission.isCompleted = true;
        mission.onCompletionAction?.Invoke();

        Debug.Log("Completed mission: " + mission.missionName);

        currentMissionIndex++;
    }


    //MISSION CONDITIONS
    private bool CheckPickupCondition()
    {

        if (players[0].GetObjectInHands() == null)
            return false;

        if (!players[0].GetObjectInHands().CompareTag("Ingredient"))
            return false;

        if (players[0].GetObjectInHands().GetComponent<Ingredient>() == null)
            return false;


        return true;
    }

    private void Mission1CompletionAction()
    {
        ScaleUpArray(workstationsPrompts);
    }

    private bool CheckMagicMushrooms()
    {
        GameObject[] ingredients = GameObject.FindGameObjectsWithTag("Ingredient");

        foreach (GameObject ingredient in ingredients)
        {
            Ingredient ingredientScript = ingredient.GetComponent<Ingredient>();

            // Check if the ingredient is magic
            if (ingredientScript != null && ingredientScript.GetIsMagic())
            {
                int uniqueID = ingredient.GetInstanceID();

                // Check if the ID is not already in the list
                if (!successfulMagicMushroomsIDs.Contains(uniqueID))
                {
                    successfulMagicMushroomsIDs.Add(uniqueID);
                    magicMushroomCount++;
                }
            }
        }

        sliderMagicMushrooms.value = magicMushroomCount;

        return magicMushroomCount >= requiredMagicMushrooms;
    }
    private void Mission2CompletionAction()
    {
        cauldron.SetActive(true);
        animScale.ScaleUp(cauldron, new(2,2,2));
        animScale.ScaleUp(potionBox);
        //animScale.ScaleUp(goal);
        //goal.GetComponentInChildren<Goal>().SetActivated(true);
    }

    private bool CheckMagicPotions()
    {
        GameObject[] bottles = GameObject.FindGameObjectsWithTag("Bottle");

        foreach (GameObject bottle in bottles)
        {
            Potion potion = bottle.GetComponent<Bottle>().GetPotion();

            // Check if the ingredients in the potion are magic
            if (potion != null &&
                potion.ingredient1.GetIsMagic() &&
                potion.ingredient2.GetIsMagic() &&
                potion.ingredient3.GetIsMagic())
            {
                int uniqueID = bottle.GetInstanceID();

                // Check if the ID is not already in the list
                if (!successfulMagicPotionsIDs.Contains(uniqueID))
                {
                    successfulMagicPotionsIDs.Add(uniqueID);
                    potionCount++;
                }
            }
        }

        sliderMagicPotions.value = potionCount;
        return potionCount >= requiredMagicPotions;
    }

    private void Mission3CompletionAction()
    {
        cauldron.SetActive(true);
        animScale.ScaleUp(cauldron, new(2, 2, 2));
        animScale.ScaleUp(potionBox);
        animScale.ScaleUp(goal);
    }

    private bool CheckServeMagicPotions()
    {
        if (!missions[2].isCompleted) return false;

        shouldSpawnCustomers = true;

        sliderServePotions.value = goalState.GetCompletedRecipesCount();

        return sliderServePotions.value >= (requiredServedPotions / 3);
    }

    private IEnumerator DelayedNewCustomer()
    {
        yield return new WaitForSeconds(customerDelayTime);

        goalState.NewCustomer();
        hasSpawnedCustomer = false; // Reset the flag to allow for the next customer spawn
    }

    private void Mission4CompletionAction()
    {
        Vector3[] spawnPositions = new Vector3[] //Order of spawnpoints corresponds which player should tp to it
        {
        gameManager.spawnpoint2.position, //Player 1 TP to spawnpoint 2
        gameManager.spawnpoint1.position, //Player 2 TP to spawnpoint 1
        gameManager.spawnpoint4.position, //Player 3 TP to spawnpoint 4
        gameManager.spawnpoint3.position //Player 4 TP to spawnpoint 3
        };

        for (int i = 0; i < players.Count; i++)
        {
            if (i < spawnPositions.Length)
            {
                players[i].gameObject.SetActive(false);
                players[i].transform.position = spawnPositions[i];
                players[i].gameObject.SetActive(true);
            } else
            {
                Debug.LogError("Invalid player index!");
                break;
            }
        }
        gameManager.spawnpoint1 = sp2;
        gameManager.spawnpoint2 = sp1;
        gameManager.spawnpoint3 = sp4;
        gameManager.spawnpoint4 = sp3;
    }
}

