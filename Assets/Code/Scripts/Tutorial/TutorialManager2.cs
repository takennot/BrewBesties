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

public class TutorialManager2 : MonoBehaviour
{
    [SerializeField] private GameManagerScript gameManager;
    [SerializeField] private SliderManager sliderManager;
    [SerializeField] private KillboxManager killboxManager;
/*    private List<int> successfulMagicPotionsIDs = new List<int>();
    private List<int> successfulMagicMushroomsIDs = new List<int>();
    private int potionCount = 0;
    private int magicMushroomCount = 0;*/

/*    [Header("Tutorial Conditions")]
    [SerializeField] private float requiredMagicMushrooms = 3f;
    [SerializeField] private float requiredMagicPotions = 3f;
    [Tooltip("Player swap will be at 1/3 of requiredServedPotions")]
    [SerializeField] private float requiredServedPotions = 6f; */

    [Header("GameObjects")]
    [SerializeField] private TriggerCount triggerCountPlayers;  
    [SerializeField] private TriggerCount triggerCountPlayers2;  
    //[SerializeField] private RespawnCheckpoint checkpointToDelete;
    [SerializeField] private GameObject[] fences;
    [SerializeField] private GameObject[] bridges;
    [SerializeField] private GameObject[] ingredientSpawners;
    private List<TutorialIngredientSpawner> spawners;
    [SerializeField] private CauldronState[] cauldrons;


    [Header("Animations")]
    [SerializeField] private AnimationScale animScale;

    [Header("Audio")]
    [SerializeField] private AudioSource source;

    [Header("Missions")]
    [SerializeField] private List<Mission> missions;

    [Header("UI")]
    [SerializeField] private Slider sliderCountPlayers;
    [SerializeField] private Slider sliderCountIngredients;
    [SerializeField] private TextTypewriter typewriter;
    [SerializeField] private List<TMP_Text> finishText = new();
    [SerializeField] private List<TMP_Text> playersInTrigger2 = new();
    [SerializeField] private List<TMP_Text> dragIngredients = new();
    //[SerializeField] private Slider sliderMagicPotions;
    //[SerializeField] private Slider sliderServePotions;

    [Header("Players")]
    [SerializeField] private List<PlayerScript> players;

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

    private int playerCount;
    private bool updateSlider = false;

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

        playerCount = gameManager.GetPlayerAmount();
        if (playerCount == 0) playerCount = 2;

        sliderCountPlayers.maxValue = playerCount;
        sliderManager.PlayEntryAnimation(sliderCountPlayers);
        sliderCountIngredients.maxValue = 6;

/*
        sliderMagicMushrooms.maxValue = requiredMagicMushrooms;
        sliderMagicPotions.maxValue = requiredMagicPotions;
        sliderServePotions.maxValue = requiredServedPotions;
*/
        InitializePlayers(playerCount);
        InitializeMissions();

        //circleTransition.SetPlayers(players);
        //circleTransition.OpenBlackScreen();

        //Scale 0,0,0
        triggerCountPlayers2.gameObject.transform.localScale = new Vector3(0, 0, 0);
        ScaleDownArrayInstantly(bridges);
        ScaleDownArrayInstantly(fences);
        ScaleDownArrayInstantly(ingredientSpawners);
        ScaleDownArrayInstantly(cauldrons);
        foreach(CauldronState cauldron in cauldrons)
        {
            cauldron.gameObject.transform.localScale = new Vector3(0, 0, 0);
            cauldron.gameObject.SetActive(false);
        }   

