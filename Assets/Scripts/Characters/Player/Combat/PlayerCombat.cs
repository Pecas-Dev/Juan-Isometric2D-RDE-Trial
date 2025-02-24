using JuanIsometric2D.StateMachine.Player;
using JuanIsometric2D.Animation.Player;
using JuanIsometric2D.GameInputSystem;
using JuanIsometric2D.VFX;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace JuanIsometric2D.Combat
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Combat Settings")]
        [Range(1.0f, 20.0f)][SerializeField] float minAttackDamage = 8f;
        [Range(1.0f, 20.0f)][SerializeField] float maxAttackDamage = 15f;
        [SerializeField] float attackCooldown = 0.5f;
        [SerializeField] float attackRange = 1f;
        [SerializeField] float knockbackForce = 5f;


        [Header("Projectile Settings")]
        [SerializeField] GameObject projectilePrefab;
        [SerializeField] ProjectilePool projectilePool;
        [SerializeField] float projectileCooldown = 0.5f;


        [Header("References")]
        [SerializeField] GameInputSO playerGameInputSO;


        [Header("Slash Visual Settings")]
        [SerializeField] float slashOffset = 0.425f;
        [SerializeField] Transform playerSwordSlash;


        [Header("Hit Feedback Settings")]
        [SerializeField] float hitFlickerDuration = 0.5f;
        [SerializeField] float flickerInterval = 0.05f;


        float currentAttackCooldown;
        float currentProjectileCooldown;


        bool canShootProjectile = true;
        bool isProcessingProjectile = false;

        bool canAttack = true;
        bool isProcessingAttack = false;


        PlayerAnimator playerAnimatorScript;


        Vector2 lastAttackDirection;


        void Awake()
        {
            projectilePool = GetComponentInChildren<ProjectilePool>();
            playerAnimatorScript = GetComponent<PlayerAnimator>();

            currentAttackCooldown = 0f;
            currentProjectileCooldown = 0f;
        }

        void Update()
        {
            HandleAttackCooldown();
            HandleAttackInput();
            UpdateSlashPosition();
            HandleProjectileCooldown();
            HandleProjectileInput();
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
            GetComponent<PlayerStateMachine>()?.PlaySwooshSound();

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
                    HealthSystem enemyHealth = hit.GetComponent<HealthSystem>();

                    if (enemyRb != null)
                    {
                        enemyRb.AddForce(lastAttackDirection * knockbackForce, ForceMode2D.Impulse);
                    }

                    if (enemyHealth != null)
                    {
                        float damage = Random.Range(minAttackDamage, maxAttackDamage);
                        enemyHealth.TakeDamage(damage);
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
            return VisualEffects.FlickerSprite(sprite, hitFlickerDuration, flickerInterval);
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

        void HandleProjectileCooldown()
        {
            if (!canShootProjectile)
            {
                currentProjectileCooldown -= Time.deltaTime;

                if (currentProjectileCooldown <= 0f)
                {
                    canShootProjectile = true;
                    isProcessingProjectile = false;
                }
            }
        }

        void HandleProjectileInput()
        {
            if (playerGameInputSO.ProjectileShotPressed && canShootProjectile && !isProcessingProjectile)
            {
                isProcessingProjectile = true;

                ShootProjectile();
                playerGameInputSO.ResetProjectileShot();
            }
            else if (playerGameInputSO.ProjectileShotPressed)
            {
                playerGameInputSO.ResetProjectileShot();
            }
        }

        void ShootProjectile()
        {
            Vector2 shootDirection;

            if (playerGameInputSO.MovementInput.magnitude > 0.1f)
            {
                shootDirection = playerGameInputSO.MovementInput.normalized;
            }
            else
            {
                shootDirection = Vector2.down;
            }

            GameObject projectile = projectilePool.GetProjectile();

            if (projectile != null)
            {
                projectile.transform.position = transform.position;

                float damage = Random.Range(minAttackDamage, maxAttackDamage);

                Projectile projectileComponent = projectile.GetComponent<Projectile>();
                projectileComponent.Initialize(shootDirection, damage);
            }

            GetComponent<PlayerStateMachine>()?.PlayProjectileShotSound();

            canShootProjectile = false;
            currentProjectileCooldown = projectileCooldown;
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