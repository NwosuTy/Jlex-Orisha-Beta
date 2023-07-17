using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creolty
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
        public bool isJumping;
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
            inputManager.Updater();
            playerLocomotion.Updater(delta);
        }

        private void LateUpdate()
        {
            ResetInputs();            
        }

        void SetAnimatorBool()
        {
            isInteracting = animator.GetBool("isInteracting");
            canRotate = animator.GetBool("canRotate");
            animator.SetBool("isJumping", isJumping);
        }

        void ResetInputs()
        {
            inputManager.JMP = false;
        }
    }
}

