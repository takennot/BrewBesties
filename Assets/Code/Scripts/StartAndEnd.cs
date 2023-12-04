using System.Collections;
using TMPro;
using UnityEngine;
using Collections.Shaders.CircleTransition;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartAndEnd : MonoBehaviour 
{
    public TMP_Text countdownText;
    public Canvas countdownCanvas;
    private GameObject[] players;

    public CircleTransition circleTransition;
    public Timer timerLevel;

    private bool isEnding;
    public bool runElementsInLevel = false;
    public int nextSceneIndex;

    [Header("Refs")]
    [SerializeField] private Goal goal;
    [SerializeField] private GameManagerScript gameManager;
    [SerializeField] private PauseMenuScript pauseMenuScript;

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

    private void Start() 
    {
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

        gameManager.runElementsInLevel = false;
    }

    private void Update() 
    {
        //Spam drop items picked up by player
        //TODO remove this shit and make it better

        Debug.Log(isEnding);

        if(isEnding) {

            // drop all players
            foreach (GameObject player in players) {
                PlayerScript playerScript = player.GetComponent<PlayerScript>();
                if (playerScript != null) {
                    playerScript.Drop(false);
                    //playerScript.DropPlayer(false); // doesnt work. => isnt needed. Drop will go to DropPlayer() automatically if needed //saga
                }
            }

            gameManager.runElementsInLevel = false;
        }
    }

    IEnumerator StartGameCountdown() 
    {
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
    }
    bool isPaused;

    public void Pause()
    {
        switch (isPaused)
        {
            case true:
                //OnResume();
                isPaused = false;
                break;
            case false:
                
                //pauseMenuCanvas.enabled = true;
                Time.timeScale = 0f;
                //resumeButton.Select();
                isPaused = true;
                AudioController audioController = FindAnyObjectByType<AudioController>();
                if (audioController != null)
                {
                    audioController.song_source.Pause();
                }
                break;
        }
    }

    public void End() 
    {
        Debug.Log("Reached End()");
        isEnding = true;

        // stop all background stuff
        foreach (PlayerScript player in gameManager.GetComponent<GameManagerScript>().GetPlayersList())
        {
            player.GetCharacterController().enabled = false;
        }

        Pause();

        int score = goal.GetScore();
        bool completedLevel = score >= pointsOneStar;

        // set star image and point requirements

        if(completedLevel)
        {
            countdownText.text = "Level Completed! \n Score achieved: " + score;

            // show stars
            star1Image.color = Color.white;

            if(score >= pointsTwoStar)
            {
                star2Image.color = Color.white;
            }
            if(score >= pointsThreeStar)
            {
                star3Image.color = Color.white;
            }

        }
        else
        {
            countdownText.text = "Level not completed! \n Score achieved: " + score;
        }

        if(tip != null)
        {
            tip.enabled = false;
        }

        starPanel.SetActive(true);

        countdownCanvas.enabled = true;

        gameManager.SaveLog();

        //StartCoroutine(LoadNextSceneAfterDelay(completedLevel));
    }

    public void OnNext()
    {
        int score = goal.GetScore();
        bool completedLevel = score >= pointsOneStar;

        // enable characters
        foreach (PlayerScript player in gameManager.GetComponent<GameManagerScript>().GetPlayersList())
        {
            player.GetCharacterController().enabled = true;
        }

        StartCoroutine(CloseBlackScreenAfterDelay());
        StartCoroutine(LoadNextSceneAfterDelay(completedLevel));
    }

    public void OnRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    IEnumerator CloseBlackScreenAfterDelay() {
        yield return new WaitForSeconds(circleStartDelay);
        countdownCanvas.enabled = false;
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
