using System.Threading.Tasks;

public class WaitCommand : Command
{
    public override async Task Execute()
    {
        Godot.GD.Print("Ended turn");
        await Task.Yield();
    }

    public override async Task UnExecute()
    {
        await Task.Yield();
    }
}