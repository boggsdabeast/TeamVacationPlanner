﻿using TeamVacationPlanner.EspnApi;

public class Program
{
    static async Task Main(string[] args)
    {
        // user input data
        int numberOfDays;
        int distance;
        var favoriteTeams = new Dictionary<string, string>();

        // constants
        var invalidMessage = "Invalid input. Please ";
        var numberOfDaysMessage = "Enter number of days to query by:";
        var distanceMessage = "Enter approx mileage apart to query by:";
        var favoriteTeamsMessage = "Enter league-team abbreviation to list favorite teams in format 'NFL=MIN'. Enter 'done' to finish:";
        var seperator = "--------------------------------------------------------------------------------------------------";

        // Prompt the user for input: Days
        Console.WriteLine(numberOfDaysMessage);

        while (!int.TryParse(Console.ReadLine(), out numberOfDays))
        {
            Console.WriteLine($"{invalidMessage}{numberOfDaysMessage}");
        }

        // Prompt the user for input: Distance
        Console.WriteLine(distanceMessage);

        while (!int.TryParse(Console.ReadLine(), out distance))
        {
            Console.WriteLine($"{invalidMessage}{distanceMessage}");
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
        Console.WriteLine(seperator);
        Console.WriteLine();
        Console.WriteLine($"Number of days entered: {numberOfDays}");
        Console.WriteLine($"Distance entered: {distance}");
        Console.WriteLine("Favorite teams entered:");
        foreach (var kvp in favoriteTeams)
        {
            Console.WriteLine($"{kvp.Key} = {kvp.Value}");
        }

        // Run API
        var espnApi = new ESPNApi();
        var (Events, Errors) = await espnApi.GetOverlappingEvents(numberOfDays, distance, favoriteTeams);

        if (Errors.Any())
        {
            Console.WriteLine();
            foreach (var error in Errors)
            {
                Console.WriteLine(error);
            }
        }
        else
        {
            // Display the table header
            Console.WriteLine();
            Console.WriteLine(seperator);
            Console.WriteLine();
            Console.WriteLine($"Count returned from query: {Events.Count}");
            Console.WriteLine("| Competition A | Competition B | Location A | Location B | Timestamp A | Timestamp B | Distance |");
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