using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCamera : MonoBehaviour
{
    [SerializeField] private GameManagerScript gameManager;
    [SerializeField] private Camera Camera1P;
    [SerializeField] private Camera Camera2P;
    [SerializeField] private Camera Camera3or4P;

    /*
    // Start is called before the first frame update
    void Start()
    {
        Camera1P.gameObject.SetActive(false);
        Camera2P.gameObject.SetActive(false);
        Camera3or4P.gameObject.SetActive(false);

        if (gameManager.GetPlayerAmount() == 1)
        {
            Debug.Log("Tutorial camera = 1P");
            Camera1P.gameObject.SetActive(true);
            Camera1P.GetComponent<CameraUIManager>().Initilize();

            foreach (TutorialLevel tutorialLevel in FindObjectsByType<TutorialLevel>(FindObjectsSortMode.None))
            {
                tutorialLevel.camera = Camera1P;
            }
        } 
        else if (gameManager.GetPlayerAmount() == 2)
        {
            Debug.Log("Tutorial camera = 2P");
            Camera2P.gameObject.SetActive(true);
            Camera1P.GetComponent<CameraUIManager>().Initilize();

            foreach (TutorialLevel tutorialLevel in FindObjectsByType<TutorialLevel>(FindObjectsSortMode.None))
            {
                tutorialLevel.camera = Camera2P;
            }
        } 
        else
        {
            Debug.Log("Tutorial camera = 3-4P");
            Camera3or4P.gameObject.SetActive(true);
            Camera1P.GetComponent<CameraUIManager>().Initilize();

            foreach (TutorialLevel tutorialLevel in FindObjectsByType<TutorialLevel>(FindObjectsSortMode.None))
            {
                tutorialLevel.camera = Camera3or4P;
            }
        }
    }
    */
}
