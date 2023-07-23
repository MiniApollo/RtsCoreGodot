using Godot;
using System;

public partial class RTSCameraController : CharacterBody3D {

	[Export]
	public float ScreenEdgeBorderThickness = 40.0f; // distance from screen edge. Used for mouse movement
	[Export]
	public int CamSpeed = 10;
	[Export] 
	public Vector2 BorderLimit = new(10,30);
	[Export]
	public Vector2 zoomLimit = new(10,30);
	[Export]
	public float scroolSpeed = 2f;
	[Export]
	public float rotationAmount = 45f; // degree

	public Vector2 MousePosition;
	public Vector2 ScreenSize; // Size of the game window.
	
	public override void _PhysicsProcess(double delta) {

		Vector3 velocity = Velocity;
		Vector3 move_direction = Vector3.Zero;
		Transform3D transform = Transform;

		MousePosition = GetViewport().GetMousePosition();
		ScreenSize = GetViewport().GetVisibleRect().Size;

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
		if (MousePosition.Y >= ScreenSize.Y - ScreenEdgeBorderThickness) {
			move_direction.Z = 1;
		}
		if (MousePosition.Y <= ScreenEdgeBorderThickness) {
			move_direction.Z = -1;
		}
		if (MousePosition.X <= ScreenEdgeBorderThickness) {
			move_direction.X = -1;
		}
		if (MousePosition.X >= ScreenSize.X - ScreenEdgeBorderThickness) {
			move_direction.X = 1;
		}

		// Mouse Camera Movement
		if (Input.IsActionPressed("MouseMove")) {
			if (MousePosition.Y <= ScreenSize.Y/2 - ScreenEdgeBorderThickness) {
				move_direction.Z = -1;
			}
			if (MousePosition.Y >= ScreenSize.Y/2 + ScreenEdgeBorderThickness) {
				move_direction.Z = 1;
			}
			if (MousePosition.X <= ScreenSize.X/2 - ScreenEdgeBorderThickness) {
				move_direction.X = -1;
			}
			if (MousePosition.X >= ScreenSize.X/2 + ScreenEdgeBorderThickness) {
				move_direction.X = 1;
			}
		}

		// Camera Rotation
		if (Input.IsActionJustPressed("rotation_left")){
			Vector3 axis = new Vector3(0, 1, 0); // Or Vector3.Right
			transform.Basis = transform.Basis.Rotated(axis, rotationAmount * (float)Math.PI/180);
			transform = transform.Orthonormalized(); // To handle precision errors
		}
		if (Input.IsActionJustPressed("rotation_right")){
			Vector3 axis = new Vector3(0, 1, 0); // Or Vector3.Right
			transform.Basis = transform.Basis.Rotated(axis, -rotationAmount * (float)Math.PI/180);
			transform = transform.Orthonormalized(); // To handle precision errors
		}

		// Normalize move_direction to not move faster diagonally
		move_direction = move_direction.Normalized();

		velocity.X = move_direction.X * CamSpeed;
		velocity.Y = move_direction.Y * CamSpeed;
		velocity.Z = move_direction.Z * CamSpeed;

		// Setting map and zoom borders
		transform.Origin.X = Mathf.Clamp( Transform.Origin.X, -BorderLimit.X, BorderLimit.X);
		transform.Origin.Y = Mathf.Clamp( Transform.Origin.Y, -zoomLimit.X, zoomLimit.Y);
		transform.Origin.Z = Mathf.Clamp( Transform.Origin.Z, -BorderLimit.Y, BorderLimit.Y);
		Transform = transform;

		Velocity = velocity;
		MoveAndSlide();
	}
}
