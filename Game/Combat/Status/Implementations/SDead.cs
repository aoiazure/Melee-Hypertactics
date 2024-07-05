using System.Threading.Tasks;

public class SDead : Status
{
    public static readonly Godot.AudioStream DeathSfx = Godot.GD.Load<Godot.AudioStream>("res://Assets/Sounds/Death/14_human_death_spin.wav");

    public new int Duration = 999;
    public override int Priority { get => 999; set => _ = value; }

    public override async Task Apply(GameActor actor)
    {
        await Task.Yield();
    }
}