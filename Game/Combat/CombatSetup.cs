
using System;
using System.Linq;
using Godot;
using Godot.Collections;

public static partial class Combat
{
    public static void SetUpAStar(Grid grid, TileMapLayer environment)
    {
        Grid = grid;
        Environment = environment;
        AstarGrid = new()
        {
            // CellSize = tileMap.TileSet.TileSize,
            Region = Environment.GetUsedRect(),
            DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never
        };
        
        var region = AstarGrid.Region;
        var usedCells = Environment.GetUsedCells();
        Vector2I coord;
        for (int x = region.Position.X; x < region.End.X; x++)
        {
            for (int y = region.Position.Y; y < region.End.Y; y++)
            {
                coord = new(x, y);
                if (!usedCells.Contains(coord)) AstarGrid.SetPointSolid(coord, true);
            }
        }
        AstarGrid.Update();
    }

    public static CombatState ReadJsonToCombatState(Dictionary json)
    {
        CombatState combatState = new();
        
        if (json.ContainsKey("GameActors"))
        {
            var gameActors = (Dictionary<string, Dictionary>)json["GameActors"];
            foreach (var actorDict in gameActors)
            {
                GameActor actor = new();
                var actorTag = actorDict.Value;
                
                if(actorTag.ContainsKey("Controller"))
                {
                    var controllerName = (string)actorTag["Controller"];
                    actor.Controller = CreateControllerFromName(actor, controllerName);
                }

                if (actorTag.ContainsKey("Initiative")) actor.Initiative = (int)actorTag["Initiative"];
                if (actorTag.ContainsKey("GridPosition"))
                {
                    var dict = (Dictionary<string, int>)actorTag["GridPosition"];
                    dict.TryGetValue("X", out int x);
                    dict.TryGetValue("Y", out int y);
                    actor.GridPosition = new(x, y);
                }
                if (actorTag.ContainsKey("Faction")) 
                {
                    actor.Faction = (Faction)Enum.Parse(typeof(Faction), (string)actorTag["Faction"]);
                }
                if (actorTag.ContainsKey("GameActorDetails"))
                {
                    var dict = (Dictionary<string, string>)actorTag["GameActorDetails"];
                    GameActorDetails details = new();
                    if (dict.TryGetValue("Name", out string name)) details.Name = name;
                    if (dict.TryGetValue("PortraitPath", out string path)) details.Portrait = GD.Load<Texture2D>(path);
                    if (dict.TryGetValue("SpritePath", out string sprite)) details.SpriteTexture = GD.Load<Texture2D>(sprite);
                    dict.TryGetValue("SpriteOffsetX", out string x);
                    dict.TryGetValue("SpriteOffsetY", out string y);
                    if (x.IsValidInt() && y.IsValidInt()) details.SpriteOffset = new(x.ToInt(), y.ToInt());

                    actor.ActorDetails = details;
                }
                if (actorTag.ContainsKey("GameActorStats"))
                {
                    var dict = (Dictionary<string, int>)actorTag["GameActorStats"];
                    dict.TryGetValue("MaximumHealth", out int maximumHealth);
                    dict.TryGetValue("CurrentHealth", out int currentHealth);
                    actor.ActorStats = new(maximumHealth, currentHealth);
                }

                combatState.GameActors.Add(actor);
            }
        }

        return combatState;
    }

    private static BaseGameActorController CreateControllerFromName(GameActor actor, string controllerName)
    {
        Type t = Type.GetType(controllerName);
        if (t == typeof(PlayerActorController))
        {
            PlayerActorController controller = new(actor, InterfaceView);
            return controller;
        }
        if (t == typeof(FighterActorController)) return new FighterActorController(actor);
        if (t == typeof(DummyActorController)) return new DummyActorController(actor);

        return null;
    }

    public static void ReadCombatState(CombatState combatState)
    {
        GameActors = new BasicInitiative();
        GameActorViewHandler.ClearViews();

        foreach (GameActor actor in combatState.GameActors)
        {
            AddGameActor(actor);
            GameActorViewHandler.AddNewActorView(actor);
            if (actor.Controller is PlayerActorController) Player = actor;
        }
    }

    public static CombatState CreateCombatState()
    {
        CombatState combatState = new();

        foreach (GameActor actor in GameActors)
        {
            combatState.GameActors.Add(actor);
        }
        return combatState;
    }

    public static void ReplayCombatState(CombatState state)
    {
        GameActorViewHandler.ClearViews();
        GameActors = new BasicInitiative();

        foreach (var actor in state.GameActors)
        {
            AddGameActor(actor);
            GameActorViewHandler.AddNewActorView(actor);
            System.Collections.Generic.List<Command> history = state.ActorActionHistory[actor.ActorDetails.Name];
            actor.Controller = new ReplayActorController(actor, history);
        }
    }

    public static void Replay()
    {
        // TODO: change this to be a dynamic reading; good enough for now tho
        var json = JsonFileReader.ReadJsonAsDictionary("res://Game/Combat/Encounters/TestCombat.json");
        var state = ReadJsonToCombatState(json);
        foreach (GameActor actor in GameActors)
        {
            System.Collections.Generic.List<Command> commands = actor.Commands;
            foreach (Command command in actor.Commands) 
            {
                var c = command;
                c.Actor = null;
                _ = commands.Append(c);
            }
            state.ActorActionHistory[actor.ActorDetails.Name] = commands;
        }
        InterfaceView.ActionMenuContainer.Hide();
        InterfaceView.ResultsMenuContainer.Hide();
        
        GD.Print("--- Running replay ---");
        ReplayCombatState(state);
        BeginCombat();
    }
}
