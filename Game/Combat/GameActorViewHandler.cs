using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public static class GameActorViewHandler
{
    private static readonly Dictionary<GameActor, GameActorView2D> actorViews = [];
    private static PlayerInterfaceView InterfaceView;


    public static void SetPlayerInterfaceView(PlayerInterfaceView view) => InterfaceView = view;

    public static GameActorView2D AddNewActorView(GameActor gameActor)
    {
        GameActorView2D actorView = new()
        {
            Grid = Combat.GetGrid()
        };
        actorView.SetActor(gameActor);
        var texture = gameActor.ActorDetails.SpriteTexture;
        var offset = gameActor.ActorDetails.SpriteOffset;
        var sprite = new Sprite2D();
        if (texture != null)
        {
            sprite.Texture = texture;
            sprite.Offset = offset;
        }
        else
        {
            sprite.Texture = GD.Load<Texture2D>("res://icon.svg");
            sprite.Scale = Vector2.One * 0.1f;
        }
        actorView.AddChild(sprite);
        Services.AddNodeToGameRoot(actorView);
        actorViews.Add(gameActor, actorView);

        return actorView;
    }

    public static GameActorView2D AddNewActorView(GameActor gameActor, GameActorView2D actorView)
    {
        actorView.SetActor(gameActor);
        actorViews.Add(gameActor, actorView);
        
        return actorView;
    }

    public static void ClearViews()
    {
        foreach (var actor in actorViews) if (Node.IsInstanceValid(actor.Value)) actorViews[actor.Key].QueueFree();
        actorViews.Clear();
    }

    public static async Task Move(GameActor gameActor, int x, int y)
    {
        gameActor.Move(x, y);
        if (actorViews.TryGetValue(gameActor, out GameActorView2D value))
        {
            await value.Move(x, y);
        }
        else
        {
            GD.PushError($"{gameActor.ActorDetails.Name} not found in views");
        }
    }

    public static async Task Damage(GameActor actor, GameActor source, DamageData data)
    {
        if (actorViews.TryGetValue(actor, out GameActorView2D view))
        {
            view.Modulate = new Color(5, 5, 5);
            // TODO: Change the Player code to something more helpful
            if (source == InterfaceView.Actor || source.ActorDetails.Name == "Player")
            {
                var ap = InterfaceView.PlayerAP;
                ap.Play("player_in");
                await ap.ToSignal(ap, AnimationPlayer.SignalName.AnimationFinished);
                view.Modulate = new Color(1, 1, 1);
                ap.Play("player_out");
                await ap.ToSignal(ap, AnimationPlayer.SignalName.AnimationFinished);
            }
            else
            {
                var ap = InterfaceView.EnemyAP;
                ap.Play("enemy_in");
                await ap.ToSignal(ap, AnimationPlayer.SignalName.AnimationFinished);
                view.Modulate = new Color(1, 1, 1);
                ap.Play("enemy_out");
                await ap.ToSignal(ap, AnimationPlayer.SignalName.AnimationFinished);
            }
        }
    }
    
    public static async Task Kill(GameActor actor)
    {
        if (actorViews.TryGetValue(actor, out GameActorView2D view))
        {
            var tween = view.CreateTween();
            tween.TweenProperty(view, "global_rotation_degrees", -90, 0.6);
            tween.SetEase(Tween.EaseType.Out);
            tween.Play();
            await tween.ToSignal(tween, Tween.SignalName.Finished);
        }
    }

}