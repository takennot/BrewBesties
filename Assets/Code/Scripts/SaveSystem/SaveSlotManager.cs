using JetBrains.Annotations;
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
            if(key.Equals("Rotatinator") && highscore >= levelreqs[0])
            {
                UpdateUnlockedLevels("Completed");
            }
        }
        else
        {
            UpdateImage(GetSceneImage(key), true);
            UpdateUnlockedLevels("Empty");
        }

    }

    public string GetLastCompletedLevelName(Dictionary<string, int> highscores)
    {
        // go through every scene backwards
        for (int i = SceneManager.sceneCountInBuildSettings -2; i > 1; i--)
        {
            string key = saveSlotSelectionManager.allSceneNames[i];
            if (key != null)
            {
                // key exists highscores
                if (highscores.ContainsKey(key))
                {
                    int savedHighscore = 0;
                    if (highscores.TryGetValue(key, out savedHighscore))
                    {
                        if(savedHighscore >= CompletionRequirements.GetLevelRequirements(key)[0])
                        {
                            return key;
                        }
                    }
                }
            }
        }
        // return null if didnt return key in for loop
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
                // everything returns +1 on allSceneSprites array
                case "tutorial basics":
                    return allSceneSprites[2+1];
                case "get a grip":
                    return allSceneSprites[3+1];
                case "completionist":
                    return allSceneSprites[4 + 1];
                case "tutorial throw drag wood":
                    return allSceneSprites[5 + 1];
                case "three islands":
                    return allSceneSprites[6 + 1];
                case "ghost house":
                    return allSceneSprites[7 + 1];
                case "moving field":
                    return allSceneSprites[8 + 1];
                case "icy platforms":
                    return allSceneSprites[9 + 1];
                case "rotatinator":
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
