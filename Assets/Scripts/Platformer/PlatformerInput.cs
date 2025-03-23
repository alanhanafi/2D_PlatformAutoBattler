using System;
using UnityEngine;

namespace Platformer
{
    public class PlatformerInput : MonoBehaviour
    {
        private PlayerInputActions playerInputActions;
        private void Awake()
        {
            playerInputActions = new PlayerInputActions();
            playerInputActions.Player.Enable();
        }

        internal Vector2 GetMovementVector()
        {
            return playerInputActions.Player.Move.ReadValue<Vector2>();
        }
        
        internal bool GetJumpPressed()
        {
            return playerInputActions.Player.Jump.WasPressedThisFrame();
        }

        internal bool GetJumpDown()
        {
            return playerInputActions.Player.Jump.IsPressed();
        }
    }
}