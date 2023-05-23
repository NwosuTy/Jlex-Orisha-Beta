using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JLexStudios
{
    public class InputManager : MonoBehaviour
    {
        public PlayerManager playerManager;
        public Controls controls;

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
        bool SPRINT;

        [Header("Input Flags")]
        public bool jumpFlag;
        public bool sprintFlag;

        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        void JumpInput()
        {
            controls.Locomotion.Jump.performed += c => JMP = true;
            //Checks if JMP is true, if yes return jumpflag as true else return false
            //JMP = true ? jumpFlag = true : jumpFlag = false;
            if(JMP)
            {
                jumpFlag = true;
            }
            else
            {
                jumpFlag = false;
            }
        }

        public void TickInput()
        {
            MovementInput();
            JumpInput();
        }

        private void OnEnable()
        {
            if(controls == null)
            {
                controls = new Controls();
                controls.Locomotion.Walk.performed += i => movementInput = i.ReadValue<Vector2>();
                controls.Locomotion.Look.performed += i => cameraLookInput = i.ReadValue<Vector2>();
            }
            controls.Enable();
        }

        private void OnDisable()
        {
            if(controls != null ) 
            {

            }
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
