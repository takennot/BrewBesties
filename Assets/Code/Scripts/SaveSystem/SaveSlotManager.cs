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

    [SerializeField] public Sprite[] allSceneSprites = new Sprite[11];

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
                UpdateImage(GetSceneImage(key), false);
                UpdateUnlockedLevels("Tutorial");
            }
            int lastCompletedLevelIndex = SceneManager.GetSceneByName(key).buildIndex;
            int lastUnlockedLevelIndex;
            if (highscore >= levelreqs[0])
            {
                lastUnlockedLevelIndex = lastCompletedLevelIndex + 1;

                UpdateImage(GetSceneImage(key), false);
                UpdateUnlockedLevels(SceneManager.GetSceneByBuildIndex(lastUnlockedLevelIndex).name);
            }

            UpdateImage(GetSceneImage(key), false);
            UpdateUnlockedLevels(key);
        }
        else
        {
            UpdateImage(GetSceneImage(key), true);
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

    private void UpdateImage(Sprite newSprite, bool empty)
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

        levelImage.sprite = newSprite;
    }

    private Sprite GetSceneImage(string name)
    {
        if (name != null)
        {
            switch (name.ToLower())
            {
                case "Tutorial Basics":
                    return allSceneSprites[2];
                case "get a grip":
                    return allSceneSprites[3];
                case "completionist":
                    return allSceneSprites[4];
                case "Tutorial Throw Drag Wood":
                    return allSceneSprites[5];
                case "Three islands":
                    return allSceneSprites[6];
                case "Ghost House":
                    return allSceneSprites[7];
                case "Moving Field":
                    return allSceneSprites[8];
                case "Icy Platforms":
                    return allSceneSprites[9];
                case "Rotatinator":
                    return allSceneSprites[10];
                default:
                    return allSceneSprites[0];
            }
        }

        return allSceneSprites[0];
    }

    private void UpdateUnlockedLevels(string levelName)
    {
        unlockedLevels.text = levelName;
    }
}
