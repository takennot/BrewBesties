using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static Tutorial1Manager;

public class Tutorial1Manager : MonoBehaviour
{
    [SerializeField] private GameManagerScript gameManager;

    [Header("Counters")]
    [SerializeField] private GameObject[] countersLeft;
    [SerializeField] private GameObject[] countersRight;
    [SerializeField] private GameObject[] countersTop;
    [SerializeField] private GameObject[] countersBottom;
    [SerializeField] private GameObject counterCenter;

    [Header("Other GameObjects")]
    [SerializeField] private GameObject[] resourceBoxes;

    [Header("Animations")]
    [SerializeField] private AnimationScale animScale;

    [Header("Missions")]
    [SerializeField] private List<Mission> missions;

    [Header("Text Properties")]
    [SerializeField] private Color colorCompleted = Color.green;
    [SerializeField] private TMP_Text headerTextUI;
    [SerializeField] private List<TMP_Text> subTextsUI;

    [Header("Players")]
    [SerializeField] private List<PlayerScript> players;

    private int currentMissionIndex = 0;

    public class Mission
    {
        public string missionName;
        public Func<bool> missionCondition;
        public bool isCompleted;
        public string headerText; 
        public List<string> subTexts;
        public List<bool> playerFulfillment;
        public Action onCompletionAction;
    }

    void Start()
    {

        ScaleDown(countersLeft);
        ScaleDown(countersRight);
        ScaleDown(countersTop);
        ScaleDown(countersBottom);
        counterCenter.transform.localScale = new(0, 0, 0);
        counterCenter.transform.localScale = new(0, 0, 0);

        int playerCount = gameManager.GetPlayerAmount();
        if(playerCount == 0) playerCount = 2;
        InitializePlayers(playerCount);

        InitializeMissions();
        DisplayMissionDetails(missions[currentMissionIndex]); // Display the first mission
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
        UpdateMissionSubtextUI();

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
    }


    void ScaleDown(GameObject[] counters)
    {
        foreach (GameObject counter in counters)
        {
            counter.transform.localScale = new(0, 0, 0);
        }
    }

    void ScaleUpAnimaton(GameObject[] counters)
    {
        foreach (GameObject counter in counters)
        {
            animScale.ScaleUp(counter);
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
            headerText = "All players hold an Ingredient",
            subTexts = new List<string>(),
            playerFulfillment = new List<bool>(),
            onCompletionAction = () => Mission1CompletionAction(),
        },
        new Mission
        {
            missionName = "Hold the correct ingredient",
            missionCondition = () => CheckCondition(),
            isCompleted = false,
            headerText = "Get Player 1 to hold a mushroom",
            subTexts = new List<string>(),
            playerFulfillment = new List<bool>(),
            onCompletionAction = () => Mission2CompletionAction(),
        }
        //ADD MISSIONS HERE
    };

        int playerCount = gameManager.GetPlayerAmount();
        if (playerCount == 0) playerCount = 2;

        // Add subtexts for each player based on the player count
        for (int i = 0; i < playerCount; i++)
        {
            missions[0].subTexts.Add($"Player {i + 1} hold an Ingredient");
            missions[0].playerFulfillment.Add(false);
        }
        for (int i = 0; i < playerCount; i++)
        {
            missions[1].subTexts.Add($"Player {i + 1} do a wicked drift!!");
            missions[1].playerFulfillment.Add(false);
        }

    }

    // ******** UI
    private void UpdateMissionSubtextUI()
    {
        if (currentMissionIndex < missions.Count)
        {
            Mission currentMission = missions[currentMissionIndex];
            for (int i = 0; i < currentMission.subTexts.Count; i++)
            {
                bool isPlayerFulfilled = currentMission.playerFulfillment[i]; // Get the fulfillment status
                UpdateSubtextUI(i, isPlayerFulfilled); // Update the UI for each player
            }
        }
    }

    private void UpdateSubtextUI(int playerIndex, bool isFulfilled)
    {
        if (isFulfilled)
        {
            //Debug.Log("Player #" + (playerIndex + 1) + " should have bold style");
            subTextsUI[playerIndex].fontStyle = FontStyles.Strikethrough;
        }
        else if (!isFulfilled)
        {
            //Debug.Log("Player #" + (playerIndex + 1) + " should have normal style");
            subTextsUI[playerIndex].fontStyle = FontStyles.Normal;
        }

    }

    private void CompleteMission(Mission mission)
    {
        mission.isCompleted = true;
        mission.onCompletionAction?.Invoke();

        // Additional actions for completing the mission...
        Debug.Log("Completed mission: " + mission.missionName);
        headerTextUI.color = colorCompleted;

        // Display the next mission if available
        currentMissionIndex++;
        if (currentMissionIndex < missions.Count)
        {
            StartCoroutine(DelayedDisplayNextMission(2.0f));
        }
    }

    private IEnumerator DelayedDisplayNextMission(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentMissionIndex < missions.Count)
        {
            DisplayMissionDetails(missions[currentMissionIndex]);
        }
    }

    private void DisplayMissionDetails(Mission mission)
    {
        headerTextUI.text = mission.headerText;

        for (int i = 0; i < subTextsUI.Count; i++)
        {
            if (i < mission.subTexts.Count)
            {
                subTextsUI[i].text = mission.subTexts[i];
            }
            else
            {
                subTextsUI[i].text = ""; // Clear remaining subtexts
            }
        }
    }

    //MISSION CONDITIONS
    private bool CheckPickupCondition()
    {
        bool allPlayersFulfilled = true; // Assume all players fulfilled the requirement

        // Check each player's fulfillment
        for (int i = 0; i < players.Count; i++)
        {
            bool playerHoldingIngredient =
                players[i].GetObjectInHands() != null &&
                players[i].GetObjectInHands().CompareTag("Ingredient") &&
                players[i].GetObjectInHands().GetComponent<Ingredient>() != null;

            // If the player is currently holding an ingredient
            if (playerHoldingIngredient)
            {
                // Mark the player as fulfilled for the current mission
                missions[0].playerFulfillment[i] = true;
            }
            else
            {
                Debug.Log("Player" + (i+1) +  " no Longer holding");
                missions[0].playerFulfillment[i] = false;
            }

            // Update the flag indicating if all players have fulfilled the requirement
            allPlayersFulfilled &= missions[0].playerFulfillment[i];
        }

        // Return the flag indicating if all players have fulfilled the requirement
        return allPlayersFulfilled;
    }

    private void Mission1CompletionAction()
    {
        ScaleUpAnimaton(countersTop);
        ScaleUpAnimaton(countersBottom);
        ScaleUpAnimaton(countersLeft);
        ScaleUpAnimaton(countersRight);

        foreach (var player in players)
        {
            //player.GetComponent<PlayerScript>().Drop();
        }
        List<Ingredient> ingredientList = FindObjectsOfType<Ingredient>().ToList();
        foreach (var ingredient in ingredientList)
        {
            Destroy(ingredient.gameObject);
        }

    }



    private bool CheckCondition()
    {
        if(players.Count == 2) { 
            

        
        }

        return false;
    }
    private void Mission2CompletionAction()
    {

    }
}


