using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManagerOld : MonoBehaviour
{
    [SerializeField] private TutorialLevel[] tutorials;
    [SerializeField] private GameManagerScript gameManager;
    [SerializeField] private bool hasPlayedCompletedTutorial;

    [SerializeField] private int sceneIndexLoad = 1; 


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Tutorial Manager got " + gameManager.GetPlayerAmount() + " players.");

        if(gameManager.GetPlayerAmount() == 0)
        {
            Debug.Log("***********Loaded tutorial with 0 selected players from the main menu, \n playing as if there is 1 Player");
            tutorials[0].isActive = true;
        } else
        {
            for (int i = 0; i < gameManager.GetPlayerAmount(); i++)
            {
                tutorials[i].isActive = true;
            }
        }

        foreach (TutorialLevel tutorial in tutorials)
        {
            if(!tutorial.isActive)
            {
                tutorial.gameObject.GetComponentInChildren<Canvas>().enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(hasPlayedCompletedTutorial)
        {
            return;
        }

        bool areAllCompleted = true;

        foreach (TutorialLevel tutorial in tutorials)
        {
            if (tutorial.isActive && !tutorial.hasCompletedTutorial)
            {
                areAllCompleted = false;
                break; 
            }
        }

        if (areAllCompleted)
        {
            // DONE WITH TUTORIALS
            Invoke("CompleteTutorial", 2.0f);
            hasPlayedCompletedTutorial = true;
        }
    }

    private void CompleteTutorial()
    {
        Debug.Log("Tutorial Manager: All active tutorials completed");
        SceneManager.LoadScene(sceneIndexLoad);
    }
}
