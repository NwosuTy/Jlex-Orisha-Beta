
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Creolty
{
    public class PlayerLocomotion : MonoBehaviour
    {
        [Header("transform")]
        public Transform cameraObject;

        [Header("Components")]
        public PlayerManager playerManager;

        [Header("Physics")]
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float gravity = -15f;
        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;
        [Tooltip("Useful for rough ground")]
        public float groundOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float groundRadius = 0.28f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask groundLayer;
        [Tooltip("Checks the radius of ground checking sphere")]
        public float groundCheckSphereRadius = 0.35f;

        [Header("Locomotion Stats")]
        public float movementSpeed = 3f;
        public float rotationSpeed = 12f;
        public float sprintSpeed = 6f;
        public float crawlSpeed = 1f;
        public float jumpHeight = 1.2f;
        public float fallingTimeOutDelta;
        public float jumpingTimeOutDelta;
        public Vector3 verticalVelocity;
        public float terminalVelocity = 53.0f;
        public Vector3 moveDirection;

        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
        }

        void Start()
        {
            groundLayer = LayerMask.GetMask("Ground");
            if(cameraObject == null)
            {
                cameraObject = Camera.main.transform;
            }
        }

        private void HandleRotation(float delta)
        {
            if (playerManager.canRotate)
            {
                Vector3 TargetDir = Vector3.zero;
                float MoveOverride = playerManager.inputManager.totalMovementInputAmount;

                TargetDir = cameraObject.forward * playerManager.inputManager.Vertical;
                TargetDir += cameraObject.right * playerManager.inputManager.Horizontal;
                TargetDir.Normalize();
                TargetDir.y = 0.0f;

                if (TargetDir == Vector3.zero)
                {
                    TargetDir = playerManager.transform.forward;
                }

                float rs = rotationSpeed;

                Quaternion tr = Quaternion.LookRotation(TargetDir);
                Quaternion transformPosition = Quaternion.Slerp(playerManager.transform.rotation, tr, rs * delta);

                playerManager.transform.rotation = transformPosition;
            }
        }

        private void Movement(float delta)
        {
            if (playerManager.isInteracting)
            {
                return;
            }
            if (!playerManager.isGrounded)
            {
                return;
            }
            moveDirection = cameraObject.transform.forward * playerManager.inputManager.Vertical;
            moveDirection += cameraObject.transform.right * playerManager.inputManager.Horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0f;

            if (playerManager.inputManager.sprintFlag)
            {
                playerManager.characterController.Move(sprintSpeed * delta * moveDirection);
            }
            else
            {
                if (playerManager.inputManager.totalMovementInputAmount > 0.5f)
                {
                    playerManager.characterController.Move(movementSpeed * delta * moveDirection);
                }
                else if (playerManager.inputManager.totalMovementInputAmount <= 0.5f)
                {
                    playerManager.characterController.Move(movementSpeed * delta * moveDirection);
                }
            }

            playerManager.playerAnimation.UpdateAnimatorValues(playerManager.inputManager.totalMovementInputAmount, 0f, delta,
                playerManager.inputManager.sprintFlag);
        }

        private void GroundedCheck()
        {
            playerManager.isGrounded = Physics.CheckSphere(playerManager.transform.position,groundCheckSphereRadius,groundLayer);
            playerManager.animator.SetBool("isGrounded",playerManager.isGrounded);
        }

        private void GravityMethod(float delta) //Jump, Falling and Grounded
        {
            if (playerManager.isGrounded)
            {
                fallingTimeOutDelta = FallTimeout;

                if (verticalVelocity.y < 0.0f)
                {
                    verticalVelocity.y = 0;
                }

                if (jumpingTimeOutDelta >= 0.0f)
                {
                    jumpingTimeOutDelta -= delta;
                    if (jumpingTimeOutDelta < 0.0f)
                    {
                        jumpingTimeOutDelta = 0.0f;
                    }
                }
            }
            else
            {
                jumpingTimeOutDelta = JumpTimeout;

                if (fallingTimeOutDelta >= 0f)
                {
                    fallingTimeOutDelta -= delta;
                }
                else
                {
                    playerManager.playerAnimation.SetTargetAnimation("Fall", true);
                }
            }

            if (verticalVelocity.y < terminalVelocity)
            {
                verticalVelocity.y += gravity * delta;
            }
            playerManager.characterController.Move(verticalVelocity * delta);
        }

        private void HandleJumping()
        {
            if(playerManager.inputManager.jumpFlag)
            {
                if (playerManager.inputManager.totalMovementInputAmount > 0f)
                {
                    moveDirection = cameraObject.forward * playerManager.inputManager.Vertical;
                    moveDirection += cameraObject.right * playerManager.inputManager.Horizontal;

                    playerManager.playerAnimation.SetTargetAnimation("Jump Start", true, 0.5f);
                    moveDirection.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    Quaternion OnJumpRotation = Quaternion.LookRotation(moveDirection);
                    playerManager.transform.rotation = OnJumpRotation;
                }
                else
                {
                    playerManager.playerAnimation.SetTargetAnimation("Jump Start", true, 0.5f);
                }
                playerManager.characterController.Move(Time.deltaTime * moveDirection);
            }
        }

        private void HandleCrawling()
        {
            // Toggle between standing to crawling.
            if (playerManager.inputManager.crawlFlag)
            {
                playerManager.isCrawling = true;

                if (playerManager.isCrawling) playerManager.playerAnimation.SetTargetAnimation("Crawling Forward", true, 0.5f);
            }
        }
        public void Updater(float delta)
        {
            GroundedCheck();
            GravityMethod(delta);
            Movement(delta);
            HandleJumping();
            HandleRotation(delta);
            HandleCrawling();
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(transform.position, groundCheckSphereRadius);
        }
    }
}
