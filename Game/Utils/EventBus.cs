using Godot;

public partial class EventBus : Node
{
    [Signal]
    public delegate void PlayerActionSelectedEventHandler();
    [Signal]
    public delegate void PlayerTurnFinishedEventHandler();

    [Signal]
    public delegate void AllActorsMoveFinishedEventHandler();
    [Signal]
    public delegate void AllActorsActionFinishedEventHandler();
    [Signal]
    public delegate void AllActorsTurnFinishedEventHandler();
}