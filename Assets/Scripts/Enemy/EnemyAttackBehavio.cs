using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackBehaviur : StateMachineBehaviour
{
    private Enemy enemy;
    private Transform parent;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        parent = animator.transform.parent;

        if (parent != null)
        {
            enemy = parent.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.setActiveAttackCollider();
                Rigidbody2D enemyRb = parent.GetComponent<Rigidbody2D>();
                enemyRb.AddForce(new Vector2(-1 * enemy.getAttackDir().x, enemy.getAttackDir().y) * enemy.jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemy != null)
        {
            enemy.setUnactiveAttackCollider();
            enemy.IsAttacking = false;
        }
    }

}
