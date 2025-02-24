using JuanIsometric2D.StateMachine.Player;
using JuanIsometric2D.Animation.Enemy;
using JuanIsometric2D.Pathfinding;

using UnityEngine;
using UnityEngine.SceneManagement;


namespace JuanIsometric2D.StateMachine.Enemy
{
    public class EnemyStateMachine : StateMachine
    {
        public enum EnemyState
        {
            Idle,
            Patrol,
            Chase,
            Attack,
            ReturnToPatrol
        }

        readonly string sceneWithoutAudio = "Isometric2D_Task3";

        bool isAudioDisabled;


        [Header("Enemy State")]
        [SerializeField, ReadOnlyInspector] EnemyState enemyCurrentState;
        public EnemyState CurrentState => enemyCurrentState;


        [Header("Component References")]
        [SerializeField] Rigidbody2D enemyRigidbody2D;
        [SerializeField] CapsuleCollider2D enemyCapsuleCollider2D;
        [SerializeField] BoxCollider2D enemyBoxCollider2D;
        [SerializeField] Animator enemyAnimator;
        public Rigidbody2D EnemyRigidbody2D => enemyRigidbody2D;
        public CapsuleCollider2D EnemyCapsuleCollider2D => enemyCapsuleCollider2D;
        public Animator EnemyAnimator => enemyAnimator;


        [Header("Script References")]
        [SerializeField] EnemyAnimator enemyAnimatorScript;
        public EnemyAnimator EnemyAnimatorScript => enemyAnimatorScript;


        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 4f;
        [SerializeField] float detectionRange = 5f;
        [SerializeField] float attackRange = 1.5f;
        public float MoveSpeed => moveSpeed;
        public float DetectionRange => detectionRange;
        public float AttackRange => attackRange;


        [Header("Attack Settings")]
        [Range(1.0f, 20.0f)][SerializeField] float minAttackDamage = 5f;
        [Range(1.0f, 20.0f)][SerializeField] float maxAttackDamage = 12f;
        [SerializeField] float attackCooldown = 1f;
        public float AttackDamage => Random.Range(minAttackDamage, maxAttackDamage);
        public float AttackCooldown => attackCooldown;


        [Header("Patrol Settings")]
        [SerializeField] Transform[] patrolPoints;
        public Transform[] PatrolPoints => patrolPoints;


        [Header("Isometric Settings")]
        [SerializeField] float isometricAngle = 45f;
        public float IsometricAngle => isometricAngle;


        [Header("Audio Settings")]
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip[] hurtSounds;
        [SerializeField] AudioClip swooshSound;


        bool isCollidingWithPlayer;
        public bool IsCollidingWithPlayer => isCollidingWithPlayer;


        static PlayerStateMachine playerInstance;
        public Transform PlayerTransform => playerInstance?.transform;


        EnemyPathfinding enemyPathfinding;
        public EnemyPathfinding EnemyPathfinding => enemyPathfinding;


        void Awake()
        {
            FindPlayer();

            enemyRigidbody2D = GetComponent<Rigidbody2D>();
            enemyCapsuleCollider2D = GetComponent<CapsuleCollider2D>();
            enemyBoxCollider2D = GetComponent<BoxCollider2D>();
            enemyAnimatorScript = GetComponent<EnemyAnimator>();
            enemyPathfinding = GetComponent<EnemyPathfinding>();
            enemyAnimator = GetComponentInChildren<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        void Start()
        {
            isAudioDisabled = sceneWithoutAudio.Contains(SceneManager.GetActiveScene().name);

            LogNullReferenceErrors();
            SwitchState(new EnemyIdleState(this));
        }

        void FindPlayer()
        {
            if (playerInstance == null)
            {
                playerInstance = FindFirstObjectByType<PlayerStateMachine>();

                if (playerInstance == null)
                {
                    Debug.LogError("No PlayerStateMachine found in scene!");
                }
            }
        }

        void LogNullReferenceErrors()
        {
            if (enemyRigidbody2D == null)
            {
                Debug.LogError("Rigidbody2D (" + gameObject.name + ") - Reference is missing!");
            }
            if (enemyCapsuleCollider2D == null)
            {
                Debug.LogError("BoxCollider2D (" + gameObject.name + ") - Reference is missing!");
            }
            if (enemyBoxCollider2D == null)
            {
                Debug.LogError("CapsuleCollider2D (" + gameObject.name + ") - Reference is missing!");
            }
            if (enemyAnimatorScript == null)
            {
                Debug.LogError("EnemyAnimator script (" + gameObject.name + ") - Reference is missing!");
            }
            if (enemyPathfinding == null)
            {
                Debug.LogError("EnemyPathfinding script (" + gameObject.name + ") - Reference is missing!");
            }
            if (enemyAnimator == null)
            {
                Debug.LogError("Animator (" + gameObject.name + ") - Reference is missing!");
            }
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                Debug.LogError("No patrol points assigned!");
            }

            if (!isAudioDisabled)
            {
                if (audioSource == null)
                {
                    Debug.LogError("AudioSource (" + gameObject.name + ") - Reference is missing!");
                }
                if (hurtSounds == null || hurtSounds.Length == 0)
                {
                    Debug.LogError("Hurt Sounds (" + gameObject.name + ") - References are missing!");
                }
                if (swooshSound == null)
                {
                    Debug.LogError("Swoosh Sound (" + gameObject.name + ") - Reference is missing!");
                }
            }
        }

        public void PlayHurtSound()
        {
            if (!isAudioDisabled && audioSource != null && hurtSounds != null && hurtSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, hurtSounds.Length);

                audioSource.clip = hurtSounds[randomIndex];
                audioSource.Play();
            }
        }

        public void PlaySwooshSound()
        {
            if (!isAudioDisabled && audioSource != null && swooshSound != null)
            {
                audioSource.clip = swooshSound;
                audioSource.Play();
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                isCollidingWithPlayer = true;
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                isCollidingWithPlayer = false;
            }
        }

        internal void SetCurrentState(EnemyState newState)
        {
            enemyCurrentState = newState;
        }
    }
}
