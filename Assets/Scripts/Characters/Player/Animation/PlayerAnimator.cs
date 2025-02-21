using UnityEngine;


namespace JuanIsometric2D.Animation.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        const string F_PLAYER_SPEED = "playerSpeed";
        const string F_PLAYER_HORIZONTAL = "playerHorizontal";
        const string F_PLAYER_VERTICAL = "playerVertical";

        const string F_ATTACK_HORIZONTAL = "attackHorizontal";
        const string F_ATTACK_VERTICAL = "attackVertical";


        SpriteRenderer playerSpriteRenderer;

        [SerializeField] Animator playerMotionAnimator;
        [SerializeField] Animator playerCombatAnimator;


        public Animator PlayerMotionAnimator => playerMotionAnimator;
        public Animator PlayerCombatAnimator => playerCombatAnimator;


        Vector2 currentDirection;


        void Awake()
        {
            playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            currentDirection = Vector2.down;
        }

        void Update()
        {
            CheckAndResetAttackAnimation();
        }

        void CheckAndResetAttackAnimation()
        {
            if (playerCombatAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !playerCombatAnimator.GetCurrentAnimatorStateInfo(0).IsName("EmptyState"))
            {
                playerCombatAnimator.Play("EmptyState");
            }
        }

        public void UpdateAnimation(Vector2 movementInput)
        {
            bool isMoving = movementInput.magnitude > 0;

            playerMotionAnimator.SetFloat(F_PLAYER_SPEED, isMoving ? 1f : 0f);

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

            playerCombatAnimator.SetFloat(F_ATTACK_HORIZONTAL, currentDirection.x);
            playerCombatAnimator.SetFloat(F_ATTACK_VERTICAL, currentDirection.y);
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
            playerCombatAnimator.SetFloat(F_ATTACK_HORIZONTAL, currentDirection.x);
            playerCombatAnimator.SetFloat(F_ATTACK_VERTICAL, currentDirection.y);

            playerCombatAnimator.CrossFadeInFixedTime("PlayerAttackBlendTree", 0.01f);
        }
    }
}
