using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

using nonogram.leveldata;

namespace nonogram.userdata
{

public static class UserData
{
    public static string Username {get; set;}
    static string filepath = "Assets/UserData/";
    public static Dictionary<string, LevelData> levelsData = new Dictionary<string, LevelData>();
    static LevelProgress levelProgress = new LevelProgress();

    static string GetFolderpath()
    {
        if (Username == "")
        {
            throw new Exception($"Username empty");
        }
        
        string folderpath = $"{filepath}{Username}/";
        if (!Directory.Exists(folderpath))
        {
            Directory.CreateDirectory(folderpath);
        }

        return folderpath;
    }

    static string GetLevelDataPath(string level)
    {
        return $"{GetFolderpath()}level{level}_progress.dat";
    }

    static string GetLevelProgressPath()
    {
        return $"{GetFolderpath()}all_level_progress.dat";
    }

    static void GreetUser()
    {
        Debug.Log($"Hello, {Username}");
    }

    public static void Initialize()
    {
        GreetUser();
        
        levelProgress = new LevelProgress();
    }

    public static bool IsLevelComplete(string level)
    {
        return levelProgress.IsLevelComplete(level);
    }

    public static void AddNewLevel(string level)
    {
        levelProgress.AddNewLevel(level);
    }

    public static void CompleteLevel(string level)
    {
        levelProgress.CompleteLevel(level);
    }

    static void UpdateLevelData(LevelData levelData)
    {
        if (levelsData.ContainsKey(levelData.level))
        {
            levelsData.Remove(levelData.level);
        }
        levelsData[levelData.level] = levelData;

        levelsData[levelData.level].PrintCellData();
    }

    public static void SaveLevelData(LevelData levelData)
    {
        try
        {
            string path = GetLevelDataPath(levelData.level);
            Debug.Log($"Saving level data to {path} ...");

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            UpdateLevelData(levelData);
            formatter.Serialize(stream, levelData);

            stream.Close();
        }
        catch(Exception e)
        {
            Debug.LogError($"Exception: {e.Message}");
            return;
        }
    }

    public static bool LoadLevelData(string level)
    {
        try
        {
            string path = GetLevelDataPath(level);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{path} not found.");
            }
            
            Debug.Log($"Reading level data from {path} ...");

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            LevelData levelData = formatter.Deserialize(stream) as LevelData;
            UpdateLevelData(levelData);

            stream.Close();
        }
        catch(Exception e)
        {
            Debug.LogError($"Exception: {e.Message}");
            return false;
        }

        return true;
    }

    public static bool ClearLevelData(string level)
    {
        try
        {
            string path = GetLevelDataPath(level);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{path} not found.");
            }

            File.Delete(path);
        }
        catch(Exception e)
        {
            Debug.LogError($"Exception: {e.Message}");
            return false;
        }

        return true;
    }

    public static void SaveLevelProgress()
    {
        try
        {

            string path = GetLevelProgressPath();
            Debug.Log($"Saving level progress to {path} ...");

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, levelProgress);

            stream.Close();
        }
        catch(Exception e)
        {
            Debug.LogError($"Exception: {e.Message}");
            return;
        }
    }

    public static bool LoadLevelProgress()
    {
        try
        {
            string path = $"{GetLevelProgressPath()}";

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{path} not found.");
            }
            Debug.Log($"Reading level progress from {path} ...");

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            levelProgress = formatter.Deserialize(stream) as LevelProgress;
            levelProgress.Print();

            stream.Close();
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception: {e.Message}");
            return false;
        }

        return true;
    }

    public static void ResetProgress()
    {
        levelProgress = new LevelProgress();
        SaveLevelProgress();

        List<string> levels = new List<string>();
        foreach (string level in levelsData.Keys)
        {
            levels.Add(level);
        }

        levelsData.Clear();

        foreach (string level in levels)
        {
            ClearLevelData(level);
        }
    }
} // end UserData class

[Serializable]
class LevelProgress
{
     Dictionary<string, bool> levels;

    public LevelProgress() : base()
    {
        levels = new Dictionary<string, bool>();
        levels.Add("01", false);
    }

    public bool IsLevelComplete(string level)
    {
        if (levels.ContainsKey(level))
        {
            return levels[level];
        }
        return false;
    }

    public void AddNewLevel(string level)
    {
        levels.Add(level, false);
    }

    public void CompleteLevel(string level)
    {
        if (levels.ContainsKey(level))
        {
            levels.Remove(level);
        }

        levels.Add(level, true);
    }

    public void Print()
    {
        Debug.Log($"levels:");
        foreach (KeyValuePair<string, bool> kvp in levels)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}");
        }
    }
} // end LevelProgress class

} // end namespace nonogram.userdata
