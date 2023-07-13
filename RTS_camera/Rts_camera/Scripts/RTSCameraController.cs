using Godot;
using System;

public partial class RTSCameraController : CharacterBody3D {

	[Export]
	public float ScreenEdgeBorderThickness = 40.0f; // distance from screen edge. Used for mouse movement
	[Export]
	public int CamSpeed = 10;
	[Export] 
	public Vector2 BorderLimit = new Vector2(10,30);
	[Export]
	public Vector2 zoomLimit = new Vector2(10,30);
	[Export]
	public float scroolSpeed = 2f;

	public Vector2 MousePosition;
	public Vector2 ScreenSize; // Size of the game window.
	
	public override void _PhysicsProcess(double delta) {

		Vector3 velocity = Velocity;
		Vector3 move_direction = Vector3.Zero;

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
		if (Input.IsActionJustPressed("MouseMove")) {
			Input.MouseMode = Input.MouseModeEnum.Hidden;
		}
		if (Input.IsActionJustReleased("MouseMove")) {
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		// Normalize move_direction to not move faster diagonally
		move_direction = move_direction.Normalized();

		velocity.X = move_direction.X * CamSpeed;
		velocity.Y = move_direction.Y * CamSpeed;
		velocity.Z = move_direction.Z * CamSpeed;

		// Setting map and zoom borders
		Transform3D limit = Transform;
		limit.Origin.X = Mathf.Clamp( Transform.Origin.X, -BorderLimit.X, BorderLimit.X);
		limit.Origin.Y = Mathf.Clamp( Transform.Origin.Y, -zoomLimit.X, zoomLimit.Y);
		limit.Origin.Z = Mathf.Clamp( Transform.Origin.Z, -BorderLimit.Y, BorderLimit.Y);
		Transform = limit;

		Velocity = velocity;
		MoveAndSlide();
	}
}
