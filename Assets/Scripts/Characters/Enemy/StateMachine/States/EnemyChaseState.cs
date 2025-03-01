using UnityEngine;


namespace JuanIsometric2D.StateMachine.Enemy
{
    public class EnemyChaseState : EnemyBaseState
    {
        public EnemyChaseState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log("Enemy entered Chase State");
            m_enemyStateMachine.SetCurrentState(EnemyStateMachine.EnemyState.Chase);

            m_enemyStateMachine.EnemyAnimatorScript.PlayMovement();
            m_enemyStateMachine.EnemyAnimatorScript.SetChaseColor();
        }

        public override void Tick(float deltaTime)
        {
            float distanceToPlayer = Vector2.Distance(m_enemyStateMachine.transform.position, m_enemyStateMachine.PlayerTransform.position);

            if (distanceToPlayer > m_enemyStateMachine.DetectionRange * 1.5f)
            {
                m_enemyStateMachine.SwitchState(new EnemyReturnToPatrolState(m_enemyStateMachine));
                return;
            }

            if (m_enemyStateMachine.IsCollidingWithPlayer)
            {
                m_enemyStateMachine.SwitchState(new EnemyAttackState(m_enemyStateMachine));
                return;
            }

            Vector2 movementDirection;

            if (m_enemyStateMachine.EnemyPathfinding != null)
            {
                movementDirection = m_enemyStateMachine.EnemyPathfinding.CalculateMovementDirection(m_enemyStateMachine.transform.position, m_enemyStateMachine.PlayerTransform.position, deltaTime, true);
            }
            else
            {
                movementDirection = (m_enemyStateMachine.PlayerTransform.position - m_enemyStateMachine.transform.position).normalized;
            }

            if (movementDirection.magnitude < 0.1f)
            {
                movementDirection = (m_enemyStateMachine.PlayerTransform.position - m_enemyStateMachine.transform.position).normalized;
            }

            Vector2 isometricDirection = RotateVectorByAngleEnemy(movementDirection, -m_enemyStateMachine.IsometricAngle);

            m_enemyStateMachine.EnemyRigidbody2D.linearVelocity = isometricDirection * m_enemyStateMachine.MoveSpeed;
            m_enemyStateMachine.EnemyAnimatorScript.UpdateAnimation(isometricDirection);
        }

        public override void Exit()
        {
            m_enemyStateMachine.EnemyRigidbody2D.linearVelocity = Vector2.zero;
        }
    }
}
