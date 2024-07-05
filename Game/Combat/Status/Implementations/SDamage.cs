using System.Threading.Tasks;
using Godot;

public class SDamage : Status
{
    private static readonly AudioStreamRandomizer damageRandomizer = GD.Load<AudioStreamRandomizer>("res://Assets/Sounds/Weapons/Sword/SwordSfx.tres");
    private static readonly AudioStreamRandomizer hurtRandomizer = GD.Load<AudioStreamRandomizer>("res://Assets/Sounds/Hurt/BasicHurtSfx.tres");

    public DamageData DamageData;

    public override int Priority { get => 1; set => _ = value; }
    public override async Task Apply(GameActor actor)
    {
        var isBlocking = actor.HasStatus<SBlocking>();
        var damage = actor.ActorStats.Damage(DamageData, isBlocking);

        var player = SoundManager.PlaySound(damageRandomizer);
        await player.ToSignal(player, AudioStreamPlayer.SignalName.Finished);
        
        if (!isBlocking) 
        {
            Services.Log.AddMessage($"{actor.ActorDetails.Name} took {damage}! {actor.ActorStats.CurrentHealth}/{actor.ActorStats.MaximumHealth}");
            SoundManager.PlaySound(hurtRandomizer);
        }
        else 
        {
            Services.Log.AddMessage($"{actor.ActorDetails.Name} blocked, halved {damage}! {actor.ActorStats.CurrentHealth}/{actor.ActorStats.MaximumHealth}");
            SoundManager.PlaySound(SBlocking.blockRandomizer);
        }
        
        await GameActorViewHandler.Damage(actor, DamageData.SourceActor, DamageData);

        if (actor.ActorStats.CurrentHealth <= 0)
        {
            Combat.RegisterDeath(actor);
        }
    }
}