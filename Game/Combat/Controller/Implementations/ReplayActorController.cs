using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public class ReplayActorController : BaseGameActorController
{
    public readonly List<Command> Commands;

    private int current = 0;

    public ReplayActorController(GameActor actor, List<Command> commands)
    {
        Actor = actor;
        Commands = commands;
        PrintCommands();
    }

    public override async Task StartTurn() => await Task.Yield();
    public override async Task DecideMovement() => await Task.Yield();
    public override async Task DecideAction() => await Task.Yield();

    public override async Task PerformAction()
    {
        if (current < Commands.Count)
        {
            var command = Commands[current];
            // if (command is MoveCommand) {
            //     var c = command as MoveCommand;
            //     GD.Print(c.targetX, c.targetY);
            // }
            command.Actor = Actor;
            await Actor.Execute(command);
            current++;
        }
        else Combat.EndCombat(); //await Task.Yield();
    }

    public override async Task EndTurn() => await Task.Yield();

    public void PrintCommands()
    {
        var msg = $"{Actor.ActorDetails.Name}'s Saved Commands:\n";
        for (int i = current; i < Commands.Count; i++) msg += $"{Commands[i]}\n";
        GD.Print(msg);
    }
}
