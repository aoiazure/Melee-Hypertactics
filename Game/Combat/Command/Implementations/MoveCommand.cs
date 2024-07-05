using System.Threading.Tasks;
using Godot;

public class MoveCommand : Command
{
    public readonly int targetX, targetY;
    public int xBefore, yBefore;

    public MoveCommand(GameActor actor, int gridX, int gridY)
    {
        Actor = actor;
        targetX = gridX;
        targetY = gridY;
    }
    public MoveCommand(GameActor actor, Vector2I gridPosition)
    {
        Actor = actor;
        targetX = gridPosition.X;
        targetY = gridPosition.Y;
    }

    public override async Task Execute()
    {
        var gridPosition = new Vector2I(Actor.GridPosition.X, Actor.GridPosition.Y);
        xBefore = gridPosition.X;
        yBefore = gridPosition.Y;
        // GD.Print($"Saving: {xBefore}, {yBefore}");
        await GameActorViewHandler.Move(Actor, targetX, targetY);
    }

    public override async Task UnExecute()
    {
        GD.Print($"Undoing: {this}");
        await GameActorViewHandler.Move(Actor, xBefore, yBefore);
    }

    public override string ToString()
    {
        return $"Move: from ({xBefore}, {yBefore}) to ({targetX}, {targetY})";
    }
}