using JuanIsometric2D.VFX;

using UnityEngine;
using System.Collections;


namespace JuanIsometric2D.Combat
{
    public class Projectile : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField] Sprite projectileSprite;


        [Header("Movement Settings")]
        [SerializeField] float projectileSpeed = 8f;
        [SerializeField] float maxLifetime = 3f;


        [Header("Impact Settings")]
        [SerializeField] float impactEffectDuration = 0.5f;  
        [SerializeField] AudioSource projectileImpactAudio;
        [SerializeField] ParticleSystem impactParticles;    


        float projectileDamage;
        float currentLifetime;
        public float CurrentLifetime => currentLifetime;


        bool hasImpacted = false;


        Vector2 direction;


        Rigidbody2D projectileRigidbody;
        SpriteRenderer spriteRenderer;
        ProjectilePool projectilePool;


        void Awake()
        {
            projectileRigidbody = GetComponent<Rigidbody2D>();
            projectileImpactAudio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            projectilePool = GetComponentInParent<ProjectilePool>();
        }

        void OnEnable()
        {
            currentLifetime = 0f;

            hasImpacted = false;

            if (spriteRenderer && projectileSprite)
            {
                spriteRenderer.sprite = projectileSprite;
            }
        }

        void Update()
        {
            if (hasImpacted)
            {
                return;
            }

            currentLifetime += Time.deltaTime;

            if (currentLifetime >= maxLifetime)
            {
                HandleImpact(false);  
            }
        }

        void FixedUpdate()
        {
            if (!hasImpacted)
            {
                projectileRigidbody.linearVelocity = direction * projectileSpeed;
            }
        }

        public void Initialize(Vector2 direction, float damage)
        {
            this.direction = direction.normalized;
            this.projectileDamage = damage;

            currentLifetime = 0f;

            hasImpacted = false;

            if (spriteRenderer && projectileSprite)
            {
                spriteRenderer.sprite = projectileSprite;
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (hasImpacted) 
            {
                return;
            }

            if (other.CompareTag("Obstacle"))
            {
                HandleImpact(true);
            }
            else if (other.CompareTag("Enemy"))
            {
                var enemyHealth = other.GetComponent<HealthSystem>();

                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(projectileDamage);

                    var enemySprite = other.GetComponentInChildren<SpriteRenderer>();

                    if (enemySprite != null)
                    {
                        if (enemyHealth.gameObject.activeInHierarchy)
                        {
                            enemyHealth.StartCoroutine(VisualEffects.FlickerSprite(enemySprite));
                        }
                    }
                }

                HandleImpact(true);
            }
        }

        void HandleImpact(bool playEffects)
        {
            if (hasImpacted)
            {
                return;
            }

            hasImpacted = true;

            projectileRigidbody.linearVelocity = Vector2.zero;

            if (spriteRenderer)
            {
                spriteRenderer.sprite = null;
            }

            if (playEffects)
            {
                if (projectileImpactAudio != null)
                {
                    projectileImpactAudio.Play();
                }

                if (impactParticles != null)
                {
                    impactParticles.Play();
                }
            }

            StartCoroutine(DeactivateAfterEffects());
        }

        IEnumerator DeactivateAfterEffects()
        {
            yield return new WaitForSeconds(impactEffectDuration);

            if (projectilePool != null)
            {
                projectilePool.ReturnProjectile(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
