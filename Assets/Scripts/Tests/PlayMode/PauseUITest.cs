using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PauseUITests
{
    private GameObject pauseUIObj;
    private PauseUI pauseUI;

    [SetUp]
    public void SetUp()
    {
        pauseUIObj = new GameObject("PauseUI");
        pauseUI = pauseUIObj.AddComponent<PauseUI>();

        // Инициализация GameData
        GameData.CurrentLevel = new Level
        {
            levelId = 1,
            bestScore = 300,
            health = 90,
            currentScore = 150,
            deadEnemies = new List<string> { "enemy1", "enemy2" }
        };

        GameData.Levels = new List<Level> { GameData.CurrentLevel };

        GameData.Score = 999;
        GameData.PlayerHealth = 60;
        GameData.DeadEnemiesId = new List<string> { "temp" };
        GameData.CurrentCheckpoint = "checkpoint-123";

        // Заглушка для GameManager.Instance
        GameManager.Instance = new GameManager();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(pauseUIObj);
        Object.DestroyImmediate(GameManager.Instance);
    }

    [Test]
    public void ToLastCheckpoint_RestoresGameData()
    {
        pauseUI.ToLastCheckpoint();

        Assert.AreEqual(GameData.CurrentLevel.currentScore, GameData.Score);
        Assert.AreEqual(GameData.CurrentLevel.health, GameData.PlayerHealth);
        CollectionAssert.AreEqual(GameData.CurrentLevel.deadEnemies, GameData.DeadEnemiesId);
    }

    [Test]
    public void NextLevel_UpdatesGameData()
    {
        GameData.Levels.Add(new Level { levelId = 2, bestScore = 500 });

        pauseUI.NextLevel(2); // id = 2

        Assert.AreEqual(2, GameData.CurrentLevel.levelId);
        Assert.AreEqual(0, GameData.Score);
        Assert.AreEqual(100, GameData.PlayerHealth);
        Assert.IsNull(GameData.CurrentCheckpoint);
        Assert.IsEmpty(GameData.DeadEnemiesId);
    }
}
