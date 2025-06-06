using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using System.Threading.Tasks;
using Unity.Services.Authentication;

public class GameCloudTests
{
    private GameObject gameCloudObject;
    private GameCloud gameCloud;

    Level level1 = Level.RestartLevel(1);
    Level level2 = Level.RestartLevel(2);

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        gameCloudObject = new GameObject();
        gameCloud = gameCloudObject.AddComponent<GameCloud>();

        // Мокаем GameData
        GameData.PlayerName = "TestPlayer";
        GameData.Levels = new List<Level> { level1, level2 };

        // Инициализация Unity Services (можно убрать если мокается полностью)
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
        {
            yield return UnityServices.InitializeAsync();
        }
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            var signInTask = AuthenticationService.Instance.SignInAnonymouslyAsync();
            yield return WaitForTask(signInTask);
        }

        yield return null;
    }

    [UnityTest]
    public IEnumerator SaveAndLoadData_WorksCorrectly()
    {
        // Сохраняем данные
        var saveTask = gameCloud.SaveData();
        yield return WaitForTask(saveTask);

        // Предварительно очищаем GameData
        GameData.PlayerName = "";
        GameData.Levels = new List<Level>();

        // Загружаем данные
        var loadTask = gameCloud.LoadData();
        yield return WaitForTask(loadTask);

        // Проверяем, что данные восстановлены
        Assert.AreEqual("TestPlayer", GameData.PlayerName);
        Assert.AreEqual(level1.levelId, GameData.Levels[0].levelId);
        Assert.AreEqual(level2.levelId, GameData.Levels[1].levelId);
    }

    private IEnumerator WaitForTask(Task task)
    {
        while (!task.IsCompleted)
            yield return null;

        if (task.IsFaulted)
            throw task.Exception;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(gameCloudObject);
        yield return null;
    }
}
