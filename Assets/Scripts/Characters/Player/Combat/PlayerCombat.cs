using JuanIsometric2D.GameInputSystem;
using JuanIsometric2D.Animation.Player;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace JuanIsometric2D.Combat
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Combat Settings")]
        [SerializeField] float attackCooldown = 0.5f;
        [SerializeField] float attackRange = 1f;
        [SerializeField] float attackDamage = 10f;
        [SerializeField] float knockbackForce = 5f;


        [Header("References")]
        [SerializeField] GameInputSO playerGameInputSO;


        [Header("Slash Visual Settings")]
        [SerializeField] float slashOffset = 0.425f;
        [SerializeField] Transform playerSwordSlash;


        [Header("Hit Feedback Settings")]
        [SerializeField] float hitFlickerDuration = 0.5f;
        [SerializeField] float flickerInterval = 0.05f;


        PlayerAnimator playerAnimatorScript;


        float currentAttackCooldown;

        bool canAttack = true;
        bool isProcessingAttack = false;


        Vector2 lastAttackDirection;


        void Awake()
        {
            playerAnimatorScript = GetComponent<PlayerAnimator>();
            currentAttackCooldown = 0f;
        }

        void Update()
        {
            HandleAttackCooldown();
            HandleAttackInput();
            UpdateSlashPosition();
        }

        void HandleAttackCooldown()
        {
            if (!canAttack)
            {
                currentAttackCooldown -= Time.deltaTime;

                if (currentAttackCooldown <= 0f)
                {
                    canAttack = true;
                    isProcessingAttack = false;
                }
            }
        }

        void HandleAttackInput()
        {
            if (playerGameInputSO.AttackPressed && canAttack && !isProcessingAttack)
            {
                isProcessingAttack = true;

                PerformAttack();
                playerGameInputSO.ResetAttack();
            }
            else if (playerGameInputSO.AttackPressed)
            {
                playerGameInputSO.ResetAttack();
            }
        }

        void PerformAttack()
        {
            float horizontalDir = playerAnimatorScript.PlayerMotionAnimator.GetFloat("playerHorizontal");
            float verticalDir = playerAnimatorScript.PlayerMotionAnimator.GetFloat("playerVertical");

            if (horizontalDir == 0 && verticalDir == 0)
            {
                lastAttackDirection = Vector2.down;
            }
            else
            {
                lastAttackDirection = new Vector2(horizontalDir, verticalDir).normalized;
            }

            playerAnimatorScript.PlayAttack();
            PositionSlashVisual();

            canAttack = false;
            currentAttackCooldown = attackCooldown;

            CheckHitbox();
        }

        void CheckHitbox()
        {
            Vector2 attackPosition = (Vector2)transform.position + (lastAttackDirection * attackRange);
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, attackRange);

            HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Enemy") && !hitEnemies.Contains(hit.gameObject))
                {
                    hitEnemies.Add(hit.gameObject);
                    Debug.Log("Enemy Hit");

                    Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
                    SpriteRenderer enemySprite = hit.GetComponentInChildren<SpriteRenderer>();

                    if (enemyRb != null)
                    {
                        enemyRb.AddForce(lastAttackDirection * knockbackForce, ForceMode2D.Impulse);
                    }

                    if (enemySprite != null)
                    {
                        StartCoroutine(FlickerSprite(enemySprite));
                    }
                }
            }
        }

        IEnumerator FlickerSprite(SpriteRenderer sprite)
        {
            float elapsedTime = 0f;
            bool isVisible = true;

            while (elapsedTime < hitFlickerDuration)
            {
                sprite.enabled = isVisible;
                isVisible = !isVisible;
                elapsedTime += flickerInterval;

                yield return new WaitForSeconds(flickerInterval);
            }

            sprite.enabled = true;
        }

        void PositionSlashVisual()
        {
            if (playerSwordSlash != null)
            {
                Vector2 slashPosition = (Vector2)transform.position + (lastAttackDirection * slashOffset);
                playerSwordSlash.position = slashPosition;
            }
        }

        void UpdateSlashPosition()
        {
            if (!canAttack && playerSwordSlash != null)
            {
                Vector2 slashPosition = (Vector2)transform.position + (lastAttackDirection * slashOffset);
                playerSwordSlash.position = slashPosition;
            }
        }

        void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                float horizontalDir = playerAnimatorScript.PlayerMotionAnimator.GetFloat("playerHorizontal");
                float verticalDir = playerAnimatorScript.PlayerMotionAnimator.GetFloat("playerVertical");
                Vector2 attackDirection = new Vector2(horizontalDir, verticalDir).normalized;

                Vector2 attackPosition = (Vector2)transform.position + (attackDirection * attackRange);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(attackPosition, attackRange);

                Vector2 slashPosition = (Vector2)transform.position + (attackDirection * slashOffset);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(slashPosition, 0.1f);
            }
        }
    }
}