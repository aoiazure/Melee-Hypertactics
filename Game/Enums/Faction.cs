using System;

[Flags]
public enum Faction
{
    Unaffiliated = 0,
    Player = 1,
    PlayerAlly = 2,
    Enemy = 4
}