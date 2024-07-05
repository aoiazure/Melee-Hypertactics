using Godot;
using System;
using System.Collections.Generic;

public partial class LogMenu : CanvasLayer
{
    private readonly static PackedScene NotificationScene = GD.Load<PackedScene>("res://Game/Combat/View/Notification/Notification.tscn");

    private List<NotificationPanel> notifications = [];

    public void AddMessage(string message)
    {
        var notification = NotificationScene.Instantiate<NotificationPanel>();
        notification.SetMessage(message);
        notifications.Add(notification);

        notification.AnimateFinished += OnNotificationFinished;

        AddChild(notification);
        notification.Position = notification.Position with {Y = (notifications.Count - 1) * NotificationPanel.VerticalOffset };
    }

    private void OnNotificationFinished(NotificationPanel panel)
    {
        notifications.Remove(panel);
        panel.QueueFree();
    }
}
