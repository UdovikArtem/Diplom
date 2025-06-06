using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ColliderDamageTest
{
    private GameObject damageObject;
    private ColliderDamage colliderDamage;
    private GameObject playerObject;
    private MockPlayer mockPlayer;

    [SetUp]
    public void Setup()
    {
        // Создаем объект с компонентом ColliderDamage
        damageObject = new GameObject("DamageObject");
        colliderDamage = damageObject.AddComponent<ColliderDamage>();
        colliderDamage.attackDamage = 25;

        var collider = damageObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        // Создаем объект игрока с тегом и мок-классом Player
        playerObject = new GameObject("Player");
        playerObject.tag = "Player";
        mockPlayer = playerObject.AddComponent<MockPlayer>();

        playerObject.AddComponent<BoxCollider2D>();
        var rb = playerObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    [Test]
    public void OnTriggerEnter2D_PlayerTakesDamage()
    {
        LogAssert.ignoreFailingMessages = true;
        // Эмуляция входа в триггер
        colliderDamage.SendMessage("OnTriggerEnter2D", playerObject.GetComponent<Collider2D>());
        LogAssert.ignoreFailingMessages = false;
        // Проверяем, был ли вызван метод TakeDamage с нужным значением
        Assert.AreEqual(25, mockPlayer.LastDamageTaken);
        Assert.IsFalse(damageObject.activeSelf);
    }

    [Test]
    public void OnTriggerEnter2D_IgnoresOtherTags()
    {
        LogAssert.ignoreFailingMessages = true;

        var otherObject = new GameObject("Enemy");
        otherObject.tag = "Enemy";
        otherObject.AddComponent<BoxCollider2D>();

        colliderDamage.SendMessage("OnTriggerEnter2D", otherObject.GetComponent<Collider2D>());
        LogAssert.ignoreFailingMessages = false;
        // Убедимся, что объект не деактивирован
        Assert.IsTrue(damageObject.activeSelf);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(damageObject);
        Object.DestroyImmediate(playerObject);
    }

    // Простая заглушка Player для тестов
    private class MockPlayer : Player
    {
        public int LastDamageTaken = 0;

        public override void TakeDamage(int damage)
        {
            LastDamageTaken = damage;
        }
    }
}