        CollectSpawners();
    }

    private void CollectSpawners()
    {
        spawners = new List<TutorialIngredientSpawner>();

        foreach (GameObject ingredientSpawner in ingredientSpawners)
        {
            TutorialIngredientSpawner[] spawner = ingredientSpawner.GetComponentsInChildren<TutorialIngredientSpawner>();
            spawners.AddRange(spawner);
        }
        foreach (TutorialIngredientSpawner spawner in spawners)
        {
            spawner.enabled = false;
        }
    }

    private void Update()
    {
        foreach (Mission mission in missions)
        {
            if (!mission.isCompleted && mission.missionCondition())
            {
                StartCoroutine(CompleteMission(mission)); // Start the completion coroutine
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
            missionName = "Players to green",
            missionCondition = () => CheckPlayerInTrigger(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            completionAction = Mission1CompletionAction(),
        },
        new Mission
        {
            missionName = "Players to green 2",
            missionCondition = () => CheckPlayerInTrigger2(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            completionAction = Mission2CompletionAction(),
        },
        new Mission
        {
            missionName = "Drag Ingredients",
            missionCondition = () => CheckDragIngredients(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            completionAction = Mission3CompletionAction(),
        },
    };
        if (playerCount == 0) playerCount = 2;

    }

    private IEnumerator CompleteMission(Mission mission)
    {
        mission.isCompleted = true;
        yield return StartCoroutine(mission.completionAction); // Use StartCoroutine here

        Debug.Log("Completed mission: " + mission.missionName);
    }


    //MISSION CONDITIONS

    private bool CheckPlayerInTrigger()
    {
        if (missions[0].isCompleted) return true;
        //Debug.Log(triggerCountPlayers.PlayerCount);
        sliderCountPlayers.value = triggerCountPlayers.PlayerCount;
        return triggerCountPlayers.PlayerCount >= playerCount;
    }

    IEnumerator Mission1CompletionAction()
    {
        updateSlider = false;
        typewriter.SetNewText(finishText);
        yield return new WaitForSeconds(0.5f);
        ScaleUpArray(fences);
        sliderManager.PlayExitAnimation(sliderCountPlayers);
        yield return new WaitForSeconds(1f);
        animScale.ScaleDown(triggerCountPlayers.gameObject);
        animScale.ScaleUp(triggerCountPlayers2.gameObject, new Vector3(7.01f, 2.7f, 7.01f));
        sliderCountPlayers.value = 0;
        yield return new WaitForSeconds(0.75f);
        sliderManager.PlayEntryAnimation(sliderCountPlayers);
        typewriter.SetNewText(playersInTrigger2);
        updateSlider = true;
    }

    private bool CheckPlayerInTrigger2()
    {
        if (!missions[0].isCompleted || !updateSlider) return false;
        sliderCountPlayers.value = triggerCountPlayers2.PlayerCount;
        return triggerCountPlayers2.PlayerCount >= playerCount;
    }

    IEnumerator Mission2CompletionAction()
    {
        typewriter.SetNewText(finishText);
        //Destroy(checkpointToDelete.gameObject);
        yield return new WaitForSeconds(0.5f);

        sliderManager.PlayExitAnimation(sliderCountPlayers);
        yield return new WaitForSeconds(1f);
        
        animScale.ScaleDown(triggerCountPlayers2.gameObject);
        yield return new WaitForSeconds(0.75f);

        sliderManager.PlayEntryAnimation(sliderCountIngredients);
        ScaleUpArray(bridges);
        ScaleDownArray(fences);
        yield return new WaitForSeconds(1f);

        ScaleUpArray(ingredientSpawners);
        yield return new WaitForSeconds(1f);

        foreach (TutorialIngredientSpawner spawner in spawners)
        {
            spawner.enabled = true;
        }
        yield return new WaitForSeconds(1f);

        foreach (CauldronState cauldron in cauldrons)
        {
            animScale.ScaleUp(cauldron.gameObject, new(2,2,2));
            cauldron.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(1f);
        
        typewriter.SetNewText(dragIngredients);
    }

    private bool CheckDragIngredients()
    {
        int count;
        count = cauldrons[0].GetIngredientCount() + cauldrons[1].GetIngredientCount();

        sliderCountIngredients.value = count;

        return cauldrons[0].GetIngredientCount() >= 3 && cauldrons[1].GetIngredientCount() >= 3;
    }

    IEnumerator Mission3CompletionAction()
    {
        yield return null;
    }

    //Invoke("LoadScene", loadSceneDelay);
    private void LoadScene()
    {
        SceneManager.LoadScene(sceneIndexLoad);
    }

    private void ScaleUpArray(GameObject[] gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            animScale.ScaleUp(gameObject);
        }
    }


    //SCALING
    private void ScaleDownArray(GameObject[] gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            animScale.ScaleDown(gameObject);
        }
    }

    private void ScaleDownArrayInstantly<T>(T[] elements) where T : Component
    {
        foreach (T element in elements)
        {
            GameObject go = element.gameObject;
            go.transform.localScale = new Vector3(0, 0, 0);
        }
    }

    private void ScaleDownArrayInstantly(GameObject[] gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.transform.localScale = new Vector3(0, 0, 0);
        }
    }
}

