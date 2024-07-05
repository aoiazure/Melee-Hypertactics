using Godot;
using System;
using System.Collections.Generic;

public partial class HighlightTilemap : TileMapLayer
{
	private readonly Vector2I TILE_MOVE = new(0, 1);
	private readonly Vector2I TILE_ATTACK = new(1, 1);

	public void PlaceMovementHighlight(Vector2I gridPosition) => SetCell(gridPosition, 0, TILE_MOVE);
	public void PlaceAttackHighlight(Vector2I gridPosition) => SetCell(gridPosition, 0, TILE_ATTACK);

	public void SetAoe(HashSet<Vector2I> Cells)
	{
		foreach (var cell in Cells) PlaceAttackHighlight(cell);
	}

	private void SetCells(Vector2I atlasCoord, HashSet<Vector2I> Cells, bool clearFirst = true)
	{
		if (clearFirst) Clear();

		foreach (Vector2I coord in Cells)
		{
			SetCell(coord, 0, atlasCoord);
		}
	}

	public bool HasCell(Vector2I gridPosition) => GetCellSourceId(gridPosition) >= 0;

}
