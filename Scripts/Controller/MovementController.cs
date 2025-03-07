using Godot;

namespace BombasticBananas.Scripts.Controller
{
    public partial class MovementController : CharacterBody2D
    {
        private const float Speed = 3000.0f;
        private const float JumpVelocity = -400.0f;

        private const double InputTimer = 0.5;
        private double currentTimer;

        private string lastInput;

        public override void _PhysicsProcess(double delta)
        {
            Vector2 velocity = Velocity;

            if (!IsOnFloor())
            {
                velocity += GetGravity() * (float) delta;
            }

            if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
            {
                velocity.Y = JumpVelocity;
            }

            if (Input.IsActionJustPressed("ui_right") && lastInput != "ui_right")
            {
                lastInput = "ui_right";
                velocity.X = Speed;
            }
            else if (Input.IsActionJustPressed("ui_left") && lastInput != "ui_left")
            {
                lastInput = "ui_left";
                velocity.X = Speed;
            }
            else
            {
                velocity.X = Mathf.MoveToward(Velocity.X, -300, Speed);
            }

            Velocity = velocity;
            MoveAndSlide();


            /*
            Vector2 velocity = Velocity;

            // Add the gravity.
            if (!IsOnFloor())
            {
                velocity += GetGravity() * (float)delta;
            }

            // Handle Jump.
            if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
            {
                velocity.Y = JumpVelocity;
            }

            // Get the input direction and handle the movement/deceleration.
            // As good practice, you should replace UI actions with custom gameplay actions.
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            if (direction != Vector2.Zero)
            {
                velocity.X = direction.X * Speed;
            }
            else
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            }

            Velocity = velocity;
            MoveAndSlide();
            */
        }
    }
}