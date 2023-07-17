using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Creolty
{
    public class InputManager : MonoBehaviour
    {
        public Controls controls;
        public PlayerManager playerManager;

        [Header("Input Stats")]
        public float totalMovementInputAmount;
        public float Horizontal;
        public float Vertical;
        public float CameraX;
        public float CameraY;
        public Vector2 movementInput; //Gets Input from device
        public Vector2 cameraLookInput; //Gets Input for Camera
        public Vector3 getPosInput;

        [Header("Status")]
        public bool JMP;
        public bool SPRINT;

        [Header("Input Flags")]
        public bool jumpFlag;
        public bool sprintFlag;

        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
        }

        void JumpInput()
        {
            controls.Locomotion.Jump.performed += c => JMP = true;
            if(JMP)
            {
                jumpFlag = true;
            }
            else
            {
                jumpFlag = false;
            }
        }

        void SprintInput()
        {
            if(SPRINT && totalMovementInputAmount > 0.5f)
            {
                sprintFlag = true;
            }
            else
            {
                sprintFlag = false;
            }
        }

        public void Updater()
        {
            MovementInput();
            SprintInput();
            JumpInput();
        }

        private void OnEnable()
        {
            if(controls == null)
            {
                controls = new Controls();
                controls.Locomotion.Walk.performed += i => movementInput = i.ReadValue<Vector2>();
                controls.Locomotion.Look.performed += i => cameraLookInput = i.ReadValue<Vector2>();


                controls.Locomotion.Sprint.performed += i => SPRINT = true;
                controls.Locomotion.Sprint.canceled += i => SPRINT = false;
            }
            controls.Enable();
        }

        private void OnDisable()
        {
            controls.Disable();
        }

        void MovementInput()
        {
            Horizontal = movementInput.x;
            Vertical = movementInput.y;
            totalMovementInputAmount = Mathf.Clamp01(Mathf.Abs(Horizontal) + Mathf.Abs(Vertical));
            CameraX = cameraLookInput.x;
            CameraY = cameraLookInput.y;
        }
    }
}
