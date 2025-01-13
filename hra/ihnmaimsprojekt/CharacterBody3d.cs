using Godot;
using System;

public partial class CharacterBody3d : CharacterBody3D
{
    // Konstanty
    public const float Speed = 5.0f;
    public const float Sensitivity = 0.1f; // Citlivost myši (pro obě osy)
    private const float MaxVerticalAngle = 90f; // Omezení vertikální rotace

    // Proměnné
    private float _verticalAngle = 0f; // Sleduje vertikální úhel rotace

    public override void _Ready()
    {
        // Schová a zachytí kurzor myši
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _PhysicsProcess(double delta)
    {
        // Pohyb postavy
        Vector3 velocity = Velocity;

        // Gravitace
        if (!IsOnFloor())
        {
            velocity.Y += GetGravity().Y * (float)delta;
        }

        // Pohyb pomocí WASD
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        Vector3 direction = new Vector3(inputDir.X, 0, inputDir.Y);

        if (direction != Vector3.Zero)
        {
            // Transformace pohybu vzhledem k rotaci postavy
            direction = GlobalTransform.Basis * direction;
            direction = direction.Normalized();

            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed * (float)delta);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed * (float)delta);
        }

        Velocity = velocity;
    }

    public override void _Input(InputEvent @event)
    {
        // Ovládání rotace kamery myší
        if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            HandleCameraRotation(mouseMotionEvent.Relative);
        }
    }

    private void HandleCameraRotation(Vector2 mouseDelta)
    {
        // Horizontální rotace postavy (osa Y)
        RotateY(-mouseDelta.X * Sensitivity);

        // Najděte kameru
        Camera3D camera = GetNode<Camera3D>("Camera3D");
        if (camera != null)
        {
            // Vertikální rotace kamery (osa X)
            _verticalAngle = Mathf.Clamp(_verticalAngle - mouseDelta.Y * Sensitivity, -MaxVerticalAngle, MaxVerticalAngle);
            camera.RotationDegrees = new Vector3(_verticalAngle, camera.RotationDegrees.Y, camera.RotationDegrees.Z);
        }
    }
}
