using System;
using System.Threading.Tasks;

public abstract class Command
{
    // Reference to the command's actor.
    public GameActor Actor;

    // Execute the command.
    public abstract Task Execute();
    // Undo its execution.
    public abstract Task UnExecute();
}
