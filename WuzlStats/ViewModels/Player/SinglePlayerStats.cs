
namespace WuzlStats.ViewModels.Player
{
    public class SinglePlayerStats : IPlayerStats
    {
        public int WinsCount { get; set; }
        public int LossesCount { get; set; }
        public string FavoriteTeam { get; set; }
    }
}