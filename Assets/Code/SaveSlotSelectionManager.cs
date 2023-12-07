using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotSelectionManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button playButton;
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

    public void OnSaveSlotSelect()
    {
        // open canvas with level selection
    }

    public void OnSaveSlotReset()
    {
        // nuke save slot
    }
}
