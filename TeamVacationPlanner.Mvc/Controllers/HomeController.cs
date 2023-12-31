﻿using TeamVacationPlanner.EspnApi;
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
                Distance = data.Distance,
                FavoriteTeams = data.FavoriteTeams
            };

            if (model.FavoriteTeams.Any())
            {
                var (Events, Errors) = await _espnApi.GetOverlappingEvents(model.NumberOfDays, model.Distance, model.FavoriteTeams);
                if (Errors.Any())
                {
                    model.Errors = Errors;
                    SaveFavoriteTeamsToSession(new FavoriteTeamModelBase());
                }

                model.Events.AddRange(Events);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(FavoriteTeamModel model)
        {
            if (model.FavoriteTeam?.ToLowerInvariant()?.Trim() == "colan")
            {
                SaveFavoriteTeamsToSession(new FavoriteTeamModelBase()
                {
                    NumberOfDays = model.NumberOfDays,
                    Distance = model.Distance,
                    FavoriteTeams = new Dictionary<string, string>
                    {
                        { "NFL", "DEN" },
                        { "MLB", "MIN" },
                        { "NBA", "MIN" },
                        { "NHL", "MIN" },
                        { "MLS", "MIN" }
                    }
                });
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                var hasChanges = false;
                var data = GetFavoriteTeamsFromSession();
                if (data.NumberOfDays != model.NumberOfDays)
                {
                    data.NumberOfDays = model.NumberOfDays;
                    hasChanges = true;
                }
                if (data.Distance != model.Distance)
                {
                    data.Distance = model.Distance;
                    hasChanges = true;
                }
                if (!data.FavoriteTeams.ContainsKey(model.SelectedSportId) || !data.FavoriteTeams.ContainsValue(model.FavoriteTeam))
                {
                    data.FavoriteTeams[model.SelectedSportId] = model.FavoriteTeam.ToUpperInvariant().Trim();
                    hasChanges = true;
                }

                if (hasChanges)
                    SaveFavoriteTeamsToSession(data);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Reset()
        {
            SaveFavoriteTeamsToSession(new FavoriteTeamModelBase());
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