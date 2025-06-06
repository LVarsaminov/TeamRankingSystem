using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TeamRanking.Core.Models
{
    public class Team
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public int Points { get; set; } = 0;


        public int PlayedMatchesCount { get; set; } = 0;
    }
}
