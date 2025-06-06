using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehavior : StateMachineBehaviour
{
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Player.Instance.InputRecived)
        {
            animator.SetTrigger("firstAttack");
            Player.Instance.InputManager();
            Player.Instance.InputRecived = false;
            Player.Instance.CanReceiveInput = true;
        }
    }

}
