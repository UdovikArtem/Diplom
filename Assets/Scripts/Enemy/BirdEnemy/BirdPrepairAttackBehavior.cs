using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BirdPrepairAttackBehavior : PrepairAttackBehaviur
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        parent = animator.transform.parent;

        if (parent != null)
        {
            enemy = parent.GetComponent<Enemy>();
            if (enemy != null)
            {
                Vector3 scale = parent.transform.localScale;
                float div = (enemy.Target.position.x - parent.position.x) / (enemy.Target.position.y - parent.position.y);
                int yAbs = 1;
                if(enemy.Target.position.y - parent.position.y < 0)
                {
                    yAbs = -1;
                }
                if (div == 1 || div == -1)
                {
                    enemy.setAttackDir(scale);
                }
                else if ((div > 0 && div > 1) || (div < 0 && div < -1))
                {
                    enemy.setAttackDir(new Vector3(scale.x, scale.y * (yAbs / Math.Abs(div)), scale.z));
                }
                else if ((div > 0 && div < 1) || (div < 0 && div > -1))
                {
                   enemy.setAttackDir(new Vector3(scale.x * Math.Abs(div), scale.y * yAbs, scale.z));
                }
                Debug.Log(enemy.getAttackDir());
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        parent = animator.transform.parent;

        if (parent != null)
        {
            parent.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }

}
