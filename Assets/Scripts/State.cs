using UnityEngine;

public interface State
{
    public State Update();
    public void FixedUpdate();
    public void LateUpdate();

}