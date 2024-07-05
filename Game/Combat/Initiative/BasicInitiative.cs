using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class BasicInitiative : IInitiative
{
    private Queue<GameActor> gameActors = [];

    public override bool HasGameActor(GameActor actor) => gameActors.Contains(actor);

    public override void AddNewGameActor(GameActor actor) => gameActors.Enqueue(actor);

    public override IEnumerator<GameActor> GetEnumerator() => gameActors.GetEnumerator();

    public override GameActor GetNextActor()
    {
        var actor = gameActors.Dequeue();
        gameActors.Enqueue(actor);
        return actor;
    }

    public override int GetNumberOfActors() => gameActors.Count;

    public override void RemoveGameActor(GameActor actor)
    {
        var tempQueue = new Queue<GameActor>();
        foreach(GameActor gameActor in from gameActor in gameActors
                                       where gameActor != actor
                                       select gameActor)
            {
                tempQueue.Enqueue(gameActor);
            }
        gameActors = tempQueue;
    }

    public override string ToString()
    {
        var msg = "";
        foreach (var actor in gameActors) msg += $"{actor.ActorDetails.Name}, ";
        return msg;
    }
}