using Godot;
using System;

public partial class GodotSoundManager : Node
{
    public AudioStreamPlayer PlaySound(AudioStream stream)
    {
        var player = CreateTempAudioStreamPlayer(stream);
        Services.AddNodeToGameRoot(player);
        player.Play();
        return player;
    }

    public AudioStreamPlayer PlayMusic(AudioStream stream) => PlaySound(stream);

    public AudioStreamPlayer CreateTempAudioStreamPlayer(AudioStream stream)
    {
        var player = new AudioStreamPlayer() { Stream = stream };
        player.Finished += player.QueueFree;
        return player;
    }
}
