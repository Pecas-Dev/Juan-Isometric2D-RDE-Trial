using JuanIsometric2D.Animation.Player;
using JuanIsometric2D.GameInputSystem;

using UnityEngine;


namespace JuanIsometric2D.StateMachine.Player
{
    public class PlayerStateMachine : StateMachine
    {
        public enum PlayerState
        {
            Idle,
            Movement
            //Attack
        }

        [Header("Component References")]
        [SerializeField] Rigidbody2D playerRigidbody2D;
        [SerializeField] CapsuleCollider2D playerCapsuleCollider2D;
        [SerializeField] Animator playerAnimator;

        public Rigidbody2D PlayerRigidbody2D => playerRigidbody2D;
        public CapsuleCollider2D PlayerCapsuleCollider2D => playerCapsuleCollider2D;
        public Animator PlayerAnimator => playerAnimator;


        [Header("Script References")]
        [SerializeField] GameInputSO playerGameInputSO;
        [SerializeField] PlayerAnimator playerAnimatorScript;

        public GameInputSO PlayerGameInputSO => playerGameInputSO;
        public PlayerAnimator PlayerAnimatorScript => playerAnimatorScript;


        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 5f;

        public float MoveSpeed => moveSpeed;


        [Header("Isometric Settings")]
        [SerializeField] float isometricAngle = 45f;

        public float IsometricAngle => isometricAngle;


        void Awake()
        {
            if (playerGameInputSO != null)
            {
                playerGameInputSO.Initialize();
            }

            playerRigidbody2D = GetComponent<Rigidbody2D>();
            playerCapsuleCollider2D = GetComponent<CapsuleCollider2D>();
            playerAnimatorScript = GetComponent<PlayerAnimator>();
            playerAnimator = GetComponentInChildren<Animator>();
        }

        void Start()
        {
            LogNullReferenceErrors();

            SwitchState(new PlayerIdleState(this));
        }

        void OnDisable()
        {
            playerGameInputSO.Uninitialize();
        }

        void LogNullReferenceErrors()
        {
            if (playerRigidbody2D == null)
            {
                Debug.LogError("Rigidbody2D (Player gameObject) - Reference is missing!");
            }

            if (playerCapsuleCollider2D == null)
            {
                Debug.LogError("CapsuleCollider2D (Player gameObject) - Reference is missing!");
            }

            if (playerGameInputSO == null)
            {
                Debug.LogError("GameInputSO (Player gameObject) - Reference is missing!");
            }

            if (playerAnimatorScript == null)
            {
                Debug.LogError("PlayerAnimator script (Player gameObject) - Reference is missing!");
            }

            if (playerAnimator == null)
            {
                Debug.LogError("Animator (Player gameObject) - Reference is missing!");
            }
        }
    }
}
