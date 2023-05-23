
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JLexStudios
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
        public float jumpHeight = 1.2f;
        public float fallingTimeOutDelta;
        public float jumpingTimeOutDelta;
        public float verticalVelocity;
        public float terminalVelocity = 53.0f;
        public Vector3 moveDirection;

        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
        }

        // Start is called before the first frame update
        void Start()
        {
            groundLayer = LayerMask.GetMask("Ground");
            if(cameraObject == null)
            {
                cameraObject = Camera.main.transform;
            }
        }

        public void HandleRotation(float delta)
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

        public void Movement(float delta)
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

            playerManager.playerAnimation.UpdateAnimatorValues
                (playerManager.inputManager.totalMovementInputAmount, 0f, delta);
        }

        public void GroundedCheck()
        {
            playerManager.isGrounded = Physics.CheckSphere(playerManager.transform.position,groundCheckSphereRadius,groundLayer);
            playerManager.animator.SetBool("isGrounded",playerManager.isGrounded);
        }

        public void GravityMethod(float delta) //Jump, Falling and Grounded
        {
            if (playerManager.isGrounded)
            {
                fallingTimeOutDelta = FallTimeout;
                // stop our velocity dropping infinitely when grounded
                if (verticalVelocity < 0.0f)
                {
                    verticalVelocity = -2f;
                }

                if (playerManager.inputManager.jumpFlag == true && jumpingTimeOutDelta <= 0.0f)
                {
                    HandleJumping();
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

            if (verticalVelocity < terminalVelocity)
            {
                verticalVelocity += gravity * delta;
            }
        }

        private void HandleJumping()
        {
            if (playerManager.inputManager.totalMovementInputAmount > 0f)
            {
                moveDirection = cameraObject.forward * playerManager.inputManager.Vertical;
                moveDirection += cameraObject.right * playerManager.inputManager.Horizontal;

                playerManager.playerAnimation.SetTargetAnimation("Jump", true, 0.5f);
                moveDirection.y = 0;
                Quaternion UnJumpRotation = Quaternion.LookRotation(moveDirection);
                playerManager.transform.rotation = UnJumpRotation;
            }
            else
            {
                playerManager.playerAnimation.SetTargetAnimation("Jump", true, 0.5f);
            }
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(transform.position, groundCheckSphereRadius);
        }
    }
}
