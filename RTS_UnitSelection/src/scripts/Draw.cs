using Godot;
using System;

public partial class Draw : Node2D {

    /*
     * This script is needed for Drag selection visual drawing 
     * because node2d has these properties
     */

	public Color SeBoxColor = new Color(Colors.DarkGreen,0.5f);

	private Vector2 DragStart;
	private Vector2 DragEnd;

    // This function is needed because _Draw can't have arguments
	public void DrawRectangle(Vector2 IDragStart, Vector2 IDragEnd) {
		DragStart = IDragStart;
		DragEnd = IDragEnd;
		QueueRedraw();
	}

	public override void _Draw (){
		DrawRect(new Rect2(DragStart, DragEnd - DragStart), SeBoxColor, true);
	}
}
