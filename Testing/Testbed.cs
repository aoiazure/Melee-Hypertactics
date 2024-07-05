using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;

public partial class Testbed : Node2D
{
    [Export]
    public Grid Grid;
    [ExportCategory("References")]
    [Export]
    public TileMapLayer EnvironmentMap;
    [Export]
    public TileMapLayer MovementMap;
    [Export]
    public PlayerInterfaceView interfaceView;


    GameActor actor;
    GameActor otherActor;

    public override void _Ready()
    {
        Services.GameWorldTree = this;
        Services.SceneTree = GetTree();
        Combat.InterfaceView = interfaceView;

        Combat.SetUpAStar(Grid, EnvironmentMap);
        StateTest();
        Combat.BeginCombat();
    }

    private void StateTest()
    {
        var json = JsonFileReader.ReadJsonAsDictionary("res://Game/Combat/Encounters/TestCombat.json");
        var combatState = Combat.ReadJsonToCombatState(json);
        Combat.ReadCombatState(combatState);
    }

    public void CombatTest()
    {
        actor = new()
        {
            ActorDetails = new() {
                Name = "Player",
                Portrait = GD.Load<Texture2D>("res://Assets/Portraits/Swordman/Crops/Swordman B.png"),
            },
            Initiative = 0,
            GridPosition = new(2, 0)
        };

        actor.Controller = new PlayerActorController(actor, interfaceView);
        GameActorView2D view = new()
        {
            Grid = Grid
        };
        view.SetActor(actor);
        view.AddChild(new Sprite2D()
        {
            Texture = GD.Load<Texture2D>("res://icon.svg"),
            Scale = Vector2.One * 0.1f,
        });
        AddChild(view);
        GameActorViewHandler.AddNewActorView(actor, view);

        Combat.AddGameActor(actor);
        Combat.Player = actor;

        otherActor = new GameActor()
        {
            ActorDetails = new() {
                Name = "Enemy",
                Portrait = GD.Load<Texture2D>("res://Assets/Portraits/Swordman/Crops/Swordman B.png"),
            },
            Initiative = 5,
            GridPosition = new Vector2I(2, 5)
        };
        // otherActor.Controller = new DummyActorController(otherActor);
        otherActor.Controller = new FighterActorController(otherActor);
        GameActorView2D otherView = new()
        {
            Grid = Grid
        };
        otherView.SetActor(otherActor);
        otherView.AddChild(new Sprite2D()
        {
            Texture = GD.Load<Texture2D>("res://icon.svg"),
            Scale = Vector2.One * 0.1f,
        });
        AddChild(otherView);
        GameActorViewHandler.AddNewActorView(otherActor, otherView);

        Combat.AddGameActor(otherActor);

        Combat.BeginCombat();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // if (Input.IsActionJustPressed("replay")) Combat.Replay();
        // if (Input.IsActionJustPressed("move")) await actor.Execute(new MoveCommand(actor, actor.GridPosition.X+1, 0));
        // if (Input.IsActionJustPressed("move")) await actor.Execute(new BasicAttackCommand(actor, global::Rotation.None));
        // if (Input.IsActionJustPressed("ui_undo")) actor.Undo(1);
        // if (Input.IsActionJustPressed("ui_redo")) actor.Redo(1);
    }

    // private void Replay()
    // {
    //     Combat.EndCombat();
        
    //     GD.Print("Running replay");
    //     var json = JsonFileReader.ReadJsonAsDictionary("res://Game/Combat/Encounters/TestCombat.json");
    //     var state = Combat.ReadJsonToCombatState(json);
    //     foreach (GameActor actor in Combat.GetGameActors())
    //     {
    //         Command[] commands = new Command[actor.Commands.Count];
    //         actor.Commands.CopyTo(commands);
    //         state.ActorActionHistory[actor.ActorDetails.Name] = actor.Commands.ToList();
    //     }

    //     interfaceView.Hide();
    //     Combat.ReplayCombatState(state);
    //     Combat.BeginCombat();
    // }
}