using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

namespace JLexStudios
{
    public class PlayerAnimation : MonoBehaviour
    {
        public PlayerManager playerManager;
        public int vertical;
        public int horizontal;

        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
        }

        private void Start()
        {
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }
        public void UpdateAnimatorValues(float VerticalMovement, float HorizontalMovement, float delta)
        {
            //Vertical
            float v = 0f;
            if (VerticalMovement > 0.55f)
            {
                v = 1f;
            }
            else if (VerticalMovement < -0.55f)
            {
                v = -1f;
            }
            else
            {
                v = 0f;
            }

            //Horizontal
            float h = 0f;
            if (HorizontalMovement > 0f && HorizontalMovement < 0.55f)
            {
                h = 0.55f;
            }
            else if (HorizontalMovement > 0.55f)
            {
                h = 1f;
            }
            else if (HorizontalMovement < 0f && HorizontalMovement > -0.55f)
            {
                h = -0.5f;
            }
            else if (HorizontalMovement < -0.55f)
            {
                h = -1f;
            }
            else
            {
                h = 0f;
            }
            playerManager.animator.SetFloat(vertical, v, 0.1f, delta);
            playerManager.animator.SetFloat(horizontal, h, 0.1f, delta);
        }

        public void SetTargetAnimation
            (string TargetAnimation, bool isInteracting, float transitionDuration = 0.2f, bool canRotate = false)
        {
            playerManager.animator.applyRootMotion = isInteracting;
            playerManager.canRotate = canRotate;
            playerManager.animator.SetBool("IsInteracting", isInteracting);
            playerManager.animator.CrossFade(TargetAnimation, transitionDuration);
        }

        protected virtual void OnAnimatorMove()
        {
            /*if (playerManager.isInteracting == false)
            {
                return;
            }*/
            Vector3 velocity = playerManager.animator.deltaPosition;
            playerManager.characterController.Move(velocity);
            playerManager.transform.rotation *= playerManager.animator.deltaRotation;
        }
    }
}
