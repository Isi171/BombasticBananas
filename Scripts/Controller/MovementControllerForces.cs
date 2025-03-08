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

        private Area2D groundChecker;
        private bool isJumping;
        private bool isDescendingDuringJump;

        private const double DescendColliderEnableTimer = 0.3;
        private bool isDescendingBetweenLayers;

        public override void _Ready()
        {
            AddConstantForce(new Vector2(SlidingLeftForce, 0));
            groundChecker = GetNode<Area2D>("GroundChecker");
        }

        public override void _PhysicsProcess(double delta)
        {
            bool isTouchingGround = groundChecker.GetOverlappingBodies().Count > 0;
            isDescendingDuringJump = isJumping && LinearVelocity.Y < 0;

            if (isDescendingDuringJump && isTouchingGround)
            {
                isJumping = false;
                isDescendingDuringJump = false;
            }

            if (Input.IsActionJustPressed("Jump") && isTouchingGround && !isJumping)
            {
                isJumping = true;
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

            if (Input.IsActionJustPressed("Descend") && !isDescendingBetweenLayers)
            {
                isDescendingBetweenLayers = true;
                SetCollisionLayerValue(3, false);
                SetCollisionMaskValue(3, false);
                GetTree().CreateTimer(DescendColliderEnableTimer).Timeout += ReEnableDescendCollision;
            }
        }

        private void ReEnableDescendCollision()
        {
            isDescendingBetweenLayers = false;
            SetCollisionLayerValue(3, true);
            SetCollisionMaskValue(3, true);
        }

        public override void _IntegrateForces(PhysicsDirectBodyState2D state)
        {
            state.LinearVelocity = new Vector2(
                Mathf.Clamp(state.LinearVelocity.X, MaxSlidingLeftVelocity, MaxSlidingRightVelocity),
                state.LinearVelocity.Y);
        }
    }
}