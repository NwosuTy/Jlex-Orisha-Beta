
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
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float gravityForce = -20f;
        [Tooltip("Useful for rough ground")]
        public float groundOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float groundRadius = 0.28f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask groundLayer;
        [Tooltip("Checks the radius of ground checking sphere")]
        public float groundCheckSphereRadius = 0.35f;

        [Header("Locomotion Stats")]
        [Space(5)]
        [Header("Basic Locomotion Stats")]
        public Vector3 moveDirection;
        public float movementSpeed = 3f;
        public float rotationSpeed = 12f;
        public float sprintSpeed = 6f;

        [Space(5)]
        [Header("Jump Stats")]
        public Vector3 verticalVelocity;
        public Vector3 jumpDirection;
        public float jumpHeight = 4f;
        public float jumpSpeed = 5f;
        public float freeFallSpeed = 2f;

        [Space(5)]
        [Header("Crawl And Crouch")]
        public Vector3 crouchOrCallDirection;
        public float crouchSpeed = 3f;
        public float crawlSpeed = 1.5f;

        [Space(5)]
        [Header("Gravity Stats")]
        public float inAirTimer = 0f;
        public float groundedVerticalVelocity = -20f; //applied when grounded
        public float fallStartVerticalVelocity = -7f; //applied whwn in air
        public bool fallingVelocitySet = false;

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
            if(playerManager.isJumping)
            {
                return;
            }
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

            if (playerManager.isJumping)
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
                (playerManager.inputManager.totalMovementInputAmount, 0f, delta,playerManager.inputManager.sprintFlag);
        }

        private void GroundedCheck()
        {
            playerManager.isGrounded = Physics.CheckSphere(playerManager.transform.position,groundCheckSphereRadius,groundLayer);
            playerManager.animator.SetBool("isGrounded",playerManager.isGrounded);
        }

        //Faling and Landing
        private void GravityMethod(float delta)
        {
            if(playerManager.isGrounded)
            {
                if(verticalVelocity.y < 0f)
                {
                    inAirTimer = 0f;
                    fallingVelocitySet = false;
                    verticalVelocity.y = groundedVerticalVelocity;
                }
            }
            else
            {
                if(playerManager.isJumping != true && fallingVelocitySet != true)
                {
                    fallingVelocitySet = true;
                    verticalVelocity.y = fallStartVerticalVelocity;
                }
                inAirTimer += delta;
                playerManager.animator.SetFloat("InAirTimer", inAirTimer);
                verticalVelocity.y += gravityForce * delta;
            }
            playerManager.characterController.Move(verticalVelocity * delta);
        }

        private void HandleFreeFallMovement(float delta)
        {
            if(!playerManager.isGrounded)
            {
                Vector3 freeFallDirection;

                freeFallDirection = cameraObject.transform.forward * playerManager.inputManager.Vertical;
                freeFallDirection += cameraObject.transform.right * playerManager.inputManager.Horizontal;
                freeFallDirection.y = 0f;

                playerManager.characterController.Move(freeFallDirection * freeFallSpeed * delta);
            }
        }

        private void HandleJumping()
        {
            if(playerManager.isInteracting) 
            {
                return;
            }
            if(playerManager.isJumping)
            {
                return;
            }
            if(!playerManager.isGrounded)
            { 
                return; 
            }

            if (playerManager.inputManager.jumpFlag)
            {
                playerManager.animator.SetBool("isJumping", true);
                playerManager.playerAnimation.SetTargetAnimation("Main Jump", false);
            }

            jumpDirection = cameraObject.transform.forward * playerManager.inputManager.Vertical;
            jumpDirection += cameraObject.transform.right * playerManager.inputManager.Horizontal;
            jumpDirection.y = 0;

            if(jumpDirection != Vector3.zero)
            {
                if (playerManager.inputManager.sprintFlag)
                {
                    jumpDirection *= 1f;
                }
                else if (playerManager.inputManager.totalMovementInputAmount > 0.5f)
                {
                    jumpDirection *= 0.5f;
                }
                else if (playerManager.inputManager.totalMovementInputAmount <= 0.5f)
                {
                    jumpDirection *= 0.25f;
                }
            }          
        }

        private void HandleJumpMovement(float delta)
        {
            if(playerManager.isJumping)
            {
                playerManager.characterController.Move(jumpDirection * jumpSpeed * delta);
            }
        }

        private void HandleCrawlingAndCrouchingMovement(float delta)
        {
            crouchOrCallDirection = cameraObject.transform.forward * playerManager.inputManager.Vertical;
            crouchOrCallDirection += cameraObject.transform.right * playerManager.inputManager.Horizontal;
            crouchOrCallDirection.y = 0;

            if(playerManager.isCrouching)
            {
                playerManager.characterController.Move(crouchOrCallDirection * crouchSpeed * delta);
            }
            else if(playerManager.isCrawling)
            {
                playerManager.characterController.Move(crouchOrCallDirection * crawlSpeed * delta);
            }
            playerManager.playerAnimation.UpdateAnimatorValues
                    (playerManager.inputManager.totalMovementInputAmount, 1f, delta, false);
        }

        public void Updater(float delta)
        {
            GroundedCheck();
            GravityMethod(delta);
            Movement(delta);
            HandleJumping();
            HandleJumpMovement(delta);
            HandleFreeFallMovement(delta);
            HandleRotation(delta);
            HandleCrawlingAndCrouchingMovement(delta);
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(transform.position, groundCheckSphereRadius);
        }
    }
}
