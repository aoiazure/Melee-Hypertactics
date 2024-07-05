using System.Collections.Generic;

sealed class ByInitiativeAscending : IComparer<GameActor>
{
    public int Compare(GameActor x, GameActor y) => x.Initiative.CompareTo(y.Initiative);
}