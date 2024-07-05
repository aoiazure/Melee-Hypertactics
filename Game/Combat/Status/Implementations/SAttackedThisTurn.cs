using System.Threading.Tasks;

public class SAttackedThisTurn : Status
{
    public override int Priority { get => 10; set => _ = value; }

    public override async Task Apply(GameActor actor)
    {
        await Task.Yield();
    }
}