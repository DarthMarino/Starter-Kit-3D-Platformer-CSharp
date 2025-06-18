using Godot;
namespace StarterKit3DPlatformer.scripts;

public partial class Hud : CanvasLayer
{
	public override void _Ready()
	{
		// Find the player and connect to its signal
		var player = GetParent().GetNode<Player>("Player");
		player.CoinCollected += OnCoinCollected;
	}
	private void OnCoinCollected(int coins)
	{
		GetNode<Godot.Label>("Coins").Text = coins.ToString();
	}
}
