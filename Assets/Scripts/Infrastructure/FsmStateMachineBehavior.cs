using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace UltraMan.Infrastructure
{
    public class FsmStateMachineBehavior : NetworkBehaviour
    {
        FsmState currentState;

        void Update()
        {
            if (currentState != null)
                currentState.UpdateLogic();
        }

        void LateUpdate()
        {
            if (currentState != null)
                currentState.UpdatePhysics();
        }

        public void ChangeState(FsmState newState)
        {
            currentState.Exit();

            currentState = newState;
            currentState.Enter();
        }

        protected void SetInitialState(FsmState state)
        {
            currentState = state;
            if (currentState != null)
                currentState.Enter();
        }
    }
}
