using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public class FighterActorController : BaseGameActorController
{
    private List<AoePattern> AvailablePatterns =
    [
        new() { Tiles = [Vector2I.Right, new(2, 0)] },
        new() { Tiles = [Vector2I.Right, new(1, 1), new(1, -1)] }
    ];

    public FighterActorController(GameActor actor) => Actor = actor;

    public override async Task DecideAction()
    {
        AoePattern selectedPattern = new();
        PatternRotation patternRotation = PatternRotation.None;
        var maxCounted = -999;
        foreach (var pattern in AvailablePatterns)
        {
            foreach (var rotation in Enum.GetValues<PatternRotation>())
            {
                if (rotation == PatternRotation.Invalid) continue;
                var count = Combat.CheckRotatedPatternTargetCount(Actor.Faction, pattern, rotation, Actor.GridPosition);
                if (count > maxCounted)
                {
                    maxCounted = count;
                    selectedPattern = pattern;
                    patternRotation = rotation;
                }
            }
        }
        if (maxCounted <= 0) QueuedCommand = new WaitCommand();
        else  QueuedCommand = new AttackCommand(Actor, patternRotation, selectedPattern, Actor.GridPosition);

        await Task.Yield();
    }

    public override async Task DecideMovement()
    {
        var playerPos = Combat.Player.GridPosition;
        var path = Combat.GetPathToPoint(Actor.GridPosition, playerPos);
        GD.Print($"Pathfinding: {path}");
        if (path.Count <= 2) QueuedCommand = new MoveCommand(Actor, Actor.GridPosition);
        else
        {
            var first = path[1];
            var second = path[2];
            if (Combat.IsPointWalkable(first))
            {
                var isSameDir = (first - Actor.GridPosition) == (second - first);
                QueuedCommand = (isSameDir && Combat.IsPointWalkable(second)) ? new MoveCommand(Actor, second) : new(Actor, first);
            }
            else QueuedCommand = new MoveCommand(Actor, Actor.GridPosition);
        }
        GD.Print(QueuedCommand);
        
        await Task.Yield();
    }

    public override async Task EndTurn()
    {
        QueuedCommand = null;
        MovablePositions.Clear();
        await Task.Yield();
    }

    public override async Task StartTurn()
    {
        await Task.Yield();
    }
}
