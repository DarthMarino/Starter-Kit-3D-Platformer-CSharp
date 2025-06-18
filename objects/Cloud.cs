using Godot;
namespace StarterKit3DPlatformerCSharp.objects;

public partial class Cloud : Node3D
{
    private float _time;
    private RandomNumberGenerator _randomNumber = new ();
    private float _randomVelocity;
    private float _randomTime;

    public override void _Ready()
    {
        _randomVelocity = _randomNumber.RandfRange(0.1f, 2.0f);
        _randomTime = _randomNumber.RandfRange(0.1f, 2.0f);
    }

    public override void _Process(double delta)
    {
        var deltaF = (float)delta;
        
        // Sine wave movement
        var pos = Position;
        pos.Y += (Mathf.Cos(_time * _randomTime) * _randomVelocity) * deltaF;
        Position = pos;
        
        _time += deltaF;
    }
}