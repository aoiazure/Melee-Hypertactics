using System;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class GameActorView2D : Node2D
{
    private const float TIME_MOVE = 0.3f;

    [Export]
    public Grid Grid;

    public GameActor Actor;

    public void SetActor(GameActor actor) {
        Actor = actor;
        GlobalPosition = Grid.CalculateMapPosition(Actor.GridPosition);
    }

    public async Task Move(int x, int y)
    {
        var targetPos = Grid.CalculateMapPosition(new Vector2I(x, y));
        var tween = CreateTween();
        tween.TweenProperty(this, "global_position", (Vector2)targetPos, TIME_MOVE);
        tween.SetEase(Tween.EaseType.InOut);
        tween.Play();
        
        await ToSignal(tween, "finished");
    }
}