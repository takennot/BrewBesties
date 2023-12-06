using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotManager : MonoBehaviour
{
    [SerializeField] private Image levelImage;
    [SerializeField] private TMP_Text progress; // in %
    [SerializeField] private TMP_Text unlockedLevels;
    private int saveSlot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSlot()
    {
        UpdateImage();
        UpdateProgress();
        UpdateUnlockedLevels();
    }

    private void UpdateImage()
    {

    }

    private void UpdateProgress()
    {

    }

    private void UpdateUnlockedLevels()
    {

    }
}
