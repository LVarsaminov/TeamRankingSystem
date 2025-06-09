using System;
using TeamRanking.Core.Models;

namespace TeamRanking.Core.Models
{
    public class Match
    {
        public int Id { get; set; }

        public int Team1Id { get; set; }
        public int Team2Id { get; set; }

        public string Team1Name { get; set; }
        public string Team2Name { get; set; }

        public int Team1Score { get; set; }
        public int Team2Score { get; set; }

        public DateTime MatchDate { get; set; }

        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
    }
}
