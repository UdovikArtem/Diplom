using NUnit.Framework;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.TestTools;

public class GameManagerTests
{
    private GameObject gameManagerObject;
    private GameManager gameManager;
    private string testName = "TestUser";
    private string testPassword = "TestPass123-";

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        gameManagerObject = new GameObject();
        gameManager = gameManagerObject.AddComponent<GameManager>();

        if (UnityServices.State != ServicesInitializationState.Initialized)
            yield return UnityServices.InitializeAsync();

        // Очистка старых данных
        string savePath = Path.Combine(Application.persistentDataPath, "save_user_file.json");
        if (File.Exists(savePath)) File.Delete(savePath);

        string userDir = Path.Combine(Application.persistentDataPath, testName);
        if (Directory.Exists(userDir)) Directory.Delete(userDir, true);

        var gameCloude = new GameObject().AddComponent<MockGameCloude>();
        GameCloud.Instance = gameCloude;
        yield return null;
    }


    [UnityTest]
    public IEnumerator SaveUser_NewUser_CreatesFiles()
    {
        var saveTask = gameManager.SaveUser(testName, testPassword);
        yield return WaitForTask(saveTask);

        Assert.IsTrue(saveTask.Result, "User was not saved successfully");

        string saveFilePath = Path.Combine(Application.persistentDataPath, "save_user_file.json");
        string userDataPath = Path.Combine(Application.persistentDataPath, testName, "data.json");

        Assert.IsTrue(File.Exists(saveFilePath), "User list file not found");
        Assert.IsTrue(File.Exists(userDataPath), "User data file not found");

        string json = File.ReadAllText(userDataPath);
        Assert.IsTrue(json.Contains(testName), "User data file does not contain username");
        Assert.IsTrue(MockGameCloude.isSaved);
    }

    private IEnumerator WaitForTask(Task<bool> task)
    {
        while (!task.IsCompleted) yield return null;
        if (task.IsFaulted) throw task.Exception;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(gameManagerObject);

        string savePath = Path.Combine(Application.persistentDataPath, "save_user_file.json");
        if (File.Exists(savePath)) File.Delete(savePath);

        string userDir = Path.Combine(Application.persistentDataPath, testName);
        if (Directory.Exists(userDir)) Directory.Delete(userDir, true);

        yield return null;
    }

    private class MockGameCloude: GameCloud
    {
        public static bool isSaved = false;

        public override Task SaveData()
        {
            isSaved = true;
            return Task.CompletedTask;
        }
    }
}
