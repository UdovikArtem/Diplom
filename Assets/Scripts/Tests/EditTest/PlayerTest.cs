using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerTests
{
    private GameObject playerObject;
    private Player player;

    [SetUp]
    public void Setup()
    {
        // Создание объекта и добавление компонента
        playerObject = new GameObject();
        player = playerObject.AddComponent<Player>();

        // Инициализация нужных компонентов и полей
        var rb = playerObject.AddComponent<Rigidbody2D>();

        // Создание необходимых трансформов для проверки
        GameObject groundCheck = new GameObject("GroundCheck");
        playerObject.transform.position = Vector3.zero;
        groundCheck.transform.parent = playerObject.transform;
        groundCheck.transform.localPosition = Vector3.zero;

        var animator = playerObject.AddComponent<Animator>();
        var spriteRenderer = playerObject.AddComponent<SpriteRenderer>();

        typeof(Player).GetField("groundCheck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(player, groundCheck.transform);

        typeof(Player).GetField("playerAnim", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(player, animator);

        typeof(Player).GetField("spriteRenderer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(player, spriteRenderer);

        typeof(Player).GetField("playerRb", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(player, rb);

        // Установка изначального здоровья
        typeof(Player).GetField("health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(player, 100);

        typeof(Player).GetField("colliders", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(player, new Collider2D[0]);

        // Мокаем GameUI.Instance
        var gameUI = new GameObject().AddComponent<MockGameUI>();
        GameUI.Instance = gameUI;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(playerObject);
    }

    [UnityTest]
    public IEnumerator TakeDamage_KillsPlayer_WhenHealthReachesZero()
    {
        MockGameUI.GameOverPageCalled = false;
        player.TakeDamage(100);

        yield return null;

        Assert.IsTrue(player.IsDead);
        Assert.IsTrue(MockGameUI.GameOverPageCalled);
        Assert.IsTrue(MockGameUI.ChangeHealthbarCalled);
    }

    [UnityTest]
    public IEnumerator TakeDamage_DosentKillsPlayer()
    {
        MockGameUI.GameOverPageCalled = false;
        player.TakeDamage(30);

        var health = player.GetType().GetField("health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(player);
        Debug.Log(player.IsDead);
        Assert.AreEqual(health, 70);
        Assert.IsFalse(player.IsDead);

        Assert.IsFalse(MockGameUI.GameOverPageCalled);
        Assert.IsTrue(MockGameUI.ChangeHealthbarCalled);
        yield return null;
    }

    [UnityTest]
    public IEnumerator InputManager_Toggles_CanReceiveInput()
    {
        player.CanReceiveInput = true;

        player.InputManager();
        Assert.IsFalse(player.CanReceiveInput);

        player.InputManager();
        Assert.IsTrue(player.CanReceiveInput);

        yield return null;
    }

    private class MockGameUI : GameUI
    {
        public static bool GameOverPageCalled = false;

        public static bool ChangeHealthbarCalled = false;
        public override void GameOverPage()
        {
            GameOverPageCalled = true;
        }

        public override void ChangeHealthbar()
        {
            ChangeHealthbarCalled = true;
        }
    }
}
