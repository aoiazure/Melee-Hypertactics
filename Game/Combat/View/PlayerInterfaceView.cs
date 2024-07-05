using System;
using System.Collections.Generic;
using Godot;


[GlobalClass]
public partial class PlayerInterfaceView : Control
{
    [ExportCategory("References")]
    [Export]
    public HighlightTilemap MoveHighlight;
    [Export]
    public HighlightTilemap AttackHighlight;
    [Export]
    public AnimationPlayer PlayerAP;
    [Export]
    public AnimationPlayer EnemyAP;
    [Export]
    public Sprite2D PlayerPortrait;
    [Export]
    public Sprite2D EnemyPortrait;
    [Export]
    public Label DescriptionLabel;
    [Export]
    public Control ActionMenuContainer;
    [Export]
    public TabContainer TabContainer;
    [Export]
    public VBoxContainer MainMenuContainer;
    [Export]
    public VBoxContainer AttackMenuContainer;
    [Export]
    public Control ResultsMenuContainer;

    [ExportGroup("Timer")]
    [Export]
    public Timer TurnTimer;
    [Export]
    public Container TimerContainer;
    [Export]
    public Label TimerLabel;

    [ExportGroup("Buttons")]
    [Export]
    public Button MoveButton;
    [Export]
    public Button AttackButton;
    [Export]
    public Button BlockButton;
    [Export]
    public Button EndTurnButton;
    [ExportSubgroup("Attack Types")]
    [Export]
    public Button ThrustButton;
    [Export]
    public Button SlashButton;
    [Export]
    public Button SweepButton;
    [ExportSubgroup("Results")]
    [Export]
    public Button RewatchButton;
    [Export]
    public Button PlayAgainButton;
    [Export]
    public Button QuitButton;

    public GameActor Actor;
    public PlayerActorController Controller;

    private TimeSpan timeSpan;

    private bool IsActive = false;
    private bool IsWaitingForMove = false;
    // Attacking
    private bool IsWaitingForSelection = false;
    private AoePattern SelectedAttackPattern;
    private PatternRotation SelectionRotation = PatternRotation.Invalid;
    private HashSet<Vector2I> SelectionAoe;

    private readonly AoePattern ThrustPattern = new()
    {
        Tiles = [Vector2I.Right, new(2, 0), new(3, 0)]
    };
    private readonly AoePattern SlashPattern = new()
    {
        Tiles = [Vector2I.Right, new(1, 1), new(0, 1)]
    };
    private readonly AoePattern SweepPattern = new()
    {
        Tiles = [Vector2I.Right, new(1, 1), new(1, -1)]
    };


