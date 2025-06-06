using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PauseUITests
{
    private GameObject pauseUIObject;
    private PauseUI pauseUI;

    private class FakeGameUI : GameUI
    {
        public bool wasPausedChanged = false;
        public override void ChangePaused()
        {
            wasPausedChanged = true;
        }
    }

    private class FakeGameManager : GameManager
    {
        public bool changeUserDataCalled = false;
        public override Task ChangeUserData()
        {
            changeUserDataCalled = true;
            return Task.CompletedTask;
        }
    }

    private FakeGameUI mockUI;
    private FakeGameManager mockManager;

    [SetUp]
    public void Setup()
    {
        pauseUIObject = new GameObject("PauseUI");
        pauseUI = pauseUIObject.AddComponent<PauseUI>();

        // Заглушка GameUI
        mockUI = pauseUIObject.AddComponent<FakeGameUI>();
        GameUI.Instance = mockUI;

        // Заглушка GameManager
        mockManager = pauseUIObject.AddComponent<FakeGameManager>();
        GameManager.Instance = mockManager;

        // Тестовые данные
        GameData.CurrentLevel = new Level
        {
            levelId = 1,
            bestScore = 50,
            currentCheckpointId = "check01",
            deadEnemies = new List<string> { "enemy1" },
            currentScore = 120,
            health = 75
        };

        GameData.Levels = new List<Level>
        {
            new Level { levelId = 1, bestScore = 50 },
            new Level { levelId = 2, bestScore = 70 }
        };
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(pauseUIObject);
    }

    [Test]
    public void ContinueGame_CallsChangePaused()
    {
        pauseUI.ContinueGame();
        Assert.IsTrue(mockUI.wasPausedChanged);
    }

    [Test]
    public void ToLastCheckpoint_LoadsCheckpointData()
    {
        pauseUI.ToLastCheckpoint();

        Assert.AreEqual(75, GameData.PlayerHealth);
        Assert.AreEqual(120, GameData.Score);
        Assert.AreEqual(GameData.CurrentLevel.deadEnemies, GameData.DeadEnemiesId);
    }

    [Test]
    public void RestartGame_ResetsAndSaves()
    {
        pauseUI.RestartGame();

        Assert.AreEqual(0, GameData.Score);
        Assert.IsNull(GameData.CurrentCheckpoint);
    }
}
