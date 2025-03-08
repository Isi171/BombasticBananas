using Godot;

namespace BombasticBananas.Scripts.Controller
{
    public partial class SimpleSlider : RigidBody2D
    {
        [Export]
        private float Speed { get; set; }
    
        private Vector2 originalPosition;

        public override void _Ready()
        {
            originalPosition = GlobalPosition;
        }

        public override void _Process(double delta)
        {
            if (GlobalPosition.X <= -1920)
            {
                GlobalPosition = originalPosition;
            }
        }

        public override void _IntegrateForces(PhysicsDirectBodyState2D state)
        {
            state.LinearVelocity = new Vector2(Speed, 0);
        }
    }
}