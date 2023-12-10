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

    [Header("Animations")]
    [SerializeField] private AnimationScale animScale;

    [Header("Audio")]
    [SerializeField] private AudioSource source;

    [Header("Missions")]
    [SerializeField] private List<Mission> missions;

    [Header("UI")]
    [SerializeField] private Slider sliderCountPlayers;
    //[SerializeField] private Slider sliderMagicPotions;
    //[SerializeField] private Slider sliderServePotions;

    [Header("Players")]
    [SerializeField] private List<PlayerScript> players;

    [Header("Ending")]
    [SerializeField] private CircleTransition circleTransition;
/*    [SerializeField] private float loadSceneDelay = 8f;
    [SerializeField] private int sceneIndexLoad = 1;*/

    AudioLowPassFilter lowPassFilter;
    private float originalVolume;

    //Original Spawnpoints
    private Transform sp1;
    private Transform sp2;
    private Transform sp3;
    private Transform sp4;

    private int playerCount;
    private int sceneIndexLoad;
    private int loadSceneDelay;

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

        playerCount = gameManager.GetPlayerAmount();
        if (playerCount == 0) playerCount = 2;

        sliderCountPlayers.maxValue = playerCount;
        sliderManager.PlayEntryAnimation(sliderCountPlayers);
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
/*        foreach (var workstationPrompt in workstationsPrompts)
        {
            workstationPrompt.transform.localScale = new Vector3(0, 0, 0);
        }
        cauldron.transform.localScale = new Vector3(0, 0, 0);
        cauldron.SetActive(false);
        goal.transform.localScale = new Vector3(0, 0, 0);
        goalState = goal.GetComponentInChildren<Goal>();
        goalState.magicMushroomPercent = 100f;
        potionBox.transform.localScale = new Vector3(0, 0, 0);*/

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
            missionName = "Count",
            missionCondition = () => CheckPlayerInTrigger(),
            isCompleted = false,
            playerFulfillment = new List<bool>(),
            onCompletionAction = () => Mission1CompletionAction(),
        },
    };
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

    private bool CheckPlayerInTrigger()
    {
        Debug.Log(triggerCountPlayers.PlayerCount);
        sliderCountPlayers.value = triggerCountPlayers.PlayerCount;
        return triggerCountPlayers.PlayerCount >= playerCount;
    }

    private void Mission1CompletionAction()
    {
        //circleTransition.CloseBlackScreen();

    }


    //Invoke("LoadScene", loadSceneDelay);
    private void LoadScene()
    {
        SceneManager.LoadScene(sceneIndexLoad);
    }
}

