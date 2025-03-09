using Godot;

namespace BombasticBananas.Scripts.Controller
{
	public partial class LevelStarter : Node
	{
		private RigidBody2D fingers;
		private Control mainMenuUi;
		private Control RunUI;
		private Control JumpUI;

		private bool isOnMainMenu;
		private Tween tweenUI;


		public override void _Ready()
		{
			fingers = GetNode<RigidBody2D>("fingers");
			mainMenuUi = GetNode<Control>("mainMenuUi");
			RunUI = GetNode<Control>("Tutorial/Run");
			JumpUI = GetNode<Control>("Tutorial/Jump");
			isOnMainMenu = true;
			
			tweenUI = GetTree().CreateTween();
		}

		public override void _Process(double delta)
		{
			if (isOnMainMenu && Input.IsAnythingPressed())
			{
				isOnMainMenu = false;
				mainMenuUi.QueueFree();
				fingers.ProcessMode = ProcessModeEnum.Inherit;
				fingers.Visible = true;
				
				tweenUI = CreateTween();
				tweenUI.TweenProperty(RunUI, "modulate", new Color(1,1,1,1), .5f);
				tweenUI.TweenInterval(4f);
				tweenUI.TweenProperty(RunUI, "modulate", new Color(1,1,1,0), 1.5f);
				tweenUI.TweenInterval(0.5f);
				tweenUI.TweenProperty(JumpUI, "modulate", new Color(1,1,1,1), .5f);
				tweenUI.TweenInterval(5f);
				tweenUI.TweenProperty(JumpUI, "modulate", new Color(1,1,1,0), 1.5f);
				
			}
		}
	}
}
