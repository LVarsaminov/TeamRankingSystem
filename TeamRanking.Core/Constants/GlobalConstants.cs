namespace TeamRanking.Core.Constants
{
    public static class GlobalConstants
    {
        public static string teamNameDoesNotExistMessage = "Team with this name does not exist!";
        public static string teamIdDoesNotExistMessage = "Team with this Id does not exist!";
        public static string matchIdDoesNotExistMessage = "Match with this Id does not exist!";

        public static string updatedMessage = "Updated successfuly!";
        public static string teamAlreadyExistsMessage = "This team already exists";
        public static string deletedSuccessfulyMessage = "Deleted!";
        public static string deletedTeamUnsuccessfulyMessage = "Cannot delete this team because it is referenced in a match.";
        public static string noMatchesExistingMessage = "There are currently no matches to be displayed.";
        public static string noTeamsExistingMessage = "There are currently no teams to be displayed.";
    }
}
