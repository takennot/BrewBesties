using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotSelectionManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button playButton;
    [SerializeField] private SaveSlotManager[] saveSlots = new SaveSlotManager[3];
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
        SceneManager.LoadScene(3);
    }

    public void OnSaveSlot1Select()
    {
       LoadSaveSlot(0);

    }
    public void OnSaveSlot2Select()
    {
        LoadSaveSlot(1);
    }
    public void OnSaveSlot3Select()
    {
        LoadSaveSlot(2);
    }

    public void OnSaveSlotReset()
    {
        // nuke save slot
    }
}
