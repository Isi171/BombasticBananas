using Godot;

namespace BombasticBananas.Scripts.Controller
{
    public partial class LevelStarter : Node
    {
        private RigidBody2D fingers;
        private Control mainMenuUi;
        private Control runUi;
        private Control jumpUi;

        private bool isOnMainMenu;

        public override void _Ready()
        {
            fingers = GetNode<RigidBody2D>("fingers");
            mainMenuUi = GetNode<Control>("mainMenuUi");
            runUi = GetNode<Control>("Tutorial/Run");
            jumpUi = GetNode<Control>("Tutorial/Jump");
            isOnMainMenu = true;
        }

        public override void _Process(double delta)
        {
            if (isOnMainMenu && Input.IsAnythingPressed())
            {
                isOnMainMenu = false;
                mainMenuUi.QueueFree();
                fingers.ProcessMode = ProcessModeEnum.Inherit;
                fingers.Visible = true;

                Tween tweenUi = GetTree().CreateTween();
                tweenUi.TweenProperty(runUi, "modulate", new Color(1, 1, 1, 1), .5f);
                tweenUi.TweenInterval(4f);
                tweenUi.TweenProperty(runUi, "modulate", new Color(1, 1, 1, 0), 1.5f);
                tweenUi.TweenInterval(0.5f);
                tweenUi.TweenProperty(jumpUi, "modulate", new Color(1, 1, 1, 1), .5f);
                tweenUi.TweenInterval(5f);
                tweenUi.TweenProperty(jumpUi, "modulate", new Color(1, 1, 1, 0), 1.5f);
            }
        }
    }
}