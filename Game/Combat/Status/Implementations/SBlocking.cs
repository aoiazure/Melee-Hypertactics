using System.Threading.Tasks;
using Godot;

public class SBlocking : Status
{
    public static readonly AudioStreamRandomizer blockRandomizer = GD.Load<AudioStreamRandomizer>("res://Assets/Sounds/Block/BlockSfx.tres");
    
    public override int Priority { get => 5; set => _ = value; }

    public override async Task Apply(GameActor actor) => await Task.Yield();
}