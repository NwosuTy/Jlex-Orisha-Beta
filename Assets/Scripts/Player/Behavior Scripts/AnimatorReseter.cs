using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JLexStudios
{
    public class AnimatorReseter : StateMachineBehaviour
    {
        public string canRotate = "canRotate";
        public bool canRotateStatus = true;

        public string isInteratciting = "isInteracting";
        public bool isInteratcitingStatus;

        public string isJumping = "isJumping";
        public bool isJumpingStatus;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(canRotate, canRotateStatus);
            animator.SetBool(isInteratciting, isInteratcitingStatus);
            animator.SetBool(isJumping, isJumpingStatus);
        }
    }
}

