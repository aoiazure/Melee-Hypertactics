using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public static partial class Combat
{
    private static IInitiative GameActors = new BasicInitiative();
    private static List<GameActor> actorsDiedThisTurn = [];

    private static readonly Dictionary<GameActor, Vector2I> actorPositions = [];
    private static readonly List<Vector2I> futureFilledPositions = [];


    private static TileMapLayer Environment;
    private static AStarGrid2D AstarGrid;
    private static Grid Grid;
    private static bool InCombat = false;

    private static AudioStreamPlayer audio;

    public static GameActor Player { get; set; }
    public static PlayerInterfaceView InterfaceView { get; set; }
    public static Grid GetGrid() => Grid;

    public static async void Loop()
    {
        await SimultaneousLoop();
        
        // EndCombat();
        if (InCombat) Loop();
    }

    private static async Task TurnBasedLoop()
    {
        foreach (var actor in GameActors)
        {
            await actor.StartTurn();
            
            await actor.Controller.DecideMovement();
            await actor.Controller.PerformAction();

            await actor.Controller.DecideAction();
            await actor.Controller.PerformAction();

            await actor.EndTurn();
        }
    }

    private static async Task SimultaneousLoop()
    {
        // Start turn
        foreach(var actor in GameActors)
        { 
            await actor.StartTurn();
        }
        
        // Movement
        // Everyone decides movement
        foreach(var actor in GameActors)
        {
            // GD.Print($"Waiting on {actor.ActorDetails.Name}");
            await actor.Controller.DecideMovement();
            if (actor.Controller.QueuedCommand != null)
            {
                MoveCommand command = actor.Controller.QueuedCommand as MoveCommand;
                futureFilledPositions.Add(new(command.targetX, command.targetY));
            }
        }
        // Everyone executes movement
        foreach (var actor in GameActors)
        {
            await actor.Controller.PerformAction();
        }
        futureFilledPositions.Clear();
        Services.EventBus.EmitSignal(EventBus.SignalName.AllActorsMoveFinished);

        // Actual action
        
        // Everyone decides action
        foreach (var actor in GameActors)
        {
            // GD.Print($"{actor.ActorDetails.Name} deciding");
            await actor.Controller.DecideAction();
        }
        // Everyone executes action
        foreach (var actor in GameActors)
        {
            // GD.Print($"{actor.ActorDetails.Name} performing");
            await actor.Controller.PerformAction();
        }
        Services.EventBus.EmitSignal(EventBus.SignalName.AllActorsActionFinished);

        // Everyone executes action
        foreach (var actor in GameActors.Reverse())
        {
            // GD.Print($"{actor.ActorDetails.Name} applying");
            GD.Print("Count before applying:", actor.GetStatusCount());
            await actor.ApplyStatuses();
        }

        await Services.AwaitTimerTimeout(.5f);
        InterfaceView.AttackHighlight.Clear();

        // End turn
        foreach (var actor in GameActors)
        {
            await actor.EndTurn();
        }

        foreach (var actor in actorsDiedThisTurn)
        {
            actor.AddStatus(new SDead());
            SoundManager.PlaySound(SDead.DeathSfx);
            await GameActorViewHandler.Kill(actor);
        }

        actorsDiedThisTurn.Clear();
        Services.EventBus.EmitSignal(EventBus.SignalName.AllActorsTurnFinished);
        
        var livingCount = GameActors.Count();
        foreach (var actor in GameActors)
        {
            if (actor.HasStatus<SDead>()) livingCount--;
        }
        GD.Print(livingCount);
        if (livingCount <= 1)
        {
            EndCombat();
            InterfaceView.EndCombat();
        }
    }

    

    public static void AddGameActor(GameActor actor)
    {
        GameActors.AddNewGameActor(actor);
        UpdateActorPosition(actor, actor.GridPosition);
    }

    public static IInitiative GetGameActors() => GameActors;

    public static void UpdateActorPosition(GameActor actor, Vector2I GridPosition) 
    {
        GD.Print($"{actor.ActorDetails.Name} new position {GridPosition}");
        actorPositions[actor] = GridPosition;
    }

    public static void BeginCombat()
    {
        audio ??= SoundManager.PlayMusic(GD.Load<AudioStream>("res://Assets/Sounds/Music/MeltdownTheme_Loopable.wav"));
        InCombat = true;
        Loop();
    }

    public static void EndCombat() 
    {
        InCombat = false;
    }

    public static Vector2I SelectGridPositionOnMap() => Environment.LocalToMap(Environment.GetLocalMousePosition());

    public static Vector2I SelectMapPositionToWorld()
    {
        var mousePos = Environment.LocalToMap(Environment.GetLocalMousePosition());
        var mapPos = Environment.ToGlobal(Environment.MapToLocal(mousePos));
        GD.Print(mousePos, mapPos);
        return (Vector2I)mapPos;
    }

    private static GameActor GetActorAtPosition(Vector2I GridPosition)
    {
        return actorPositions.FirstOrDefault(x => x.Value == GridPosition).Key;
    }

    public static async Task AttackCells(GameActor source, DamageData damageData, HashSet<Vector2I> cells)
    {
        // foreach (var v in cells) GD.Print(v);
        foreach (Vector2I coord in cells)
        {
            if (actorPositions.ContainsValue(coord))
            {
                var actor = GetActorAtPosition(coord);
                GD.Print($"{source.ActorDetails.Name} => {actor.ActorDetails.Name} {coord} [{damageData.Amount}]");
                actor.Damage(source, damageData);
            }
            InterfaceView.AttackHighlight.PlaceAttackHighlight(coord); await Services.AwaitTimerTimeout(0.1f);
        }
    }

    public static void RegisterDeath(GameActor actor)
    {
        // The dead don't act.
        GD.Print("KILLED");
        actor.Controller = new DummyActorController(actor);
        actorsDiedThisTurn.Add(actor);
    }
}
