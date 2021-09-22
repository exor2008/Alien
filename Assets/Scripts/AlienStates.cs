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
            if (alien.reactionResolver.IsGoingHunting())
            {
                return new ChooseVictim(alien);
            }
            // Beak door
            else
            {
                GameObject closestDoor;
                if (alien.reactionResolver.IsGoingBrakeDoor() 
                    && alien.FindClosestReachableClosedDoor(out closestDoor)
                    )
                {
                    Door door = closestDoor.GetComponent<Door>();
                    GameObject approach;
                    
                    if (door.GetClosestApproach(alien.transform.position, alien.navAgent, out approach))
                    {
                        return new GoToBreakState(alien, closestDoor, approach.transform.position);
                    }
                }
            }
            return new RoamState(alien, alien.reactionResolver.RoamTime());
        }
    }

    public class RoamState : AlienState
    {
        float startTime;
        float roamTime;
        GameObject roamPoint;
        public RoamState (Alien alien, float _roamTime) : base(alien) 
        {
            startTime = Time.time;
            roamTime = _roamTime;
            //roamTime = alien.reactionResolver.roamTime();
            alien.SetRunSpeed();
            alien.StartNav();
            if(Find.OneOfNReachableClosest(
                5, 
                alien.transform.position, 
                alien.navAgent, 
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
            if (alien.isExposed() && alien.reactionResolver.IsGoingStalking())
            {
                float stalkTime = alien.reactionResolver.StalkTime();
                return new StalkState(alien, alien.Target.transform, stalkTime);
            }
            if(alien.navAgent.remainingDistance < 1e-6)
            {
                return new RoamState(alien, Time.time - startTime);
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
            if (alien.reactionResolver.IsHuntDeadBody() && alien.FindClosestReachableDeadBody(out victim))
            {
                return new GoToDeadBodyState(alien, victim);
            }
            else if(alien.FindClosestReachableTarget(out victim))
            {
                return new HuntState(alien, victim);
            }
            return new SpawnState(alien);
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
            if (alien.navAgent.remainingDistance <= 1e-6)
            {
                alien.Destination = victim.transform.position;
            }
            if (alien.isExposed() && alien.reactionResolver.IsGoingStalking())
            {
                float stalkTime = alien.reactionResolver.StalkTime();
                return new StalkState(alien, alien.Target.transform, stalkTime);
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
            lenght = alien.reactionResolver.PrepareJumpLenght();
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
            alien.Target = null;
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
                    door.GetClosestApproach(alien.transform.position, alien.navAgent, out approach);
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
            alien.EscapeSpawner = null;
            alien.DestroyCapturedDeadBody();
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
                if (alien.reactionResolver.IsGoingHunting())
                {
                    return new ChooseVictim(alien);
                }
                else
                {
                    return new StalkState(
                        alien,
                        alien.Target.transform,
                        alien.reactionResolver.StalkTime());
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

    public class GoToDeadBodyState : AlienState
    {
        GameObject deadBody;
        public GoToDeadBodyState(Alien alien, GameObject _deadBody)
            : base(alien)
        {
            alien.StartNav();
            deadBody = _deadBody;
            Debug.Log("Alien going to the dead body");
            alien.Target = deadBody;
            alien.Destination = deadBody.transform.position;
        }
        public override State Update()
        {
            return this;
        }
    }

    public class CarryDeadBodyState : AlienState
    {
        GameObject deadBody;
        public CarryDeadBodyState(Alien alien, GameObject _deadBody)
            : base(alien)
        {
            deadBody = _deadBody;
            deadBody.transform.SetParent(alien.transform);
            alien.Target = null;
            Debug.Log("Alien escaping with the dead body");
        }
        public override State Update()
        {
            return new EscapeState(alien);
        }
    }

    public class ReactionResolver
    {
        const float STALK_CHANSE = .5f;
        const float HUNT_CHANSE = 0.4f;
        const float BREAK_CHANSE = 0.3f;
        const float HUNT_DEADBODY_CHANSE = 0.5f;
        Alien alien;
        public ReactionResolver(Alien _alien)
        {
            alien = _alien;
        }

        public bool IsGoingStalking()
        {
            return Utils.Rand() + alien.angryLevel < STALK_CHANSE;
        }
        public bool IsGoingHunting()
        {
            return Utils.Rand() - alien.angryLevel < HUNT_CHANSE;
        }
        public bool IsGoingBrakeDoor()
        {
            return Utils.Rand() - alien.angryLevel < BREAK_CHANSE;
        }
        public bool IsHuntDeadBody()
        {
            return Utils.Rand() < HUNT_DEADBODY_CHANSE;
        }
        public float PrepareJumpLenght()
        {
            return Random.Range(.5f, 4f);
        }
        public float StalkTime()
        {
            return Random.Range(3, 10);
        }
        public float RoamTime()
        {
            return Random.Range(3, 10);
        }
    }
}