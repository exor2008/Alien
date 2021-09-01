using UnityEngine;

public abstract class State
{
    protected Alien alien;
    public State(Alien _alien)
    {
        alien = _alien;
    }
    public abstract State Update();
    public abstract void FixedUpdate();
    public abstract void LateUpdate();

}

public class IdleState: State
{
    public IdleState(Alien alien) : base(alien) 
    {
        Debug.Log("Alien became Idle");
    }
    public override State Update()
    {
        return this;
    }
    public override void FixedUpdate() { }
    public override void LateUpdate() { }
}

public class HuntState : State
{
    RaycastHit hit;
    public HuntState(Alien alien) : base(alien) 
    {
        Debug.Log("Alien start Hunting");
        alien.SetRunSpeed();
    }
    public override State Update()
    {
        alien.MoveToClosestTarget();
        if ((alien.isTargetVisible(out hit)) && (hit.distance <= alien.jumpDistance))
        {
            return new JumpAttackState(alien);
        }
        return this;
    }
    public override void FixedUpdate() { }
    public override void LateUpdate() { }
}

public class JumpAttackState : State
{
    RaycastHit hit;
    public JumpAttackState(Alien alien) : base(alien) 
    {
        Debug.Log("Alien jumps!");
        alien.SetJumpSpeed();
    }
    public override State Update()
    {
        alien.MoveToClosestTarget();
        if ((alien.isTargetVisible(out hit)) && (hit.distance <= alien.killDistance))
        {
            Unit operative = hit.collider.GetComponent<Unit>();
            return new KillState(alien, operative);
        }
        return this;
    }
    public override void FixedUpdate() { }
    public override void LateUpdate() { }
}

public class KillState : State
{
    RaycastHit hit;
    Unit operative;
    public KillState(Alien alien, Unit _operative) : base(alien)
    {
        Debug.Log("Alien killed operative");
        operative = _operative;
    }
    public override State Update()
    {
        operative.Die();
        return new EscapeState(alien);
    }
    public override void FixedUpdate() { }
    public override void LateUpdate() { }
}

public class EscapeState : State
{
    GameObject spawner;
    bool isSpawner;
    float distToSpawner;
    public EscapeState(Alien alien) : base(alien) 
    {
        Debug.Log("Alien escaping");
        alien.SetRunSpeed();
        isSpawner = alien.FindClosestSpawner(out spawner);
    }
    public override State Update()
    {
        if (isSpawner)
        {
            alien.MoveToClosestSpawner();
            distToSpawner = Vector3.Distance(alien.transform.position, spawner.transform.position);
            if (distToSpawner < 0.6)
            {
                return new HideState(alien);
            }
            return this;
        }
        else
        {
            return new IdleState(alien);
        }
    }
    public override void FixedUpdate() { }
    public override void LateUpdate() { }
}

public class HideState : State
{
    public HideState(Alien alien) : base(alien) { }
    public override State Update()
    {
        alien.Hide();
        return new IdleState(alien);
    }
    public override void FixedUpdate() { }
    public override void LateUpdate() { }
}