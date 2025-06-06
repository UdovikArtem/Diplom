using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;

public class GameUITests
{
    private GameObject gameUIObj;
    private GameUI gameUI;

    private GameObject gameUIPage;
    private GameObject pausePage;
    private GameObject gameOverPage;
    private GameObject finishLevelPage;
    private GameObject slidingArea;
    private Scrollbar healthbar;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI scoreText;

    private class MockPlayer : Player
    {
        private void Awake()
        {
            Instance = this;
        }
    }

    [SetUp]
    public void SetUp()
    {
        // Создаём игрока
        var playerObj = new GameObject("Player", typeof(MockPlayer));
        Player.Instance = playerObj.GetComponent<MockPlayer>(); // Подмена статического Player.Instance

        // Создаём GameUI и подставляем необходимые компоненты
        gameUIObj = new GameObject("GameUI");
        gameUI = gameUIObj.AddComponent<GameUI>();

        nameText = new GameObject("NameText").AddComponent<TextMeshProUGUI>();
        scoreText = new GameObject("ScoreText").AddComponent<TextMeshProUGUI>();
        healthbar = new GameObject("Healthbar").AddComponent<Scrollbar>();
        slidingArea = new GameObject("SlidingArea");

        gameUIPage = new GameObject("GameUIPage");
        pausePage = new GameObject("PausePage");
        gameOverPage = new GameObject("GameOverPage");
        finishLevelPage = new GameObject("FinishLevelPage");

        // Присваиваем через сериализацию
        typeof(GameUI).GetField("nameText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameUI, nameText);
        typeof(GameUI).GetField("scoreText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameUI, scoreText);
        typeof(GameUI).GetField("healthbar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameUI, healthbar);
        typeof(GameUI).GetField("slidingArea", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameUI, slidingArea);
        typeof(GameUI).GetField("gameUIPage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameUI, gameUIPage);
        typeof(GameUI).GetField("pausePage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameUI, pausePage);
        typeof(GameUI).GetField("gameOverPage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameUI, gameOverPage);
        typeof(GameUI).GetField("finishLevelPage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gameUI, finishLevelPage);

        GameUI.Instance = gameUI;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameUIObj);
        Object.DestroyImmediate(GameObject.Find("Player"));
    }

    [Test]
    public void GameOverPage_SetsCorrectUIState()
    {
        gameUIPage.SetActive(true);
        gameOverPage.SetActive(false);

        gameUI.GameOverPage();

        Assert.IsFalse(gameUIPage.activeSelf);
        Assert.IsTrue(gameOverPage.activeSelf);
        Assert.AreEqual(0, Time.timeScale);
    }

    [Test]
    public void ChangePaused_TogglesPauseCorrectly()
    {
        // Начало — не на паузе
        gameUI.ChangePaused(); // Включить паузу
        Assert.IsTrue(pausePage.activeSelf);
        Assert.IsFalse(gameUIPage.activeSelf);
        Assert.AreEqual(0, Time.timeScale);

        gameUI.ChangePaused(); // Снять паузу
        Assert.IsFalse(pausePage.activeSelf);
        Assert.IsTrue(gameUIPage.activeSelf);
        Assert.AreEqual(1, Time.timeScale);
    }

    [Test]
    public void FinishLevelPage_ActivatesCorrectUI()
    {
        finishLevelPage.SetActive(false);
        gameUIPage.SetActive(true);

        gameUI.FinishLevelPage();

        Assert.IsTrue(finishLevelPage.activeSelf);
        Assert.IsFalse(gameUIPage.activeSelf);
        Assert.IsTrue(gameUI.IsFinished);
    }

    [Test]
    public void ChangeHealthbar_ReflectsPlayerHealth()
    {
        Player.Instance.Health = 75;
        gameUI.ChangeHealthbar();
        Assert.AreEqual(0.75f, healthbar.size);
        Assert.IsTrue(slidingArea.activeSelf);

        Player.Instance.Health = 0;
        gameUI.ChangeHealthbar();
        Assert.AreEqual(0.0f, healthbar.size);
        Assert.IsFalse(slidingArea.activeSelf);
    }

    [Test]
    public void ChangeScore_UpdatesScoreText()
    {
        GameData.Score = 123;
        gameUI.ChangeScore();
        Assert.AreEqual("Score: 123", scoreText.text);
    }

    [Test]
    public void NameText_SetsCorrectlyOnStart()
    {
        GameData.PlayerName = "TestUser";
        gameUI.GetType().GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(gameUI, null); ;
        Assert.AreEqual("TestUser", nameText.text);
    }
}
