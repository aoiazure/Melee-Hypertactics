using Godot;

public partial class ActionButton : Button
{
    [Export]
    private Label DescriptionLabel;
    [Export(PropertyHint.MultilineText)]
    private string Description;

    public override void _Ready()
    {
        base._Ready();
        MouseEntered += OnMouseEntered;
    }

    public void OnMouseEntered()
    {
        if (DescriptionLabel != null) DescriptionLabel.Text = Description;
    }
}