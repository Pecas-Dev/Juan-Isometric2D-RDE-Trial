using UnityEngine;


namespace JuanIsometric2D.Animation.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        const string F_PLAYER_SPEED = "playerSpeed";
        const string F_PLAYER_HORIZONTAL = "playerHorizontal";
        const string F_PLAYER_VERTICAL = "playerVertical";


        SpriteRenderer playerSpriteRenderer;
        Animator playerAnimator;

        void Awake()
        {
            playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            playerAnimator = GetComponentInChildren<Animator>();
        }

        public void UpdateAnimation(Vector2 movementInput)
        {
            bool isMoving = movementInput.magnitude > 0;

            playerAnimator.SetFloat(F_PLAYER_SPEED, isMoving ? 1f : 0f);

            if (isMoving)
            {
                playerAnimator.SetFloat(F_PLAYER_HORIZONTAL, movementInput.x);
                playerAnimator.SetFloat(F_PLAYER_VERTICAL, movementInput.y);
            }

            if(!isMoving)
            {
                playerAnimator.SetFloat(F_PLAYER_HORIZONTAL, 0.0f);
                playerAnimator.SetFloat(F_PLAYER_VERTICAL, 0.0f);
            }
        }

        public void PlayIdle()
        {
            playerAnimator.Play("PlayerIdle");
        }

        public void PlayMovement()
        {
            playerAnimator.CrossFadeInFixedTime("PlayerMovementBlendTree", 0.01f);
        }
    }
}
