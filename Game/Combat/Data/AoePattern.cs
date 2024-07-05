using System;
using System.Collections.Generic;
using Godot;
using System.Linq;

public struct AoePattern
{
    // Forward is Vector2I.Right
    public HashSet<Vector2I> Tiles;

    public static HashSet<Vector2I> Rotated(HashSet<Vector2I> Cells, PatternRotation type, Vector2I origin)
    {
        var set = new HashSet<Vector2I>();
        foreach (var rotated in from Vector2I v in Cells
                                let rotated = RotateVector(v, type)
                                where rotated != Vector2I.Zero
                                select rotated)
        {
            set.Add(origin + rotated);
        }

        return set;
    }

    public static Vector2I RotateVector(Vector2I vector, PatternRotation type)
    {
        var vec = (Vector2) vector;
        Vector2 vector2 = type switch
        {
            PatternRotation.None => vector,
            PatternRotation.Quarter => vec.Rotated((float)(Math.PI / 2)),
            PatternRotation.Half => vec.Rotated((float)Math.PI),
            PatternRotation.ThreeQuarter => vec.Rotated((float)(3 * Math.PI / 2)),
            PatternRotation.Invalid => throw new NotImplementedException(),
            // Return empty
            _ => Vector2I.Zero,
        };

        return (Vector2I)vector2.Round();
    }
}