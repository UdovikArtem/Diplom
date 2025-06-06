using System;
using Unity.Mathematics;
using UnityEngine;

public class EnemyAnim : MonoBehaviour
{
    [SerializeField]
    protected Animator enemyAnim;

    private float lastXPosition;
    // Start is called before the first frame update
    void Start()
    {
        lastXPosition = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeXPosition();
    }

    public void PlayerFoundIsTrue()
    {
        enemyAnim.SetBool("player_found", true);
    }

    public void PlayerFoundIsFalse()
    {
        enemyAnim.SetBool("player_found", false);
    }

    public void Attack()
    {
        enemyAnim.SetTrigger("attack");
    }

    void ChangeXPosition()
    {
        if (GetComponent<Enemy>().IsOnGround)
        {
            float distance = math.abs(transform.position.x - lastXPosition);
            lastXPosition = transform.position.x;
            enemyAnim.SetFloat("x_value", distance);
        }
        else
        {
            enemyAnim.SetFloat("x_value", 0);
        }
    }

    virtual public void Death()
    {
        enemyAnim.SetTrigger("death");
    }
}
