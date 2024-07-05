using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public class PlayerActorController : BasePlayerController
{
    private bool HasMoved = false;

    public PlayerActorController(GameActor player, PlayerInterfaceView playerInterfaceView)
    {
        Actor = player;
        InterfaceView = playerInterfaceView;
        InterfaceView.Actor = Actor;
        InterfaceView.Controller = this;
    }

    public override async Task StartTurn()
    {
        // Enable input
        InterfaceView.SetActive(true);
        await Task.Yield();
    }

    public override async Task DecideMovement()
    {
        if (HasMoved) return;

        MovablePositions = Actor.GetMovablePositions();
        var msg = "";
        foreach (Vector2I v in MovablePositions) 
        {
            InterfaceView.MoveHighlight.PlaceMovementHighlight(v);
            msg += $"{v} ";
        }
        GD.Print($"Movable {msg}");

        while (!HasMoved)
        {
            await Services.EventBus.ToSignal(Services.EventBus, EventBus.SignalName.PlayerActionSelected);
            if (QueuedCommand != null && QueuedCommand is MoveCommand)
            {
                HasMoved = true;
                return;
            }
        }
    }

    public override async Task DecideAction()
    {
        InterfaceView.SelectAction();
        await Services.EventBus.ToSignal(Services.EventBus, EventBus.SignalName.PlayerActionSelected);
    }



    public override async Task EndTurn()
    {
        HasMoved = false;
        QueuedCommand = null;
        MovablePositions.Clear();
        InterfaceView.SetActive(false);
        await Task.Yield();
    }

}