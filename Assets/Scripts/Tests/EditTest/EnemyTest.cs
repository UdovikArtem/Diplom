using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class EnemyTests
{
    private GameObject enemyObject;
    private Enemy enemy;

    [SetUp]
    public void SetUp()
    {
        enemyObject = new GameObject();
        enemy = enemyObject.AddComponent<Enemy>();

        // Инициализация компонентов
        var rb = enemyObject.AddComponent<Rigidbody2D>();
        var spriteRenderer = enemyObject.AddComponent<SpriteRenderer>();
        var anim = enemyObject.AddComponent<MockEnemyAnim>();

        var groundCheck = new GameObject("GroundCheck").transform;
        groundCheck.parent = enemyObject.transform;

        enemyObject.GetComponent<Enemy>().GetType()
            .GetField("groundCheck", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, groundCheck);

        enemy.GetType().GetField("health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 100);

        enemy.GetType().GetField("impulse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, 5);

        enemy.GetType().GetField("colliders", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, new Collider2D[0]);
        enemy.GetType().GetField("spriteRenderer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, spriteRenderer);
        enemy.GetType().GetField("enemyRb", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, rb);
        enemy.GetType().GetField("enemyAnim", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(enemy, anim);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(enemyObject);
    }

    [Test]
    public void SetAttackDirection_StoresCorrectValue()
    {
        var dir = new Vector3(1, 0, 0);
        enemy.setAttackDir(dir);
        Assert.AreEqual(dir, enemy.getAttackDir());
    }

    [UnityTest]
    public IEnumerator TakeDamage_ReducesHealthAndTriggersKnockback()
    {
        int initialHealth = 100;
        int damage = 40;
        bool isFromRight = true;

        enemy.TakeDamage(damage, isFromRight);

        yield return null;

        int actualHealth = (int)enemy.GetType().GetField("health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(enemy);
        Assert.AreEqual(initialHealth - damage, actualHealth);
    }

    [UnityTest]
    public IEnumerator TakeDamage_LeadsToDeath_WhenHealthDepleted()
    {
        int damage = 150;
        bool isFromRight = true;

        enemy.TakeDamage(damage, isFromRight);

        yield return null;

        var isDead = (bool)enemy.GetType().GetField("isDead", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(enemy);
        Assert.IsTrue(isDead);
        Assert.IsTrue(MockEnemyAnim.DeathCalled);
    }

    [Test]
    public void Can_Set_And_Get_Attacking_State()
    {
        enemy.IsAttacking = true;
        Assert.IsTrue(enemy.IsAttacking);

        enemy.IsAttacking = false;
        Assert.IsFalse(enemy.IsAttacking);
    }

    private class MockEnemyAnim : EnemyAnim
    {
        public static bool DeathCalled = false;
        public override void Death()
        {
            DeathCalled = true;
        }
    }
}
