using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    
    public float timeRemaining = 0;
    public float timeSeconds = 10f;
    public float timeMinutes = 0;

    [Header("Start timer running or not?")]
    private bool timerIsRunning = false;

    private StartAndEnd startAndEnd;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip tickingClip;
    public AudioClip timerDoneClip;

    [Header("Slowdown")]
    [SerializeField] private float slowDownWeightStart = 1f;
    [SerializeField] private float slowDownWeightEnd = 0.7f;
    [SerializeField] private float timeToSlowDown = 15f;
    private float slowDownWeight = 1;

    private void Start()
    {
        startAndEnd = GameObject.Find("Start & End").GetComponent<StartAndEnd>();
        timeRemaining = timeSeconds + 0.99f + (timeMinutes * 60);
        DisplayTime(timeRemaining);
    }
    void Update()
    {
        //Prototype Debugging tools
        if(Input.GetKeyDown(KeyCode.Keypad1)) {
            timerIsRunning = true;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            timerIsRunning = false;
            timeRemaining = timeSeconds + (timeMinutes * 60);
            DisplayTime(timeRemaining);
        }


        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                slowDownWeight = Mathf.Lerp(slowDownWeightStart, slowDownWeightEnd, Mathf.Clamp01(1 - (timeRemaining / timeToSlowDown)));
                //Debug.Log(slowDownWeight);

                timeRemaining -= Time.deltaTime * slowDownWeight;
                DisplayTime(timeRemaining);
                if(timeRemaining < 20) {
                    timerText.color = Color.red;

                    if (!audioSource.isPlaying)
                    {
                        //audioSource.clip = tickingClip;

                        audioSource.Play();
                    }
                }
            }
            else
            {
                Debug.Log("Time has run out!");

                audioSource.Stop();
                audioSource.PlayOneShot(timerDoneClip);
                timeRemaining = 0;
                DisplayTime(timeRemaining);
                startAndEnd.End();
                timerIsRunning = false;
            }
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public void StartTimer() {
        timerIsRunning = true;
    }
}