    public override void _Ready()
    {
        GameActorViewHandler.SetPlayerInterfaceView(this);

        MoveButton.Pressed += OnMoveButtonPressed;
        AttackButton.Pressed += OnAttackButtonPressed;
        BlockButton.Pressed += OnBlockButtonPressed;
        EndTurnButton.Pressed += OnEndTurnButtonPressed;
        
        ThrustButton.Pressed += () => { SelectedAttackPattern = ThrustPattern; SelectAttack(); };
        SlashButton.Pressed += () => { SelectedAttackPattern = SlashPattern; SelectAttack(); };
        SweepButton.Pressed += () => { SelectedAttackPattern = SweepPattern; SelectAttack(); };

        RewatchButton.Pressed += Combat.Replay;
        PlayAgainButton.Pressed += () => GetTree().ReloadCurrentScene();
        QuitButton.Pressed += () => GetTree().Quit();

        TurnTimer.Timeout += OnTurnTimerTimeout;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!TurnTimer.IsStopped())
        {
            timeSpan = TimeSpan.FromSeconds(TurnTimer.TimeLeft); 
            string time = string.Format("{0:D2}:{1:D3}",
                timeSpan.Seconds, timeSpan.Milliseconds);
            TimerLabel.Text = time;
        }
    }

    public void OnTurnTimerTimeout()
    {
        if (IsWaitingForMove)
        {
            SetQueuedCommand(new MoveCommand(Actor, new(Actor.GridPosition.X, Actor.GridPosition.Y)));
            MoveHighlight.Clear();
            IsWaitingForMove = false;
        }
        else
        {
            SetQueuedCommand(new WaitCommand());
        }
    }

    public void SetActive(bool active)
    {
        IsActive = active;
        if (IsActive) StartTurn();
        else EndTurn();
    }

    public void SelectAction()
    {
        if (!TurnTimer.IsStopped())
        {
            TabContainer.CurrentTab = 0;
            ActionMenuContainer.Show();
        } else { SetQueuedCommand(new WaitCommand()); }
    }

    public async void OnMoveButtonPressed()
    {
        MoveButton.Disabled = true;
        IsWaitingForMove = true;
        ActionMenuContainer.Hide();
        await ToSignal(Services.EventBus, EventBus.SignalName.PlayerActionSelected);
        IsWaitingForMove = false;
        ActionMenuContainer.Show();
    }

    public void OnAttackButtonPressed()
    {
        TabContainer.CurrentTab = 1;
    }

    public void OnBlockButtonPressed()
    {
        SetQueuedCommand(new BlockCommand(Actor));
        ActionMenuContainer.Hide();
    }

    public void OnEndTurnButtonPressed()
    {
        SetQueuedCommand(new WaitCommand());
        ActionMenuContainer.Hide();
    }

    private async void SelectAttack()
    {
        IsWaitingForSelection = true;
        ActionMenuContainer.Hide();
        await ToSignal(Services.EventBus, EventBus.SignalName.PlayerActionSelected);
        IsWaitingForSelection = false;
        SelectedAttackPattern = default;
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        if (!IsActive || Controller == null || Actor == null)  return;
        if (Input.IsActionJustPressed("select_grid")) GD.Print("yua");
        MoveActor();
        Attack();
    }


    private void MoveActor()
    {
        if (!IsWaitingForMove || !Input.IsActionJustPressed("select_grid")) return;

        var validPositions = Actor.GetMovablePositions();
        var pos = Combat.SelectGridPositionOnMap();
        GD.Print($"Selected {pos}");
        if (validPositions.Contains(pos))
        {
            SetQueuedCommand(new MoveCommand(Actor, pos.X, pos.Y));
            MoveHighlight.Clear();
            IsWaitingForMove = false;
        }
    }

    private async void Attack()
    {
        if (!IsWaitingForSelection) return;

        var oldRotation = SelectionRotation;
        SelectionRotation = GetRotationFromMouse();

        // If new direction, update highlighting
        if (oldRotation != SelectionRotation)
        {
            AttackHighlight.Clear();
            SelectionAoe = AoePattern.Rotated(SelectedAttackPattern.Tiles, SelectionRotation, Actor.GridPosition);

            AttackHighlight.SetAoe(SelectionAoe);
        }

        if (Input.IsActionJustPressed("select_grid")) 
        {
            SetQueuedCommand(new AttackCommand(Actor, new DamageData(2), SelectionRotation, SelectedAttackPattern, Actor.GridPosition));
            AttackHighlight.Clear();
            TurnTimer.Stop();
            await ToSignal(Services.EventBus, EventBus.SignalName.PlayerActionSelected);
        }
    }

    private void SetQueuedCommand(Command command)
    {
        Controller.QueuedCommand = command;
        Services.EventBus.EmitSignal(EventBus.SignalName.PlayerActionSelected);
        
    }

    private void StartTurn()
    {
        MoveButton.Disabled = false;
        AttackButton.Disabled = false;
        IsWaitingForMove = true;

        DescriptionLabel.Text = "Turn Start.";
        ActionMenuContainer.Hide();
        TurnTimer.Start();
        TimerContainer.Show();
    }

    private void EndTurn()
    {
        ActionMenuContainer.Hide();
        MoveHighlight.Clear();
        AttackHighlight.Clear();
        TimerContainer.Hide();
        TurnTimer.Stop();
        SelectionRotation = PatternRotation.Invalid;
    }

    public void EndCombat()
    {
        ActionMenuContainer.Hide();
        ResultsMenuContainer.Show();
    }


    #region Helpers

    // Returns Rotation of Vector2.Right based on mouse position relative to player actor.
    public PatternRotation GetRotationFromMouse()
    {
        if (!IsActive || Controller == null || Actor == null) return global::PatternRotation.None;

        var currentPosition = Actor.GridPosition;
        var mousePosition = Combat.SelectGridPositionOnMap();
        var direction = ((Vector2)(mousePosition - currentPosition)).Normalized();
        var dot = direction.Dot(Vector2.Right);
        if(dot > 0.5) return global::PatternRotation.None;
        else if(dot < 0.5 && dot > -0.5)
        {
            if (direction.Y > 0) return global::PatternRotation.Quarter;
            else return global::PatternRotation.ThreeQuarter;
        }
        else if(dot < -0.5) return global::PatternRotation.Half;

        return global::PatternRotation.Invalid;
    }

    #endregion
    
}