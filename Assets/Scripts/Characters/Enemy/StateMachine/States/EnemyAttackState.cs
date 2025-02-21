using UnityEngine;


namespace JuanIsometric2D.StateMachine.Enemy
{
    public class EnemyAttackState : EnemyBaseState
    {
        float attackCooldown = 1f;
        float timeSinceLastAttack = 0f;

        public EnemyAttackState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log("Enemy entered Attack State");

            m_enemyStateMachine.EnemyAnimatorScript.PlayMovement();
            m_enemyStateMachine.EnemyAnimatorScript.SetAttackColor();

            timeSinceLastAttack = attackCooldown;
        }

        public override void Tick(float deltaTime)
        {
            if (!m_enemyStateMachine.IsCollidingWithPlayer)
            {
                m_enemyStateMachine.SwitchState(new EnemyChaseState(m_enemyStateMachine));
                return;
            }

            timeSinceLastAttack += deltaTime;

            if (timeSinceLastAttack >= attackCooldown)
            {
                PerformAttack();
                timeSinceLastAttack = 0f;
            }

            Vector2 directionToPlayer = (m_enemyStateMachine.PlayerTransform.position - m_enemyStateMachine.transform.position).normalized;
            m_enemyStateMachine.EnemyAnimatorScript.UpdateAnimation(directionToPlayer);
        }

        public override void Exit()
        {
        }

        void PerformAttack()
        {
            Debug.Log("Enemy attacking player!");
        }
    }
}