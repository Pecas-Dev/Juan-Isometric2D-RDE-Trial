using UnityEngine;


namespace JuanIsometric2D.StateMachine.Enemy
{
    public class EnemyReturnToPatrolState : EnemyBaseState
    {
        Transform targetPatrolPoint;

        public EnemyReturnToPatrolState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log("Enemy returning to patrol route");

            m_enemyStateMachine.EnemyAnimatorScript.PlayMovement();
            m_enemyStateMachine.EnemyAnimatorScript.SetDefaultColor();

            targetPatrolPoint = GetNearestPatrolPoint();
        }

        public override void Tick(float deltaTime)
        {
            float distanceToPlayer = Vector2.Distance(m_enemyStateMachine.transform.position, m_enemyStateMachine.PlayerTransform.position);

            if (distanceToPlayer <= m_enemyStateMachine.DetectionRange)
            {
                m_enemyStateMachine.SwitchState(new EnemyChaseState(m_enemyStateMachine));
                return;
            }

            Vector2 directionToPatrolPoint = (targetPatrolPoint.position - m_enemyStateMachine.transform.position).normalized;
            Vector2 isometricDirection = RotateVectorByAngleEnemy(directionToPatrolPoint, -m_enemyStateMachine.IsometricAngle);

            m_enemyStateMachine.EnemyRigidbody2D.linearVelocity = isometricDirection * m_enemyStateMachine.MoveSpeed;
            m_enemyStateMachine.EnemyAnimatorScript.UpdateAnimation(isometricDirection);

            float distanceToPatrolPoint = Vector2.Distance(m_enemyStateMachine.transform.position, targetPatrolPoint.position);

            if (distanceToPatrolPoint < 0.1f)
            {
                m_enemyStateMachine.SwitchState(new EnemyIdleState(m_enemyStateMachine));
            }
        }

        public override void Exit()
        {
            m_enemyStateMachine.EnemyRigidbody2D.linearVelocity = Vector2.zero;
        }

        Transform GetNearestPatrolPoint()
        {
            Transform nearestPoint = null;

            float nearestDistance = float.MaxValue;

            foreach (Transform patrolPoint in m_enemyStateMachine.PatrolPoints)
            {
                float distance = Vector2.Distance(m_enemyStateMachine.transform.position, patrolPoint.position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPoint = patrolPoint;
                }
            }

            return nearestPoint;
        }
    }
}