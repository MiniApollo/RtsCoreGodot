using Godot;
using System;

public partial class RTSCameraController : CharacterBody3D {

    [Export]
    public float screenEdgeBorderThickness = 40.0f;
    [Export]
    public int camSpeed = 10;
    [Export]
    public Vector2 borderLimit = new Vector2(10, 30);
    [Export]
    public Vector2 zoomLimit = new Vector2(10, 30);
    [Export]
    public float scroolSpeed = 2f;
    [Export]
    public float rotationAmount = 45f; // in degrees

    public Vector2 mousePosition;
    public Vector2 screenSize;
    public Vector3 axis = new Vector3(0, 1, 0); // Rotation axis
    public Vector3 move_direction = Vector3.Zero;

    public override void _PhysicsProcess(double delta) {

        Vector3 velocity = Velocity;
        Transform3D transform = Transform;

        mousePosition = GetViewport().GetMousePosition();
        screenSize = GetViewport().GetVisibleRect().Size;

        // Input Actions must be set right forward ...
        move_direction.X = Input.GetActionStrength("right") - Input.GetActionStrength("left");
        move_direction.Z = Input.GetActionStrength("back") - Input.GetActionStrength("forward");

        // Zoom in out
        move_direction.Y = Input.GetActionStrength("zoom_out") - Input.GetActionStrength("zoom_in");

        // Scrool wheel
        if (Input.IsActionJustPressed("zoom_in")) {
            move_direction.Y -= scroolSpeed * Convert.ToSingle(delta);
        }
        else if (Input.IsActionJustPressed("zoom_out")) {
            move_direction.Y += scroolSpeed * Convert.ToSingle(delta);
        }

        // ScreenBorder Movement
        if (mousePosition.Y >= screenSize.Y - screenEdgeBorderThickness) {
            move_direction.Z = 1;
        }
        if (mousePosition.Y <= screenEdgeBorderThickness) {
            move_direction.Z = -1;
        }
        if (mousePosition.X <= screenEdgeBorderThickness) {
            move_direction.X = -1;
        }
        if (mousePosition.X >= screenSize.X - screenEdgeBorderThickness) {
            move_direction.X = 1;
        }

        // Mouse Camera Movement
        if (Input.IsActionPressed("MouseMove")) {
            if (mousePosition.Y <= screenSize.Y / 2 - screenEdgeBorderThickness) {
                move_direction.Z = -1;
            }
            if (mousePosition.Y >= screenSize.Y / 2 + screenEdgeBorderThickness) {
                move_direction.Z = 1;
            }
            if (mousePosition.X <= screenSize.X / 2 - screenEdgeBorderThickness) {
                move_direction.X = -1;
            }
            if (mousePosition.X >= screenSize.X / 2 + screenEdgeBorderThickness) {
                move_direction.X = 1;
            }
        }

        // Camera Rotation
        if (Input.IsActionJustPressed("rotation_left")) {
            transform.Basis = transform.Basis.Rotated(axis, rotationAmount * (float)Math.PI / 180);
            transform = transform.Orthonormalized(); // To handle precision errors
        }
        if (Input.IsActionJustPressed("rotation_right")) {
            transform.Basis = transform.Basis.Rotated(axis, -rotationAmount * (float)Math.PI / 180);
            transform = transform.Orthonormalized(); // To handle precision errors
        }

        // Normalize move_direction to not move faster diagonally
        move_direction = move_direction.Normalized();

        // Set velocity multiplied by camSpeed
        velocity.X = move_direction.X * camSpeed;
        velocity.Y = move_direction.Y * camSpeed;
        velocity.Z = move_direction.Z * camSpeed;

        // Setting map and zoom borders
        transform.Origin.X = Mathf.Clamp(Transform.Origin.X, -borderLimit.X, borderLimit.X);
        transform.Origin.Y = Mathf.Clamp(Transform.Origin.Y, -zoomLimit.X, zoomLimit.Y);
        transform.Origin.Z = Mathf.Clamp(Transform.Origin.Z, -borderLimit.Y, borderLimit.Y);
        Transform = transform;

        Velocity = velocity;
        MoveAndSlide();
    }
}
