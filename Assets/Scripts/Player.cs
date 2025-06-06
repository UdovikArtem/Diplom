using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    private float inputHorizontal;
    private bool inputJump;
    private bool dashInput;
    private bool attackInput;

    [SerializeField]
    private Transform obstacleCheck;
    [SerializeField]
    private LayerMask obstacleLayer;

    [Header("For attack")]
    [SerializeField]
    private Transform attackPos;
    [SerializeField]
    private LayerMask whatIsEnemies;
    [SerializeField]
    private float attackRange;
    private bool canReceiveInput = true;
    private bool inputRecived;
    private bool isAttacking;

    [Header("Specifications")]
    [SerializeField]
    private float speed = 10;
    [SerializeField] 
    private int damage = 10;
    [SerializeField]
    private int health = 100;
    [SerializeField]
    private float jumpForce = 20;

    [Header("Jump check")]
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private LayerMask groundLayer;

    private bool isOnGround;

    [Header("Animation")]
    [SerializeField]
    private Animator playerAnim; 
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [Header("Dashing")]
    [SerializeField] private float dashingVelocity = 14f;
    [SerializeField] private float dashingTime = 0.5f;

    private Vector2 dashingDir;

    private bool isDashing = false;
    private bool canDashing = true;

    private Rigidbody2D playerRb;
    private Collider2D[] colliders;

    private bool isDead = false;
    private bool isToLeft = false;
    // Start is called before the first frame update

    public bool IsDead { get{ return isDead; } set { isDead = value; } }
    public bool CanReceiveInput { get { return canReceiveInput; } set { canReceiveInput = value; } }

    public bool InputRecived { get => inputRecived; set => inputRecived = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public int Health { get => health; set => health = value; }
    public bool IsDashing { get => isDashing; set => isDashing = value; }
    public Transform ObstacleCheck { get => obstacleCheck; set => obstacleCheck = value; }
    public LayerMask ObstacleLayer { get => obstacleLayer; set => obstacleLayer = value; }
    public float InputHorizontal { get => inputHorizontal; set => inputHorizontal = value; }
    public bool InputJump { get => inputJump; set => inputJump = value; }
    public bool DashInput { get => dashInput; set => dashInput = value; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        health = GameData.PlayerHealth;
        if (health <= 0)
        {
            health = 100;
        }
        playerRb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        isOnGround = Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.5f, 0.2f), CapsuleDirection2D.Horizontal, 0, groundLayer);
        if (!isDead && !GameUI.Instance.IsFinished && !GameUI.Instance.IsPaused)
        {
            playerAnim.SetBool("is_dashing", isDashing);
            dashInput = Input.GetKeyUp(KeyCode.LeftShift);
            attackInput = Input.GetMouseButtonDown(0);
            inputHorizontal = Input.GetAxis("Horizontal");
            if (inputHorizontal != 0)
            {
                Move();
            }
            if (attackInput)
            {
                Attack();
            }
            if (dashInput)
            {
                Dash();
            }

            playerAnim.SetBool("is_ground", isOnGround);

            if (isDashing)
            {
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
                playerRb.velocity = dashingDir.normalized * dashingVelocity;
            }
            else
            {
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
                playerRb.velocity = new Vector2(0, playerRb.velocity.y);
            }
        }
        else if (isDead && isOnGround) {
            
            playerRb.bodyType = RigidbodyType2D.Static;
        }
        else if (isDead && !isOnGround)
        {
            playerRb.bodyType = RigidbodyType2D.Dynamic;
        }
    }


    private void FixedUpdate()
    {
        if (!isDead)
        {
            inputJump = Input.GetKey(KeyCode.Space);
            if (inputJump)
            {
                Jump();
            }

            if (playerRb.velocity.y > 10)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, 10);
            }
        }
    }

    public void Jump()
    {
        if (isOnGround && !isDashing)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void Move()
    {
        if (inputHorizontal > 0)
        {
            transform.localScale = new Vector3(0.8f, 0.8f, 0.8f); ;
            isToLeft = false;
        }
        else if (inputHorizontal < 0)
        {
            transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);
            isToLeft = true;
        }

        if ((!isAttacking) && !isDashing)
        {
            if (!Physics2D.OverlapBox(obstacleCheck.position, new Vector2(0.1f, 1.5f), 0, obstacleLayer))
            {
                transform.Translate(Vector2.right * Time.deltaTime * speed * inputHorizontal);
            }

            if (inputHorizontal > 0 || inputHorizontal < 0)
            {
                playerAnim.SetFloat("speed", 1);
            }
            else
            {
                playerAnim.SetFloat("speed", 0);
            }
        }
    }

    public void Dash()
    {
        if (dashInput && canDashing && isOnGround)
        {
            isDashing = true;
            canDashing = false;
            dashingDir = new Vector2(Input.GetAxis("Horizontal"), playerRb.velocity.y);
            if (dashingDir == Vector2.zero)
            {
                dashingDir = new Vector2(transform.localScale.x, playerRb.velocity.y);
            }
            StartCoroutine(StopDashing());
        }
    }

    public void Attack()
    {
        if(!isDashing && isOnGround)
        {
            if (canReceiveInput)
            {
                inputRecived = true;
                canReceiveInput = false;
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
                HashSet<Enemy> damagedEnemies = new HashSet<Enemy>();

                for (int i = 0; i < hitColliders.Length; i++)
                {
                    Enemy enemy = hitColliders[i].GetComponent<Enemy>();
                    if (enemy != null && !damagedEnemies.Contains(enemy))
                    {
                        enemy.TakeDamage(damage, isToLeft);
                        damagedEnemies.Add(enemy);
                    }
                }
            }
            else
            {
                return;
            }
        }
    }

    virtual public void TakeDamage(int damage)
    {
        health -= damage;

        GameUI.Instance.ChangeHealthbar();
        if (health <= 0)
        {
            isDead = true;
            Debug.Log("Deth");

            playerAnim.SetTrigger("death");
            groundCheck.localPosition = new Vector2(-0.01f, -0.4f);
            GameUI.Instance.GameOverPage();

            foreach (var item in colliders)
            {
                Debug.Log("Death");
                item.enabled = false;
            }
        }
    }

    public void InputManager()
    {
        if (!canReceiveInput)
        {
            canReceiveInput = true;
        }
        else
        {
            canReceiveInput = false;
        }
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashingTime);
        canDashing = true;
        isDashing = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
