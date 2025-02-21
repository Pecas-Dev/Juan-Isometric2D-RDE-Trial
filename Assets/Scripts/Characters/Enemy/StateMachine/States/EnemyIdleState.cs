using UnityEngine;


namespace JuanIsometric2D.StateMachine.Enemy
{
    public class EnemyIdleState : EnemyBaseState
    {
        float idleTimer;
        float idleDuration = 2f;

        public EnemyIdleState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log("Enemy entered Idle State");
            m_enemyStateMachine.EnemyAnimatorScript.PlayIdle();

            idleTimer = 0f;
        }

        public override void Tick(float deltaTime)
        {
            float distanceToPlayer = Vector2.Distance(m_enemyStateMachine.transform.position, m_enemyStateMachine.PlayerTransform.position);

            if (distanceToPlayer <= m_enemyStateMachine.DetectionRange)
            {
                m_enemyStateMachine.SwitchState(new EnemyChaseState(m_enemyStateMachine));
                return;
            }

            idleTimer += deltaTime;

            if (idleTimer >= idleDuration)
            {
                m_enemyStateMachine.SwitchState(new EnemyPatrolState(m_enemyStateMachine));
            }
        }

        public override void Exit()
        {
        }
    }
}