using Godot;
using Godot.Collections;
using System.Collections.Generic;

partial class Camera_Selection : Camera3D {

	public List<PhysicsBody3D> unitList = new List<PhysicsBody3D>();
	public List<PhysicsBody3D> unitsSelected = new List<PhysicsBody3D>();
	public List<List<PhysicsBody3D>> controlGroups = new List<List<PhysicsBody3D>>();

	// LayerMask number
	[Export]
	public int clickable = 1;
	[Export]
	public int ground = 2;
	[Export]
	public int ui = 4;

	[Export]
	public MeshInstance3D groundMarker = new MeshInstance3D();

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

			leftClickSelect(hit,unitsSelected);

			if(hit.Count > 0) {
				dragSelect(dragStartVector,(Vector3)hit["position"]);
			}
		}
		if (Input.IsActionPressed("RightClick")) {
			var hit = rayToMousePosition(mousePosition);
			rightClick(hit,unitsSelected);
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

		var selected = space.IntersectShape(query,256); // default max select number is 32 so "intersect_shape(query, <number>)"

		for (int i = 0; i < selected.Count; i++) {
			PhysicsBody3D unitToAdd = (PhysicsBody3D)selected[i]["collider"];
			if (unitToAdd.CollisionLayer == clickable) {
				Unit_Selection.DragSelect(unitToAdd,unitsSelected);
			}
		}
	}
	public bool hitSomeThing(Dictionary hitPoint) {
		if(hitPoint != null && hitPoint.Count > 0){
			return true;
		}
		return false;
	}

	public void leftClickSelect(Dictionary hit, List<PhysicsBody3D> unitToAdd){
		if (hitSomeThing(hit)) {
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
				groundMarker.Visible = false;
			}
		}
		else if (!hitSomeThing(hit) && !Input.IsActionPressed("Shift")) {
			Unit_Selection.DeselectAll(unitsSelected);
			groundMarker.Visible = false;
		}
	}

	public void rightClick(Dictionary hit, List<PhysicsBody3D> unitsToMove){
		if (hitSomeThing(hit)) {
			PhysicsBody3D collider = (PhysicsBody3D)hit["collider"];
			if (collider.CollisionLayer != ui) {
				groundMarker.Position = (Vector3)hit["position"];
				groundMarker.Rotation = new (0,0,0);
				groundMarker.Visible = true;

				if (unitsSelected.Count > 0) {
					for (int i = 0; i < unitsSelected.Count; i++) {
						var unit = GetNode<pathFinding>(unitsToMove[i].GetPath());
						unit.CalculatemovementTarget((Vector3)hit["position"]);
					}
				}
			}
			else {
				groundMarker.Visible = false;
			}
		}
	}
}
