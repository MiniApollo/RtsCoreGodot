using Godot;
using System;
using System.Collections.Generic;

namespace Unit_Selection{

	// TODO UnitMovement
	static class Unit_Selection {

		public static void ClickSelect(CharacterBody3D UnitToAdd, List<CharacterBody3D> UnitsSelected) {

			DeselectAll(UnitsSelected);
			UnitsSelected.Add(UnitToAdd);
			UnitToAdd.GetChild<MeshInstance3D>(0).Show();
		}
		public static void ShiftClickSelect(CharacterBody3D UnitToAdd, List<CharacterBody3D> UnitsSelected) {
			if (!UnitsSelected.Contains(UnitToAdd)) {
				UnitsSelected.Add(UnitToAdd);
				UnitToAdd.GetChild<MeshInstance3D>(0).Show();
			}
			else {
				UnitToAdd.GetChild<MeshInstance3D>(0).Hide();
				UnitsSelected.Remove(UnitToAdd);
			}
		}
		public static void DragSelect(CharacterBody3D UnitToAdd, List<CharacterBody3D> UnitsSelected) {
			if (!UnitsSelected.Contains(UnitToAdd)) {

				UnitsSelected.Add(UnitToAdd);
				UnitToAdd.GetChild<MeshInstance3D>(0).Show();
			}

		}
		public static void Deselect(CharacterBody3D UnitToDeselect, List<CharacterBody3D> UnitsSelected) {
			UnitsSelected.Remove(UnitToDeselect);
		}
		public static void DeselectAll(List<CharacterBody3D> UnitsSelected) {
			for (int i = 0; i < UnitsSelected.Count; i++) {

				UnitsSelected[i].GetChild<MeshInstance3D>(0).Show();
			}
			UnitsSelected.Clear();
		}
	}
}
