﻿namespace WuzlStats.ViewModels.Home
{
    public class PlayerResult
    {
        public string PlayerName { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

        public double Order
        {
            get
            {
                var wins = (double)Wins;
                var losses = (double)Losses;

                if (losses <= 0)
                    return wins;
                if (wins <= 0)
                    return 0.1 / losses;
                return wins / losses;
            }
        }
    }
}