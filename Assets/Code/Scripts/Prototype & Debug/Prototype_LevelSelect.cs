using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class Prototype_LevelSelect : MonoBehaviour
{
    [SerializeField] private int level_index = 1;
    [SerializeField] private TextMeshProUGUI current_level;
    private string[] sceneNames; // To store scene names

    private Canvas canvas_menu;
    private bool isMenuVisible;

    private void Awake() {
        canvas_menu = GetComponentInParent<Canvas>();
        canvas_menu.enabled = false;
    }

    void Start()
    {
        // Fetch all scene names from the build settings
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        sceneNames = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++)
        {
            sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
        }

        UpdateCurrentLevelText();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            isMenuVisible = !isMenuVisible;
            canvas_menu.enabled = isMenuVisible;
        }    
    }

    void UpdateCurrentLevelText()
    {
        current_level.text = sceneNames[level_index];
    }

    public void NextLevel()
    {
        Debug.Log("Next Level ");
        level_index = (level_index + 1) % sceneNames.Length;
        UpdateCurrentLevelText();
    }

    public void PreviousLevel()
    {
        level_index = (level_index - 1 + sceneNames.Length) % sceneNames.Length;
        UpdateCurrentLevelText();
    }

    public void RestartLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        UpdateCurrentLevelText();
    }

    public void EnterScene()
    {
        SceneManager.LoadScene(sceneNames[level_index]);
    }
}
