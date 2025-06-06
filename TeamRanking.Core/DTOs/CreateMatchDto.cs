using System;

namespace TeamRanking.Core.DTOs
{
    public class CreateMatchDto
    {
        public string Team1Name { get; set; }
        public string Team2Name { get; set; }
        public int? Team1Score { get; set; }
        public int? Team2Score { get; set; }
        public DateTime MatchDate { get; set; }
    }
}