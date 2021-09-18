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
    }

    public class SpawnState : AlienState
    {
        public SpawnState(Alien alien) : base(alien)
        {
            alien.StopNav();
            alien.SetRunSpeed();
            Debug.Log("Alien is Spawn State");
        }
        public override State Update()
        {
            // Hunt
            GameObject victim;
            if (alien.FindClosestReachableTarget(out victim)
                && alien.reactionResolver.isGoingHunting())
            {
                return new ChooseVictim(alien);
            }
            // Brake door
            else
            {
                GameObject closestDoor;
                if (alien.FindClosestReachableClosedDoor(out closestDoor)
                    && alien.reactionResolver.isGoingBrakeDoor())
                {
                    Door door = closestDoor.GetComponent<Door>();
                    GameObject approach = door.GetClosestApproach(
                        alien.transform.position, alien.navMeshAgent);
                    if (approach)
                    {
                        return new GoToBreakState(alien, closestDoor, approach.transform.position);
                    }
                }
            }
            return new RoamState(alien);
        }
    }

    public class RoamState : AlienState
    {
        float startTime;
        float roamTime;
        GameObject roamPoint;
        public RoamState (Alien alien) : base(alien) 
        {
            startTime = Time.time;
            roamTime = alien.reactionResolver.roamTime();
            alien.SetRunSpeed();
            alien.StartNav();
            if(Find.OneOfNReachableClosest(
                5, 
                alien.transform.position, 
                alien.navMeshAgent, 
                alien.GetRoamPoints(),
                out roamPoint))
            {
                alien.Destination = roamPoint.transform.position;
            }
            Debug.Log("Alien begin roaming");
        }
        public override State Update()
        {
            if (Time.time - startTime > roamTime)
            {
                return new SpawnState(alien);
            }
            if (alien.isExposed() && alien.reactionResolver.isGoingStalking())
            {
                float stalkTime = alien.reactionResolver.stalkTime();
                return new StalkState(alien, alien.GetTarget().transform, stalkTime);
            }
            return this;
        }
    }

    public class ChooseVictim : AlienState
    {
        GameObject victim;
        public ChooseVictim(Alien alien) : base(alien) 
        {
            Debug.Log("Alien choosing victim");
        }
        public override State Update()
        {
            if (alien.FindClosestReachableTarget(out victim))
            {
                return new HuntState(alien, victim);
            }
            return new IdleState(alien);
        }
    }
    public class HuntState : AlienState
    {
        GameObject victim;
        RaycastHit hit;
        public HuntState(Alien alien, GameObject _victim) : base(alien)
        {
            alien.StartNav();
            Debug.Log("Alien start Hunting");
            alien.SetRunSpeed();
            alien.Destination = _victim.transform.position;
            alien.Target = _victim;
            victim = _victim;
        }
        public override State Update()
        {
            if (!alien.isPathComplete())
            {
                return new SpawnState(alien);
            }
            if (alien.navMeshAgent.remainingDistance <= 1e-6)
            {
                alien.Destination = victim.transform.position;
            }
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
                return new JumpAttackState(alien, alien.Target.transform.position);
            }
            return this;
        }
    }
    public class JumpAttackState : AlienState
    {
        //RaycastHit hit;
        public JumpAttackState(Alien alien, Vector3 position) : base(alien)
        {
            alien.StartNav();
            Debug.Log("Alien jumps!");
            alien.SetJumpSpeed();
            alien.Destination = position;
        }
        public override State Update()
        {
            return this;
        }
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
    }

    public class EscapeState : AlienState
    {
        GameObject spawner;
        bool isSpawner;

        GameObject doorObj;
        GameObject approach;
        Door door;
        public EscapeState(Alien alien) : base(alien)
        {
            alien.StartNav();
            Debug.Log("Alien escaping");
            alien.SetEscapeSpeed();
            isSpawner = alien.FindClosestReachableSpawner(out spawner);
            if (isSpawner)
            {
                alien.Destination = spawner.transform.position;
                alien.EscapeSpawner = spawner;
            }
        }
        public override State Update()
        {
            if (!isSpawner)
            {
                // no accessible spawner
                if (alien.FindClosestReachableClosedDoor(out doorObj))
                {
                    door = doorObj.GetComponent<Door>();
                    approach = door.GetClosestApproach(alien.transform.position, alien.navMeshAgent);
                    return new GoToBreakState(alien, doorObj, approach.transform.position);
                }
            }
            if (!alien.isPathComplete())
            {
                // spawner isn't accessible anymore
                // try to find new one
                return new EscapeState(alien);
            }
            return this;
        }
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
                    return new ChooseVictim(alien);
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
    }
    public class GoToBreakState : AlienState
    {
        public GoToBreakState(Alien alien, GameObject obj, Vector3 target)
            : base(alien)
        {
            alien.SetRunSpeed();
            alien.Destination = target;
            alien.StartNav();
            alien.ToBreak = obj;
            Debug.Log("Alien going to break the door");
        }
        public override State Update()
        {
            return this;
        }
    }

    public class BreakState : AlienState
    {
        public BreakState(Alien alien, GameObject _obj)
            : base(alien)
        {
            alien.StopNav();
            Breakable item = _obj.GetComponentInParent<Breakable>();
            item.Break(alien.gameObject);
            alien.ToBreak = null;
            Debug.Log("Alien broke the door");
        }
        public override State Update()
        {
            return new EscapeState(alien);
        }
    }

    public class ReactionResolver
    {
        const float STALK_CHANSE = .7f;
        const float HUNT_CHANSE = 0.2f;
        const float BRAKE_CHANSE = 0.5f;
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
        public bool isGoingBrakeDoor()
        {
            return Utils.Rand() - alien.angryLevel < BRAKE_CHANSE;
        }
        public float prepareJumpLenght()
        {
            return Random.Range(.5f, 4f);
        }
        public float stalkTime()
        {
            return Random.Range(3, 10);
        }
        public float roamTime()
        {
            return Random.Range(3, 10);
        }
    }
}