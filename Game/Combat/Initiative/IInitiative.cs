

using System.Collections;
using System.Collections.Generic;

public abstract class IInitiative : IEnumerable<GameActor>
{
    public abstract bool HasGameActor(GameActor actor);
    public abstract void AddNewGameActor(GameActor actor);
    public abstract void RemoveGameActor(GameActor actor);
    public abstract GameActor GetNextActor();
    public abstract int GetNumberOfActors();
    public abstract IEnumerator<GameActor> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}