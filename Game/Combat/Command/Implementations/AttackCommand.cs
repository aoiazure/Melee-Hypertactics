using System.Threading.Tasks;
using Godot;

public class AttackCommand : Command
{
    public AoePattern Pattern;
    public Vector2I Origin;

    public readonly PatternRotation Rotation;
    private DamageData DamageData;

    public AttackCommand(GameActor actor, PatternRotation rotation, AoePattern pattern, Vector2I origin)
    {
        Actor = actor;
        Rotation = rotation;
        Pattern = pattern;
        Origin = origin;
    }

    public AttackCommand(GameActor actor, DamageData damageData, PatternRotation rotation, AoePattern pattern, Vector2I origin)
    {
        Actor = actor;
        DamageData = damageData;
        Rotation = rotation;
        Pattern = pattern;
        Origin = origin;
    }

    public override async Task Execute()
    {
        if (DamageData.Equals(default(DamageData))) DamageData = new(2);
        GD.Print("Attacking");
        await Combat.AttackCells(Actor, DamageData, AoePattern.Rotated(Pattern.Tiles, Rotation, Origin));
    }

    public override async Task UnExecute()
    {
        GD.Print("Undoing attack");
        await Combat.AttackCells(Actor, new DamageData(-DamageData.Amount), AoePattern.Rotated(Pattern.Tiles, Rotation, Origin));
    }

    public override string ToString()
    {
        return $"Attacking for {DamageData.Amount}";
    }
}