using NUnit.Framework;
using System.Collections;
using Unity.Services.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class MenuUIHandlerTests
{
    private MenuUIHandler handler;

    [TestCase("Password1!", ExpectedResult = true)]
    [TestCase("pass1!", ExpectedResult = false)] // слишком короткий
    [TestCase("PASSWORD123!", ExpectedResult = false)] // нет строчной
    [TestCase("password123!", ExpectedResult = false)] // нет заглавной
    [TestCase("Password!", ExpectedResult = false)] // нет цифры
    [TestCase("Password123", ExpectedResult = false)] // нет символа
    [TestCase("P@ssword1", ExpectedResult = true)] // валидный
    public bool PasswordValidationTest(string password)
    {
        return handler.isPasswordValid(password);
    }

    private GameObject menuUIObject;
    private MenuUIHandler menuUI;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        UnityServices.InitializeAsync();
        // Создаем объект и компонент
        menuUIObject = new GameObject("MenuUIHandler");
        menuUI = menuUIObject.AddComponent<MenuUIHandler>();


        var go = new GameObject();
        handler = go.AddComponent<MenuUIHandler>();

        // Фиктивные значения для GameData
        GameData.Levels = new System.Collections.Generic.List<Level>();
        GameData.Levels.Add(new Level());

        Level level = new Level();
        level.levelId = 1;
        level.isAvailable = true;
        level.isFinished = false;
        level.isFirstStart = false;
        level.health = 85;
        level.currentCheckpointId = "second";
        level.deadEnemies = new System.Collections.Generic.List<string> { "enemy_1", "enemy_2" };
        level.currentScore = 1234;
        GameData.Levels[0] = level;
        

        yield return null;
    }

    [UnityTest]
    public IEnumerator OpenLevel_SetsGameDataCorrectly()
    {
        // Запускаем метод
        menuUI.OpenLevel(1);
        yield return new WaitForSeconds(10f); // подождем завершения корутины загрузки

        Level current = GameData.CurrentLevel;

        Assert.AreEqual(85, GameData.Levels[0].health);
        Assert.AreEqual("second", GameData.CurrentCheckpoint);
        CollectionAssert.AreEqual(new[] { "enemy_1", "enemy_2" }, GameData.DeadEnemiesId);
        Assert.AreEqual(1234, GameData.Score);
        Assert.AreEqual(current.levelId, 1);
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Object.Destroy(menuUIObject);
        yield return null;
    }
}
