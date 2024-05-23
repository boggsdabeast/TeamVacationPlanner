using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Text.Json;

namespace TeamVacationPlanner.EspnApi
{
    public class ESPNApi
    {
        private const string _geocodeUrl = $"https://atlas.microsoft.com/search/address/json?api-version=1.0&subscription-key=";
        private const string _apiBaseUrl = "https://site.api.espn.com/apis/site/v2/sports";
        private readonly HttpClient _httpClient;
        private readonly List<Event> _hardcodedEvents = new();
        private readonly string _apiKey = string.Empty;
        private readonly double _nullDistance = -1;
        private readonly int _earthRadius = 3959; // Earth's radius in miles
        private readonly Dictionary<string, double> _addressDistance = new();

        public ESPNApi()
        {
            var id = 1;
            var year = 2024;
            _httpClient = new HttpClient();
            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 2, 24),
                Name = "MNUFC @ Austin",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "Q2 Stadium", Address = new Address() { State = "TX" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 3, 9),
                Name = "MNUFC @ Orlando",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "INTER CO Stadium", Address = new Address() { State = "FL" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 3, 30),
                Name = "MNUFC @ Phili",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "Subaru Park", Address = new Address() { State = "PA" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 4, 21),
                Name = "MNUFC @ Charlotte",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "Bank of America Stadium", Address = new Address() { State = "NC" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 5, 4),
                Name = "MNUFC @ Atlanta",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "Mercedes-Benz Stadium", Address = new Address() { State = "GA" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 5, 25),
                Name = "MNUFC @ Colorado",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "Dicks Sporting Goods Park", Address = new Address() { State = "CO" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 5, 29),
                Name = "MNUFC @ LAFC",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "BMO Stadium", Address = new Address() { State = "LA" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 6, 15),
                Name = "MNUFC @ Seattle",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "Lumen Field", Address = new Address() { State = "WA" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 6, 19),
                Name = "MNUFC @ Dallas",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "Toyota Stadium", Address = new Address() { State = "TX" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 6, 29),
                Name = "MNUFC @ Portland",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "Providence Park", Address = new Address() { State = "OR" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 7, 7),
                Name = "MNUFC @ LA",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "Dignity Health Sports Park", Address = new Address() { State = "LA" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 7, 13),
                Name = "MNUFC @ Houston",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "Shell Energy Stadium", Address = new Address() { State = "TX" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 8, 31),
                Name = "MNUFC @ San Jose",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "PayPal Park", Address = new Address() { State = "CA" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 9, 14),
                Name = "MNUFC @ St Louis",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "CITYPARK", Address = new Address() { State = "MO" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 9, 21),
                Name = "MNUFC @ KC",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "Children's Mercy Park", Address = new Address() { State = "KC" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 10, 21),
                Name = "MNUFC @ Salt Lake",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "America First Field", Address = new Address() { State = "UT" } } } }
            });

            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 5, 20),
                Name = "Cargill Plague Week 1",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "Long Beach", Address = new Address() { State = "CA" } } } }
            });

            _hardcodedEvents.Add(new Event()
            {
                Id = id++,
                Date = new DateTime(year, 5, 27),
                Name = "Cargill Plague Week 2",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "Long Beach", Address = new Address() { State = "CA" } } } }
            });

            var host = new HostBuilder()
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            })
            .Build();

            var configuration = host.Services.GetRequiredService<IConfiguration>();
            if (configuration != null)
                _apiKey = configuration["ApiKeys:AzureMaps"];
        }

        public async Task<(List<SportsEvent> Events, List<string> Errors)> GetOverlappingEvents(int numberOfDays, int searchDistance, Dictionary<string, string> favoriteTeams)
        {
            try
            {
                var favTeamId = 0;
                var url = _apiBaseUrl;
                var currentDate = DateTime.UtcNow.Date;
                var events = new List<Event>();
                var items = new List<SportsEvent>();

                foreach (var item in favoriteTeams)
                {
                    switch (item.Key.Trim().ToUpperInvariant())
                    {
                        case "MLB":
                            url = $"{_apiBaseUrl}/baseball/mlb/teams";
                            break;
                        case "NFL":
                            url = $"{_apiBaseUrl}/football/nfl/teams";
                            break;
                        case "NBA":
                            url = $"{_apiBaseUrl}/basketball/nba/teams";
                            break;
                        case "NHL":
                            url = $"{_apiBaseUrl}/hockey/nhl/teams";
                            break;
                        case "MLS":
                            url = $"{_apiBaseUrl}/soccer/usa.1/teams";
                            break;
                        default:
                            break;
                    }

                    var response = await _httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ESPNApiResponse>(json);
                        favTeamId = result.Sports.First().Leagues.Select(x => x.Teams).First().FirstOrDefault(x => x.Team.Abbreviation == item.Value.Trim().ToUpperInvariant()).Team.Id;

                        var response2 = await _httpClient.GetAsync($"{url}/{favTeamId}/schedule");
                        if (response2.IsSuccessStatusCode)
                        {
                            var json2 = await response2.Content.ReadAsStringAsync();
                            var result2 = JsonConvert.DeserializeObject<ESPNApiResponse2>(json2);
                            var filtered = result2.Events.Where(x => x.Date > currentDate && x.Competitions.First().Competitors.FirstOrDefault(x => x.Id == favTeamId).HomeAway.ToLower().Trim() == "away").ToList();

                            if (item.Key.ToUpperInvariant().Trim() == "NFL" && result2.Season.Name.ToLowerInvariant().Trim() == "preseason")
                            {
                                for (int i = 1; i < 19; i++)
                                {
                                    for (int j = DateTime.Now.Year - 1; j < DateTime.Now.Year + 1; j++)
                                    {
                                        var weeklyUrl = $"{_apiBaseUrl}/football/nfl/scoreboard?seasontype=2&week={i}&dates={j}";
                                        var weeklyResponse = await _httpClient.GetAsync(weeklyUrl);
                                        if (weeklyResponse.IsSuccessStatusCode)
                                        {
                                            var weeklyJson = await weeklyResponse.Content.ReadAsStringAsync();
                                            var weeklyResult = JsonConvert.DeserializeObject<ESPNApiResponse2>(weeklyJson);
                                            var weeklyCompetition = weeklyResult.Events.FirstOrDefault(x => x.Competition.Competitors.Any(y => /*y.HomeAway.ToLowerInvariant().Trim() == "away" && */y.Team.Abbreviation.ToUpperInvariant().Trim() == item.Value.ToUpperInvariant().Trim()));
                                            if (weeklyCompetition?.Date > currentDate)
                                                filtered.Add(weeklyCompetition);
                                        }
                                    }
                                }
                            }
                            else if (item.Key.ToUpperInvariant().Trim() == "MLS" && item.Value.ToUpperInvariant().Trim() == "MIN")
                            {
                                foreach (var hardcodeEvent in _hardcodedEvents)
                                {
                                    if (hardcodeEvent.Date >= currentDate)
                                        filtered.Add(hardcodeEvent);
                                }
                            }

                            events.AddRange(filtered);
                        }
                    }
                    else
                    {
                        throw new Exception("Failed to retrieve data from the ESPN API.");
                    }
                }

                var filteredEvents = events;
                List<(Event Event1, Event Event2)> eventsWithDifferentVenues = new();

                for (int i = 0; i < filteredEvents.Count; i++)
                {
                    for (int j = i + 1; j < filteredEvents.Count; j++)
                    {
                        var eventA = filteredEvents[i];
                        var eventB = filteredEvents[j];

                        var venueA = eventA.Competitions[0].Venue;
                        var venueB = eventB.Competitions[0].Venue;

                        // Check if both events are within the rangeInDays
                        var eventDateRange = Math.Abs((eventB.Date - eventA.Date).Days);
                        if (eventDateRange <= numberOfDays)
                        {
                            var locationA = venueA.Address.StateName ?? venueA.Address.City.Split(',').Last().Trim();
                            var locationB = venueB.Address.StateName ?? venueB.Address.City.Split(',').Last().Trim();

                            var distance = _nullDistance;
                            var key = $"{eventA.Name} - {eventB.Name}";
                            if (_addressDistance.TryGetValue(key, out var distanceApart))
                            {
                                distance = distanceApart;
                            }
                            else
                            {
                                distance = await GetDistanceBetweenAddressesAsync($"{venueA.FullName} {venueA.AddressName}", $"{venueB.FullName} {venueB.AddressName}");
                                _addressDistance.Add(key, distance);
                            }

                            // Compare the venues of the two events
                            if (venueA.FullName != venueB.FullName && (locationA == locationB || distance <= searchDistance))
                            {
                                items.Add(new SportsEvent
                                {
                                    CompetitionA = eventA.Name,
                                    CompetitionB = eventB.Name,
                                    LocationA = $"{venueA.AddressName} ({venueA.FullName})",
                                    LocationB = $"{venueB.AddressName} ({venueB.FullName})",
                                    DateTimeA = eventA.Date,
                                    DateTimeB = eventB.Date,
                                    Distance = distance
                                });

                                eventsWithDifferentVenues.Add((eventB, eventA));
                            }
                        }
                    }
                }

                return (items.Distinct().OrderBy(x => x.DateTimeA).ToList(), new List<string>());
        }
            catch (Exception ex)
            {
                var errorMessage = new List<string>() { "Please re-enter data in proper format to get valid results." };
#if DEBUG
                if (!string.IsNullOrWhiteSpace(ex.Message))
                    errorMessage.Add(ex.Message);
                if (ex.InnerException != null)
                    errorMessage.Add(ex.InnerException.ToString());
                if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                    errorMessage.Add(ex.StackTrace);
#endif

                return (new List<SportsEvent>(), errorMessage);
            }
}

        private async Task<double> GetDistanceBetweenAddressesAsync(string address1, string address2)
        {
            if (!string.IsNullOrWhiteSpace(_apiKey))
            {
                using var httpClient = new HttpClient();
                
                // Step 1: Get latitude and longitude for address1
                var response = await httpClient.GetAsync($"{_geocodeUrl}{_apiKey}&query={Uri.EscapeDataString(address1)}");
                response.EnsureSuccessStatusCode();
                var (lat1, lng1) = await GetCoordinatesFromAzureMapsAsync(response);

                // Step 2: Get latitude and longitude for address2
                var response2 = await httpClient.GetAsync($"{_geocodeUrl}{_apiKey}&query={Uri.EscapeDataString(address2)}");
                response2.EnsureSuccessStatusCode();
                var (lat2, lng2) = await GetCoordinatesFromAzureMapsAsync(response2);

                // Step 3: Calculate distance using Haversine formula (you can use other formulas as well)                
                var dLat = DegreeToRadian(lat2 - lat1);
                var dLng = DegreeToRadian(lng2 - lng1);
                var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                           Math.Cos(DegreeToRadian(lat1)) * Math.Cos(DegreeToRadian(lat2)) *
                           Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
                var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                var distance = _earthRadius * c;

                return Math.Round(distance, 2);
            }

            return _nullDistance;
        }

        private async Task<(double, double)> GetCoordinatesFromAzureMapsAsync(HttpResponseMessage response)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(jsonResponse);
            if (result.RootElement.TryGetProperty("results", out JsonElement resultsElement) && resultsElement.GetArrayLength() > 0)
            {
                var position = result.RootElement
                    .GetProperty("results")[0]
                    .GetProperty("position");
                double latitude = position.GetProperty("lat").GetDouble();
                double longitude = position.GetProperty("lon").GetDouble();
                return (latitude, longitude);
            }
            else
            {
                throw new Exception($"No results found for the specified address. {result.RootElement}");
            }
        }

        private double DegreeToRadian(double degree)
        {
            return degree * Math.PI / 180;
        }
    }
}