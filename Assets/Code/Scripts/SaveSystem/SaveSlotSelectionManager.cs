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
    [SerializeField] private Button playButton;
    [SerializeField] private SaveSlotManager[] saveSlots = new SaveSlotManager[3];
    [SerializeField] public SceneAsset[] allGameScenes = new SceneAsset[10];
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
                else
                {
                    Debug.Log(EventSystem.current.currentSelectedGameObject.name);
                }
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
        // TODO load canvas with level selection
        SceneManager.LoadScene(2);
    }

    public void OnSaveSlot0Select()
    {
       LoadSaveSlot(0);

    }
    public void OnSaveSlot1Select()
    {
        LoadSaveSlot(1);
    }
    public void OnSaveSlot2Select()
    {
        LoadSaveSlot(2);
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
}
