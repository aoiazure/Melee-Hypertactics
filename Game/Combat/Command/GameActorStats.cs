using System;
using Godot;

public class GameActorStats
{
    public int MaximumHealth;
    public int CurrentHealth;

    public GameActorStats(int maxHealth, int curHealth)
    {
        MaximumHealth = maxHealth;
        CurrentHealth = curHealth;
    }

    public int Damage(DamageData data, bool blocked = false)
    {
        var amount = data.Amount;
        if (blocked) amount = Mathf.CeilToInt((float)amount/2);

        CurrentHealth = Math.Clamp(CurrentHealth - amount, 0, MaximumHealth);
        GD.Print($"{CurrentHealth}/{MaximumHealth}");
        return amount;
    }
}