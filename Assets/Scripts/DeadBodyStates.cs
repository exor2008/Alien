using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DeadBodyStatesNamespace
{
    public class IdleState : State
    {
        DeadBody deadBody;
        public IdleState(DeadBody _deadBody)
        {
            deadBody = _deadBody;
        }

        public State Update()
        {
            return this;
        }
    }
}