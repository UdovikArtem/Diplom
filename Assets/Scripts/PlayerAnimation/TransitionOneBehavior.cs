using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionOneBehaivior : StateMachineBehaviour
{
    [SerializeField]
    private string trigerName;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.CanReceiveInput = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Player.Instance.InputRecived)
        {
            animator.SetTrigger(trigerName);
            Player.Instance.InputManager();
            Player.Instance.InputRecived = false;
            Player.Instance.CanReceiveInput = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player.Instance.IsAttacking = false;
    }

}
