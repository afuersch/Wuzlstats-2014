
namespace WuzlStats.ViewModels.Player
{
    /// <summary>
    /// A common interface for statistics of a player.
    /// </summary>
    public interface IPlayerStats
    {
        int WinsCount { get; set; }
        int LossesCount { get; set; }
        string FavoriteTeam { get; set; }
        string FavoriteOpponent { get; set; }
    }
}