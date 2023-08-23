using Godot;
using System.Collections.Generic;

partial class Camera_Selection : Camera3D {

	public List<PhysicsBody3D> UnitList = new List<PhysicsBody3D>();
	public List<PhysicsBody3D> UnitsSelected = new List<PhysicsBody3D>();
	public List<List<PhysicsBody3D>> ControlGroups = new List<List<PhysicsBody3D>>();

	// LayerMask number
	public int clickable = 1;
	public int ground = 2;
	public int ui = 4;

	private const float RayLength = 1000.0f;
	private Vector2 MousePosition;

	private Vector2 DragPosition;
	private RectangleShape2D selectionBox = new RectangleShape2D();

	public override void _PhysicsProcess(double delta) {

		MousePosition = GetViewport().GetMousePosition();

		if (Input.IsActionJustPressed("LeftClick")){
			DragPosition = MousePosition;
		}
		else if (Input.IsActionPressed("LeftClick")) {
			GetChild<Draw>(0).DrawRectangle(DragPosition, MousePosition);
		}
		else if (Input.IsActionJustReleased("LeftClick")) {
			GetChild<Draw>(0).DrawRectangle(DragPosition, MousePosition);
			GetChild<Draw>(0).DrawRectangle(new (0,0), new (0,0));

			selectionBox.Size = (MousePosition - DragPosition).Abs();

			var hit = raycastMousePos(MousePosition);

			if (hit != null && hit.Count > 0) {
				// TODO Unit Movement
				//
				PhysicsBody3D col = (PhysicsBody3D)hit["collider"];

				if (col.CollisionLayer == clickable) {
					if (Input.IsActionPressed("Shift")) {
						Unit_Selection.ShiftClickSelect(col, UnitsSelected);
					}
					else {
						Unit_Selection.ClickSelect(col, UnitsSelected);
					}
				}
				else if (!Input.IsActionPressed("Shift") && col.CollisionLayer != ui){
					Unit_Selection.DeselectAll(UnitsSelected);
				}
			}
			else if (hit == null || hit.Count < 1 && !Input.IsActionPressed("Shift")) {
				Unit_Selection.DeselectAll(UnitsSelected);
			}
		}
		if (Input.IsActionPressed("RightClick")) {
			var hit = raycastMousePos(MousePosition);

			if (hit != null && hit.Count > 0) {
				PhysicsBody3D col = (PhysicsBody3D)hit["collider"];
				if (col.CollisionLayer != ui) {
					GD.Print("Right Click Pos: ", MousePosition);
					// TODO Ground Marker
					//
				}
			}
		}
	}
	public Godot.Collections.Dictionary raycastMousePos(Vector2 MousePosition){

		// TODO Drag Selection
		//
		var space = GetWorld3D().DirectSpaceState;
		var cam = this;

		Vector3 from = cam.ProjectRayOrigin(MousePosition);
		Vector3 to = from + cam.ProjectRayNormal(MousePosition) * RayLength;
		var query = PhysicsRayQueryParameters3D.Create(from,to); // third argument mask 1,2
		var hit = space.IntersectRay(query);

		return hit;
	}
}
