using UnityEngine.InputSystem;
using UnityEngine;


namespace JuanIsometric2D.GameInputSystem
{
    [CreateAssetMenu(fileName = "GameInputSO", menuName = "ScriptableObjects/Input/GameInputSO")]
    public class GameInputSO : ScriptableObject
    {
        public Vector2 MovementInput { get; private set; } = Vector2.zero;

        public bool AttackPressed { get; private set; } = false;


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

            playerInputActions.Player.PlayerAttack.performed += OnAttackPerformed;
        }

        public void Uninitialize()
        {
            playerInputActions.Player.PlayerMove.performed -= OnMovePerformed;
            playerInputActions.Player.PlayerMove.canceled -= OnMoveCanceled;

            playerInputActions.Player.PlayerAttack.performed -= OnAttackPerformed;

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



        // ################### ATTACK ##########################

        void OnAttackPerformed(InputAction.CallbackContext context)
        {
            AttackPressed = true;
        }

        public void ResetAttack()
        {
            AttackPressed = false;
        }

        // ########################################################

    }
}

