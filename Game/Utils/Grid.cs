using System;
using Godot;

[GlobalClass]
public partial class Grid : Resource
{
    [Export]
    public Vector2I Size = new(5, 5);
    [Export]
    public Vector2I CellSize = new(32, 16);

    // Returns cell center in pixels.
    public virtual Vector2I CalculateMapPosition(Vector2I gridPosition) => gridPosition * CellSize + CellSize/2;

    // Returns coordinates of cell on grid given a map position.
    public virtual Vector2I CalculateGridCoordinates(Vector2I mapPosition) => mapPosition / CellSize;

    public bool IsWithinBounds(Vector2I cellCoordinates) 
    {
        return cellCoordinates.X >= 0 && cellCoordinates.X < Size.X && cellCoordinates.Y >= 0 && cellCoordinates.Y < Size.Y;
    }

    public Vector2I Clamp(Vector2I gridPosition)
    {
        var result = gridPosition;
        result.X = Math.Clamp(result.X, 0, Size.X - 1);
        result.Y = Math.Clamp(result.Y, 0, Size.Y - 1);
        return result;
    }

    public int AsIndex(Vector2I cell) => cell.X + Size.X * cell.Y;
}