using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager
{
    private State currentState;

    public StateManager(State _currentState)
    {
        currentState = _currentState;
    }

    public void Updtae()
    {
        State nextState = currentState?.Update();

        if (nextState != null)
        {
            SwitchState(nextState);
        }
    }

    public void SwitchState(State newState)
    {
        currentState = newState;
    }

    public State GetCurrentState()
    {
        return currentState;
    }
}