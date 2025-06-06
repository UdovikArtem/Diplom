using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WalkBehavior : StateMachineBehaviour
{
    private float lastSpeed;

    private Enemy parEnemy;
    private Transform enemy;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.transform.parent;

        if (enemy != null)
        {
            parEnemy = enemy.GetComponent<Enemy>();
            if (parEnemy != null)
            {
                lastSpeed = parEnemy.speed;
                parEnemy.speed = 0;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (parEnemy != null)
        {
            parEnemy.speed = lastSpeed;
        }
    }
}
