using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltraMan.Infrastructure
{
    public class FsmState
    {
        public string name;
        protected FsmStateMachineBehavior stateMachine;

        public FsmState(string name, FsmStateMachineBehavior stateMachine)
        {
            this.name = name;
            this.stateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public virtual void UpdateLogic() { }
        public virtual void UpdatePhysics() { }
        public virtual void Exit() { }
    }
}