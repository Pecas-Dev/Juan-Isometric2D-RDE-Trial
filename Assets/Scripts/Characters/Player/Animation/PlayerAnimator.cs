using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;


namespace JuanIsometric2D.Animation.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        readonly string[] scenesWithoutCombat = { "Isometric2D_Task2", "Isometric2D_Task3" };

        bool isCombatDisabledScene;


        const string F_PLAYER_SPEED = "playerSpeed";
        const string F_PLAYER_HORIZONTAL = "playerHorizontal";
        const string F_PLAYER_VERTICAL = "playerVertical";

        const string F_ATTACK_HORIZONTAL = "attackHorizontal";
        const string F_ATTACK_VERTICAL = "attackVertical";


        [SerializeField] Animator playerMotionAnimator;
        [SerializeField] Animator playerCombatAnimator;


        public Animator PlayerMotionAnimator => playerMotionAnimator;
        public Animator PlayerCombatAnimator => playerCombatAnimator;


        Vector2 currentDirection;


        SpriteRenderer playerSpriteRenderer;


        void Awake()
        {
            isCombatDisabledScene = scenesWithoutCombat.Contains(SceneManager.GetActiveScene().name);

            Animator[] childAnimators = GetComponentsInChildren<Animator>();

            foreach (Animator animatiors in childAnimators)
            {
                if (animatiors.gameObject.CompareTag("MotionAnimation"))
                {
                    playerMotionAnimator = animatiors;
                }
                else if (!isCombatDisabledScene && animatiors.gameObject.CompareTag("WeaponAnimation")) 
                {
                    playerCombatAnimator = animatiors;
                }
            }

            playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

            currentDirection = Vector2.down;
        }

        void Start()
        {
            LogNullReferenceErrors();
        }

        void Update()
        {
            if (!isCombatDisabledScene)
            {
                CheckAndResetAttackAnimation();
            }
        }

        void CheckAndResetAttackAnimation()
        {
            if (playerCombatAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !playerCombatAnimator.GetCurrentAnimatorStateInfo(0).IsName("EmptyState"))
            {
                playerCombatAnimator.Play("EmptyState");
            }
        }

        public void UpdateAnimation(Vector2 movementInput, float speedMultiplier)
        {
            bool isMoving = movementInput.magnitude > 0;

            playerMotionAnimator.SetFloat(F_PLAYER_SPEED, isMoving ? speedMultiplier : 0f);

            if (isMoving)
            {
                playerMotionAnimator.SetFloat(F_PLAYER_HORIZONTAL, movementInput.x);
                playerMotionAnimator.SetFloat(F_PLAYER_VERTICAL, movementInput.y);

                currentDirection = movementInput;
            }

            if (!isMoving)
            {
                playerMotionAnimator.SetFloat(F_PLAYER_HORIZONTAL, 0.0f);
                playerMotionAnimator.SetFloat(F_PLAYER_VERTICAL, 0.0f);

                currentDirection = Vector2.down;
            }

            if (!isCombatDisabledScene && playerCombatAnimator != null)
            {
                playerCombatAnimator.SetFloat(F_ATTACK_HORIZONTAL, currentDirection.x);
                playerCombatAnimator.SetFloat(F_ATTACK_VERTICAL, currentDirection.y);
            }
        }

        public void PlayIdle()
        {
            playerMotionAnimator.Play("PlayerIdle");
        }

        public void PlayMovement()
        {
            playerMotionAnimator.CrossFadeInFixedTime("PlayerMovementBlendTree", 0.01f);
        }

        public void PlayAttack()
        {
            if (!isCombatDisabledScene && playerCombatAnimator != null)
            {
                playerCombatAnimator.SetFloat(F_ATTACK_HORIZONTAL, currentDirection.x);
                playerCombatAnimator.SetFloat(F_ATTACK_VERTICAL, currentDirection.y);

                playerCombatAnimator.CrossFadeInFixedTime("PlayerAttackBlendTree", 0.01f);
            }
        }

        void LogNullReferenceErrors()
        {
            if (playerMotionAnimator == null)
            {
                Debug.LogError("Motion animator not found!");
            }
            if (playerCombatAnimator == null && !isCombatDisabledScene)
            {
                Debug.LogError("Combat animator not found!");
            }
        }
    }
}
