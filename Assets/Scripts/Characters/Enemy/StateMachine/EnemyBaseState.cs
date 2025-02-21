using UnityEngine;


namespace JuanIsometric2D.StateMachine.Enemy
{
    public abstract class EnemyBaseState : State
    {
        protected EnemyStateMachine m_enemyStateMachine;

        public EnemyBaseState(EnemyStateMachine enemyStateMachine)
        {
            this.m_enemyStateMachine = enemyStateMachine;
        }

        protected Vector2 RotateVectorByAngleEnemy(Vector2 input, float angleDegrees)
        {
            float rad = angleDegrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            float rotatedX = input.x * cos - input.y * sin;
            float rotatedY = input.x * sin + input.y * cos;

            return new Vector2(rotatedX, rotatedY);
        }
    }
}
