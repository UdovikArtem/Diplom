using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SaveUserNameData
{
    public string name;
    public string password;

    public SaveUserNameData(string name, string password)
    {
        this.name = name;
        this.password = password;
    }
}

[System.Serializable]
public class ListUsersData
{
    public List<SaveUserNameData> users = new List<SaveUserNameData>();
}

[System.Serializable]
public class SaveUserData
{
    public string name;
    public List<Level> levels;

    public SaveUserData() { }

    public string Name { get => name; set => name = value; }
}

[System.Serializable]
public class Level
{
    public int levelId;
    public int health = 100;
    public int bestScore = 0;
    public int currentScore = 0;
    public string currentCheckpointId = null;
    public List<string> deadEnemies = new List<string>();
    public bool isFinished = false;
    public bool isAvailable;
    public bool isFirstStart = true;

    public static Level RestartLevel(int levelId, int bestScore = 0)
    {
        Level level = new Level();
        level.levelId = levelId;
        level.deadEnemies = new List<string>();
        level.isFinished = false;
        level.currentCheckpointId = null;
        level.currentScore = 0;
        level.health = 100;
        level.isFirstStart = false;
        level.isAvailable = true;
        level.bestScore = bestScore;
        return level;
    }
}
