using UnityEngine;
using System.Linq;


namespace JuanIsometric2D.StateMachine.Enemy
{
    public class EnemyPatrolState : EnemyBaseState
    {
        Transform currentPatrolPoint;
        Transform[] randomizedPatrolPoints;

        int currentPatrolIndex;

        float arrivalTimer;
        float waitTimeAtPoint = 2f;

        bool isWaitingAtPoint;

        public EnemyPatrolState(EnemyStateMachine enemyStateMachine) : base(enemyStateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log("Enemy entered Patrol State");
            m_enemyStateMachine.EnemyAnimatorScript.PlayMovement();

            if (randomizedPatrolPoints == null || randomizedPatrolPoints.Length == 0)
            {
                randomizedPatrolPoints = m_enemyStateMachine.PatrolPoints.OrderBy(x => Random.value).ToArray();
                currentPatrolIndex = GetNearestPatrolPointIndex();
            }

            currentPatrolPoint = randomizedPatrolPoints[currentPatrolIndex];

            arrivalTimer = 0f;

            isWaitingAtPoint = false;
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

            if (distanceToPatrolPoint < 0.1f)
            {
                if (!isWaitingAtPoint)
                {
                    isWaitingAtPoint = true;
                    m_enemyStateMachine.EnemyAnimatorScript.PlayIdle();
                }

                arrivalTimer += deltaTime;

                m_enemyStateMachine.EnemyRigidbody2D.linearVelocity = Vector2.zero;
                m_enemyStateMachine.EnemyAnimatorScript.UpdateAnimation(Vector2.zero);

                if (arrivalTimer >= waitTimeAtPoint)
                {
                    currentPatrolIndex = (currentPatrolIndex + 1) % randomizedPatrolPoints.Length;
                    currentPatrolPoint = randomizedPatrolPoints[currentPatrolIndex];

                    arrivalTimer = 0f;
                    isWaitingAtPoint = false;

                    m_enemyStateMachine.EnemyAnimatorScript.PlayMovement();
                }

                return;
            }

            if (isWaitingAtPoint)
            {
                isWaitingAtPoint = false;
                m_enemyStateMachine.EnemyAnimatorScript.PlayMovement();
            }

            Vector2 directionToPatrolPoint = (currentPatrolPoint.position - m_enemyStateMachine.transform.position).normalized;
            Vector2 isometricDirection = RotateVectorByAngleEnemy(directionToPatrolPoint, -m_enemyStateMachine.IsometricAngle);

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
