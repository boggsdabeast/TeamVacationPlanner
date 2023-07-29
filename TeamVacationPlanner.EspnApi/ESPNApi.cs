using Newtonsoft.Json;

namespace TeamVacationPlanner.EspnApi
{
    public class ESPNApi
    {
        private const string ApiBaseUrl = "https://site.api.espn.com/apis/site/v2/sports";
        private readonly HttpClient _httpClient;
        private readonly List<Event> _hardcodedEvents = new();

        public ESPNApi()
        {
            _httpClient = new HttpClient();
            _hardcodedEvents.Add(new Event()
            {
                Id = 1,
                Date = new DateTime(2023, 8, 20),
                Name = "MNUFC @ NYC",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "Citi Field", Address = new Address() { State = "NY" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = 2,
                Date = new DateTime(2023, 9, 2),
                Name = "MNUFC @ San Jose",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "PayPal Park", Address = new Address() { State = "CA" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = 3,
                Date = new DateTime(2023, 9, 20),
                Name = "MNUFC @ LA",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "Dignity Health Sports Park", Address = new Address() { State = "CA" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = 4,
                Date = new DateTime(2023, 10, 4),
                Name = "MNUFC @ LAFC",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "BMO Stadium", Address = new Address() { State = "CA" } } } }
            });
            _hardcodedEvents.Add(new Event()
            {
                Id = 5,
                Date = new DateTime(2023, 10, 21),
                Name = "MNUFC @ KC",
                Competitions = new List<Competition> { new Competition() { Venue = new Venue() { FullName = "Children's Mercy Park", Address = new Address() { State = "KC" } } } }
            });
        }

        public async Task<(List<SportsEvent> Events, List<string> Errors)> GetOverlappingEvents(int numberOfDays, Dictionary<string, string> favoriteTeams)
        {
            try
            {
                var favTeamId = 0;
                var url = ApiBaseUrl;
                var currentDate = DateTime.Now;
                var events = new List<Event>();
                var items = new List<SportsEvent>();

                foreach (var item in favoriteTeams)
                {
                    switch (item.Key.TrimEnd().ToUpperInvariant())
                    {
                        case "MLB":
                            url = $"{ApiBaseUrl}/baseball/mlb/teams";
                            break;
                        case "NFL":
                            url = $"{ApiBaseUrl}/football/nfl/teams";
                            break;
                        case "NBA":
                            url = $"{ApiBaseUrl}/basketball/nba/teams";
                            break;
                        case "NHL":
                            url = $"{ApiBaseUrl}/hockey/nhl/teams";
                            break;
                        case "MLS":
                            url = $"{ApiBaseUrl}/soccer/usa.1/teams";
                            break;
                        default:
                            break;
                    }

                    var response = await _httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ESPNApiResponse>(json);
                        favTeamId = result.Sports.First().Leagues.Select(x => x.Teams).First().FirstOrDefault(x => x.Team.Abbreviation == item.Value.TrimEnd().ToUpperInvariant()).Team.Id;

                        var response2 = await _httpClient.GetAsync($"{url}/{favTeamId}/schedule");
                        if (response2.IsSuccessStatusCode)
                        {
                            var json2 = await response2.Content.ReadAsStringAsync();
                            var result2 = JsonConvert.DeserializeObject<ESPNApiResponse2>(json2);
                            var filtered = result2.Events.Where(x => x.Date > currentDate && x.Competitions.First().Competitors.FirstOrDefault(x => x.Id == favTeamId).HomeAway == "away").ToList();

                            if (item.Key.ToUpperInvariant().TrimEnd() == "MLS" && item.Value.ToUpperInvariant().TrimEnd() == "MIN")
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
                        if (eventDateRange <= numberOfDays /*&& eventDateRange >= 0 && eventA.date >= currentDate && eventB.date >= currentDate*/)
                        {
                            var locationA = venueA.Address.StateName ?? venueA.Address.City.Split(',').Last().TrimStart();
                            var locationB = venueB.Address.StateName ?? venueB.Address.City.Split(',').Last().TrimStart();

                            // Compare the venues of the two events
                            if (venueA.FullName != venueB.FullName && locationA == locationB)
                            {
                                items.Add(new SportsEvent
                                {
                                    CompetitionA = eventA.Name,
                                    CompetitionB = eventB.Name,
                                    LocationA = $"{venueA.FullName}, {venueA.Address.City}, {venueA.Address.StateName}, {venueA.Address.ZipCode}",
                                    LocationB = $"{venueB.FullName}, {venueB.Address.City}, {venueB.Address.StateName}, {venueB.Address.ZipCode}",
                                    DateTimeA = eventA.Date,
                                    DateTimeB = eventB.Date
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
    }
}