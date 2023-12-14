using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotManager : MonoBehaviour
{
    [SerializeField] private Image levelImage;
    [SerializeField] private TMP_Text progress; // in %
    [SerializeField] private TMP_Text unlockedLevels;
    [SerializeField] private SaveSlotSelectionManager saveSlotSelectionManager;
    // maybe have arraylist with images?

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSlot(Dictionary<string, int> highscores)
    {
        string key = GetLastCompletedLevelName(highscores);
        if (key != null)
        {
            highscores.TryGetValue(key, out int highscore);

            List<int> levelreqs = CompletionRequirements.GetLevelRequirements(key);
            if (levelreqs[0] == 1)
            {
                UpdateImage(false);
                UpdateProgress();
                UpdateUnlockedLevels("Tutorial");
            }
            int lastCompletedLevelIndex = SceneManager.GetSceneByName(key).buildIndex;
            int lastUnlockedLevelIndex;
            if (highscore >= levelreqs[0])
            {
                lastUnlockedLevelIndex = lastCompletedLevelIndex + 1;

                UpdateImage(false);
                UpdateProgress();
                UpdateUnlockedLevels(SceneManager.GetSceneByBuildIndex(lastUnlockedLevelIndex).name);
            }

            UpdateImage(false);
            UpdateProgress();
            UpdateUnlockedLevels(key);
        }
        else
        {
            UpdateImage(true);
            UpdateProgress();
            UpdateUnlockedLevels("Empty");
        }

    }

    public string GetLastCompletedLevelName(Dictionary<string, int> highscores)
    {
        for (int i = SceneManager.sceneCountInBuildSettings -1; i > 1; i--)
        {
            string key = saveSlotSelectionManager.allSceneNames[i];
            Debug.Log(key);
            if (key != null)
            {
                if (highscores.ContainsKey(saveSlotSelectionManager.allSceneNames[i]))
                {
                    return saveSlotSelectionManager.allSceneNames[i];
                }
            }
        }
        return null;
    }

    private void UpdateImage(/*Image image,*/ bool empty)
    {
        if (empty)
        {
            levelImage.color = Color.grey;
        }
        else 
        {
            levelImage.color = Color.white;
            //levelImage = image;
        }
        
    }

    private void UpdateProgress()
    {

    }

    private void UpdateUnlockedLevels(string levelName)
    {
        unlockedLevels.text = levelName;
    }
}
