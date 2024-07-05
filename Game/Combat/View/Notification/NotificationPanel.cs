using Godot;
using System;

public partial class NotificationPanel : Control
{
    [Signal]
    public delegate void AnimateFinishedEventHandler(NotificationPanel panel);

    public const int VerticalOffset = 175;

    [Export]
    private Label Label;
    [Export]
    public PanelContainer Container;

    public override void _Ready()
    {
        base._Ready();
        Animate();
    }

    public void SetMessage(string message)
    {
        Label.Text = message;
    }

    public void Animate()
    {
        var tween = CreateTween();
        tween.TweenProperty(Container, "position:x", 0, .25);
        tween.Parallel().TweenProperty(Container, "custom_minimum_size:x", 1600, 0.25);
        tween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad);
        tween.Chain().TweenProperty(Container, "custom_minimum_size:x", 2000, 2);
        tween.Chain().TweenProperty(Container, "position:x", 1600, .25);
        tween.Parallel().TweenProperty(Container, "custom_minimum_size:x", 0, .25);
        
        tween.Finished += () => EmitSignal(SignalName.AnimateFinished, this);
        tween.Play();
    }
}
