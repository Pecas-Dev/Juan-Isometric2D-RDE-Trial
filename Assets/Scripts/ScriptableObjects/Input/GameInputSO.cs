using UnityEngine;
using UnityEngine.InputSystem;


namespace JuanIsometric2D.GameInputSystem
{
    [CreateAssetMenu(fileName = "GameInputSO", menuName = "ScriptableObjects/Input/GameInputSO")]
    public class GameInputSO : ScriptableObject
    {
        public Vector2 MovementInput { get; private set; } = Vector2.zero;

        PlayerInputActions playerInputActions;

        public void Initialize()
        {
            if (playerInputActions != null)
            {
                return;
            }

            playerInputActions = new PlayerInputActions();

            playerInputActions.Player.Enable();

            playerInputActions.Player.PlayerMove.performed += OnMovePerformed;
            playerInputActions.Player.PlayerMove.canceled += OnMoveCanceled;
        }

        public void Uninitialize()
        {
            playerInputActions.Player.PlayerMove.performed -= OnMovePerformed;
            playerInputActions.Player.PlayerMove.canceled -= OnMoveCanceled;

            playerInputActions.Disable();
        }


        // ################### MOVEMENT ##########################

        void OnMovePerformed(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();

            if (MovementInput.magnitude > 1f)
            {
                MovementInput = MovementInput.normalized;
            }
        }

        void OnMoveCanceled(InputAction.CallbackContext context)
        {
            MovementInput = Vector2.zero;
        }

        // ########################################################
    }
}

