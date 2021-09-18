using UnityEngine;

namespace Game.OperativeStatesNamespace
{
    public abstract class OperativeState : State
    {
        protected Operative operative;
        public OperativeState(Operative _operative)
        {
            operative = _operative;
        }
        public abstract State Update();
    }

    public class IdleState : OperativeState
    {
        public IdleState(Operative operative) : base(operative)
        {
            Debug.Log(string.Format("{0} became Idle", operative.Name()));
        }
        public override State Update()
        {
            return this;
        }
    }

    public class GoToInteractState : OperativeState
    {
        public GoToInteractState(Operative operative, GameObject obj, Vector3 target)
            : base(operative)
        {
            operative.Destination = target;
            operative.StartNav();
            operative.SetObjectToInteract(obj);
            Debug.Log(string.Format("{0} going to interact", operative.Name()));
        }
        public override State Update()
        {
            return this;
        }
    }

    public class InteractState : OperativeState
    {
        public InteractState(Operative operative, GameObject _obj)
            : base(operative)
        {
            operative.StopNav();
            Interactable item = _obj.GetComponentInParent<Interactable>();
            item.Interact(operative.gameObject);
            operative.SetObjectToInteract(null);
        }
        public override State Update()
        {
            return new IdleState(operative);
        }
    }

    public class MoveState : OperativeState
    {
        public MoveState(Operative _operative) : base(_operative)
        {
            operative.StartNav();
        }
        public override State Update()
        {
            if (operative.IsDestinationReached() && operative.isRotateNeeded)
            {
                return new RotateState(operative);
            }
            return this;
        }
    }

    public class RotateState : OperativeState
    {
        Quaternion lookRotation;
        Vector3 dir;
        public RotateState(Operative _operative) : base(_operative)
        {
            operative.StopNav();
            lookRotation = Quaternion.identity;
        }
        public override State Update()
        {
            if (!operative.IsRotationFinished(out dir))
            {
                lookRotation = Quaternion.LookRotation(dir);
                operative.SmoothLookAt(lookRotation);
                return this;
            }
            operative.isRotateNeeded = false;
            operative.TargetRotation = operative.transform.forward;
            return new IdleState(operative);
        }
    }
}