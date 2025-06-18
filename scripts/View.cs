using Godot;
namespace StarterKit3DPlatformerCSharp.scripts;

public partial class View : Node3D
{
    [ExportGroup("Properties")]
    [Export] public Node Target { get; set; }
    
    [ExportGroup("Zoom")]
    [Export] public float ZoomMinimum { get; set; } = 16.0f;
    [Export] public float ZoomMaximum { get; set; } = 4.0f;
    [Export] public float ZoomSpeed { get; set; } = 10.0f;
    
    [ExportGroup("Rotation")]
    [Export] public float RotationSpeed { get; set; } = 120.0f;
    
    private Vector3 _cameraRotation;
    private float _zoom = 10.0f;
    private Camera3D _camera;

    public override void _Ready()
    {
        _camera = GetNode<Camera3D>("Camera");
        _cameraRotation = RotationDegrees; // Initial rotation
    }

    public override void _PhysicsProcess(double delta)
    {
        // Set position and rotation to targets
        if (Target is Node3D target3D)
        {
            Position = Position.Lerp(target3D.Position, (float)(delta * 4));
        }
        RotationDegrees = RotationDegrees.Lerp(_cameraRotation, (float)(delta * 6));
        
        _camera.Position = _camera.Position.Lerp(new Vector3(0, 0, _zoom), (float)(8 * delta));
        
        HandleInput((float)delta);
    }

    // Handle input
    private void HandleInput(float delta)
    {
        // Rotation
        var input = Vector3.Zero;
        
        input.Y = Input.GetAxis("camera_left", "camera_right");
        input.X = Input.GetAxis("camera_up", "camera_down");
        
        _cameraRotation += input.LimitLength() * RotationSpeed * delta;
        _cameraRotation.X = Mathf.Clamp(_cameraRotation.X, -80.0f, -10.0f);
        
        // Zooming
        _zoom += Input.GetAxis("zoom_in", "zoom_out") * ZoomSpeed * delta;
        _zoom = Mathf.Clamp(_zoom, ZoomMaximum, ZoomMinimum);
    }
}