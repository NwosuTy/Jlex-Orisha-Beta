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
        public bool CRAWL;
        public bool hasCrawled = false;

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

        void CrawlingInput()
        {
            controls.Locomotion.CrouchCrawl.performed += c => CRAWL = true;
            if(CRAWL)
            {
                if(playerManager.isCrouching != true && playerManager.isCrawling != true)
                {
                    playerManager.isCrouching = true;
                }
                else if(playerManager.isCrouching == true)
                {
                    if(hasCrawled != true)
                    {
                        hasCrawled = true;
                        playerManager.isCrouching = false;
                        playerManager.isCrawling = true;
                    }
                    else
                    {
                        playerManager.isCrawling = false;
                        playerManager.isCrouching = false;
                        hasCrawled = false;
                    } 
                }
                else if(playerManager.isCrawling == true)
                {
                    playerManager.isCrawling = false;
                    playerManager.isCrouching = true;
                }
            }
        }

        public void Updater()
        {
            MovementInput();
            SprintInput();
            JumpInput();
            CrawlingInput();
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
