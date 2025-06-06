using System;

namespace TeamRanking.Core.DTOs
{
    public class UpdateMatchDto
    {
        public int? Team1Score { get; set; }
        public int? Team2Score { get; set; }
        public DateTime MatchDate { get; set; }
    }
}
