using Godot;
namespace StarterKit3DPlatformer.scripts;

public partial class Player : CharacterBody3D
{
    [Signal] public delegate void CoinCollectedEventHandler(int coins);
    
    [ExportSubgroup("Components")]
    [Export] public Node3D View { get; set; }
    
    [ExportSubgroup("Properties")]
    [Export] public float MovementSpeed { get; set; } = 250.0f;
    [Export] public float JumpStrength { get; set; } = 7.0f;
    
    private Vector3 _movementVelocity;
    private float _rotationDirection;
    private float _gravity;
    private bool _previouslyFloored;
    private bool _jumpSingle = true;
    private bool _jumpDouble = true;
    private int _coins;
    
    private CpuParticles3D _particlesTrail;
    private AudioStreamPlayer _soundFootsteps;
    private Node3D _model;
    private AnimationPlayer _animation;

    public override void _Ready()
    {
        _particlesTrail = GetNode<CpuParticles3D>("ParticlesTrail");
        _soundFootsteps = GetNode<AudioStreamPlayer>("SoundFootsteps");
        _model = GetNode<Node3D>("Character");
        _animation = GetNode<AnimationPlayer>("Character/AnimationPlayer");
        
        View = GetParent().GetNode<Node3D>("View");
    }

    // Functions
    public override void _PhysicsProcess(double delta)
    {
        var deltaF = (float)delta;
        
        // Handle functions
        HandleControls(deltaF);
        HandleGravity(deltaF);
        HandleEffects(deltaF);
        
        // Movement
        var appliedVelocity = Velocity.Lerp(_movementVelocity, deltaF * 10.0f);
        appliedVelocity.Y = -_gravity;
        Velocity = appliedVelocity;
        MoveAndSlide();
        
        // Rotation
        if (new Vector2(Velocity.Z, Velocity.X).Length() > 0)
        {
            _rotationDirection = new Vector2(Velocity.Z, Velocity.X).Angle();
        }
        var rot = Rotation;
        rot.Y = Mathf.LerpAngle(rot.Y, _rotationDirection, deltaF * 10.0f);
        Rotation = rot;
        
        // Falling/respawning
        if (Position.Y < -10)
        {
            GetTree().ReloadCurrentScene();
        }
        
        // Animation for scale (jumping and landing)
        _model.Scale = _model.Scale.Lerp(Vector3.One, deltaF * 10.0f);
        
        // Animation when landing
        if (IsOnFloor() && _gravity > 2 && !_previouslyFloored)
        {
            _model.Scale = new Vector3(1.25f, 0.75f, 1.25f);
            GetNode<AudioManager>("/root/Audio").Play("res://sounds/land.ogg");
        }
        _previouslyFloored = IsOnFloor();
    }

    // Handle animation(s)
    private void HandleEffects(float delta)
    {
        _particlesTrail.Emitting = false;
        _soundFootsteps.StreamPaused = true;
        
        if (IsOnFloor())
        {
            var horizontalVelocity = new Vector2(Velocity.X, Velocity.Z);
            var speedFactor = horizontalVelocity.Length() / MovementSpeed / delta;
            
            if (speedFactor > 0.05f)
            {
                if (_animation.CurrentAnimation != "walk")
                {
                    _animation.Play("walk", 0.1f);
                }
                if (speedFactor > 0.3f)
                {
                    _soundFootsteps.StreamPaused = false;
                    _soundFootsteps.PitchScale = speedFactor;
                }
                if (speedFactor > 0.75f)
                {
                    _particlesTrail.Emitting = true;
                }
            }
            else if (_animation.CurrentAnimation != "idle")
            {
                _animation.Play("idle", 0.1f);
            }
        }
        else if (_animation.CurrentAnimation != "jump")
        {
            _animation.Play("jump", 0.1f);
        }
    }

    // Handle movement input
    private void HandleControls(float delta)
    {
        // Movement
        var input = Vector3.Zero;
        
        input.X = Input.GetAxis("move_left","move_right");
        input.Z = Input.GetAxis("move_forward", "move_back");
        if (View != null)
        {
            input = input.Rotated(Vector3.Up, View.Rotation.Y);
        }
        
        
        if (input.Length() > 1)
        {
            input = input.Normalized();
        }
        _movementVelocity = input * MovementSpeed * delta;
        
        // Jumping
        if (!Input.IsActionJustPressed("jump")) return;
        if (_jumpSingle || _jumpDouble)
        {
            Jump();
        }
    }

    // Handle gravity
    private void HandleGravity(float delta)
    {
        _gravity += 25.0f * delta;
        if (!(_gravity > 0) || !IsOnFloor()) return;
        _jumpSingle = true;
        _gravity = 0.0f;
    }

    // Jumping
    private void Jump()
    {
        GetNode<AudioManager>("/root/Audio").Play("res://sounds/jump.ogg");
        _gravity = -JumpStrength;
        _model.Scale = new Vector3(0.5f, 1.5f, 0.5f);
        
        if (_jumpSingle)
        {
            _jumpSingle = false;
            _jumpDouble = true;
        }
        else
        {
            _jumpDouble = false;
        }
    }
    
    // Collecting coins
    public void CollectCoin()
    {
        _coins += 1;
        EmitSignal(SignalName.CoinCollected, _coins);
    }
}