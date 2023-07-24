using Godot;
using System;
using System.Collections.Generic;

namespace PlayerCamera {
	partial class Camera_Selection : Camera3D {

		public List<CharacterBody3D> UnitList = new List<CharacterBody3D>();
		public List<CharacterBody3D> UnitsSelected = new List<CharacterBody3D>();
		public List<List<CharacterBody3D>> ControlGroups = new List<List<CharacterBody3D>>();

		public override void _PhysicsProcess(double delta) {

		}

	}
}
