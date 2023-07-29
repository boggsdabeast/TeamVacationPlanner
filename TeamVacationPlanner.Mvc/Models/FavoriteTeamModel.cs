using TeamVacationPlanner.EspnApi;

namespace TeamVacationPlanner.Mvc.Models
{
    public class FavoriteTeamModelBase
    {
        public int NumberOfDays { get; set; } = 7;
        public Dictionary<string, string> FavoriteTeams { get; set; } = new();
    }

    public class FavoriteTeamModel : FavoriteTeamModelBase
    {
        public string SelectedSportId { get; set; }
        public string FavoriteTeam { get; set; }
        public List<SportData> Sports { get; set; } = new();
        public List<SportsEvent> Events { get; set; } = new();
        public List<string> Errors = new();
    }
}