using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public class GameActor
{
    
    public GameActorDetails ActorDetails;
    public GameActorStats ActorStats;

    public Vector2I GridPosition;

    public Faction Faction;

    private float initiative;
    public float Initiative { get => initiative + (float)Speed / 100; set => initiative = value; }
    public readonly int Speed = 0;


    public BaseGameActorController Controller;

    private PriorityQueue<Status, int> Statuses = new();
    private readonly List<Status> StatusTypes = [];
    private List<Command> commands = [];
    private int current = 0;

    public async Task StartTurn()
    {
        await Controller.StartTurn();
    }

    public async Task EndTurn()
    {
        await Controller.EndTurn();
    }

    #region Execution

    public List<Command> Commands { get { return commands; } set { commands = value; } }

    public void Redo(int levels)
    {
        for (int i = 0; i < levels; i++) 
            if (current < commands.Count - 1) commands[current++].Execute();
    }

    public void Undo(int levels)
    {
        for (int i = 0; i < levels; i++) 
            if (current > 0) commands[--current].UnExecute();
    }

    public async Task Execute(Command command)
    {
        await command.Execute();
        commands.Add(command);
        current += 1;
    }

    #endregion

    // Returns a set of all positions this actor can currently move to.
    public HashSet<Vector2I> GetMovablePositions()
    {
        HashSet<Vector2I> set = [ GridPosition ];

        for (int x = 1; x < 3; x++)
        {
            var coord = new Vector2I(GridPosition.X + x, GridPosition.Y);
            if (Combat.GetGrid().IsWithinBounds(coord) && Combat.IsPointWalkable(coord)) set.Add(coord);
            else break;
        }
        for (int x = -1; x > -3; x--)
        {
            var coord = new Vector2I(GridPosition.X + x, GridPosition.Y);
            if (Combat.GetGrid().IsWithinBounds(coord) && Combat.IsPointWalkable(coord)) set.Add(coord);
            else break;
        }

        for (int y = 1; y < 3; y++)
        {
            var coord = new Vector2I(GridPosition.X, GridPosition.Y + y);
            if (Combat.GetGrid().IsWithinBounds(coord) && Combat.IsPointWalkable(coord)) set.Add(coord);
            else break;
        }
        for (int y = -1; y > -3; y--)
        {
            var coord = new Vector2I(GridPosition.X, GridPosition.Y + y);
            if (Combat.GetGrid().IsWithinBounds(coord) && Combat.IsPointWalkable(coord)) set.Add(coord);
            else break;
        }

        return set; 
    }

    public int GetStatusCount() => Statuses.Count;

    public void PrintStatuses()
    {
        var msg = "";
        var newStatuses = new PriorityQueue<Status, int>();
        for(int i = 0; i < Statuses.Count; i++)
        {
            Statuses.TryDequeue(out Status status, out int _);
            msg += status.GetType();
            newStatuses.Enqueue(status, status.Priority);
        }
        GD.Print(msg);
        Statuses = newStatuses;
    }

    public void AddStatus<T>() where T : Status => AddStatus(default(T));
    public void AddStatus(Status status)
    {
        Statuses.Enqueue(status, status.Priority);
        StatusTypes.Add(status);
        GD.Print("Count:", Statuses.Count, StatusTypes.Count);
        PrintStatuses();
    }

    public bool HasStatus<T>() where T : Status => StatusTypes.Any(x => x is T);

    public async Task ApplyStatuses()
    {
        GD.Print("Count while applying:", Statuses.Count, StatusTypes.Count);
        var newStatuses = new PriorityQueue<Status, int>();
        for(int i = 0; i < Statuses.Count; i++)
        {
            Statuses.TryDequeue(out Status status, out int priority);
            await status.Apply(this);
            status.Duration--;
            if (status.Duration > 0) newStatuses.Enqueue(status, status.Duration);
            else StatusTypes.Remove(status);
        }
        
        Statuses = newStatuses;
    }

    public void Move(int x, int y)
    {
        GridPosition = new(x, y);
        Combat.UpdateActorPosition(this, GridPosition);
    }

    public void Damage(GameActor source, DamageData data)
    {
        if (ActorStats == default) return;
        SDamage damageStatus = new()
        {
            DamageData = data with { SourceActor = source },
        };
        AddStatus(damageStatus);
        GD.Print("Count after being damaged:", Statuses.Count);
    }
}
