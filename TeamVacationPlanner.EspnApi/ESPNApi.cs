using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace TeamVacationPlanner.EspnApi
{
    public class ESPNApi
    {
        private const string ApiBaseUrl = "https://site.api.espn.com/apis/site/v2/sports";
        private readonly HttpClient _httpClient;
        private readonly List<Event> _hardcodedEvents = new();
        private readonly string _apiKey;
        private readonly double _nullDistance = -1;
        private readonly Dictionary<string, double> _addressDistance = new();

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

            var host = new HostBuilder()
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            })
            .Build();

            var configuration = host.Services.GetRequiredService<IConfiguration>();
            if (configuration != null)
                _apiKey = configuration["ApiKeys:BingMaps"];
        }

        public async Task<(List<SportsEvent> Events, List<string> Errors)> GetOverlappingEvents(int numberOfDays, int searchDistance, Dictionary<string, string> favoriteTeams)
        {
            try
            {
                var favTeamId = 0;
                var url = ApiBaseUrl;
                var currentDate = DateTime.UtcNow.Date;
                var events = new List<Event>();
                var items = new List<SportsEvent>();

                foreach (var item in favoriteTeams)
                {
                    switch (item.Key.Trim().ToUpperInvariant())
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
                        favTeamId = result.Sports.First().Leagues.Select(x => x.Teams).First().FirstOrDefault(x => x.Team.Abbreviation == item.Value.Trim().ToUpperInvariant()).Team.Id;

                        var response2 = await _httpClient.GetAsync($"{url}/{favTeamId}/schedule");
                        if (response2.IsSuccessStatusCode)
                        {
                            var json2 = await response2.Content.ReadAsStringAsync();
                            var result2 = JsonConvert.DeserializeObject<ESPNApiResponse2>(json2);
                            var filtered = result2.Events.Where(x => x.Date > currentDate && x.Competitions.First().Competitors.FirstOrDefault(x => x.Id == favTeamId).HomeAway.ToLower().Trim() == "away").ToList();

                            if (item.Key.ToUpperInvariant().Trim() == "MLS" && item.Value.ToUpperInvariant().Trim() == "MIN")
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
                var geocodeUrl = $"http://dev.virtualearth.net/REST/v1/Locations/";
                var routeUrl = $"http://dev.virtualearth.net/REST/v1/Routes/";

                // Step 1: Get latitude and longitude for address1
                HttpResponseMessage response = await httpClient.GetAsync($"{geocodeUrl}{Uri.EscapeDataString(address1)}?key={_apiKey}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var lat1 = ParseLatitude(json);
                var lng1 = ParseLongitude(json);

                // Step 2: Get latitude and longitude for address2
                response = await httpClient.GetAsync($"{geocodeUrl}{Uri.EscapeDataString(address2)}?key={_apiKey}");
                response.EnsureSuccessStatusCode();
                var json2 = await response.Content.ReadAsStringAsync();
                var lat2 = ParseLatitude(json2);
                var lng2 = ParseLongitude(json2);

                // Step 3: Calculate distance using Haversine formula (you can use other formulas as well)
                var earthRadius = 3959; // Earth's radius in miles
                var dLat = DegreeToRadian(lat2 - lat1);
                var dLng = DegreeToRadian(lng2 - lng1);
                var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                           Math.Cos(DegreeToRadian(lat1)) * Math.Cos(DegreeToRadian(lat2)) *
                           Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
                var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                var distance = earthRadius * c;

                return Math.Round(distance, 2);
            }

            return _nullDistance;
        }

        private double ParseLatitude(string json)
        {
            dynamic data = JsonConvert.DeserializeObject(json);
            // Implement logic to parse latitude from the JSON response
            // Example: return latitude from the JSON response
            return data.resourceSets[0].resources[0].geocodePoints[0].coordinates[0];
        }

        private double ParseLongitude(string json)
        {
            dynamic data = JsonConvert.DeserializeObject(json);
            // Implement logic to parse longitude from the JSON response
            // Example: return longitude from the JSON response
            return data.resourceSets[0].resources[0].geocodePoints[0].coordinates[1];
        }

        private double DegreeToRadian(double degree)
        {
            return degree * Math.PI / 180;
        }
    }
}