using Godot;

namespace BombasticBananas.Scripts.Controller
{
    public partial class MovementController : RigidBody2D
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
        private bool isTouchingGround;
        private bool isJumping;
        private bool isDescendingDuringJump;

        private bool isFlying;

        private const double DescendColliderEnableTimer = 0.3;
        private bool isDescendingBetweenLayers;

        private const float IndexMaxAngle = -20;
        private const float IndexMinAngle = 55;
        private const float FuckMaxAngle = -20;
        private const float FuckMinAngle = 40;

        private Bone2D index;
        private Bone2D fuck;

        public override void _Ready()
        {
            AddConstantForce(new Vector2(SlidingLeftForce, 0));
            groundChecker = GetNode<Area2D>("GroundChecker");
            index = GetNode<Bone2D>("Visuals/Skeleton2D/Hand/Index");
            fuck = GetNode<Bone2D>("Visuals/Skeleton2D/Hand/Fuck");
        }

        public override void _Process(double delta)
        {
            if (CanJump())
            {
                isJumping = true;
                LinearVelocity = new Vector2(LinearVelocity.X, 0);
                ApplyImpulse(new Vector2(0, JumpForce));
            }
        }

        private bool CanJump()
        {
            if (!isTouchingGround)
            {
                return false;
            }

            if (isJumping)
            {
                return false;
            }

            if (Input.IsActionJustPressed("Jump"))
            {
                return true;
            }

            return false;
        }

        public override void _PhysicsProcess(double delta)
        {
            bool previousIsFlying = isFlying;
            if (GlobalPosition.Y <= -300)
            {
                isFlying = true;
            }
            else
            {
                isFlying = false;
            }

            if (isFlying)
            {
                if (previousIsFlying == false)
                {
                    ConstantForce = Vector2.Zero;
                    LinearVelocity = new Vector2(0, 0);
                }

                GravityScale = 0;
                Fly();
            }
            else
            {
                if (previousIsFlying == true)
                {
                    AddConstantForce(new Vector2(SlidingLeftForce, 0));
                }

                GravityScale = 1;
                WalkAndJump();
            }
        }

        private void Fly()
        {
        }

        private void WalkAndJump()
        {
            isTouchingGround = groundChecker.GetOverlappingBodies().Count > 0;
            isDescendingDuringJump = isJumping && LinearVelocity.Y < 0;

            if (isDescendingDuringJump && isTouchingGround)
            {
                isJumping = false;
                isDescendingDuringJump = false;
            }

            if (Input.IsActionJustPressed(RightFingerAction) && lastInput != RightFingerAction)
            {
                lastInput = RightFingerAction;
                ApplyImpulse(new Vector2(MovementForce, 0));

                index.RotationDegrees = IndexMaxAngle;
                fuck.RotationDegrees = FuckMinAngle;
            }

            if (Input.IsActionJustPressed(LeftFingerAction) && lastInput != LeftFingerAction)
            {
                lastInput = LeftFingerAction;
                ApplyImpulse(new Vector2(MovementForce, 0));

                index.RotationDegrees = IndexMinAngle;
                fuck.RotationDegrees = FuckMaxAngle;
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