using System.Threading.Tasks;

public abstract class Status
{
    public int Duration = 0;
    public abstract int Priority { get; set; }
    public abstract Task Apply(GameActor actor);
}