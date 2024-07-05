using Godot;
using Godot.Collections;

public static partial class Combat
{
    public static Array<Vector2I> GetPathToPoint(Vector2I startPosition, Vector2I targetPosition)
    {
        return AstarGrid.GetIdPath(startPosition, targetPosition, true);
    }

    public static bool IsPointWalkable(Vector2I gridPosition) {
        return 
            (
                (!actorPositions.ContainsValue(gridPosition)) || 
                GetActorAtPosition(gridPosition).HasStatus<SDead>()
            ) && 
            (!AstarGrid.IsPointSolid(gridPosition)) &&
            (!futureFilledPositions.Contains(gridPosition));
    }

    public static int CheckRotatedPatternTargetCount(Faction userFaction, AoePattern pattern, PatternRotation rotation, Vector2I origin)
    {
        var opposingFaction = GetOpposingFaction(userFaction);
        var count = 0;
        foreach (Vector2I gridPosition in AoePattern.Rotated(pattern.Tiles, rotation, origin))
        {
            var actor = GetActorAtPosition(gridPosition);
            if (actor == default) continue;
            if ((actor.Faction & opposingFaction) == actor.Faction) count++;
            else if ((actor.Faction & userFaction) == userFaction) count--;
        }
        return count;
    }

    public static Faction GetOpposingFaction(Faction userFaction)
    {
        if (userFaction == Faction.Player) return Faction.Enemy;
        if (userFaction == Faction.PlayerAlly) return Faction.Enemy;
        if (userFaction == Faction.Enemy) return Faction.Player | Faction.PlayerAlly;
        
        return Faction.Unaffiliated;
    }

}
