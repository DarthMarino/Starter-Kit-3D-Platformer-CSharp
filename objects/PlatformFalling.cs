using Godot;
using StarterKit3DPlatformerCSharp.scripts;
namespace StarterKit3DPlatformerCSharp.objects;

public partial class PlatformFalling : Node3D
{
    private bool _falling;
    private float _gravity;

    public override void _Ready()
    {
        // Connect the body_entered signal if this platform has an Area3D child
        var area = GetNode<Area3D>("Area3D");
        area.BodyEntered += OnBodyEntered;
    }

    public override void _Process(double delta)
    {
        var deltaF = (float)delta;
        
        // Animate scale back to normal
        Scale = Scale.Lerp(Vector3.One, deltaF * 10.0f);
        
        // Apply gravity
        var pos = Position;
        pos.Y -= _gravity * deltaF;
        Position = pos;
        
        // Remove platform if it falls too far
        if (Position.Y < -10)
        {
            QueueFree();
        }
        
        // Increase gravity if falling
        if (_falling)
        {
            _gravity += 0.25f;
        }
    }

    private void OnBodyEntered(Node3D body)
    {
        if (!_falling)
        {
            GetNode<AudioManager>("/root/Audio").Play("res://sounds/fall.ogg");
            Scale = new Vector3(1.25f, 1.0f, 1.25f); // Animate scale
        }
        _falling = true;
    }
}