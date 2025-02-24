using UnityEngine;


namespace JuanIsometric2D.StateMachine
{
    public abstract class StateMachine : MonoBehaviour
    {
        protected State currentState;

        public void SwitchState(State newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState?.Enter();
        }

        void Update()
        {
            currentState?.Tick(Time.deltaTime);
        }
    }
}
