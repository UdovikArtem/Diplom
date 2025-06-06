using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepairAttackBehaviur : StateMachineBehaviour
{

    protected Enemy enemy;
    protected Transform parent;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        parent = animator.transform.parent;

        if (parent != null)
        {
            enemy = parent.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.setAttackDir(parent.transform.localScale);
            }
        }
    }
}
