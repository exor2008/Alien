using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.DeadBodyStatesNamespace;

public class DeadBody : Unit
{
    void Start()
    {
        destination = transform.position;
        targetRotation = transform.forward;
        StopNav();
        stateManager = new StateManager(new IdleState(this));
    }

    void Update()
    {
        
    }
}
