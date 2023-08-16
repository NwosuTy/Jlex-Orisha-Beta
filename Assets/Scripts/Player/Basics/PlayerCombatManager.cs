using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creolty
{
    public class PlayerCombatManager : MonoBehaviour
    {
        public PlayerManager playerManager;

        void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
        }
    }
}
