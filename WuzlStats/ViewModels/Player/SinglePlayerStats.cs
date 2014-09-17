
namespace WuzlStats.ViewModels.Player
{
    public class SinglePlayerStats : IPlayerStats
    {
        #region IPlayerStats

        public int WinsCount { get; set; }
        public int LossesCount { get; set; }
        public string FavoriteTeam { get; set; }
        public string FavoriteOpponent { get; set; }

        #endregion IPlayerStats
    }
}