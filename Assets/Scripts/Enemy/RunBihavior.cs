using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunBihavior : StateMachineBehaviour
{
    public float speed;

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
                parEnemy.speed = speed;
            }
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.transform.parent;

        if (enemy != null)
        {
            parEnemy = enemy.GetComponent<Enemy>();
            if (parEnemy != null && parEnemy.CanSpeedChange)
            {
                parEnemy.speed = speed;
            }
        }
    }
}
