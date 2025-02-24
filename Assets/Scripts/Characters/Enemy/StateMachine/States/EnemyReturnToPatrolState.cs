using UnityEngine;


namespace JuanIsometric2D.StateMachine.Enemy
{
    public class EnemyReturnToPatrolState : EnemyBaseState
    {
        Transform targetPatrolPoint;


        float arrivalThreshold = 0.5f;
        float waitTimeAtPoint = 0.5f;
        float arrivalTimer = 0f;


        Vector2 lastValidDirection;


        bool isArriving = false;


        public EnemyReturnToPatrolState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log("Enemy returning to patrol route");
            m_enemyStateMachine.SetCurrentState(EnemyStateMachine.EnemyState.ReturnToPatrol);

            m_enemyStateMachine.EnemyAnimatorScript.PlayMovement();
            m_enemyStateMachine.EnemyAnimatorScript.SetDefaultColor();

            targetPatrolPoint = GetNearestPatrolPoint();

            arrivalTimer = 0f;
            isArriving = false;
            lastValidDirection = Vector2.zero;
        }

        public override void Tick(float deltaTime)
        {
            float distanceToPlayer = Vector2.Distance(m_enemyStateMachine.transform.position, m_enemyStateMachine.PlayerTransform.position);

            if (distanceToPlayer <= m_enemyStateMachine.DetectionRange)
            {
                m_enemyStateMachine.SwitchState(new EnemyChaseState(m_enemyStateMachine));
                return;
            }

            float distanceToPatrolPoint = Vector2.Distance(m_enemyStateMachine.transform.position, targetPatrolPoint.position);

            if (distanceToPatrolPoint < arrivalThreshold)
            {
                if (!isArriving)
                {
                    isArriving = true;
                    m_enemyStateMachine.EnemyRigidbody2D.linearVelocity = Vector2.zero;
                }

                arrivalTimer += deltaTime;

                if (arrivalTimer >= waitTimeAtPoint)
                {
                    m_enemyStateMachine.SwitchState(new EnemyIdleState(m_enemyStateMachine));
                    return;
                }

                m_enemyStateMachine.EnemyRigidbody2D.linearVelocity = Vector2.zero;
                m_enemyStateMachine.EnemyAnimatorScript.UpdateAnimation(Vector2.zero);

                return;
            }

            Vector2 movementDirection;

            if (m_enemyStateMachine.EnemyPathfinding != null)
            {
                movementDirection = m_enemyStateMachine.EnemyPathfinding.CalculateMovementDirection(m_enemyStateMachine.transform.position, targetPatrolPoint.position, deltaTime, false);

                if (movementDirection.magnitude < 0.1f)
                {
                    movementDirection = lastValidDirection;
                }
                else
                {
                    lastValidDirection = movementDirection;
                }
            }
            else
            {
                movementDirection = (targetPatrolPoint.position - m_enemyStateMachine.transform.position).normalized;
            }

            Vector2 isometricDirection = RotateVectorByAngleEnemy(movementDirection, -m_enemyStateMachine.IsometricAngle);

            float speedMultiplier = Mathf.Lerp(0.5f, 1f, Mathf.Clamp01(distanceToPatrolPoint / (arrivalThreshold * 2)));

            m_enemyStateMachine.EnemyRigidbody2D.linearVelocity = isometricDirection * m_enemyStateMachine.MoveSpeed * speedMultiplier;
            m_enemyStateMachine.EnemyAnimatorScript.UpdateAnimation(isometricDirection);
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