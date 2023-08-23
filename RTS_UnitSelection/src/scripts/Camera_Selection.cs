using Godot;
using System.Collections.Generic;

partial class Camera_Selection : Camera3D {

	public List<PhysicsBody3D> unitList = new List<PhysicsBody3D>();
	public List<PhysicsBody3D> unitsSelected = new List<PhysicsBody3D>();
	public List<List<PhysicsBody3D>> controlGroups = new List<List<PhysicsBody3D>>();

	// LayerMask number
	public int clickable = 1;
	public int ground = 2;
	public int ui = 4;

	private const float rayLength = 1000.0f;
	private Vector2 mousePosition;

	private Vector2 dragPosition;
	private RectangleShape2D selectionBox = new RectangleShape2D();

	public override void _PhysicsProcess(double delta) {

		mousePosition = GetViewport().GetMousePosition();

		if (Input.IsActionJustPressed("LeftClick")){
			dragPosition = mousePosition;
		}
		else if (Input.IsActionPressed("LeftClick")) {
			GetChild<Draw>(0).DrawRectangle(dragPosition, mousePosition);
		}
		else if (Input.IsActionJustReleased("LeftClick")) {
			GetChild<Draw>(0).DrawRectangle(dragPosition, mousePosition);
			GetChild<Draw>(0).DrawRectangle(new (0,0), new (0,0));

			selectionBox.Size = (mousePosition - dragPosition).Abs();

			var hit = raycastMousePos(mousePosition);

			if (hit != null && hit.Count > 0) {
				// TODO Unit Movement
				//
				PhysicsBody3D col = (PhysicsBody3D)hit["collider"];

				if (col.CollisionLayer == clickable) {
					if (Input.IsActionPressed("Shift")) {
						Unit_Selection.ShiftClickSelect(col, unitsSelected);
					}
					else {
						Unit_Selection.ClickSelect(col, unitsSelected);
					}
				}
				else if (!Input.IsActionPressed("Shift") && col.CollisionLayer != ui){
					Unit_Selection.DeselectAll(unitsSelected);
				}
			}
			else if (hit == null || hit.Count < 1 && !Input.IsActionPressed("Shift")) {
				Unit_Selection.DeselectAll(unitsSelected);
			}
		}
		if (Input.IsActionPressed("RightClick")) {
			var hit = raycastMousePos(mousePosition);

			if (hit != null && hit.Count > 0) {
				PhysicsBody3D col = (PhysicsBody3D)hit["collider"];
				if (col.CollisionLayer != ui) {
					GD.Print("Right Click Pos: ", mousePosition);
					// TODO Ground Marker
					//
				}
			}
		}
	}
	public Godot.Collections.Dictionary raycastMousePos(Vector2 mousePosition){

		// TODO Drag Selection
		//
		var space = GetWorld3D().DirectSpaceState;
		var cam = this;

		Vector3 from = cam.ProjectRayOrigin(mousePosition);
		Vector3 to = from + cam.ProjectRayNormal(mousePosition) * rayLength;
		var query = PhysicsRayQueryParameters3D.Create(from,to); // third argument mask 1,2
		var hit = space.IntersectRay(query);

		return hit;
	}
}
