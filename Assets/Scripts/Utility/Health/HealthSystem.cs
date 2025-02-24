using JuanIsometric2D.StateMachine.Player;
using JuanIsometric2D.StateMachine.Enemy;

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using JuanIsometric2D.VFX;


namespace JuanIsometric2D.Combat
{
    public class HealthSystem : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] float maxHealth = 100f;
        [SerializeField] float currentHealth;


        [Header("UI References")]
        [SerializeField] Image healthBarFill;
        [SerializeField] CanvasGroup healthBarCanvasGroup;


        [Header("UI Animation Settings")]
        [SerializeField] float fadeOutDelay = 3f;
        [SerializeField] float fadeSpeed = 2f;


        float lastHealthChangeTime;


        bool isFading;


        [Header("Player References")]
        [SerializeField] SpriteRenderer characterSprite;


        Coroutine fadeCoroutine;


        void Awake()
        {
            currentHealth = maxHealth;
            SetHealthBarAlpha(0f);
            characterSprite = GetComponentInChildren<SpriteRenderer>();
        }

        void Update()
        {
            if (!isFading && healthBarCanvasGroup.alpha > 0 && Time.time - lastHealthChangeTime > fadeOutDelay)
            {
                StartFadeOut();
            }
        }

        public void TakeDamage(float damage)
        {
            currentHealth = Mathf.Max(0f, currentHealth - damage);

            UpdateHealthBar();
            ShowHealthBar();

            lastHealthChangeTime = Time.time;

            if (characterSprite != null)
            {
                StartCoroutine(VisualEffects.FlickerSprite(characterSprite));
            }

            if (TryGetComponent<PlayerStateMachine>(out var playerStateMachine))
            {
                playerStateMachine.PlayHurtSound();
            }
            else if (TryGetComponent<EnemyStateMachine>(out var enemyStateMachine))
            {
                enemyStateMachine.PlayHurtSound();
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(float healAmount, bool playEffect = false)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);

            UpdateHealthBar();
            ShowHealthBar();

            lastHealthChangeTime = Time.time;

            if (playEffect && characterSprite != null)
            {
                StartCoroutine(VisualEffects.HealingEffect(characterSprite));
            }
        }


        void UpdateHealthBar()
        {
            if (healthBarFill != null)
            {
                healthBarFill.fillAmount = currentHealth / maxHealth;
            }
        }

        void ShowHealthBar()
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            SetHealthBarAlpha(1f);
            isFading = false;
        }

        void StartFadeOut()
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeOutHealthBar());
        }

        void SetHealthBarAlpha(float alpha)
        {
            if (healthBarCanvasGroup != null)
            {
                healthBarCanvasGroup.alpha = alpha;
            }
        }

        IEnumerator FadeOutHealthBar()
        {
            isFading = true;

            float currentAlpha = healthBarCanvasGroup.alpha;

            while (currentAlpha > 0)
            {
                currentAlpha -= Time.deltaTime * fadeSpeed;
                SetHealthBarAlpha(currentAlpha);

                yield return null;
            }

            isFading = false;
        }

        void Die()
        {
            Debug.Log($"{gameObject.name} has died!");

            gameObject.SetActive(false);
        }

        public float GetHealthPercentage()
        {
            return currentHealth / maxHealth;
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }

        public bool IsAlive()
        {
            return currentHealth > 0;
        }
    }
}