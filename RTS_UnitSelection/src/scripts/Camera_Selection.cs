using Godot;
using System.Collections.Generic;

partial class Camera_Selection : Camera3D {

	public List<PhysicsBody3D> UnitList = new List<PhysicsBody3D>();
	public List<PhysicsBody3D> UnitsSelected = new List<PhysicsBody3D>();
	public List<List<PhysicsBody3D>> ControlGroups = new List<List<PhysicsBody3D>>();

	// Mask number
	public int clickable = 1;
	public int ground = 2;
	public int ui = 4;

	private const float RayLength = 1000.0f;

	public override void _Input(InputEvent @event) {
		// if mouse pressed
		if (@event is InputEventMouseButton eventMouseButton) {
			// if mouse button index left or right
			if (Input.IsActionJustReleased("LeftClick") || Input.IsActionJustReleased("RightClick")) {
				var space = GetWorld3D().DirectSpaceState;
				var cam = this;

				Vector3 from = cam.ProjectRayOrigin(eventMouseButton.Position);
				Vector3 to = from + cam.ProjectRayNormal(eventMouseButton.Position) * RayLength;
				var query = PhysicsRayQueryParameters3D.Create(from,to); // third argument mask 1,2
				var hit = space.IntersectRay(query);

				// if hit something
				if (hit != null && hit.Count > 0) {
					PhysicsBody3D col = (PhysicsBody3D)hit["collider"];
					if (Input.IsActionJustReleased("LeftClick")) {
						/* 
						 * TODO Player and enemy unit type
						 */
						// if it is clickable
						if (col.CollisionLayer == clickable) {
							//Shift Click
							if (Input.IsActionPressed("Shift")) {
								Unit_Selection.ShiftClickSelect(col, UnitsSelected);
							}
							else {
								Unit_Selection.ClickSelect(col, UnitsSelected);
							}
						}
						// hit anything except ui and shift not pressed
						else if (!Input.IsActionPressed("Shift") && col.CollisionLayer != ui){
							Unit_Selection.DeselectAll(UnitsSelected);
						}
					}
					else if (Input.IsActionJustReleased("RightClick")) {
						// TODO Ground Marker
					}
				}
				// hit nothing with left click and shift not pressed
				else if (hit == null || hit.Count < 1 && Input.IsActionJustReleased("LeftClick") && !Input.IsActionPressed("Shift")) {
					Unit_Selection.DeselectAll(UnitsSelected);
				}
			}
		}

	}
}
