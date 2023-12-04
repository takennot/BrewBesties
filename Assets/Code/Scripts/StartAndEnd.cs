using System.Collections;
using TMPro;
using UnityEngine;
using Collections.Shaders.CircleTransition;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;
//using static System.Net.Mime.MediaTypeNames;

public class StartAndEnd : MonoBehaviour {
    public TMP_Text countdownText;
    public TMP_Text scoreText;
    public Canvas countdownCanvas;
    private GameObject[] players;
    [SerializeField] Button buttonRestart;
    [SerializeField] Button buttonNext;

    public CircleTransition circleTransition;
    public Timer timerLevel;

    private bool isEnding;
    public int nextSceneIndex;

    [SerializeField] private Goal goal;
    [SerializeField] private GameObject gameManager;

    [SerializeField] private TMP_Text tip;

    [Header("Delays")]
    [SerializeField] int timeToWaitForStart = 3;
    [SerializeField] private float circleStartDelay = 3f;
    [SerializeField] private float levelEndDelay = 9f;
    public bool hasStarted = false;

    [Header("Win conditions")]
    [SerializeField] private int pointsOneStar = 300;
    [SerializeField] private int pointsTwoStar = 450;
    [SerializeField] private int pointsThreeStar = 600;

    [Header("StarsPanelStuff")]
    [SerializeField] private GameObject starPanel;
    [SerializeField] private TMP_Text star1Text;
    [SerializeField] private TMP_Text star2Text;
    [SerializeField] private TMP_Text star3Text;

    [SerializeField] private Image star1Image;
    [SerializeField] private Image star2Image;
    [SerializeField] private Image star3Image;

    private void Start() {
        players = GameObject.FindGameObjectsWithTag("Player");

        //goal = GameObject.Find("Goal").GetComponent<Goal>();

        foreach (GameObject player in players) {
            PlayerScript playerScript = player.GetComponent<PlayerScript>();
            if (playerScript != null) {
                playerScript.enabled = false;
            }
        }
        
        AudioController audioController = FindAnyObjectByType<AudioController>();
        if (audioController)
        {
            audioController.song_source.clip = audioController.songStress4min;
            audioController.song_source.Stop();
            audioController.song_source.Play();
        }

        starPanel.SetActive(false);
        star1Image.color = Color.gray;
        star2Image.color = Color.gray;
        star3Image.color = Color.gray;

        star1Text.text = "" + pointsOneStar;
        star2Text.text = "" + pointsTwoStar;
        star3Text.text = "" + pointsThreeStar;

        StartCoroutine(StartGameCountdown());
    }

    // score Countdown stuff
    bool scoreCountdown = false;
    int score = 0;
    string text = "Score: ";
    float count = 0f;

    private void Update() {
        //Spam drop items picked up by player
        //TODO remove this shit and make it better
        if(isEnding) {
            foreach (GameObject player in players) {
                PlayerScript playerScript = player.GetComponent<PlayerScript>();
                if (playerScript != null) {
                    playerScript.Drop(false);
                    //playerScript.DropPlayer(false); // doesnt work. => isnt needed. Drop will go to DropPlayer() automatically if needed //saga
                }
            }
        }

        if (scoreCountdown)
        {
            if (count < score)
            {
                if (count > score)
                    count = score;

                count += Time.deltaTime * 200;
            }
            else
            {
                FinishShowScore();
                scoreCountdown = false;
            }

            // show stars
            if (count >= pointsOneStar)
            {
                ShowStarImage(1);
            }
            if (count >= pointsTwoStar)
            {
                ShowStarImage(2);
            }
            if (count >= pointsThreeStar)
            {
                ShowStarImage(3);
            }

            scoreText.text = text + (int)count;

        }
    }

    private void ShowScore()
    {
        scoreCountdown = true;

        score = 100; // TA BORTTTT

        scoreText.text = text + "0";
    }

    private void FinishShowScore()
    {
        bool completedLevel = score >= pointsOneStar;

        if (!completedLevel)
        {
            buttonNext.enabled = false;
        }

        Debug.Log("Done!!!!!!!!!");
    }

    private void ShowStarImage(int starNr)
    {
        switch (starNr)
        {
            case 1:
                star1Image.color = Color.white;
                break;
            case 2:
                star1Image.color = Color.white;
                star2Image.color = Color.white;
                break;
            case 3:
                star1Image.color = Color.white;
                star2Image.color = Color.white;
                star3Image.color = Color.white;
                break;
        }
    }

    IEnumerator StartGameCountdown() {
        int countdownTime = timeToWaitForStart;

        while (countdownTime > 0) {
            countdownText.text = "Starting in " + countdownTime.ToString() + "...";
            yield return new WaitForSeconds(1f);
            countdownTime--;
            if(countdownTime < 0.01f) {
                hasStarted = true;
                timerLevel.StartTimer();
            }
        }

        countdownCanvas.enabled = false;

        // Enable the players
        foreach (GameObject player in players) {
            PlayerScript playerScript = player.GetComponent<PlayerScript>();
            if (playerScript != null) {
                playerScript.enabled = true;
                playerScript.StartFootSteps();
            }
        }

        countdownText.text = "";
    }

    private bool isPaused = false;

    public void Pause()
    {
        switch (isPaused)
        {
            case true:
                
                isPaused = false;
                goal.SetActivated(!isPaused);
                break;
            case false:

                isPaused = true;
                goal.SetActivated(!isPaused);
                break;
        }
    }

    public void End() {
        Debug.Log("Reached End()");
        isEnding = true;

        foreach (PlayerScript player in gameManager.GetComponent<GameManagerScript>().GetPlayersList()) 
        { 
            player.GetCharacterController().enabled = false;
        }

        Pause();

        score = goal.GetScore();

        if(tip != null)
        {
            tip.enabled = false;
        }

        starPanel.SetActive(true);

        countdownCanvas.enabled = true;

        gameManager.GetComponent<GameManagerScript>().SaveLog();

        buttonRestart.Select();

        Debug.Log("Start the thing!");

        ShowScore();

        //StartCoroutine(LoadNextSceneAfterDelay(completedLevel));
    }

    public void OnNext()
    {
        foreach (PlayerScript player in gameManager.GetComponent<GameManagerScript>().GetPlayersList())
        {
            player.GetCharacterController().enabled = true;
        }
        countdownCanvas.enabled = false;

        StartCoroutine(CloseBlackScreenAfterDelay());
        StartCoroutine(LoadNextSceneAfterDelay(true));
    }

    public void OnRestart()
    {
        countdownCanvas.enabled = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator CloseBlackScreenAfterDelay() {
        yield return new WaitForSeconds(circleStartDelay);
        
        circleTransition.CloseBlackScreen();
    }

    IEnumerator LoadNextSceneAfterDelay(bool finishedLevel) {
        yield return new WaitForSeconds(levelEndDelay);

        if (finishedLevel)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
    }
}
