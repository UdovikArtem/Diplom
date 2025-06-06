using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    protected EnemyAnim enemyAnim;

    [SerializeField]
    protected string enemyID;

    public string EnemyID => enemyID;

    [SerializeField]
    private int health = 100;
    public float speed;
    public float jumpForce;
    [SerializeField] 
    protected int impulse;
    [SerializeField]
    protected int score;

    protected Rigidbody2D enemyRb;

    [SerializeField] 
    protected SpriteRenderer spriteRenderer;

    protected Transform target, player;

    protected bool isOnGround;

    [Header("Movement")]
    [SerializeField] 
    protected GameObject[] wayPoints;
    [SerializeField]
    protected int nextWayPoint = 1;
    protected float distToPoint;
    [SerializeField]
    protected Transform obstacleCheck;
    [SerializeField]
    protected LayerMask obstacleLayer;
    [SerializeField]
    protected Transform groundCheck;

    [Header("For attack")]
    [SerializeField]
    protected bool isCanAttack = true;
    protected bool isAttacking;
    [SerializeField] 
    protected bool isPlayerInTrigger = false;
    protected Vector3 attackDir;
    [SerializeField]
    protected Collider2D attackTerget;
    [SerializeField]
    protected float lookRadius;
    [SerializeField]
    protected GameObject attackCollider;
    [SerializeField]
    protected float attackCooldown = 4;

    protected bool isDead = false;

    protected bool canSpeedChange;

    protected Collider2D[] colliders;
    public bool IsOnGround { get { return isOnGround; } }
    public Transform Target { get { return target; } }
    public bool CanSpeedChange {  get { return canSpeedChange; } }
    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }

    public int Health { get => health; set => health = value; }


    protected void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }

    protected void OnBecameVisible()
    {
        gameObject.SetActive(true);
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(enemyID))
        {
            enemyID = Guid.NewGuid().ToString();
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this); // сохраняем изменения в сцену
            #endif
        }
    }

    public Vector3 getAttackDir()
    {
        return attackDir;
    }

    public void setAttackDir(Vector3 attackDir)
    {
        this.attackDir = attackDir;
    }
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
        if (!isDead)
        {
            isOnGround = Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.5f, 0.2f), CapsuleDirection2D.Horizontal, 0, obstacleLayer);

            if (!Player.Instance.IsDead)
            {
                if (isPlayerInTrigger && isCanAttack && isOnGround)
                {
                    Attack();
                }
                else if (!isAttacking)
                {
                    if (Vector2.Distance(transform.position, target.position) <= lookRadius)
                    {
                        enemyAnim.PlayerFoundIsTrue();
                        RunToPlayer();
                    }
                    else
                    {
                        enemyAnim.PlayerFoundIsFalse();
                        Move();
                    }
                    if (Physics2D.OverlapBox(obstacleCheck.position, new Vector2(0.1f, 0.45f), 0, obstacleLayer) && isOnGround)
                    {
                        Debug.Log("Jump");
                        enemyRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    }
                }

                if (enemyRb.velocity.y > 7)
                {
                    enemyRb.velocity = new Vector2(enemyRb.velocity.x, 7);
                }
            }
        }
    }

    protected void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            player = other.transform;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            player = null;
        }
    }

    virtual protected void RunToPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        TurnToTarget();
    }

    virtual protected void Move()
    {
        distToPoint = Vector2.Distance(transform.position, wayPoints[nextWayPoint].transform.position);

        transform.position = Vector2.MoveTowards(transform.position, wayPoints[nextWayPoint].transform.position, speed * Time.deltaTime);

        TakeTurn();
        if (distToPoint < 0.2f)
        {
            ChooseNextWaypoint();
        }
    }

    virtual protected void TurnToTarget()
    {
        if (target.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    virtual protected void TakeTurn()
    {
        if (transform.position.x > wayPoints[nextWayPoint].transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        
    }

    virtual protected void ChooseNextWaypoint()
    {
        nextWayPoint++;
        
        if (nextWayPoint == wayPoints.Length)
        {
            nextWayPoint = 0;
        }
    }

    virtual public void TakeDamage(int damage, bool isFormRight)
    {
        health -= damage;
        if (health > 0)
        {
            if (isFormRight)
            {
                enemyRb.AddForce(new Vector2(-1, 1) * impulse, ForceMode2D.Impulse);
            }
            else
            {
                enemyRb.AddForce(new Vector2(1, 1) * impulse, ForceMode2D.Impulse);
            }
        }
        if(spriteRenderer.color == Color.red)
        {
            StopCoroutine(ChangeColor());
        }
        StartCoroutine(ChangeColor());

        Debug.Log("Damage Taken !");
        if (health <= 0)
        {
            Death();
        }

    }

    virtual protected void Death()
    {
        isDead = true;
        enemyAnim.Death();
        enemyRb.bodyType = RigidbodyType2D.Static;
        foreach (Collider2D colider in colliders)
        {
            colider.enabled = false;
        }
        GameData.DeadEnemiesId.Add(enemyID);
        ChangeScore();
        StartCoroutine(DestroyAfterDeath());
    }

    virtual protected void ChangeScore()
    {
        GameData.Score += score;
        GameUI.Instance.ChangeScore();
    }

    virtual protected void Attack()
    {
        Player player = target.GetComponent<Player>();
        enemyAnim.Attack();
        isAttacking = true;

        StartCoroutine(AttackCooldown(attackCooldown));
    }

    virtual public void setActiveAttackCollider()
    {
        attackCollider.SetActive(true);
    }

    virtual public void setUnactiveAttackCollider()
    {
        attackCollider.SetActive(false);
    }

    virtual protected IEnumerator AttackCooldown(float seconds)
    {
        isCanAttack = false;
        Debug.Log("Start coroutine");
        yield return new WaitForSeconds(seconds);
        isCanAttack = true;
    }

    protected IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(30);

        Destroy(gameObject);
    }

    virtual protected IEnumerator ChangeColor()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
