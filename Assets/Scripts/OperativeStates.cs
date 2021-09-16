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
        public abstract void FixedUpdate();
        public abstract void LateUpdate();
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
        public override void FixedUpdate() { }
        public override void LateUpdate() { }
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
        public override void FixedUpdate() { }
        public override void LateUpdate() { }
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
        public override void FixedUpdate() { }
        public override void LateUpdate() { }
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
                operative.isRotateNeeded = false;
                return new RotateState(operative);
            }
            return this;
        }
        public override void FixedUpdate() { }
        public override void LateUpdate() { }
    }

    public class RotateState : OperativeState
    {
        Quaternion lookRotation;
        public RotateState(Operative _operative) : base(_operative)
        {
            operative.StopNav();
            lookRotation = Quaternion.identity;
        }
        public override State Update()
        {
            if (!operative.IsRotationFinished(lookRotation))
            {
                Vector3 dir = (operative.TargetRotation - operative.transform.position).normalized;
                lookRotation = Quaternion.LookRotation(dir);
                operative.SmoothLookAt(lookRotation);
                return this;
            }
            return new IdleState(operative);
        }
        public override void FixedUpdate() { }
        public override void LateUpdate() { }
    }
}