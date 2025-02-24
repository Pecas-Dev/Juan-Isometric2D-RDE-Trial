using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace JuanIsometric2D.Combat
{
    public class EnemyDodgeAbility : MonoBehaviour
    {
        [Header("Dodge Settings")]
        [SerializeField] float dodgeDetectionRadius = 3f;
        [SerializeField] float dodgeSpeed = 12f;
        [SerializeField] float dodgeDuration = 0.3f;
        [SerializeField] float cooldownDuration = 3f;

        [SerializeField] LayerMask projectileLayer;
        [SerializeField] LayerMask obstacleLayer;


        [Header("Detection Settings")]
        [SerializeField] int numberOfDodgeDirections = 8;
        [SerializeField] float dodgeDirectionCheckDistance = 1.5f;


        [Header("UI Settings")]
        [SerializeField] Image cooldownFillImage;
        [SerializeField] CanvasGroup cooldownCanvasGroup;

        [SerializeField] float uiFadeDelay = 1f;
        [SerializeField] float uiFadeSpeed = 2f;


        bool canDodge = true;
        bool isDodging = false;


        Color defaultColor;


        Rigidbody2D enemyRigidbody;
        SpriteRenderer enemySprite;


        Coroutine fadeCoroutine;


        void Awake()
        {
            enemyRigidbody = GetComponent<Rigidbody2D>();
            enemySprite = GetComponentInChildren<SpriteRenderer>();

            defaultColor = enemySprite.color;

            if (cooldownCanvasGroup != null)
            {
                cooldownCanvasGroup.alpha = 0f;
            }
            if (cooldownFillImage != null)
            {
                cooldownFillImage.fillAmount = 1f;
            }
        }

        void Update()
        {
            if (!canDodge || isDodging)
            {
                return;
            }

            var enemyStateMachine = GetComponent<StateMachine.Enemy.EnemyStateMachine>();

            if (enemyStateMachine.CurrentState != StateMachine.Enemy.EnemyStateMachine.EnemyState.Chase)
            {
                return;
            }

            CheckForProjectiles();
        }

        void CheckForProjectiles()
        {
            Collider2D[] projectiles = Physics2D.OverlapCircleAll(transform.position, dodgeDetectionRadius, projectileLayer);

            foreach (Collider2D projectile in projectiles)
            {
                Vector2 projectileDirection = (projectile.transform.position - transform.position).normalized;
                Vector2 projectileVelocity = projectile.GetComponent<Rigidbody2D>().linearVelocity.normalized;

                float dotProduct = Vector2.Dot(projectileDirection, projectileVelocity);

                if (dotProduct < -0.5f)
                {
                    TryDodge(projectileDirection);
                    break;
                }
            }
        }

        void TryDodge(Vector2 projectileDirection)
        {
            Vector2 bestDodgeDirection = Vector2.zero;

            float bestSafety = -1f;

            for (int i = 0; i < numberOfDodgeDirections; i++)
            {
                float angle = (360f / numberOfDodgeDirections) * i;

                Vector2 dodgeDirection = Quaternion.Euler(0, 0, angle) * Vector2.right;

                float directionSimilarity = Vector2.Dot(dodgeDirection, projectileDirection);

                if (directionSimilarity > 0.5f)
                {
                    continue;
                }

                RaycastHit2D obstacleHit = Physics2D.Raycast(transform.position, dodgeDirection, dodgeDirectionCheckDistance, obstacleLayer);

                if (obstacleHit.collider == null)
                {
                    float safety = 1f - Mathf.Abs(directionSimilarity);

                    if (safety > bestSafety)
                    {
                        bestSafety = safety;
                        bestDodgeDirection = dodgeDirection;
                    }
                }
            }

            if (bestSafety >= 0)
            {
                StartCoroutine(PerformDodge(bestDodgeDirection));
            }
        }

        IEnumerator PerformDodge(Vector2 dodgeDirection)
        {
            isDodging = true;
            canDodge = false;
            enemySprite.color = Color.green;

            float originalSpeed = enemyRigidbody.linearVelocity.magnitude;

            Vector2 originalVelocity = enemyRigidbody.linearVelocity;

            ShowCooldownUI();

            float elapsedTime = 0f;

            while (elapsedTime < dodgeDuration)
            {
                enemyRigidbody.linearVelocity = dodgeDirection * dodgeSpeed;

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            enemyRigidbody.linearVelocity = originalVelocity.normalized * originalSpeed;

            enemySprite.color = defaultColor;
            isDodging = false;

            float cooldownElapsed = 0f;

            while (cooldownElapsed < cooldownDuration)
            {
                cooldownElapsed += Time.deltaTime;

                if (cooldownFillImage != null)
                {
                    cooldownFillImage.fillAmount = cooldownElapsed / cooldownDuration;
                }

                yield return null;
            }

            StartFadeOut();

            canDodge = true;
        }

        void ShowCooldownUI()
        {
            if (cooldownCanvasGroup != null)
            {
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }

                cooldownCanvasGroup.alpha = 1f;

                if (cooldownFillImage != null)
                {
                    cooldownFillImage.fillAmount = 0f;
                }
            }
        }

        void StartFadeOut()
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeOutUI());
        }

        IEnumerator FadeOutUI()
        {
            if (cooldownCanvasGroup == null)
            {
                yield break;
            }

            yield return new WaitForSeconds(uiFadeDelay);

            float currentAlpha = cooldownCanvasGroup.alpha;

            while (currentAlpha > 0)
            {
                currentAlpha -= Time.deltaTime * uiFadeSpeed;
                cooldownCanvasGroup.alpha = currentAlpha;

                yield return null;
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, dodgeDetectionRadius);

            if (Application.isPlaying)
            {
                Gizmos.color = Color.cyan;

                for (int i = 0; i < numberOfDodgeDirections; i++)
                {
                    float angle = (360f / numberOfDodgeDirections) * i;

                    Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;

                    Gizmos.DrawRay(transform.position, direction * dodgeDirectionCheckDistance);
                }
            }
        }
    }
}