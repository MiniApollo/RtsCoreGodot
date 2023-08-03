using Godot;
using System.Collections.Generic;

// TODO UnitMovement
static class Unit_Selection {

	public static void ClickSelect(PhysicsBody3D UnitToAdd, List<PhysicsBody3D> UnitsSelected) {

		DeselectAll(UnitsSelected);
		UnitsSelected.Add(UnitToAdd);
		UnitToAdd.GetChild<MeshInstance3D>(0).Show();
	}
	public static void ShiftClickSelect(PhysicsBody3D UnitToAdd, List<PhysicsBody3D> UnitsSelected) {
		if (!UnitsSelected.Contains(UnitToAdd)) {
			UnitsSelected.Add(UnitToAdd);
			UnitToAdd.GetChild<MeshInstance3D>(0).Show();
		}
		else {
			UnitToAdd.GetChild<MeshInstance3D>(0).Hide();
			UnitsSelected.Remove(UnitToAdd);
		}
	}
	public static void DragSelect(PhysicsBody3D UnitToAdd, List<PhysicsBody3D> UnitsSelected) {
		if (!UnitsSelected.Contains(UnitToAdd)) {

			UnitsSelected.Add(UnitToAdd);
			UnitToAdd.GetChild<MeshInstance3D>(0).Show();
		}

	}
	public static void Deselect(PhysicsBody3D UnitToDeselect, List<PhysicsBody3D> UnitsSelected) {
		UnitsSelected.Remove(UnitToDeselect);
	}
	public static void DeselectAll(List<PhysicsBody3D> UnitsSelected) {
		for (int i = 0; i < UnitsSelected.Count; i++) {
			UnitsSelected[i].GetChild<MeshInstance3D>(0).Hide();
		}
		UnitsSelected.Clear();
	}
}
