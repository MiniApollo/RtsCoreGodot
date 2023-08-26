using Godot;
using Godot.Collections;
using System.Collections.Generic;

partial class Camera_Selection : Camera3D {

	public List<PhysicsBody3D> unitList = new List<PhysicsBody3D>();
	public List<PhysicsBody3D> unitsSelected = new List<PhysicsBody3D>();
	public List<List<PhysicsBody3D>> controlGroups = new List<List<PhysicsBody3D>>();

	// LayerMask number
	public int clickable = 1;
	public int ground = 2;
	public int ui = 4;

	private const float rayLength = 100.0f;
	private Vector2 mousePosition;

	private Vector2 drawStartPosition;
	private Vector3 dragStartVector;

	private BoxShape3D selectionBox = new BoxShape3D();

	public override void _PhysicsProcess(double delta) {

		mousePosition = GetViewport().GetMousePosition();

		if (Input.IsActionJustPressed("LeftClick")){
			drawStartPosition = mousePosition;
			if(rayToMousePosition(mousePosition).Count > 0) {
				dragStartVector = (Vector3)rayToMousePosition(mousePosition)["position"];
			}
		}
		else if (Input.IsActionPressed("LeftClick")) {
			GetChild<Draw>(0).DrawRectangle(drawStartPosition, mousePosition);
		}
		else if (Input.IsActionJustReleased("LeftClick")) {
			GetChild<Draw>(0).DrawRectangle(drawStartPosition, mousePosition);
			GetChild<Draw>(0).DrawRectangle(new (0,0), new (0,0));

			var hit = rayToMousePosition(mousePosition);

			clickSelection(hit);

			if(hit.Count > 0) {
				dragSelect(dragStartVector,(Vector3)hit["position"]);
			}
		}
		if (Input.IsActionPressed("RightClick")) {
			var hit = rayToMousePosition(mousePosition);
			rightClick(hit);
		}
	}
	public Dictionary rayToMousePosition(Vector2 mousePosition){

		var space = GetWorld3D().DirectSpaceState;
		var cam = this;

		Vector3 from = cam.ProjectRayOrigin(mousePosition);
		Vector3 to = from + cam.ProjectRayNormal(mousePosition) * rayLength;
		var query = PhysicsRayQueryParameters3D.Create(from,to); // third argument mask 1,2
		var hit = space.IntersectRay(query);

		return hit;
	}

	public void dragSelect(Vector3 dragStart, Vector3 dragEnd){
		/*
		 * Size takes world cordinates(meters) and mousePosition is in viewport cordinates
		 * need a raycast to know where is mouse position in world 
		 * to calculate distance and center between dragStart dragEnd for Size, position(origin)
		 */
		var space = GetWorld3D().DirectSpaceState;
		var query = new PhysicsShapeQueryParameters3D();

		// abs needed because Size cannot be negative
		selectionBox.Size = new Vector3(dragStart.X - dragEnd.X, rayLength, dragStart.Z - dragEnd.Z).Abs();
		query.Shape = selectionBox;

		Transform3D transform = query.Transform;
		// rayLenght can't be half the units be on top of the box and not get selected 
		transform.Origin = new Vector3((dragEnd.X + dragStart.X)/2 , rayLength/4, (dragEnd.Z + dragStart.Z)/2);
		query.Transform = transform;

		var selected = space.IntersectShape(query); // default max select number is 32 so "intersect_shape(query, <number>)"

		for (int i = 0; i < selected.Count; i++) {
			PhysicsBody3D unitToAdd = (PhysicsBody3D)selected[i]["collider"];
			if (unitToAdd.CollisionLayer == clickable) {
				Unit_Selection.DragSelect(unitToAdd,unitsSelected);
			}
		}
	}

	public void clickSelection(Dictionary hit){
		if (hit != null && hit.Count > 0) {
			// TODO Unit Movement
			//
			PhysicsBody3D collider = (PhysicsBody3D)hit["collider"];

			if (collider.CollisionLayer == clickable) {
				if (Input.IsActionPressed("Shift")) {
					Unit_Selection.ShiftClickSelect(collider, unitsSelected);
				}
				else {
					Unit_Selection.ClickSelect(collider, unitsSelected);
				}
			}
			else if (!Input.IsActionPressed("Shift") && collider.CollisionLayer != ui){
				Unit_Selection.DeselectAll(unitsSelected);
			}
		}
		else if (hit == null || hit.Count < 1 && !Input.IsActionPressed("Shift")) {
			Unit_Selection.DeselectAll(unitsSelected);
		}
	}

	public void rightClick(Dictionary hit){
		if (hit != null && hit.Count > 0) {
			PhysicsBody3D collider = (PhysicsBody3D)hit["collider"];
			if (collider.CollisionLayer != ui) {
				// TODO Ground Marker
				//
			}
		}
	}
}
