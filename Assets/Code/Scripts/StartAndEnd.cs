using System.Collections;
using TMPro;
using UnityEngine;
using Collections.Shaders.CircleTransition;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;
using System.ComponentModel;
using System.Collections.Generic;
//using static System.Net.Mime.MediaTypeNames;

public class StartAndEnd : MonoBehaviour 
{
    [Header("Scene")]
    public bool isTutorial = false;

    [Header("Refs")]
    public TMP_Text countdownText;
    public TMP_Text scoreText;
    public TMP_Text nextLevelText;
    public Canvas countdownCanvas;
    [SerializeField] private Animator wipeAnimation;
    private GameObject[] players;
    //[SerializeField] private Animator animWipe;

    public CircleTransition circleTransition;
    public Timer timerLevel;

    private bool isEnding;
    private bool hasStartedCircleTransition;

    bool completedLevel;

    [SerializeField] private Goal goal;
    [SerializeField] private GameManagerScript gameManager;

    [SerializeField] private TMP_Text tip;

    [Header("Scene")]
    public bool shouldLoadSpecifiedLevel;
    public int specifiedLevelIndex = 1;
    private int nextSceneIndex;

    [Header("Delays")]
    [SerializeField] public int timeToWaitForStart = 3;
    [SerializeField] private float circleStartDelay = 3f;
    [SerializeField] private float levelEndDelay = 8f;
    public bool hasStarted = false;

    [Header("Win conditions")]

    [SerializeField] private CompletionRequirements.RequirementsForLevels winConditions;
    private int pointsOneStar;
    private int pointsTwoStar;
    private int pointsThreeStar;

    [Header("StarsPanelStuff")]
    [SerializeField] private GameObject starPanel;
    [SerializeField] private GameObject endOptionsPanel;
    [SerializeField] private TMP_Text star1Text;
    [SerializeField] private TMP_Text star2Text;
    [SerializeField] private TMP_Text star3Text;

    [SerializeField] private Image star1Image;
    [SerializeField] private Image star2Image;
    [SerializeField] private Image star3Image;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSourceShuffle;
    [SerializeField] private AudioClip shuffle_audio;
    float pitchShuffle = 1;

    [SerializeField] private AudioSource audioSourceOther;
    [SerializeField] private AudioClip pling_audio;
    bool played1 = false;
    bool played2 = false;
    bool played3 = false;

    private void Start() 
    {
        pointsOneStar = CompletionRequirements.GetLevelRequirements(winConditions)[0];
        pointsTwoStar = CompletionRequirements.GetLevelRequirements(winConditions)[1];
        pointsThreeStar = CompletionRequirements.GetLevelRequirements(winConditions)[2];
        players = GameObject.FindGameObjectsWithTag("Player");

        //goal = GameObject.Find("Goal").GetComponent<Goal>();

        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerScript>().GetCharacterController().enabled = false;
        }

        AudioController audioController = FindAnyObjectByType<AudioController>();
        if (audioController)
        {
            audioController.song_source.clip = audioController.songStress4min;
            audioController.song_source.Stop();
            audioController.song_source.Play();
        }

        played1 = false;
        played2 = false;
        played3 = false;

        starPanel.SetActive(false);
        endOptionsPanel.SetActive(false);

        star1Image.color = Color.gray;
        star2Image.color = Color.gray;
        star3Image.color = Color.gray;

        if(!isTutorial)
        {
            star1Text.text = "" + pointsOneStar;
            star2Text.text = "" + pointsTwoStar;
            star3Text.text = "" + pointsThreeStar;
        }
        else
        {
            star1Text.text = "";
            star2Text.text = "" + pointsOneStar;
            star3Text.text = "";
        }

