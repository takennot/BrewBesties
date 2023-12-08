using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuData : MonoBehaviour
{
    [SerializeField] public static int playerAmount;
    [SerializeField] public static bool isArcade;
    /// <summary>
    /// Which save slot to write & read from. Can be only 0, 1 & 2
    /// </summary>
    [SerializeField] private static int saveSlot;
    /// <summary>
    /// List of highscores where key is level name and value is level highscore
    /// </summary>
    [SerializeField] public static Dictionary<string, int> levelHighscores = new Dictionary<string, int>();
    //[SerializeField] public Dictionary<string, int> levelHighscoresww = new Dictionary<string, int>();
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public bool UpdateHighscore(string levelName, int score)
    {
        bool savedScore;
        if (levelHighscores.ContainsKey(levelName))
        {
            if (score > levelHighscores[levelName])
            {
                Debug.Log(levelHighscores[levelName]);
                levelHighscores[levelName] = score;
                savedScore = true;
            }
            else
            {
                savedScore = false;
            }
        }
        else
        {
            levelHighscores.Add(levelName, score);
            savedScore = true;
        }
        if(savedScore)
        {
            SaveManager.SaveGame(saveSlot, levelHighscores);
        }
        
        return savedScore;
    }

    public static void SetSaveSlot(int newSaveSlot)
    {
        saveSlot = newSaveSlot;
        levelHighscores = SaveManager.GetHighscores(saveSlot);
        Debug.Log(levelHighscores.Count);
    }
    public static int GetSaveSlot()
    {
        return saveSlot;
    }
}
