using Godot;

namespace BombasticBananas.Scripts.Controller
{
	public partial class LevelStarter : Node
	{
		private PackedScene fingersScene;
		private Control mainMenuUi;

		private bool isOnMainMenu;


		public override void _Ready()
		{
			fingersScene = GD.Load<PackedScene>("res://Scenes/fingers.tscn");
			mainMenuUi = GetNode<Control>("mainMenuUi");
			isOnMainMenu = true;
		}

		public override void _Process(double delta)
		{
			if (isOnMainMenu && Input.IsAnythingPressed())
			{
				isOnMainMenu = false;
				mainMenuUi.QueueFree();
				Node fingers = fingersScene.Instantiate();
				AddChild(fingers);
			}
		}
	}
}
