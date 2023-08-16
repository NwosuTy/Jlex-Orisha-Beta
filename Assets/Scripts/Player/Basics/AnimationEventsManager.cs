using Creolty;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimationEventsManager : MonoBehaviour
{
    [Header("Components")]
    public PlayerManager playerManager;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    public void ApplyJumpForce()
    {
        playerManager.playerLocomotion.verticalVelocity.y =
            Mathf.Sqrt(playerManager.playerLocomotion.jumpHeight * -3f * playerManager.playerLocomotion.gravityForce);
    }
}
