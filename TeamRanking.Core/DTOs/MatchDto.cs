using System;

public class MatchDto
{
    public int Id { get; set; }
    public int? Team1Score { get; set; }
    public int? Team2Score { get; set; }
    public DateTime MatchDate { get; set; }

    public string Team1Name { get; set; }
    public string Team2Name { get; set; }
}