using UnityEngine;
using System.Linq;


namespace JuanIsometric2D.StateMachine.Enemy
{
    public class EnemyPatrolState : EnemyBaseState
    {
        Transform currentPatrolPoint;
        Transform[] randomizedPatrolPoints;


        int currentPatrolIndex;


        float arrivalThreshold = 0.5f;


        public EnemyPatrolState(EnemyStateMachine enemyStateMachine, int startIndex = -1, Transform[] existingPatrolPoints = null) : base(enemyStateMachine)
        {
            if (existingPatrolPoints != null)
            {
                randomizedPatrolPoints = existingPatrolPoints;
                currentPatrolIndex = startIndex;
            }
        }

        public override void Enter()
        {
            Debug.Log("Enemy entered Patrol State");
            m_enemyStateMachine.SetCurrentState(EnemyStateMachine.EnemyState.Patrol);

            m_enemyStateMachine.EnemyAnimatorScript.PlayMovement();

            if (randomizedPatrolPoints == null || randomizedPatrolPoints.Length == 0)
            {
                randomizedPatrolPoints = m_enemyStateMachine.PatrolPoints.OrderBy(x => Random.value).ToArray();
                currentPatrolIndex = GetNearestPatrolPointIndex();
            }

            currentPatrolPoint = randomizedPatrolPoints[currentPatrolIndex];
        }

        public override void Tick(float deltaTime)
        {
            float distanceToPlayer = Vector2.Distance(m_enemyStateMachine.transform.position, m_enemyStateMachine.PlayerTransform.position);

            if (distanceToPlayer <= m_enemyStateMachine.DetectionRange)
            {
                m_enemyStateMachine.SwitchState(new EnemyChaseState(m_enemyStateMachine));
                return;
            }

            float distanceToPatrolPoint = Vector2.Distance(m_enemyStateMachine.transform.position, currentPatrolPoint.position);

            if (distanceToPatrolPoint < arrivalThreshold)
            {
                int nextPatrolIndex = (currentPatrolIndex + 1) % randomizedPatrolPoints.Length;

                m_enemyStateMachine.SwitchState(new EnemyIdleState(m_enemyStateMachine, true, nextPatrolIndex, randomizedPatrolPoints));
                return;
            }

            Vector2 movementDirection;

            if (m_enemyStateMachine.EnemyPathfinding != null)
            {
                movementDirection = m_enemyStateMachine.EnemyPathfinding.CalculateMovementDirection(m_enemyStateMachine.transform.position, currentPatrolPoint.position, deltaTime, false);
            }
            else
            {
                movementDirection = (currentPatrolPoint.position - m_enemyStateMachine.transform.position).normalized;
            }

            Vector2 isometricDirection = RotateVectorByAngleEnemy(movementDirection, -m_enemyStateMachine.IsometricAngle);

            m_enemyStateMachine.EnemyRigidbody2D.linearVelocity = isometricDirection * m_enemyStateMachine.MoveSpeed;
            m_enemyStateMachine.EnemyAnimatorScript.UpdateAnimation(isometricDirection);
        }

        public override void Exit()
        {
            m_enemyStateMachine.EnemyRigidbody2D.linearVelocity = Vector2.zero;
        }

        int GetNearestPatrolPointIndex()
        {
            float nearestDistance = float.MaxValue;

            int nearestIndex = 0;

            for (int i = 0; i < randomizedPatrolPoints.Length; i++)
            {
                float distance = Vector2.Distance(m_enemyStateMachine.transform.position, randomizedPatrolPoints[i].position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }
    }
}
