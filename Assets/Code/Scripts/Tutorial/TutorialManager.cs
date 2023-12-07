using Collections.Shaders.CircleTransition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static CustomerOrder;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameManagerScript gameManager;
    [SerializeField] private SliderManager sliderManager;
    [SerializeField] private KillboxManager killboxManager;
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
    [SerializeField] private AudioController audioController;

    [Header("Animations")]
    [SerializeField] private AnimationScale animScale;

    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip playersDissapear;
    [SerializeField] private AudioClip playerAppear;

    [Header("Missions")]
    [SerializeField] private List<Mission> missions;

    [Header("UI")]
    [SerializeField] private Color colorCompleted = Color.green;
    [SerializeField] private Slider sliderMagicMushrooms;
    [SerializeField] private Slider sliderMagicPotions;
    [SerializeField] private Slider sliderServePotions;

    [Header("Players")]
    [SerializeField] private List<PlayerScript> players;

    [Header("Customers")]
    private bool hasSpawnedCustomer = false;
    private bool shouldSpawnCustomers = false;
    [SerializeField] private float customerDelayTime = 3.0f;

    [Header("Ending")]
    [SerializeField] private CircleTransition circleTransition;
    [SerializeField] private float loadSceneDelay = 8f;
    [SerializeField] private int sceneIndexLoad = 1;

    AudioLowPassFilter lowPassFilter;
    private float originalVolume;

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

        //circleTransition.SetPlayers(players);
        //circleTransition.OpenBlackScreen();

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
            missionName = "Serve x magic potions",
            missionCondition = () => CheckServeMagicPotions(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            onCompletionAction = () => Mission4CompletionAction(),
        },
        new Mission
        {
            missionName = "Serve x potions",
            missionCondition = () => CheckServePotions(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            onCompletionAction = () => Mission5CompletionAction(),
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

        sliderManager.PlayEntryAnimation(sliderMagicMushrooms);
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

        sliderManager.PlayExitAnimation(sliderMagicMushrooms);
        sliderManager.PlayEntryAnimation(sliderMagicPotions);
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
        goal.GetComponentInChildren<Goal>().SetActivated(true);

        sliderManager.PlayExitAnimation(sliderMagicPotions);
        sliderManager.PlayEntryAnimation(sliderServePotions);
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

        for (int i = 0; i < players.Count; i++)
        {
            players[i].gameObject.SetActive(false);
            players[i].transform.position = new Vector3(-17.5f, 0, -13.5f);
        }
        source.PlayOneShot(playersDissapear);

        audioController = FindObjectOfType<AudioController>();

        originalVolume = audioController.song_source.volume;
        /*
        float cutoffFrequency = 500;
        float resonanceQ = 1;

        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            AudioLowPassFilter lowPassFilter = audioSource.GetComponent<AudioLowPassFilter>();

            if (lowPassFilter == null)
            {
                lowPassFilter = audioSource.gameObject.AddComponent<AudioLowPassFilter>();
            }

            lowPassFilter.cutoffFrequency = cutoffFrequency;
            lowPassFilter.lowpassResonanceQ = resonanceQ;
        }
        */

        StartCoroutine(FadeVolume(originalVolume, 0.02f, 0.75f));
        StartCoroutine(SwapPlayersCoroutine());

        goalState.playersCollidingWIth = 0;
        goalState.magicMushroomPercent = 0.5f;
    }

    private IEnumerator FadeVolume(float startVolume, float targetVolume, float duration)
    {
        float currentTime = 0;
        float start = startVolume;
        float end = targetVolume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(start, end, currentTime / duration);

            audioController.song_source.volume = newVolume;

            yield return null;
        }

        audioController.song_source.volume = targetVolume; // Ensure final volume matches target
    }

    private IEnumerator SwapPlayersCoroutine()
    {
        shouldSpawnCustomers = false;
        yield return new WaitForSeconds(4f);
        StartCoroutine(FadeVolume(0.02f, originalVolume, 6f));
        Debug.Log("Hello");
        Transform[] spawnPoints = new Transform[] //Order of spawnpoints corresponds which player should tp to it
        {
        gameManager.spawnpoint2, //Player 1 TP to spawnpoint 2
        gameManager.spawnpoint1, //Player 2 TP to spawnpoint 1
        gameManager.spawnpoint4, //Player 3 TP to spawnpoint 4
        gameManager.spawnpoint3 //Player 4 TP to spawnpoint 3
        };
        killboxManager.SetSpawnpoint(sp2, 1);
        killboxManager.SetSpawnpoint(sp1, 2);
        killboxManager.SetSpawnpoint(sp4, 3);
        killboxManager.SetSpawnpoint(sp3, 4);
        gameManager.spawnpoint1 = sp2;
        gameManager.spawnpoint2 = sp1;
        gameManager.spawnpoint3 = sp4;
        gameManager.spawnpoint4 = sp3;

        for (int i = 0; i < players.Count; i++)
        {
            if (i < spawnPoints.Length)
            {
                players[i].transform.position = spawnPoints[i].position;

                killboxManager.respawnVFXInstance = Instantiate(killboxManager.respawnVFX, spawnPoints[i]);
                Destroy(killboxManager.respawnVFXInstance, 1);
                source.PlayOneShot(playerAppear);
                players[i].gameObject.SetActive(true);
                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                Debug.LogError("Invalid player index!");
                break;
            }
        }
        shouldSpawnCustomers = true;
    }

    private bool CheckServePotions()
    {
        if (!missions[3].isCompleted) return false;

        sliderServePotions.value = goalState.GetCompletedRecipesCount();

        return sliderServePotions.value >= requiredServedPotions;
    }

    private void Mission5CompletionAction()
    {
        //circleTransition.CloseBlackScreen();
        Invoke("LoadScene", loadSceneDelay);
    }


    private void LoadScene()
    {
        SceneManager.LoadScene(sceneIndexLoad);
    }
}

