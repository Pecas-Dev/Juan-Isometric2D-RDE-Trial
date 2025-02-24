using UnityEngine;


namespace JuanIsometric2D.StateMachine.Player
{
    public class PlayerMovementState : PlayerBaseState
    {
        float currentSpeedMultiplier = 1f;


        public PlayerMovementState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log("Player is in Movement State");

            m_playerStateMachine.SetCurrentState(PlayerStateMachine.PlayerState.Movement);
            m_playerStateMachine.PlayerAnimatorScript.PlayMovement();

            currentSpeedMultiplier = m_playerStateMachine.CurrentSpeedMultiplier;
        }

        public override void Tick(float deltaTime)
        {
            Vector2 input = m_playerStateMachine.PlayerGameInputSO.MovementInput;

            m_playerStateMachine.HandleInventoryInput();

            if (input == Vector2.zero)
            {
                m_playerStateMachine.SwitchState(new PlayerIdleState(m_playerStateMachine));
                return;
            }

            MovePlayer(input, deltaTime);
            m_playerStateMachine.PlayerAnimatorScript.UpdateAnimation(input, currentSpeedMultiplier);
        }

        public override void Exit()
        {
            m_playerStateMachine.PlayerRigidbody2D.linearVelocity = Vector2.zero;
        }

        void MovePlayer(Vector2 input, float deltaTime)
        {
            Vector2 isometricDirection = RotateVectorByAngle(input, -m_playerStateMachine.IsometricAngle);
            Vector2 velocity = isometricDirection * m_playerStateMachine.MoveSpeed * currentSpeedMultiplier;

            m_playerStateMachine.PlayerRigidbody2D.linearVelocity = velocity;
        }

        Vector2 RotateVectorByAngle(Vector2 input, float angleDegrees)
        {
            float rad = angleDegrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            float rotatedX = input.x * cos - input.y * sin;
            float rotatedY = input.x * sin + input.y * cos;

            return new Vector2(rotatedX, rotatedY);
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            currentSpeedMultiplier = multiplier;
        }
    }
}
