using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class Frog : MonoBehaviour
{
    private CharacterController2D character;
    private IState state;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        character = GetComponent<CharacterController2D>();
        // Start in the Idle State
        state = new IdleState(this);
    }

    private void Update()
    {
        // Update Frog's State
        state.Update();

        anim.SetBool("IsGrounded", character.IsGrounded);
        anim.SetFloat("JumpY", character.Rigidbody.velocity.y);
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
    private class IdleState : State<Frog>
    {
        private float elapsed;

        public IdleState(Frog owner, float time = 2.5f) : base(owner)
        {
            elapsed = time;
        }

        public override void Update()
        {
            elapsed -= Time.deltaTime;

            if (elapsed <= 0)
            {
                Owner.state = new ThinkState(Owner);
            }
        }
    }
    private class FlipState : State<Frog>
    {
        public FlipState(Frog owner) : base(owner) { }

        public override void Update()
        {
            Owner.character.Flip();
            Owner.state = new ThinkState(Owner);
        }
    }
    private class ThinkState : State<Frog>
    {
        public ThinkState(Frog owner) : base(owner) { }

        public override void Update()
        {
            var state = Random.Range(0, 7);

            if (state <= 1)
            {
                Owner.state = new IdleState(Owner);
            }
            else if (state <= 3)
            {
                Owner.state = new FlipState(Owner);
            }
            else
            {
                Owner.state = new JumpState(Owner);
            }
        }
    }
    private class JumpState : State<Frog>
    {
        public float leap = 3f;
        public float height = 1f;
        private float airElapsed = .3f;

        public JumpState(Frog owner) : base(owner) {}

        public override void Update()
        {
            var character = Owner.character;

            airElapsed -= Time.deltaTime;
            
            if (airElapsed < 0 && character.IsGrounded)
            {
                Owner.state = new ThinkState(Owner);
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
