using Godot;

public static class Services
{
    // public readonly static ILogService Log = new GodotLogService();
    // public static ISfxService Sfx;

    private static SceneTree _sceneTree;
    public static SceneTree SceneTree { get { return null; } set { _sceneTree = value; } }

    private static Node _gameTree;
    public static Node GameWorldTree { get => null; set => _gameTree = value; }

    public static EventBus EventBus { get => _sceneTree.Root.GetNode<EventBus>("/root/EventBus/"); }
    public static LogMenu Log { get => _sceneTree.Root.GetNode<LogMenu>("/root/NotificationLog/"); }
    public static GodotSoundManager SoundManager { get => _sceneTree.Root.GetNode<GodotSoundManager>("/root/GodotSoundManager"); }

    public static SceneTreeTimer CreateTimer(
        float time, bool processAlways = true, bool processInPhysics = false, bool ignoreTimeScale = false
            ) => _sceneTree.CreateTimer(time, processAlways, processInPhysics, ignoreTimeScale);

    public static SignalAwaiter AwaitProcessFrame()
    {
        return _sceneTree.ToSignal(_sceneTree, SceneTree.SignalName.ProcessFrame);
    }

    public static SignalAwaiter AwaitTimerTimeout(float time) 
    {
        var timer = CreateTimer(time);
        return timer.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
    }

    public static void AddNodeToGameRoot(Node node) => _gameTree.AddChild(node);

    public static void AddNodeToSceneRoot(Node node) => _sceneTree.Root.AddChild(node);

    public static double GetTimeElapsedInSecondsSince(ulong timeStamp) => (Time.GetTicksMsec() - timeStamp) / 1000;
}