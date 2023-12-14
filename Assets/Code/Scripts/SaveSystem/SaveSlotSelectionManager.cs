using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotSelectionManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Canvas levelSelectCanvas;
    [SerializeField] private Button playButton;
    [SerializeField] private Button saveSlot1;
    [SerializeField] private Button fakeButton;
    [SerializeField] private SaveSlotManager[] saveSlots = new SaveSlotManager[3];
    [SerializeField] public string[] allSceneNames = new string[11];
    [SerializeField] public Button[] levelButtons = new Button[7];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canvas.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                canvas.gameObject.SetActive(false);
                playButton.Select();
            }
            if (Input.GetKeyDown(KeyCode.Joystick1Button2))
            {
                // EventSystem.current.currentSelectedGameObject
                if (EventSystem.current.currentSelectedGameObject.transform.parent.name.Equals("SaveSlot0"))
                {
                    SaveSlotReset(0);
                }
                else if (EventSystem.current.currentSelectedGameObject.transform.parent.name.Equals("SaveSlot1"))
                {
                    SaveSlotReset(1);
                }
                else if (EventSystem.current.currentSelectedGameObject.transform.parent.name.Equals("SaveSlot2"))
                {
                    SaveSlotReset(2);
                }
            }
        }
        if (levelSelectCanvas.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                levelSelectCanvas.gameObject.SetActive(false);
                canvas.gameObject.SetActive(true);
                saveSlot1.Select();
            }
        }
    }

    public void PreviewSaveSlots()
    {
        int i = 0;
        
        foreach (SaveSlotManager saveSlotManager in saveSlots)
        {
            saveSlotManager.UpdateSlot(SaveManager.GetHighscores(i));
            i++;
        }
    }

    private void LoadSaveSlot(int slot)
    {
        MainMenuData.SetSaveSlot(slot);
        // TODO update for new levels
        //SceneManager.LoadScene(2);
        levelSelectCanvas.gameObject.SetActive(true);
        canvas.gameObject.SetActive(false);

        foreach (Button button in levelButtons)
        {
            button.enabled = false;
            button.interactable = false;
            //button.enabled = false;
        }
        Debug.Log(saveSlots[slot]);
        if (saveSlots[slot].GetLastCompletedLevelName(SaveManager.GetHighscores(slot)) == null)
        {
            levelButtons[0].interactable = true;
            levelButtons[0].enabled = true;
        }
        else
        {
            switch (saveSlots[slot].GetLastCompletedLevelName(SaveManager.GetHighscores(slot)).ToLower())
            {
                case "rotatinator":
                    levelButtons[7].interactable = true;
                    levelButtons[7].enabled = true;
                    goto case "plate magic";
                case "plate magic":
                    levelButtons[6].interactable = true;
                    levelButtons[7].interactable = true;
                    levelButtons[6].enabled = true;
                    levelButtons[7].enabled = true;
                    goto case "icy platforms";
                case "icy platforms":
                    levelButtons[5].interactable = true;
                    levelButtons[6].interactable = true;
                    levelButtons[5].enabled = true;
                    levelButtons[6].enabled = true;
                    goto case "ghost house";
                case "ghost house":
                    levelButtons[4].interactable = true;
                    levelButtons[5].interactable = true;
                    levelButtons[4].enabled = true;
                    levelButtons[5].enabled = true;
                    goto case "moving field";
                case "moving field":
                    levelButtons[3].interactable = true;
                    levelButtons[4].interactable = true;
                    levelButtons[3].enabled = true;
                    levelButtons[4].enabled = true;
                    goto case "tutorial throw drag wood";
                case "tutorial throw drag wood":
                    levelButtons[2].interactable = true;
                    levelButtons[3].interactable = true;
                    levelButtons[2].enabled = true;
                    levelButtons[3].enabled = true;
                    goto case "get a grip";
                case "get a grip":
                    levelButtons[1].interactable = true;
                    levelButtons[2].interactable = true;
                    levelButtons[1].enabled = true;
                    levelButtons[2].enabled = true;
                    goto case "tutorial basics";
                case "tutorial basics":
                    levelButtons[0].interactable = true;
                    levelButtons[1].interactable = true;
                    levelButtons[0].enabled = true;
                    levelButtons[1].enabled = true;
                    break;
            }
        }
        fakeButton.Select();
    }

    public void OnSaveSlotSelect(int saveSlot)
    {
        LoadSaveSlot(saveSlot);
    }

    public void SaveSlotReset(int saveSlot)
    {
        // nuke save slot
        string savePath = Path.Combine(Application.persistentDataPath, "SaveFiles", "save_" + saveSlot + ".bestie");

        // check if file exists
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("sicksess");
        }
        else
        {
            Debug.Log("no file");
        }

        PreviewSaveSlots();
    }
    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
        // if above doesnt work I guess?
        //SceneManager.LoadScene(allGameScenes[levelIndex].name);
    }
}
