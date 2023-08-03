using Godot;
using System;

public partial class Draw : Node2D {

	public Color SeBoxColor = new Color(Colors.DarkGreen,0.5f);

	private Vector2 DragStart;
	private Vector2 DragEnd;

	public void DrawRectangle(Vector2 IDragStart, Vector2 IDragEnd) {
		DragStart = IDragStart;
		DragEnd = IDragEnd;
		QueueRedraw();
	}

	public override void _Draw (){
		DrawRect(new Rect2(DragStart, DragEnd - DragStart), SeBoxColor, true);
	}
}
