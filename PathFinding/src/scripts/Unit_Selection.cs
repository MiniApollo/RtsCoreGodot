using Godot;
using System.Collections.Generic;

static class Unit_Selection {

	public static void ClickSelect(PhysicsBody3D unitToAdd, List<PhysicsBody3D> unitsSelected) {

		DeselectAll(unitsSelected);
		unitsSelected.Add(unitToAdd);
		unitToAdd.GetChild<MeshInstance3D>(0).Show();
	}
	public static void ShiftClickSelect(PhysicsBody3D unitToAdd, List<PhysicsBody3D> unitsSelected) {
		if (!unitsSelected.Contains(unitToAdd)) {
			unitsSelected.Add(unitToAdd);
			unitToAdd.GetChild<MeshInstance3D>(0).Show();
		}
		else {
			unitToAdd.GetChild<MeshInstance3D>(0).Hide();
			unitsSelected.Remove(unitToAdd);
		}
	}
	public static void DragSelect(PhysicsBody3D unitToAdd, List<PhysicsBody3D> unitsSelected) {
		if (!unitsSelected.Contains(unitToAdd)) {

			unitsSelected.Add(unitToAdd);
			unitToAdd.GetChild<MeshInstance3D>(0).Show();
		}

	}
	public static void Deselect(PhysicsBody3D UnitToDeselect, List<PhysicsBody3D> unitsSelected) {
		unitsSelected.Remove(UnitToDeselect);
	}
	public static void DeselectAll(List<PhysicsBody3D> unitsSelected) {
		for (int i = 0; i < unitsSelected.Count; i++) {
			unitsSelected[i].GetChild<MeshInstance3D>(0).Hide();
		}
		unitsSelected.Clear();
	}
}
