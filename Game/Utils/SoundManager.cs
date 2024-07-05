using Godot;

public static class SoundManager
{
    public static AudioStreamPlayer PlaySound(AudioStream stream) => Services.SoundManager.PlaySound(stream);
    public static AudioStreamPlayer PlayMusic(AudioStream stream) => Services.SoundManager.PlayMusic(stream);
}