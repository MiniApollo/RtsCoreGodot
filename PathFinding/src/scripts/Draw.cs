using Godot;

public partial class Draw : Node2D {

    /*
	 * This script is needed for Drag selection visual drawing 
	 * because node2d has these properties
	 */

    public Color selectionBoxColor = new(Colors.DarkGreen, 0.5f);

    private Vector2 dragStart;
    private Vector2 dragEnd;

    // This function is needed because _Draw can't have arguments
    public void DrawRectangle(Vector2 aDragStart, Vector2 aDragEnd) {
        dragStart = aDragStart;
        dragEnd = aDragEnd;
        QueueRedraw();
    }

    public override void _Draw() {
        DrawRect(new Rect2(dragStart, dragEnd - dragStart), selectionBoxColor, true);
    }
}
