using UnityEngine;

namespace Game.AlienStatesNamespace
{
    public abstract class AlienState : State
    {
        protected Alien alien;
        public AlienState(Alien _alien)
        {
            alien = _alien;
        }
        public abstract State Update();
        public abstract void FixedUpdate();
        public abstract void LateUpdate();
    }

    public class IdleState : AlienState
    {
        public IdleState(Alien alien) : base(alien)
        {
            alien.StopNav();
            Debug.Log("Alien became Idle");
        }
        public override State Update()
        {
            return this;
        }
        public override void FixedUpdate() { }
        public override void LateUpdate() { }
    }

    public class HuntState : AlienState
    {
        RaycastHit hit;
        public HuntState(Alien alien) : base(alien)
        {
            alien.StartNav();
            Debug.Log("Alien start Hunting");
            alien.SetRunSpeed();
        }
        public override State Update()
        {
            alien.MoveToClosestReachableTarget();
            if (alien.isExposed() && alien.reactionResolver.isGoingStalking())
            {
                float stalkTime = alien.reactionResolver.stalkTime();
                return new StalkState(alien, alien.GetTarget().transform, stalkTime);
            }
            if ((alien.isTargetVisible(out hit)) && (hit.distance <= alien.jumpDistance))
            {
                return new PrepareJumpState(alien);
            }
            return this;
        }
        public override void FixedUpdate() { }
        public override void LateUpdate() { }
    }

    public class PrepareJumpState : AlienState
    {
        float lenght;
        float start;
        public PrepareJumpState(Alien alien) : base(alien)
        {
            alien.StopNav();
            Debug.Log("Alien prepares attack");
            start = Time.time;
            lenght = alien.reactionResolver.prepareJumpLenght();
        }
        public override State Update()
        {
            if (Time.time - start > lenght)
            {
                return new JumpAttackState(alien);
            }
            return this;
        }
        public override void FixedUpdate() { }
        public override void LateUpdate() { }
    }
    public class JumpAttackState : AlienState
    {
        RaycastHit hit;
        public JumpAttackState(Alien alien) : base(alien)
        {
            alien.StartNav();
            Debug.Log("Alien jumps!");
            alien.SetJumpSpeed();
        }
        public override State Update()
        {
            alien.MoveToClosestReachableTarget();
            if ((alien.isTargetVisible(out hit)) && (hit.distance <= alien.killDistance))
            {
                Operative operative = hit.collider.GetComponent<Operative>();
                return new KillState(alien, operative);
            }
            return this;
        }
        public override void FixedUpdate() { }
        public override void LateUpdate() { }
    }

    public class KillState : AlienState
    {
        Operative operative;
        public KillState(Alien alien, Operative _operative) : base(alien)
        {
            alien.StopNav();
            alien.Chill();
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

    public class EscapeState : AlienState
    {
        GameObject spawner;
        bool isSpawner;
        float distToSpawner;
        public EscapeState(Alien alien) : base(alien)
        {
            alien.StartNav();
            Debug.Log("Alien escaping");
            alien.SetEscapeSpeed();
            isSpawner = alien.FindClosestReachableSpawner(out spawner);
        }
        public override State Update()
        {
            if (isSpawner)
            {
                alien.MoveToClosestReachableSpawner();
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

    public class HideState : AlienState
    {
        public HideState(Alien alien) : base(alien) { }
        public override State Update()
        {
            alien.StartNav();
            Debug.Log("Alien hides");
            alien.Hide();
            return new IdleState(alien);
        }
        public override void FixedUpdate() { }
        public override void LateUpdate() { }
    }

    public class StalkState : AlienState
    {
        Transform target;
        float start;
        float lenght;
        public StalkState(Alien alien, Transform _target, float _lenght) : base(alien)
        {
            alien.StopNav();
            alien.Angry();
            Debug.Log("Alien stalking");
            target = _target;
            start = Time.time;
            lenght = _lenght;
        }
        public override State Update()
        {
            alien.transform.LookAt(target);
            if (Time.time - start > lenght)
            {
                if (alien.reactionResolver.isGoingHunting())
                {
                    return new HuntState(alien);
                }
                else
                {
                    return new StalkState(
                        alien,
                        alien.GetTarget().transform,
                        alien.reactionResolver.stalkTime());
                }
            }
            return this;
        }
        public override void FixedUpdate() { }
        public override void LateUpdate() { }
    }

    public class ReactionResolver
    {
        const float STALK_CHANSE = .7f;
        const float HUNT_CHANSE = 0.2f;
        Alien alien;
        public ReactionResolver(Alien _alien)
        {
            alien = _alien;
        }

        public bool isGoingStalking()
        {
            return Utils.Rand() + alien.angryLevel < STALK_CHANSE;
        }
        public bool isGoingHunting()
        {
            return Utils.Rand() - alien.angryLevel < HUNT_CHANSE;
        }
        public float prepareJumpLenght()
        {
            return Random.Range(.5f, 4f);
        }
        public float stalkTime()
        {
            return Random.Range(3, 10);
        }
    }
}