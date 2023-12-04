using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private Goal goal;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        UpdateScore();

        //goal = FindObjectOfType<Goal>();

        if(scoreText == null)
            scoreText = GetComponentInChildren<TMP_Text>();

        if (scoreText == null)
        {
            Debug.LogWarning("Goal not found in game manager, SHUTTING DOWN");
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(scoreText == null)
        {
            return;
        }

        UpdateScore();

    }

    private void UpdateScore()
    {
        score = goal.GetScore();
        scoreText.text = "Score: " + score;
    }
}
