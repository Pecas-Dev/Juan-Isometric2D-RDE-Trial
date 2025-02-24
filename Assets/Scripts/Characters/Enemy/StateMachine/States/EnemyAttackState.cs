using JuanIsometric2D.Combat;

using UnityEngine;


namespace JuanIsometric2D.StateMachine.Enemy
{
    public class EnemyAttackState : EnemyBaseState
    {
        float timeSinceLastAttack;


        float exitBufferTime = 0.5f;
        float currentExitBuffer = 0f;


        bool isExiting = false;


        public EnemyAttackState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log("Enemy entered Attack State");
            m_enemyStateMachine.SetCurrentState(EnemyStateMachine.EnemyState.Attack);

            m_enemyStateMachine.EnemyAnimatorScript.PlayMovement();
            m_enemyStateMachine.EnemyAnimatorScript.SetAttackColor();

            PerformAttack();

            timeSinceLastAttack = 0f;

            //currentExitBuffer = 0f;
            //isExiting = false;

            m_enemyStateMachine.EnemyRigidbody2D.mass = 800f;
            m_enemyStateMachine.EnemyRigidbody2D.linearDamping = 10f;
        }

        public override void Tick(float deltaTime)
        {
            if (!m_enemyStateMachine.IsCollidingWithPlayer && !isExiting)
            {
                isExiting = true;
                currentExitBuffer = 0f;
            }

            if (isExiting)
            {
                currentExitBuffer += deltaTime;

                if (currentExitBuffer >= exitBufferTime)
                {
                    m_enemyStateMachine.SwitchState(new EnemyChaseState(m_enemyStateMachine));
                    return;
                }
            }

            timeSinceLastAttack += deltaTime;

            if (timeSinceLastAttack >= m_enemyStateMachine.AttackCooldown)
            {
                PerformAttack();
                timeSinceLastAttack = 0f;
            }

            Vector2 directionToPlayer = (m_enemyStateMachine.PlayerTransform.position - m_enemyStateMachine.transform.position).normalized;

            m_enemyStateMachine.EnemyAnimatorScript.UpdateAnimation(directionToPlayer);

            if (m_enemyStateMachine.EnemyRigidbody2D.linearVelocity.magnitude > 0.1f)
            {
                m_enemyStateMachine.EnemyRigidbody2D.linearVelocity *= 0.9f;
            }
            else if (!isExiting)
            {
                m_enemyStateMachine.EnemyRigidbody2D.linearVelocity = Vector2.zero;
            }
        }

        public override void Exit()
        {
            m_enemyStateMachine.EnemyRigidbody2D.mass = 1f;
            m_enemyStateMachine.EnemyRigidbody2D.linearDamping = 0f;
        }

        void PerformAttack()
        {
            if(m_enemyStateMachine.IsCollidingWithPlayer)
            {
                HealthSystem playerHealth = m_enemyStateMachine.PlayerTransform.GetComponent<HealthSystem>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(m_enemyStateMachine.AttackDamage);
                }

                m_enemyStateMachine.PlaySwooshSound();

                Debug.Log("Enemy performed attack!");
            }
        }
    }
}