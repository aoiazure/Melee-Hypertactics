using System.Threading.Tasks;

public class BlockCommand : Command
{
    public BlockCommand(GameActor actor) => Actor = actor;
    
    public override async Task Execute()
    {
        Actor.AddStatus(new SBlocking());
        await Task.Yield(); 
    }

    public override Task UnExecute()
    {
        throw new System.NotImplementedException();
    }
}