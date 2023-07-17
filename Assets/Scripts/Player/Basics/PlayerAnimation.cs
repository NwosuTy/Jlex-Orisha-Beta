using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creolty
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
        public void UpdateAnimatorValues
            (float VerticalMovement, float HorizontalMovement, float delta, bool isSprinting)
        {
            float snappedVeritcal = 0f;
            float snappedHorizontal = 0f;

            #region vertical
            if (VerticalMovement > 0.55f)
            {
                snappedVeritcal = 1f;
            }
            else if (VerticalMovement < -0.55f)
            {
                snappedVeritcal = -1f;
            }
            else
            {
                snappedVeritcal = 0f;
            }
            #endregion
            #region horizontal
            
            if (HorizontalMovement > 0f && HorizontalMovement < 0.55f)
            {
                snappedHorizontal = 0.55f;
            }
            else if (HorizontalMovement > 0.55f)
            {
                snappedHorizontal = 1f;
            }
            else if (HorizontalMovement < 0f && HorizontalMovement > -0.55f)
            {
                snappedHorizontal = -0.5f;
            }
            else if (HorizontalMovement < -0.55f)
            {
                snappedHorizontal = -1f;
            }
            else
            {
                snappedHorizontal = 0f;
            }
            #endregion

            if(isSprinting)
            {
                snappedVeritcal = 2f;
                snappedHorizontal = HorizontalMovement;
            }
            playerManager.animator.SetFloat(vertical, snappedVeritcal, 0.1f, delta);
            playerManager.animator.SetFloat(horizontal, snappedHorizontal, 0.1f, delta);
        }

        public void SetTargetAnimation
            (string TargetAnimation, bool isInteracting, float transitionDuration = 0.2f, bool canRotate = false)
        {
            playerManager.animator.applyRootMotion = isInteracting;
            playerManager.canRotate = canRotate;
            playerManager.animator.SetBool("isInteracting", isInteracting);
            playerManager.animator.CrossFade(TargetAnimation, transitionDuration);
        }

        protected virtual void OnAnimatorMove()
        {
            if (playerManager.isInteracting == false)
            {
                return;
            }
            Vector3 velocity = playerManager.animator.deltaPosition;
            playerManager.characterController.Move(velocity);
            playerManager.transform.rotation *= playerManager.animator.deltaRotation;
        }
    }
}
