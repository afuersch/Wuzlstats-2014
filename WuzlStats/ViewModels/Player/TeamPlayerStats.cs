
namespace WuzlStats.ViewModels.Player
{
    public class TeamPlayerStats : IPlayerStats
    {
        public int WinsCount { get; set; }
        public int LossesCount { get; set; }
        public string FavoriteTeam { get; set; }
        public string FavoritePartner { get; set; }
        public string FavoritePosition { get; set; }
    }
}