using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameUITests
{
    private GameUI gameUI;
    private GameObject gameUIObj;

    [SetUp]
    public void SetUp()
    {
        gameUIObj = new GameObject("GameUI");
        gameUI = gameUIObj.AddComponent<GameUI>();

        // Инициализация зависимостей
        gameUIObj.AddComponent<Canvas>(); // чтобы активировать GameObject
        var scoreGO = new GameObject("ScoreText");
        var scoreText = scoreGO.AddComponent<TextMeshProUGUI>();
        gameUIObj.AddComponent<Canvas>(); // workaround for TMPro

        var hpBarGO = new GameObject("Healthbar");
        var scrollbar = hpBarGO.AddComponent<Scrollbar>();

        var slidingArea = new GameObject("SlidingArea");

        var pausePage = new GameObject("PausePage");
        var gamePage = new GameObject("GamePage");

        gameUI.GetType().GetField("scoreText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(gameUI, scoreText);
        gameUI.GetType().GetField("pausePage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(gameUI, pausePage);
        gameUI.GetType().GetField("gameUIPage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(gameUI, gamePage);
        gameUI.GetType().GetField("finishLevelPage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(gameUI, new GameObject("FinishLevelPage"));
        gameUI.GetType().GetField("healthbar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(gameUI, scrollbar);
        gameUI.GetType().GetField("slidingArea", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(gameUI, slidingArea);

        pausePage.SetActive(false);
        gamePage.SetActive(true);

        // Задаем статические данные
        GameData.Score = 1000;

        // Мокаем Player.Instance
        var playerGO = new GameObject("Player");
        var mockPlayer = playerGO.AddComponent<Player>();
        Player.Instance = mockPlayer;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameUIObj);
        Object.DestroyImmediate(Player.Instance.gameObject);
    }

    [Test]
    public void ChangeScore_UpdatesScoreText()
    {
        gameUI.ChangeScore();
        var score = gameUI.GetType().GetField("scoreText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(gameUI) as TextMeshProUGUI;
        Assert.AreEqual("Score: 1000", score.text);
    }

    [Test]
    public void ChangePaused_TogglesPauseState()
    {
        gameUI.ChangePaused();
        Assert.IsTrue(gameUI.IsPaused);

        gameUI.ChangePaused();
        Assert.IsFalse(gameUI.IsPaused);
    }

    [Test]
    public void ChangeHealthbar_SetsHealthCorrectly()
    {
        var scrollbar = (Scrollbar)gameUI.GetType()
            .GetField("healthbar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(gameUI);

        Player.Instance.Health = 80;
        gameUI.ChangeHealthbar();
        Assert.AreEqual(0.8f, scrollbar.size, 0.01f);
    }

    [Test]
    public void FinishLevelPage_SetsFinishedState()
    {
        gameUI.FinishLevelPage();
        Assert.IsTrue(gameUI.IsFinished);
    }
}
