using UnityEngine;


namespace JuanIsometric2D.StateMachine.Player
{
    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log("Player is in Idle State");

            m_playerStateMachine.PlayerAnimatorScript.PlayIdle();

            m_playerStateMachine.PlayerAnimatorScript.UpdateAnimation(Vector2.zero);
        }

        public override void Tick(float deltaTime)
        {
            Vector2 input = m_playerStateMachine.PlayerGameInputSO.MovementInput;

            if (input != Vector2.zero)
            {
                m_playerStateMachine.SwitchState(new PlayerMovementState(m_playerStateMachine));
            }
        }

        public override void Exit()
        {
        }
    }
}
