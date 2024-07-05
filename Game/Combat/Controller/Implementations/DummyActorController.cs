using System.Threading.Tasks;

public class DummyActorController : BaseGameActorController
{
    public DummyActorController(GameActor actor) => Actor = actor;

    public override async Task StartTurn() => await Task.Yield();
    public override async Task DecideMovement()
    {
        QueuedCommand = new MoveCommand(Actor, Actor.GridPosition);
        await Task.Yield();
    }


    public override async Task DecideAction()
    {
        QueuedCommand = new WaitCommand();
        await Task.Yield();
    }
    
    public override async Task PerformAction() => await Task.Yield(); //Services.AwaitTimerTimeout(1.0f);

    public override async Task EndTurn() => await Task.Yield();

}