        if (!isTutorial)
        {
            StartCoroutine(StartGameCountdown());
        }
        else
        {
            hasStarted = true;
            countdownCanvas.enabled = false;
            countdownText.text = "";
            timerLevel.DisableTimer();

            foreach (GameObject player in players)
            {
                player.GetComponent<PlayerScript>().GetCharacterController().enabled = true;
            }
        }
    }

    // score Countdown stuff
    bool scoreCountdown = false;
    int score = 0;
    string text = "Score: ";
    float count = 0f;

    private bool hasPressedButton = false;
    private void Update() {

        //foreach (PlayerScript player in gameManager.GetPlayersList())
        //{
        //    Debug.Log(player + " " + player.GetCharacterController().enabled);
        //}

        //Spam drop items picked up by player
        //TODO remove this shit and make it better
        if(isEnding && !hasStartedCircleTransition) {
            foreach (GameObject player in players) {
                PlayerScript playerScript = player.GetComponent<PlayerScript>();
                if (playerScript != null) {
                    playerScript.Drop(false);
                    //playerScript.DropPlayer(false); // doesnt work. => isnt needed. Drop will go to DropPlayer() automatically if needed //saga
                }
            }

            // or here????

            if (endOptionsPanel.activeSelf && !hasPressedButton)
            {
                if ((Input.GetKeyDown(KeyCode.Joystick1Button0) && completedLevel))
                {
                    OnNext();
                    hasPressedButton = true;
                }
                else if (Input.GetKeyDown(KeyCode.Joystick1Button1))
                {
                    StartCoroutine(OnRestart());
                    hasPressedButton = true;
                }
                else if (Input.GetKeyDown(KeyCode.Joystick1Button2))
                {
                    SceneManager.LoadScene(0);
                    hasPressedButton = true;
                }
            }
            
        }

        if (scoreCountdown)
        {
            if (score != 0) pitchShuffle = 1 + (count / score);
            else            pitchShuffle = 1;

            
            audioSourceShuffle.pitch = pitchShuffle;

            starPanel.SetActive(true);

            if (count < score)
            {
                count += Time.deltaTime * 200;
            }
            else
            {
                if (count > score)
                    count = score;

                FinishShowScore();
                scoreCountdown = false;
            }

            // show stars
            if(!isTutorial)
            {
                if (count >= pointsThreeStar)
                {
                    ShowStarImage(3);
                }
                else if (count >= pointsTwoStar)
                {
                    ShowStarImage(2);
                }
                else if (count >= pointsOneStar)
                {
                    completedLevel = score >= pointsOneStar;

                    ShowStarImage(1);
                }
            }
            else
            {
                completedLevel = true;
                TutorialStars();
            }
            
            scoreText.text = text + (int)count;

        }

        if (completedLevel)
        {
            nextLevelText.fontStyle = FontStyles.Normal;
        }
        else
        {
            nextLevelText.fontStyle = FontStyles.Strikethrough;
        }
    }

    private void TutorialStars()
    {
        ShowStarImage(2);
        star1Image.enabled = false;
        star3Image.enabled = false;
    }

    private void ShowScore()
    {
        scoreCountdown = true;
        audioSourceShuffle.clip = shuffle_audio;
        audioSourceShuffle.Play();

        //score = 700; // TA BORTTTT --------------------------------------------

        scoreText.text = text + "0";

        // here?
    }

    private void FinishShowScore()
    {
        audioSourceShuffle.Stop();
        endOptionsPanel.SetActive(true);
        Debug.Log("Done!!!!!!!!!");
    }

    private void ShowStarImage(int starNr)
    {
        switch (starNr)
        {
            case 1:
                star1Image.color = Color.white;

                if (!played1)
                {
                    audioSourceOther.pitch = 1.0f;
                    audioSourceOther.PlayOneShot(pling_audio);
                    played1 = true;
                }

                break;
            case 2:
                star1Image.color = Color.white;
                star2Image.color = Color.white;

                if (!played2)
                {
                    audioSourceOther.pitch = 1.2f;
                    audioSourceOther.PlayOneShot(pling_audio);
                    played2 = true;
                }

                break;
            case 3:
                star1Image.color = Color.white;
                star2Image.color = Color.white;
                star3Image.color = Color.white;

                if (!played3)
                {
                    audioSourceOther.pitch = 1.8f;
                    audioSourceOther.PlayOneShot(pling_audio);
                    played3 = true;
                }

                break;
        }
    }

    IEnumerator StartGameCountdown() {
        
        yield return new WaitForSeconds(0.7f); //Should not be used. Used so to wait for the start wipe to finish

        int countdownTime = timeToWaitForStart;

        while (countdownTime > 0) {
            countdownText.text = "Starting in " + countdownTime.ToString() + "...";
            yield return new WaitForSeconds(1f);
            countdownTime--;
            if(countdownTime < 0.01f) {
                hasStarted = true;

                foreach (GameObject player in players)
                {
                    player.GetComponent<PlayerScript>().GetCharacterController().enabled = true;
                }

                timerLevel.StartTimer();
            }
        }

        countdownCanvas.enabled = false;

        countdownText.text = "";
    }

    //private bool isPaused = false;

    /// <summary>
    /// Will Pause or unpause the state of the game. 
    /// </summary>
    public void Pause()
    {
        Debug.Log("Pause or UnPause");

        gameManager.PauseGame();
    }

    public void End() {
        Debug.Log("Reached End()");
        isEnding = true;

        foreach (PlayerScript player in gameManager.GetPlayersList()) 
        { 
            player.GetCharacterController().enabled = false;
        }

        Pause();

        if(!isTutorial)
        {
            score = goal.GetScore();
        }
        else
        {
            score = 1;
        }

        if(tip != null)
        {
            tip.enabled = false;
        }

        countdownCanvas.enabled = true;

        gameManager.SaveLog();


        Debug.Log("Start the thing!");

        if (FindAnyObjectByType<MainMenuData>())
        {
            FindAnyObjectByType<MainMenuData>().UpdateHighscore(SceneManager.GetActiveScene().name, score);
        }
        else
        {
            Debug.LogWarning("No main menu data, so no save file for you bitchhh");
        }

        ShowScore();

        //StartCoroutine(LoadNextSceneAfterDelay(completedLevel));
        // or here?
    }

    public void OnNext()
    {
        //foreach (GameObject player in players)
        //{
        //    player.GetComponent<PlayerScript>().GetCharacterController().enabled = true;
        //}

        countdownCanvas.enabled = false;

        if(!isTutorial) 
        {
            Pause();
            StartCoroutine(CloseBlackScreenAfterDelay());
        }
        else
        {
            wipeAnimation.SetTrigger("End");
        }

        StartCoroutine(LoadNextSceneAfterDelay(true));
    }

    IEnumerator OnRestart()
    {
        countdownCanvas.enabled = false;
        wipeAnimation.SetTrigger("End");
        FadeAllAudioSources(1f, 0f);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator CloseBlackScreenAfterDelay() {
        yield return new WaitForSeconds(circleStartDelay);
        
        circleTransition.CloseBlackScreen();
    }

    IEnumerator LoadNextSceneAfterDelay(bool finishedLevel) {
        yield return new WaitForSeconds(0.3f);
        foreach (PlayerScript player in gameManager.GetPlayersList())
        {
            player.GetCharacterController().enabled = true;
            player.enabled = true;
        }

        FadeAllAudioSources(2f, 5.5f);
        yield return new WaitForSeconds(levelEndDelay);
        if (finishedLevel)
        {
            if (shouldLoadSpecifiedLevel)
            {
                SceneManager.LoadScene(specifiedLevelIndex);
            } 
            else
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
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        yield return null;
    }

    private void FadeAllAudioSources(float fadeDuration, float waitBeforeFading)
    {
        // Get all AudioSources in the scene
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in allAudioSources)
        {
            StartCoroutine(FadeAudioSource(audioSource, fadeDuration, waitBeforeFading));
        }
    }

    // Coroutine to fade a single AudioSource
    private IEnumerator FadeAudioSource(AudioSource audioSource, float fadeDuration, float waitBeforeFading)
    {
        yield return new WaitForSeconds(waitBeforeFading);
        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0f; // Ensure volume is zero at the end
        audioSource.Stop(); // Stop the audio source
    }

    public int GetPlayerAmount()
    {
        return gameManager.GetPlayerAmount();
    }
}
