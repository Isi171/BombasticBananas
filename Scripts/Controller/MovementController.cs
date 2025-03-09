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
		
		private const int StartFlyingAboveAltitude = -320;
		private const int StopFlyingBelowAltitude = 0;
		private Vector2 CurrentDir;
		private float WindForce = -500f;
		private Marker2D FlyHandTarget;
		private Control FlyUI;
		private bool TutorialShown = false;

		private Bone2D index;
		private Bone2D fuck;
		private Node2D WalkingHand;
		private Node2D FlyingHand;
		private Tween tween;
		private Tween tweenArm;
		private Tween tweenSound;
		private Tween tweenUI;
		
		private AudioStreamPlayer2D FingerTap;
		private AudioStreamPlayer2D WindSound;

		public override void _Ready()
		{
			AddConstantForce(new Vector2(SlidingLeftForce, 0));
			groundChecker = GetNode<Area2D>("GroundChecker");
			index = GetNode<Bone2D>("Visuals/Skeleton2D/Wrist/Hand/Index");
			fuck = GetNode<Bone2D>("Visuals/Skeleton2D/Wrist/Hand/Fuck");
			WalkingHand = GetNode<Node2D>("Visuals/Sprites/WalkingHand");
			FlyingHand = GetNode<Node2D>("Visuals/Sprites/FlyingHand");
			FingerTap = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
			WindSound = GetNode<AudioStreamPlayer2D>("FlySounds");
			FlyHandTarget = GetNode<Marker2D>("Visuals/FlyTarget");
			FlyUI = GetNode<Control>("../Tutorial/Fly");
			
			tween = GetTree().CreateTween();
			tweenArm = GetTree().CreateTween();
			tweenSound = GetTree().CreateTween();
			tweenUI = GetTree().CreateTween();
			
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
			if (Position.Y <= StartFlyingAboveAltitude && !isFlying)
			{
				isFlying = true;
			}
			else if (Position.Y > StopFlyingBelowAltitude && isFlying)
			{
				isFlying = false;
			}

			if (isFlying)
			{
				if (!previousIsFlying)
				{
					ConstantForce = Vector2.Zero;
					LinearVelocity = new Vector2(0, 0);
					SetAllCollisions(false);
					FlyingHand.Visible = true;
					WalkingHand.Visible = false;
					
					tweenArm.Kill(); 
					tweenArm = CreateTween();
					tweenArm.TweenProperty(GetNode<Marker2D>("Visuals/ArmTarget"), "global_position", new Vector2(-1600,-1000), 0.5f);
					
					tweenSound.Kill(); 
					tweenSound = CreateTween();
					tweenSound.TweenProperty(WindSound, "volume_db", 0, 0.5f);
					
					if (!TutorialShown)
					{
						TutorialShown = true;
						tweenUI = CreateTween();
						tweenUI.TweenProperty(FlyUI, "modulate", new Color(1,1,1,1), .5f);
						tweenUI.TweenInterval(5f);
						tweenUI.TweenProperty(FlyUI, "modulate", new Color(1,1,1,0), 1.5f);
					}
				}

				GravityScale = 0;
				Fly();
			}
			else
			{
				if (previousIsFlying)
				{
					AddConstantForce(new Vector2(SlidingLeftForce, 0));
					SetAllCollisions(true);
					FlyingHand.Visible = false;
					WalkingHand.Visible = true;
					
					tweenArm.Kill(); 
					tweenArm = CreateTween();
					tweenArm.TweenProperty(GetNode<Marker2D>("Visuals/ArmTarget"), "global_position", new Vector2(-800,-1700), 0.5f);
					
					tweenSound.Kill(); 
					tweenSound = CreateTween();
					tweenSound.TweenProperty(WindSound, "volume_db", -10, 2f);
					tweenSound.TweenProperty(WindSound, "pitch_scale", 1.0, 1f);
				}

				GravityScale = 1;
				WalkAndJump();
			}
		}

		private void SetAllCollisions(bool value)
		{
			SetCollisionLayerValue(1, value);
			SetCollisionMaskValue(1, value);
			SetCollisionLayerValue(3, value);
			SetCollisionMaskValue(3, value);
		}
		
		private void Fly()
		{
			//movement
			CurrentDir = Input.GetVector("FlyLeft", "FlyRight", "FlyUp", "FlyDown");
			float sail = CurrentDir.Y * WindForce;
			Vector2 ModifiedForce = new Vector2(CurrentDir.X * 300 - Mathf.Abs(sail) -50, - sail);
			ApplyForce(ModifiedForce);
			
			//Animation
			FlyHandTarget.Position = new Vector2(FlyHandTarget.Position.X, CurrentDir.Y * 500 - 80);
			
			//Audio
			WindSound.PitchScale = 1.0f + CurrentDir.Y * -0.5f;
			return;
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
				FingerTap.Play();

				tween.Kill(); // Abort the previous animation
				tween = CreateTween();
				tween.Parallel().TweenProperty(index, "rotation_degrees", IndexMaxAngle, 0.1f);
				tween.Parallel().TweenProperty(fuck, "rotation_degrees", FuckMinAngle, 0.1f);
			}

			if (Input.IsActionJustPressed(LeftFingerAction) && lastInput != LeftFingerAction)
			{
				lastInput = LeftFingerAction;
				ApplyImpulse(new Vector2(MovementForce, 0));
				FingerTap.Play();

				tween.Kill(); // Abort the previous animation
				tween = CreateTween();
				tween.Parallel().TweenProperty(index, "rotation_degrees", IndexMinAngle, 0.1f);
				tween.Parallel().TweenProperty(fuck, "rotation_degrees", FuckMaxAngle, 0.1f);
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
