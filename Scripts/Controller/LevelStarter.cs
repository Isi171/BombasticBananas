using Godot;

namespace BombasticBananas.Scripts.Controller
{
	public partial class LevelStarter : Node
	{
		private RigidBody2D fingers;
		private Control mainMenuUi;

		private bool isOnMainMenu;


		public override void _Ready()
		{
			fingers = GetNode<RigidBody2D>("fingers");
			mainMenuUi = GetNode<Control>("mainMenuUi");
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
			}
		}
	}
}
