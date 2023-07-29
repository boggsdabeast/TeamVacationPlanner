using TeamVacationPlanner.EspnApi;

public class Program
{
    static async Task Main(string[] args)
    {
        // user input data
        int numberOfDays; 
        var favoriteTeams = new Dictionary<string, string>();

        // constants
        var invalidMessage = "Invalid input. Please ";
        var numberOfDaysMessage = "Enter number of days to query by:";
        var favoriteTeamsMessage = "Enter league-team abbreviation to list favorite teams in format 'NFL=MIN'. Enter 'done' to finish:";
        var seperator = "---------------------------------------------------------------------------------------";

        // Prompt the user for input
        Console.WriteLine(numberOfDaysMessage);

        while (!int.TryParse(Console.ReadLine(), out numberOfDays))
        {
            Console.WriteLine($"{invalidMessage}{numberOfDaysMessage}");
        }

        // Prompt the user for input
        Console.WriteLine(favoriteTeamsMessage);

        // Loop to read input until the user enters "done"
        while (true)
        {
            var input = Console.ReadLine();

            // Check if the user wants to exit the input loop
            if (input.ToLower() == "done" && favoriteTeams.Count > 0)
                break;
            else if (input.ToLower() == "colan")
            {
                favoriteTeams = new()
                {
                    { "NFL", "DEN" },
                    { "MLB", "MIN" },
                    { "NBA", "MIN" },
                    { "NHL", "MIN" },
                    { "MLS", "MIN" }
                };
                break;
            }

            // Split the input into key and value based on the '=' character
            var keyValue = input.Split('=');

            // Ensure that the input has the correct format (key=value)
            if (keyValue.Length == 2)
            {
                var key = keyValue[0].Trim().ToUpperInvariant();
                var value = keyValue[1].Trim().ToUpperInvariant();

                // Add the key-value pair to the dictionary
                favoriteTeams[key] = value;
            }
            else
            {
                Console.WriteLine($"{invalidMessage}{favoriteTeamsMessage}");
            }
        }

        // Display the dictionary contents
        Console.WriteLine();
        Console.WriteLine($"Number of days entered: {numberOfDays}");
        Console.WriteLine();
        Console.WriteLine("Favorite teams entered:");
        foreach (var kvp in favoriteTeams)
        {
            Console.WriteLine($"{kvp.Key} = {kvp.Value}");
        }

        // Run API
        var espnApi = new ESPNApi();
        var (Events, Error) = await espnApi.GetOverlappingEvents(numberOfDays, favoriteTeams);

        if (!string.IsNullOrWhiteSpace(Error))
        {
            Console.WriteLine(Error);
        }
        else
        {
            // Display the table header
            Console.WriteLine();
            Console.WriteLine($"Count returned from query: {Events.Count}");
            Console.WriteLine(seperator);
            Console.WriteLine("| Competition A | Competition B | Location A | Location B | Timestamp A | Timestamp B |");
            Console.WriteLine(seperator);

            // Display each SportsEvent row in the table
            foreach (var sportsEvent in Events)
            {
                Console.WriteLine(sportsEvent.Row);
            }

            // Display the table footer
            Console.WriteLine(seperator);
        }

        // Pause the program execution and keep the console open
        Console.ReadKey();
    }
}