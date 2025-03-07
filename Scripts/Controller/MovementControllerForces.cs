using Godot;

namespace BombasticBananas.Scripts.Controller
{
    public partial class MovementControllerForces : RigidBody2D
    {
        private const float SlidingLeftForce = -300f;
        private const float MaxSlidingLeftVelocity = -400f;
        private const float MaxSlidingRightVelocity = 300f;
        private const float JumpForce = -600f;
        private const float MovementForce = 100f;

        private const string RightFingerAction = "RightFinger";
        private const string LeftFingerAction = "LeftFinger";
        private string lastInput;

        public override void _Ready()
        {
            AddConstantForce(new Vector2(SlidingLeftForce, 0));
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Input.IsActionJustPressed("Jump"))
            {
                ApplyImpulse(new Vector2(0, JumpForce));
            }

            if (Input.IsActionJustPressed(RightFingerAction) && lastInput != RightFingerAction)
            {
                lastInput = RightFingerAction;
                ApplyImpulse(new Vector2(MovementForce, 0));
            }
            else
            {
                if (Input.IsActionJustPressed(LeftFingerAction) && lastInput != LeftFingerAction)
                {
                    lastInput = LeftFingerAction;
                    ApplyImpulse(new Vector2(MovementForce, 0));
                }
            }
        }

        public override void _IntegrateForces(PhysicsDirectBodyState2D state)
        {
            state.LinearVelocity = new Vector2(
                Mathf.Clamp(state.LinearVelocity.X, MaxSlidingLeftVelocity, MaxSlidingRightVelocity),
                state.LinearVelocity.Y);
        }
    }
}