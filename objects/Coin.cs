using Godot;
using StarterKit3DPlatformerCSharp.scripts;
namespace StarterKit3DPlatformerCSharp.objects;

public partial class Coin : Area3D
{
    private float _time;
    private bool _grabbed;
    
    public override void _Ready()
    {
        BodyEntered += _OnBodyEntered;
    }
    private void _OnBodyEntered(Node3D body)
    {
        if (body is not Player player || _grabbed) return;
        
        player.CollectCoin();
        GetNode<AudioManager>("/root/Audio").Play("res://sounds/coin.ogg");
        GetNode<CpuParticles3D>("Particles").Emitting = false;
        
        _grabbed = true;
        QueueFree(); // Freeing the entire coin seems like a better approach to just the mesh.
    }

    // Rotating, animating up and down
    public override void _Process(double delta)
    {
        RotateY(2.0f * (float)delta); // Rotation
        var pos = Position;
        pos.Y += (Mathf.Cos(_time * 5.0f) * 1.0f) * (float)delta; // Sine movement
        Position = pos;
        _time += (float)delta;
    }
}