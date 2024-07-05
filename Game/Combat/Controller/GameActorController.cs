using System.Threading.Tasks;
using System.Collections.Generic;
using Godot;

public abstract class BaseGameActorController
{
    public GameActor Actor;
    public Command QueuedCommand;
    protected HashSet<Vector2I> MovablePositions = [];

    public abstract Task StartTurn();
    public abstract Task DecideMovement();
    public abstract Task DecideAction();
    public virtual async Task PerformAction() => await Actor.Execute(QueuedCommand);
    public abstract Task EndTurn();
}