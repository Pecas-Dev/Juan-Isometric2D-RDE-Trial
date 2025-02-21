using UnityEngine;
using JuanIsometric2D.Animation.Enemy;


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

        [Header("Component References")]
        [SerializeField] Rigidbody2D enemyRigidbody2D;
        [SerializeField] CapsuleCollider2D enemyCapsuleCollider2D;
        [SerializeField] Animator enemyAnimator;

        public Rigidbody2D EnemyRigidbody2D => enemyRigidbody2D;
        public CapsuleCollider2D EnemyBoxCollider2D => enemyCapsuleCollider2D;
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


        [Header("Patrol Settings")]
        [SerializeField] Transform[] patrolPoints;
        public Transform[] PatrolPoints => patrolPoints;


        [Header("Target Reference")]
        [SerializeField] Transform playerTransform;
        public Transform PlayerTransform => playerTransform;


        [Header("Isometric Settings")]
        [SerializeField] float isometricAngle = 45f;
        public float IsometricAngle => isometricAngle;


        bool isCollidingWithPlayer;
        public bool IsCollidingWithPlayer => isCollidingWithPlayer;


        void Awake()
        {
            enemyRigidbody2D = GetComponent<Rigidbody2D>();
            enemyCapsuleCollider2D = GetComponent<CapsuleCollider2D>();
            enemyAnimatorScript = GetComponent<EnemyAnimator>();
            enemyAnimator = GetComponentInChildren<Animator>();
        }

        void Start()
        {
            LogNullReferenceErrors();
            SwitchState(new EnemyIdleState(this));
        }

        void LogNullReferenceErrors()
        {
            if (enemyRigidbody2D == null)
            {
                Debug.LogError("Rigidbody2D (Enemy gameObject) - Reference is missing!");
            }
            if (enemyCapsuleCollider2D == null)
            {
                Debug.LogError("BoxCollider2D (Enemy gameObject) - Reference is missing!");
            }
            if (enemyAnimatorScript == null)
            {
                Debug.LogError("EnemyAnimator script (Enemy gameObject) - Reference is missing!");
            }
            if (enemyAnimator == null)
            {
                Debug.LogError("Animator (Enemy gameObject) - Reference is missing!");
            }
            if (playerTransform == null)
            {
                Debug.LogError("Player Transform reference is missing!");
            }
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                Debug.LogError("No patrol points assigned!");
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                isCollidingWithPlayer = true;
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                isCollidingWithPlayer = false;
            }
        }
    }
}
