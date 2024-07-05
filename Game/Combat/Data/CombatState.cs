using System.Collections.Generic;

public class CombatState
{
    public List<GameActor> GameActors = [];
    public Dictionary<string, List<Command>> ActorActionHistory = [];

    public Godot.Collections.Dictionary ToJson()
    {
        using Godot.Collections.Dictionary baseDictionary = [];
        var actorsDict = new Godot.Collections.Dictionary();
        foreach (GameActor actor in GameActors)
        {
            var dict = new Godot.Collections.Dictionary();
            var name = actor.ActorDetails.Name;
            var actorDict = new Godot.Collections.Dictionary
            {
                // Controller
                ["Controller"] = actor.Controller.GetType().ToString(),
                // Initiative
                ["Initiative"] = actor.Initiative,
                // Grid position
                ["GridPosition"] = new Godot.Collections.Dictionary()
                {
                    ["X"] = actor.GridPosition.X,
                    ["Y"] = actor.GridPosition.Y
                },
                ["Faction"] = actor.Faction.ToString(),
                // Details
                ["GameActorDetails"] = new Godot.Collections.Dictionary
                {
                    ["Name"] = name,
                    ["PortraitPath"] = actor.ActorDetails.Portrait.ResourcePath
                },
                // Stats
                ["GameActorStats"] = new Godot.Collections.Dictionary
                {
                    ["MaximumHealth"] = actor.ActorStats.MaximumHealth,
                    ["CurrentHealth"] = actor.ActorStats.CurrentHealth
                }
            };

            actorsDict[name] = actorDict;
        }

        baseDictionary["GameActors"] = actorsDict;
        return baseDictionary;
    } 
}