using TeamVacationPlanner.EspnApi;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TeamVacationPlanner.Mvc.Models;
using System.Text.Json;
using System.Text;

namespace TeamVacationPlanner.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private const string FavoriteTeamsSessionKey = "FavoriteTeams";
        private readonly List<SportData> _sports = new()
        {
            new SportData { Id = "MLB", Name = "Baseball" },
            new SportData { Id = "NBA", Name = "Basketball" },
            new SportData { Id = "NFL", Name = "Football" },
            new SportData { Id = "NHL", Name = "Hockey" },
            new SportData { Id = "MLS", Name = "Soccer" }
        };

        private readonly ILogger<HomeController> _logger;
        private readonly ESPNApi _espnApi;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _espnApi = new ESPNApi();
        }
        
        public async Task<IActionResult> Index()
        {
            var data = GetFavoriteTeamsFromSession();
            var model = new FavoriteTeamModel
            {
                Sports = _sports,
                NumberOfDays = data.NumberOfDays,
                FavoriteTeams = data.FavoriteTeams
            };

            if (model.FavoriteTeams.Any())
            {
                var (Events, Error) = await _espnApi.GetOverlappingEvents(model.NumberOfDays, model.FavoriteTeams);
                if (!string.IsNullOrWhiteSpace(Error))
                    model.Errors = Error;

                model.Events.AddRange(Events);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(FavoriteTeamModel model)
        {
            if (ModelState.IsValid)
            {
                var hasChanges = false;
                var data = GetFavoriteTeamsFromSession();
                if (data.NumberOfDays != model.NumberOfDays)
                {
                    data.NumberOfDays = model.NumberOfDays;
                    hasChanges = true;
                }
                if (model.FavoriteTeam.ToLowerInvariant() == "colan")
                {
                    data.FavoriteTeams.Add("NFL", "DEN");
                    data.FavoriteTeams.Add("MLB", "MIN");
                    data.FavoriteTeams.Add("NBA", "MIN");
                    data.FavoriteTeams.Add("NHL", "MIN");
                    data.FavoriteTeams.Add("MLS", "MIN");
                    hasChanges = true;
                }
                else if (!data.FavoriteTeams.ContainsKey(model.SelectedSportId) || !data.FavoriteTeams.ContainsValue(model.FavoriteTeam))
                {
                    data.FavoriteTeams[model.SelectedSportId] = model.FavoriteTeam;
                    hasChanges = true;
                }

                if (hasChanges)
                    SaveFavoriteTeamsToSession(data);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private FavoriteTeamModelBase GetFavoriteTeamsFromSession()
        {
            var byteArrayData = HttpContext.Session.Get(FavoriteTeamsSessionKey);
            if (byteArrayData != null)
            {
                var serializedData = Encoding.UTF8.GetString(byteArrayData);
                var json = JsonSerializer.Deserialize<FavoriteTeamModelBase>(serializedData);
                return json;
            }

            return new FavoriteTeamModelBase();
        }

        private void SaveFavoriteTeamsToSession(FavoriteTeamModelBase model)
        {
            var serializedData = JsonSerializer.Serialize(model);
            var byteArrayData = Encoding.UTF8.GetBytes(serializedData);
            HttpContext.Session.Set(FavoriteTeamsSessionKey, byteArrayData);
        }
    }
}