
namespace WuzlStats.ViewModels.Player
{
    public class TeamPlayerStats : IPlayerStats
    {
        #region IPlayerStats

        public int WinsCount { get; set; }
        public int LossesCount { get; set; }
        public string FavoriteTeam { get; set; }
        public string FavoriteOpponent { get; set; }

        #endregion IPlayerStats

        public string FavoritePartner { get; set; }
        public string FavoritePosition { get; set; }
    }
}