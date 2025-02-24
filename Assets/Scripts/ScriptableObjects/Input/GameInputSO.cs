using UnityEngine;
using UnityEngine.InputSystem;


namespace JuanIsometric2D.GameInputSystem
{
    [CreateAssetMenu(fileName = "GameInputSO", menuName = "ScriptableObjects/Input/GameInputSO")]
    public class GameInputSO : ScriptableObject
    {
        public Vector2 MovementInput { get; private set; } = Vector2.zero;

        public bool AttackPressed { get; private set; } = false;
        public bool ProjectileShotPressed { get; private set; } = false;


        public bool InventoryTogglePressed { get; private set; } = false;


        PlayerInputActions playerInputActions;


        public void Initialize()
        {
            if (playerInputActions != null)
            {
                return;
            }

            playerInputActions = new PlayerInputActions();

            playerInputActions.Player.Enable();
            playerInputActions.UI.Enable();

            playerInputActions.Player.PlayerMove.performed += OnMovePerformed;
            playerInputActions.Player.PlayerMove.canceled += OnMoveCanceled;

            playerInputActions.Player.PlayerAttack.performed += OnAttackPerformed;

            playerInputActions.Player.ShootProjectile.performed += OnProjectileShot;


            playerInputActions.UI.ToggleInventory.performed += OnInventoryToggle;
        }

        public void Uninitialize()
        {
            playerInputActions.Player.PlayerMove.performed -= OnMovePerformed;
            playerInputActions.Player.PlayerMove.canceled -= OnMoveCanceled;

            playerInputActions.Player.PlayerAttack.performed -= OnAttackPerformed;

            playerInputActions.Player.ShootProjectile.performed -= OnProjectileShot;


            playerInputActions.UI.ToggleInventory.performed -= OnInventoryToggle;

            playerInputActions.Disable();
        }

        // --------------------- PLAYER ---------------------------

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



        // ################### PROJECTILE ##########################

        void OnProjectileShot(InputAction.CallbackContext context)
        {
            ProjectileShotPressed = true;
        }

        public void ResetProjectileShot()
        {
            ProjectileShotPressed = false;
        }

        // ########################################################



        // ########################################################
        // ########################################################


        // ---------------------- UI ------------------------------


        // ################# INVENTORY TOGGLE ######################

        void OnInventoryToggle(InputAction.CallbackContext context)
        {
            InventoryTogglePressed = true;
        }

        public void ResetInventoryToggle()
        {
            InventoryTogglePressed = false;
        }

        // ########################################################
    }
}

