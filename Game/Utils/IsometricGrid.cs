using Godot;

[GlobalClass]
public partial class IsometricGrid : Grid
{
    [Export]
    public Vector2I Offset;

    public override Vector2I CalculateMapPosition(Vector2I gridPosition)
    {
        var result = new Vector2I();
        var halfCellSize = CellSize / 2;
        result.X = (gridPosition.X - gridPosition.Y) * halfCellSize.X + Offset.X;
        result.Y = (gridPosition.X + gridPosition.Y) * halfCellSize.Y + Offset.Y;
        return result;
    }

    public override Vector2I CalculateGridCoordinates(Vector2I mapPosition)
    {
        var result = new Vector2I();
        var halfCellSize = CellSize / 2;
        result.X = (mapPosition.X / halfCellSize.X + mapPosition.Y / halfCellSize.Y) / 2;
        result.Y = (mapPosition.Y / halfCellSize.Y - (mapPosition.X / halfCellSize.X)) / 2;
        return result;
    }
}