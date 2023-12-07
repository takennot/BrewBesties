using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public static class SaveManager
{
    public static void SaveGame(int saveSlot, Dictionary<string, int> highscores)
    {
        // %userprofile%\AppData\LocalLow\<companyname>\<productname>
        //string logPath = Application.persistentDataPath + "\\SaveFiles\\" + AnalyticsSessionInfo.sessionId + ".txt";
        //string savePath = Path.Combine(Application.persistentDataPath, "SaveFiles", "save_" + MainMenuData.saveSlot + ".bestie");
        string savePath = GetSavePath(saveSlot);


        // Ensure the directory exists
        string directoryPath = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
            //FileStream fs = File.Create(logPath);

        using (StreamWriter sw = File.CreateText(savePath))
        {
            string stringBuilder = "";
            foreach (KeyValuePair<string, int> pair in highscores)
            {
                stringBuilder += string.Format("Level:{0}:Highscore:{1}:\n", pair.Key, pair.Value);
                //sw.Write(string.Format("Level: {0}, Highscore: {1}", pair.Key, pair.Value));
            }
            sw.Write(stringBuilder);
        }

        Console.WriteLine($"Data saved to {savePath}");
    }

    public static Dictionary<string, int> GetHighscores(int saveSlot)
    {
        Dictionary<string, int> levelHighscores = new Dictionary<string, int>();
        string savePath = GetSavePath(saveSlot);
        StreamReader streamReader = new StreamReader(savePath);
        while (!streamReader.EndOfStream)
        {
            string savedHighscore = streamReader.ReadLine();
            string[] values = savedHighscore.Split(':');

            levelHighscores.Add(values[1], Int32.Parse(values[3]));
        }
        streamReader.Close();
        return levelHighscores;
    }

    private static string GetSavePath(int saveSlot)
    {
        return Path.Combine(Application.persistentDataPath, "SaveFiles", "save_" + MainMenuData.saveSlot + ".bestie");
    }
}
