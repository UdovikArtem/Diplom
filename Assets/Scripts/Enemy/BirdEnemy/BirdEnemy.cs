using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BirdEnemy : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameData.CurrentLevel.deadEnemies.Contains(enemyID))
        {
            Destroy(gameObject);
        }
        enemyAnim = GetComponent<EnemyAnim>();
        enemyRb = GetComponent<Rigidbody2D>();
        target = Player.Instance.transform;

        colliders = GetComponents<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        canSpeedChange = true;
        if (!isDead)
        {
            if (!isAttacking)
            {
                enemyRb.bodyType = RigidbodyType2D.Static;
            }
            if (!Player.Instance.IsDead)
            {
                if (isPlayerInTrigger && isCanAttack)
                {
                    Attack();
                }
                else if (!isAttacking)
                {
                    if (Vector2.Distance(transform.position, target.position) <= lookRadius)
                    {
                        enemyAnim.PlayerFoundIsTrue();
                        RunToPlayer();
                        if (Vector2.Distance(transform.position, target.position) <= 2.5f)
                        {
                            canSpeedChange = false;
                            speed = 0;
                        }
                    }
                    else
                    {
                        enemyAnim.PlayerFoundIsFalse();
                        Move();
                    }
                    if (Physics2D.OverlapBox(obstacleCheck.position, new Vector2(0.2f, 1f), 0, obstacleLayer))
                    {
                        if (target.position.y >= transform.position.y)
                        {
                            enemyRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                        }
                        else
                        {
                            enemyRb.AddForce(Vector2.down * jumpForce, ForceMode2D.Impulse);
                        }
                    }
                }
            }
        }
        if (isDead)
        {
            if (Physics2D.OverlapBox(groundCheck.position, new Vector2(0.5f, 0.2f), 0, obstacleLayer))
            {
                enemyRb.bodyType = RigidbodyType2D.Static;
            }
        }
        
    }

    protected override void Death()
    {
        base.Death();
        enemyRb.bodyType = RigidbodyType2D.Dynamic;
    }
}
