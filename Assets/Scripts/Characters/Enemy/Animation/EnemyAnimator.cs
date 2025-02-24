using UnityEngine;


namespace JuanIsometric2D.Animation.Enemy
{
    public class EnemyAnimator : MonoBehaviour
    {
        const string F_ENEMY_HORIZONTAL = "enemyHorizontal";
        const string F_ENEMY_VERTICAL = "enemyVertical";
        const string F_ENEMY_SPEED = "enemySpeed";


        Color defaultColor;


        SpriteRenderer enemySpriteRenderer;
        Animator enemyAnimator;


        void Awake()
        {
            enemySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            enemyAnimator = GetComponentInChildren<Animator>();

            defaultColor = enemySpriteRenderer.color;
        }

        public void UpdateAnimation(Vector2 movementInput)
        {
            bool isMoving = movementInput.magnitude > 0;

            enemyAnimator.SetFloat(F_ENEMY_SPEED, isMoving ? 1f : 0f);

            if (isMoving)
            {
                enemyAnimator.SetFloat(F_ENEMY_HORIZONTAL, movementInput.x);
                enemyAnimator.SetFloat(F_ENEMY_VERTICAL, movementInput.y);
            }
            else
            {
                enemyAnimator.SetFloat(F_ENEMY_HORIZONTAL, 0.0f);
                enemyAnimator.SetFloat(F_ENEMY_VERTICAL, 0.0f);
            }
        }

        public void PlayIdle()
        {
            enemyAnimator.Play("Enemy_Idle");
            SetDefaultColor();
        }

        public void PlayMovement()
        {
            enemyAnimator.CrossFadeInFixedTime("EnemyMovementBlendTree", 0.01f);
            SetDefaultColor();
        }

        public void SetChaseColor()
        {
            enemySpriteRenderer.color = Color.blue;
        }

        public void SetAttackColor()
        {
            enemySpriteRenderer.color = Color.red;
        }

        public void SetDefaultColor()
        {
            enemySpriteRenderer.color = defaultColor;
        }
    }
}
