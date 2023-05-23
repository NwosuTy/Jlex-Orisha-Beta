using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JLexStudios
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("Components")]
        public InputManager inputManager;
        public PlayerAnimation playerAnimation;
        public PlayerLocomotion playerLocomotion;
        public Animator animator;
        public CharacterController characterController;

        [Header("Status")]
        public bool isInteracting;
        public bool isGrounded;
        public bool canRotate;


        // Start is called before the first frame update
        void Awake()
        {
            inputManager = GetComponent<InputManager>();
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            playerAnimation = GetComponent<PlayerAnimation>();
        }

        // Update is called once per frame
        void Update()
        {
            float delta = Time.deltaTime;
            SetAnimatorBool();
            inputManager.TickInput();
            playerLocomotion.GroundedCheck();
            playerLocomotion.GravityMethod(delta);
            playerLocomotion.Movement(delta);
            playerLocomotion.HandleRotation(delta);
        }

        private void LateUpdate()
        {
            ResetInputs();            
        }

        void SetAnimatorBool()
        {
            isInteracting = animator.GetBool("isInteracting");
            canRotate = animator.GetBool("canRotate");
        }

        void ResetInputs()
        {
            inputManager.JMP = false;
        }
    }
}
