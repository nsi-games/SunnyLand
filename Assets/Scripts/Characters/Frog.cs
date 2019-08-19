using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class Frog : MonoBehaviour
{
    private CharacterController2D character;
    private IState state;

    private void Awake()
    {
        character = GetComponent<CharacterController2D>();
        // Start in the Idle State
        state = new Idle(this);
    }
    private void Update()
    {
        // Update Frog's State
        state.Update();
    }

    #region State Machine
    private interface IState
    {
        void Update();
    }
    private abstract class State<TOwner> : IState
    {
        protected State(TOwner owner)
        {
            Owner = owner;
        }
        protected TOwner Owner { get; private set; }
        public abstract void Update();
    }
    private class Idle : State<Frog>
    {
        private float elapsed;
        public Idle(Frog owner, float time = 2.5f) : base(owner)
        {
            elapsed = time;
        }
        public override void Update()
        {
            elapsed -= Time.deltaTime;

            if (elapsed <= 0)
            {
                Owner.state = new Think(Owner);
            }
        }
    }
    private class Flip : State<Frog>
    {
        public Flip(Frog owner) : base(owner) { }
        public override void Update()
        {
            Owner.character.Flip();
            Owner.state = new Think(Owner);
        }
    }
    private class Think : State<Frog>
    {
        public Think(Frog owner) : base(owner) { }
        public override void Update()
        {
            var state = Random.Range(0, 7);

            if (state <= 1)
            {
                Owner.state = new Idle(Owner);
            }
            else if (state <= 3)
            {
                Owner.state = new Flip(Owner);
            }
            else
            {
                Owner.state = new Jump(Owner);
            }
        }
    }
    private class Jump : State<Frog>
    {
        public float leap = 3f;
        public float height = 1f;
        private float airElapsed = .3f;
        public Jump(Frog owner) : base(owner) {}
        public override void Update()
        {
            var character = Owner.character;

            airElapsed -= Time.deltaTime;
            
            if (airElapsed < 0 && character.IsGrounded)
            {
                Owner.state = new Think(Owner);
            }
            else
            {
                float offsetX = character.IsFacingRight ? leap : -leap;
                character.Move(offsetX);
                character.Jump(height);
            }
        }
    }
    #endregion
}