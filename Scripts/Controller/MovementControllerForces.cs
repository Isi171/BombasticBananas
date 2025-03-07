using Godot;

namespace BombasticBananas.Scripts.Controller
{
    public partial class MovementControllerForces : RigidBody2D
    {
        private const float SlidingLeftForce = -300f;
        private const float MaxSlidingLeftVelocity = -400f;
        private const float MaxSlidingRightVelocity = 300f;
        private const float JumpForce = -500f;
        private const float MovementForce = 100f;

        private string lastInput;

        public override void _Ready()
        {
            AddConstantForce(new Vector2(SlidingLeftForce, 0));
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Input.IsActionJustPressed("ui_accept"))
            {
                ApplyImpulse(new Vector2(0, JumpForce));
            }

            if (Input.IsActionJustPressed("ui_right") && lastInput != "ui_right")
            {
                lastInput = "ui_right";
                ApplyImpulse(new Vector2(MovementForce, 0));
            }
            else if (Input.IsActionJustPressed("ui_left") && lastInput != "ui_left")
            {
                lastInput = "ui_left";
                ApplyImpulse(new Vector2(MovementForce, 0));
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