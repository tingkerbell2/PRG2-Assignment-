using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ver1;
using static System.Runtime.InteropServices.JavaScript.JSType;

//Student Number: S10267852
//Student Name: Ee Ting Li
//Partner Name: Lau Jia Qi

//Features implemented by Ting Li: 1,4,7,8 + advanced feature (a)
//Features implemented by Jia Qi: 2,3,5,6,9 + advanced feature (b)

Terminal terminal = new Terminal("Changi Airport Terminal 5");

//Call the functions for loading data about airlines, boarding gates and flights
loadAirlines();
loadBoardingGates();
loadFlights();
Console.WriteLine();
Console.WriteLine();
Console.WriteLine();
Console.WriteLine();
Main();


//Display Menu Method
void displayMenu()
{
    try
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("Welcome to Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine("1. List All Flights");
        Console.WriteLine("2. List Boarding Gates");
        Console.WriteLine("3. Assign a Boarding Gate to a Flight");
        Console.WriteLine("4. Create Flight");
        Console.WriteLine("5. Display Airline Flights");
        Console.WriteLine("6. Modify Flight Details");
        Console.WriteLine("7. Display Flight Schedule");
        Console.WriteLine("8. Process all unassigned flights to boarding gates");
        Console.WriteLine("9. Display total fee per airline for the day");
        Console.WriteLine("0. Exit");
        Console.WriteLine();
        Console.WriteLine("Please select your option:");
    }

    catch (Exception ex)
    {
        Console.WriteLine("An error occurred: " + ex.Message);
    }

}

// Feature 1
void loadAirlines() 
{
    string filePath = "airlines.csv";

    if (!File.Exists(filePath))
    {
        Console.WriteLine($"Error: File '{filePath}' does not exist.");
        return;
    }

    try
    {
        using (StreamReader sr = new StreamReader(filePath))
        {
            string header = sr.ReadLine();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] data = line.Split(",");

                string airlineName = data[0];
                string airlineCode = data[1];

                // Create airline object
                Airline airline = new Airline(airlineName, airlineCode);
                terminal.AddAirline(airline);
            }

            //Print the appropriate message and number of airlines loaded
            Console.WriteLine("Loading Airlines...");
            Console.WriteLine($"{terminal.Airlines.Count()} Airlines Loaded!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

// Feature 1
void loadBoardingGates() 
{
    //Declare the file path
    string filePath = "boardinggates.csv";
    //Check if the file exists, if it does not exist, print an error message and return
    if (!File.Exists(filePath))
    {
        Console.WriteLine($"Error: File '{filePath}' does not exist.");
        return;
    }
    try
    {
        using (StreamReader sr = new StreamReader(filePath))
        {
            string ? header = sr.ReadLine();
            //Declare a string variable to store each line of the file
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                //Use split to separate the "," in the csv file
                string[] data = line.Split(",");
                //Trim the data to remove any whitespace
                string gateName = data[0].Trim();
                //Parse the data to get the boolean values
                bool supportsDDJB = bool.Parse(data[1].Trim());
                bool supportsCFFT = bool.Parse(data[2].Trim());
                bool supportsLWTT = bool.Parse(data[3].Trim());
                //Check if the boarding gate already exists in the terminal
                if (!terminal.boardingGates.ContainsKey(gateName))
                {
                    BoardingGate gate = new BoardingGate(gateName, supportsDDJB, supportsCFFT, supportsLWTT);
                    terminal.AddBoardingGate(gate);
                }
            }
            //Print the appropriate message and number of boarding gates loaded
            Console.WriteLine("Loading Boarding Gates...");
            //Use the Count property to get the number of boarding gates loaded instead of hardcoding
            Console.WriteLine($"{terminal.boardingGates.Count()} Boarding Gates Loaded!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}


// Feature 2
void loadFlights()
{
    string filePath = "flights.csv";

    // Check if the file exists before proceeding
    if (!File.Exists(filePath))
    {
        Console.WriteLine($"Error: File '{filePath}' does not exist.");
        return;
    }

    try
    {
        using (StreamReader sr = new StreamReader(filePath))
        {
            string header = sr.ReadLine(); // Skip header row
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                string[] data = line.Split(',');

                // Validate minimum required fields
                if (data.Length < 4)
                {
                    Console.WriteLine("Error: Incomplete flight entry detected. Skipping...");
                    continue;
                }

                string flightNumber = data[0].Trim();
                string origin = data[1].Trim();
                string destination = data[2].Trim();
                string rawTime = data[3].Trim();
                string specialRequestCode = data.Length > 4 ? data[4].Trim() : "None"; // Handle missing Special Request Code

                // Validate flight number (non-empty)
                if (string.IsNullOrWhiteSpace(flightNumber))
                {
                    Console.WriteLine("Error: Flight number is missing. Skipping entry...");
                    continue;
                }

                // Validate origin and destination (non-empty)
                if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(destination))
                {
                    Console.WriteLine("Error: Origin or destination is missing. Skipping entry...");
                    continue;
                }

                // Validate expected time
                if (!DateTime.TryParse(rawTime, out DateTime expectedTime))
                {
                    Console.WriteLine($"Error: Invalid date format for flight '{flightNumber}'. Skipping entry...");
                    continue;
                }

                Flight flight;

                // Assign appropriate flight type based on special request code
                switch (specialRequestCode)
                {
                    case "CFFT":
                        flight = new CFFTFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 150);
                        break;
                    case "DDJB":
                        flight = new DDJBFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 300);
                        break;
                    case "LWTT":
                        flight = new LWTTFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 500);
                        break;
                    default:
                        flight = new NORMFlight(flightNumber, origin, destination, expectedTime, "Scheduled");
                        break;
                }

                // Assign the special request code to the flight
                flight.specialRequestCode = specialRequestCode;

                // Extract airline code from flight number
                string[] flightNumberParts = flightNumber.Split(' ');
                if (flightNumberParts.Length < 2)
                {
                    Console.WriteLine($"Error: Invalid flight number format '{flightNumber}'. Skipping entry...");
                    continue;
                }
                string airlineCode = flightNumberParts[0];

                // Validate and assign airline to the flight
                if (terminal.Airlines.ContainsKey(airlineCode))
                {
                    Airline airline = terminal.Airlines[airlineCode];
                    flight.airline = airline;
                    airline.AddFlight(flight);
                }
                else
                {
                    Console.WriteLine($"Warning: Airline code '{airlineCode}' not recognized. Flight added without airline association.");
                }

                // Add flight to terminal
                terminal.AddFlight(flight);
            }
            Console.WriteLine("Loading Flights...");
            Console.WriteLine($"{terminal.flights.Count()} Flights Loaded!");
        }
    }
    catch (Exception ex)
    {
        // Handle unexpected errors gracefully
        Console.WriteLine($"Error: {ex.Message}");
    }
}


//Feature 3 - option 1 (current date)
void DisplayAllFlights()
{
    // Print header for the flight list
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Flights for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");

    // Print column headers with proper formatting
    Console.WriteLine("{0,-16}{1,-23}{2,-24}{3,-23}{4,-20}",
                      "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time");

    // Check if there are any flights in the terminal
    if (terminal.flights.Count == 0)
    {
        // Display message if no flights are found
        Console.WriteLine("No flights available.");
        return;
    }

    // Loop through each flight in the terminal
    foreach (Flight flight in terminal.flights.Values)
    {
        try
        {
            // Extract airline code from flight number by splitting it at spaces
            string[] flightNumParts = flight.flightNumber.Split(" ");
            string airlineCode = flightNumParts.Length > 0 ? flightNumParts[0] : "Unknown";

            // Initialize airline name with a default value
            string airlineName = "Unknown";

            // Check if the airline code exists in the terminal's airline records
            if (terminal.Airlines.ContainsKey(airlineCode))
            {
                // Retrieve the airline name from the dictionary
                airlineName = terminal.Airlines[airlineCode].Name;
            }
            else
            {
                // Display a warning if the airline code is not recognized
                Console.WriteLine($"Warning: Airline code '{airlineCode}' not recognized for flight '{flight.flightNumber}'.");
            }

            // Print the formatted flight details
            Console.WriteLine($"{flight.flightNumber,-16}{airlineName,-23}{flight.origin,-24}{flight.destination,-23}{flight.expectedTime:dd/MM/yyyy hh:mm:ss tt}");
        }
        catch (Exception ex)
        {
            // Handle any errors that occur while processing the flight details
            Console.WriteLine($"Error displaying flight '{flight.flightNumber}': {ex.Message}");
        }
    }
}

//Feature 4 - option 2 (No need for validation)
void ListBoardingGates()
{
    //Print the header for the list of boarding gates
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Boarding Gates for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-16}{1,-23}{2,-23}{3,-23}", "Gate Name", "DDJB", "CFFT", "LWTT");
    // Iterate over boarding gates and print their details
    //We use terminal.boardingGates.Values to get the values of the dictionary
    foreach (BoardingGate gate in terminal.boardingGates.Values)
    {
        //Print the details of each boarding gate
        Console.WriteLine($"{gate.gateName,-16}{gate.supportsDDJB,-23}{gate.supportsCFFT,-23}{gate.supportsLWTT,-23}");
    }
}



//Feature 5 - option 3
void AssignBoardingGateToFlight()
{
    // Display header for assigning a boarding gate
    Console.WriteLine("=============================================");
    Console.WriteLine("Assign a Boarding Gate to a Flight");
    Console.WriteLine("=============================================");

    while (true)
    {
        // Prompt user for flight number
        Console.WriteLine("Enter Flight Number: ");
        string flightNumber = Console.ReadLine().Trim();

        // Validate if the flight exists
        if (string.IsNullOrWhiteSpace(flightNumber) || !terminal.flights.ContainsKey(flightNumber))
        {
            Console.WriteLine("Flight not found or input is empty. Please enter a valid flight number.");
            continue; // Prompt user again if input is invalid
        }

        // Prompt user for Boarding Gate
        Console.WriteLine("Enter Boarding Gate Number: ");
        string gateNum = Console.ReadLine().Trim();

        // Validate if the boarding gate exists
        if (string.IsNullOrWhiteSpace(gateNum) || !terminal.boardingGates.ContainsKey(gateNum))
        {
            Console.WriteLine("Boarding gate not found or input is empty. Please enter a valid boarding gate number.");
            continue; // Prompt user again if input is invalid
        }

        // Get the Flight object from the terminal's flight dictionary
        Flight selectedFlight = terminal.flights[flightNumber];

        // Determine the Special Request Code based on flight type
        string specialRequestCode;
        if (selectedFlight is CFFTFlight)
        {
            specialRequestCode = "CFFT";
        }
        else if (selectedFlight is DDJBFlight)
        {
            specialRequestCode = "DDJB";
        }
        else if (selectedFlight is LWTTFlight)
        {
            specialRequestCode = "LWTT";
        }
        else
        {
            specialRequestCode = "None"; // Default for NORMFlight or other types
        }

        // Display flight and gate details
        Console.WriteLine($"Flight Number: {flightNumber}");
        Console.WriteLine($"Origin: {selectedFlight.origin}");
        Console.WriteLine($"Destination: {selectedFlight.destination}");
        Console.WriteLine($"Expected Time: {selectedFlight.expectedTime:dd/MM/yyyy hh:mm:ss tt}");
        Console.WriteLine($"Special Request Code: {specialRequestCode}");
        Console.WriteLine($"Boarding Gate Name: {gateNum}");

        // Get the Boarding Gate object from the terminal's boarding gates dictionary
        BoardingGate gate = terminal.boardingGates[gateNum];

        // Display the capabilities of the boarding gate
        Console.WriteLine($"Supports DDJB: {gate.supportsDDJB}");
        Console.WriteLine($"Supports CFFT: {gate.supportsCFFT}");
        Console.WriteLine($"Supports LWTT: {gate.supportsLWTT}");

        // Prompt user if they want to update the status of the flight
        Console.WriteLine("Would you like to update the status of the flight? (Y/N)");
        string choice = Console.ReadLine().Trim().ToUpper();

        if (choice == "Y")
        {
            // Display flight status options
            Console.WriteLine("1. Delayed");
            Console.WriteLine("2. Boarding");
            Console.WriteLine("3. On Time");
            Console.WriteLine("Please select the new status of the flight: ");

            string newStatus = Console.ReadLine().Trim();

            // Update flight status based on user selection
            switch (newStatus)
            {
                case "1":
                    selectedFlight.status = "Delayed";
                    break;
                case "2":
                    selectedFlight.status = "Boarding";
                    break;
                case "3":
                    selectedFlight.status = "On Time";
                    break;
                default:
                    Console.WriteLine("Invalid choice. Status remains unchanged.");
                    continue; // Prompt user again if invalid input
            }
        }

        // Confirm the boarding gate assignment
        Console.WriteLine($"Flight {flightNumber} has been assigned to Boarding Gate {gateNum}!");
        break; // Exit loop after successful assignment
    }
}


//Feature 6 - option 4
void CreateFlight()
{
    bool addAnotherFlight = true;

    while (addAnotherFlight)
    {
        // Prompt user for flight specifications
        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine();

        // Ensure the flight number is unique
        if (terminal.flights.ContainsKey(flightNumber))
        {
            Console.WriteLine($"Flight {flightNumber} already exists. Please use a unique flight number.");
            continue;
        }
        Console.Write("Enter Origin: ");
        string origin = Console.ReadLine();
        Console.Write("Enter Destination: ");
        string destination = Console.ReadLine();
        Console.Write("Enter Expected Departure/Arrival Time (dd/mm/yyyy hh:mm): ");
        DateTime expectedTime = Convert.ToDateTime(Console.ReadLine());

        Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
        string specialRequestCode = Console.ReadLine().ToUpper();

        Flight newFlight;
        switch (specialRequestCode)
        {
            case "CFFT":
                newFlight = new CFFTFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 150);
                break;
            case "DDJB":
                newFlight = new DDJBFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 300);
                break;
            case "LWTT":
                newFlight = new LWTTFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 500);
                break;
            default:
                newFlight = new NORMFlight(flightNumber, origin, destination, expectedTime, "Scheduled");
                specialRequestCode = "None";
                break;
        }

        // Assign the special request code to the flight
        newFlight.specialRequestCode = specialRequestCode;

        // Assign the airline to the flight
        string airlineCode = flightNumber.Split(' ')[0];
        if (terminal.Airlines.ContainsKey(airlineCode))
        {
            newFlight.airline = terminal.Airlines[airlineCode];
        }

        // Add flight to dictionary
        terminal.AddFlight(newFlight);

        // Append new flight to csv (if needed)
        string flightData = $"{flightNumber},{origin},{destination},{expectedTime:yyyy-MM-dd HH:mm},{specialRequestCode}";

        Console.WriteLine($"Flight {flightNumber} has been added!");

        // Prompt to add another flight
        Console.Write("Would you like to add another flight? [Y/N]: ");
        string choice = Console.ReadLine().ToUpper();
        if (choice == "N")
        {
            addAnotherFlight = false;
        }
        else if (choice != "Y")
        {
            Console.WriteLine("Invalid option.");
        }
    }
}


//Feature 7 - option 5 (Current Date)
void DisplayFlightDetails()
{
    //List all the airlines available
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-16}{1,-23}", "Airline Code", "Airline Name");


    // Check if there are any airlines available
    if (terminal.Airlines.Count == 0)
    {
        Console.WriteLine("No airlines available.");
        // Exit if no airlines are available
        return;
    }

    //Use a loop to display the airline code and name
    foreach (Airline airline in terminal.Airlines.Values)
    {
        Console.WriteLine($"{airline.Code,-16}{airline.Name,-23}");
    }

    //Prompt the user to enter the airline code
    Console.Write("Enter Airline Code: ");
    string? airlineCode = Console.ReadLine().Trim().ToUpper();

    //Validate the airline code
    if (string.IsNullOrEmpty(airlineCode))
    {
        Console.WriteLine("Airline code cannot be empty. Please try again.");
        return; // Exit if input is empty
    }

    // Check if the airline code exists in the dictionary
    if (!terminal.Airlines.ContainsKey(airlineCode))
    {
        Console.WriteLine("Invalid airline code. Please check the available airline codes and try again.");
        return; // Exit if airline code doesn't exist
    }

    // Retrieve the airline object
    Airline selectedAirline = terminal.Airlines[airlineCode];

    //List all flights for the selected airline
    Console.WriteLine("=============================================");
    Console.WriteLine($"List of Flights for {selectedAirline.Name}");
    Console.WriteLine("=============================================");

    // Header for the flight list table
    Console.WriteLine("{0,-16}{1,-23}{2,-23}{3,-23}{4,-30}",
                      "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time");

    // Flag to track if any flight is found
    bool flightsFound = false;

    // Iterate through the flights in the terminal
    foreach (Flight flight in terminal.flights.Values)
    {
        // Filter flights based on the airline code
        if (flight.flightNumber.StartsWith(airlineCode))
        {
            // Validate flight information
            if (string.IsNullOrEmpty(flight.flightNumber) || string.IsNullOrEmpty(flight.origin) || string.IsNullOrEmpty(flight.destination))
            {
                Console.WriteLine($"Warning: Flight {flight.flightNumber} has missing information and will be skipped.");
                continue; // Skip flights with missing details
            }

            // Print flight details in the same formatted table style
            Console.WriteLine("{0,-16}{1,-23}{2,-23}{3,-23}{4,-30}",
                              flight.flightNumber,
                              selectedAirline.Name,
                              flight.origin,
                              flight.destination,
                              flight.expectedTime.ToString("dd/MM/yyyy hh:mm:ss tt"));

            flightsFound = true; // Set flag to true since at least one flight was found
        }
    }

    //If no flights are found
    if (!flightsFound)
    {
        //Display a message if no flights are found
        Console.WriteLine("No flights available for the selected airline.");
    }
}


//Feature 8 - option 6 (Current Date)
void ModifyFlightDetails()
{
    // Display all airlines
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-16}{1,-23}", "Airline Code", "Airline Name");

    //Use a loop to display the airline code and name
    foreach (Airline airline in terminal.Airlines.Values)
    {
        Console.WriteLine($"{airline.Code,-16}{airline.Name,-23}");
    }

    // Prompt user for airline code
    Console.WriteLine("Enter Airline Code: ");
    string? airlineCode = Console.ReadLine().Trim().ToUpper();

    // Validate airline code
    if (string.IsNullOrEmpty(airlineCode) || !terminal.Airlines.ContainsKey(airlineCode))
    {
        Console.WriteLine("Invalid airline code. Please try again.");
        return;
    }

    // Retrieve the selected airline
    Airline selectedAirline = terminal.Airlines[airlineCode];

    // Display all flights for the selected airline
    Console.WriteLine($"List of Flights for {selectedAirline.Name}");
    Console.WriteLine("{0,-16}{1,-23}{2,-23}{3,-23}{4,-30}",
                      "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time");

    // Display flights for the selected airline
    foreach (Flight flight in terminal.flights.Values)
    {
        // Filter flights based on the airline code using StartsWith method
        if (flight.flightNumber.StartsWith(airlineCode))
        {
            Console.WriteLine("{0,-16}{1,-23}{2,-23}{3,-23}{4,-30}",
                              flight.flightNumber,
                              selectedAirline.Name,
                              flight.origin,
                              flight.destination,
                              flight.expectedTime.ToString("dd/MM/yyyy hh:mm:ss tt"));
        }
    }

    // Prompt user to select a flight
    Console.WriteLine("\nChoose an existing Flight to modify or delete: ");
    string flightNumber = Console.ReadLine().Trim();

    // Validate flight number
    if (!terminal.flights.ContainsKey(flightNumber))
    {
        Console.WriteLine("Invalid flight number. Please try again.");
        return;
    }


    // Retrieve the selected flight
    Flight selectedFlight = terminal.flights[flightNumber];

    // Display the modification options
    Console.WriteLine("1. Modify Flight");
    Console.WriteLine("2. Delete Flight");
    // Prompt user for option
    Console.WriteLine("Choose an option: ");
    // Read user input
    string option = Console.ReadLine().Trim();

    //If the user chooses to modify the flight
    if (option == "1")
    {
        //Display the options for modification - user can choose what to modify
        Console.WriteLine("1. Modify Basic Information");
        Console.WriteLine("2. Modify Status");
        Console.WriteLine("3. Modify Special Request Code");
        Console.WriteLine("4. Modify Boarding Gate");
        //Prompt the user to choose an option
        Console.WriteLine("Choose an option: ");
        string modifyOption = Console.ReadLine();

        // Perform the selected modification
        if (modifyOption == "1")
        {
            // Modify basic information
            // Prompt user for new origin
            Console.Write("Enter new Origin: ");
            selectedFlight.origin = Console.ReadLine().Trim();

            // Prompt user for new destination
            Console.Write("Enter new Destination: ");
            selectedFlight.destination = Console.ReadLine().Trim();

            // Prompt user for new expected time
            Console.Write("Enter new Expected Departure/Arrival Time (dd/MM/yyyy hh:mm): ");
            string expectedTimeInput = Console.ReadLine().Trim();

            // Validate and parse the expected time
            // TryParse converts string (expectedTimeInput) to Datatime object
            if (DateTime.TryParse(expectedTimeInput, out DateTime expectedTime))
            {
                // Update the expected time
                selectedFlight.expectedTime = expectedTime;
            }
            else
            {
                // Display error message if the input is invalid
                Console.WriteLine("Invalid date format. Please enter the date in the format dd/MM/yyyy hh:mm:ss tt.");
                return;
            }
        }
        //If the user chooses to modify the status
        else if (modifyOption == "2")
        {
            // Modify status
            Console.WriteLine("1. Delayed");
            Console.WriteLine("2. Boarding");
            Console.WriteLine("3. On Time");
            Console.Write("Enter new status (1/2/3): ");
            string status = Console.ReadLine();
            if (status == "1")
                selectedFlight.status = "Delayed";
            else if (status == "2")
                selectedFlight.status = "Boarding";
            else if (status == "3")
                selectedFlight.status = "On Time";
        }
        else if (modifyOption == "3")
        {
            // Modify special request code
            Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
            string newCode = Console.ReadLine().Trim().ToUpper();

            if (newCode == "CFFT")
                terminal.flights[flightNumber] = new CFFTFlight(selectedFlight.flightNumber, selectedFlight.origin, selectedFlight.destination, selectedFlight.expectedTime, selectedFlight.status, 150);
            else if (newCode == "DDJB")
                terminal.flights[flightNumber] = new DDJBFlight(selectedFlight.flightNumber, selectedFlight.origin, selectedFlight.destination, selectedFlight.expectedTime, selectedFlight.status, 300);
            else if (newCode == "LWTT")
                terminal.flights[flightNumber] = new LWTTFlight(selectedFlight.flightNumber, selectedFlight.origin, selectedFlight.destination, selectedFlight.expectedTime, selectedFlight.status, 500);
            else if (newCode == "NONE")
                terminal.flights[flightNumber] = new NORMFlight(selectedFlight.flightNumber, selectedFlight.origin, selectedFlight.destination, selectedFlight.expectedTime, selectedFlight.status);
            else
                Console.WriteLine("Invalid Special Request Code.");
        }
        else if (modifyOption == "4")
        {
            // Modify boarding gate
            Console.Write("Enter new Boarding Gate: ");
            string newGate = Console.ReadLine().Trim();

            if (terminal.boardingGates.ContainsKey(newGate))
            {
                terminal.boardingGates[newGate].Flight = selectedFlight;
                Console.WriteLine($"Boarding Gate updated to {newGate}");
            }
            else
            {
                Console.WriteLine("Invalid Boarding Gate.");
            }
        }
        else
        {
            Console.WriteLine("Invalid modification choice.");
        }
        //Console.WriteLine("Flight updated!");
    }
    else if (option == "2")
    {
        // Delete flight
        Console.WriteLine("Are you sure you want to delete this flight? (Y/N): ");
        string deleteChoice = Console.ReadLine().Trim().ToUpper();
        if (deleteChoice == "Y")
        {
            terminal.flights.Remove(flightNumber);
            Console.WriteLine($"Flight number {flightNumber} has been successfully removed.");
            return;
        }
        else if (deleteChoice == "N")
        {
            Console.WriteLine("Deletion Cancelled.");
            return;
        }
        else
        {
            Console.WriteLine("Invalid option.");
        }
    }
    else
    {
        Console.WriteLine("Invalid option.");
        return;
    }

    // Display updated flight information
    Console.WriteLine("Flight updated!");
    Console.WriteLine($"Flight Number: {selectedFlight.flightNumber}");
    Console.WriteLine($"Airline Name: {selectedAirline.Name}");
    Console.WriteLine($"Origin: {selectedFlight.origin}");
    Console.WriteLine($"Destination: {selectedFlight.destination}");
    Console.WriteLine($"Expected Departure/Arrival Time: {selectedFlight.expectedTime:dd/MM/yyyy hh:mm:ss tt}");
    Console.WriteLine($"Status: {selectedFlight.status}");

    // Display special request code
    if (selectedFlight is CFFTFlight)
        Console.WriteLine("Special Request Code: CFFT");
    else if (selectedFlight is DDJBFlight)
        Console.WriteLine("Special Request Code: DDJB");
    else if (selectedFlight is LWTTFlight)
        Console.WriteLine("Special Request Code: LWTT");
    else
        Console.WriteLine("Special Request Code: None");

    Console.WriteLine($"Boarding Gate: {(terminal.boardingGates.ContainsKey(flightNumber) ? terminal.boardingGates[flightNumber].gateName : "Unassigned")}");
}


//Feature 9 - option 7
void displayScheduledflights()
{
    // Display header for scheduled flights
    Console.WriteLine("=============================================");
    Console.WriteLine("Flight Schedule for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-15}{1,-25}{2,-25}{3,-25}{4,-35}{5,-15}{6,-15}",
                      "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time", "Status", "Boarding Gate");

    // Check if there are flights to display
    if (terminal.flights.Count == 0)
    {
        Console.WriteLine("No scheduled flights available.");
        return;
    }

    // Create a list of all flights from the dictionary
    List<Flight> sortedFlights = new List<Flight>(terminal.flights.Values);

    // Sort the list using IComparable interface implemented in the Flight class
    sortedFlights.Sort();

    // Iterate over each flight and display details in sorted order 
    foreach (Flight flight in sortedFlights)
    {
        try
        {
            // Extract the airline code from the flight number
            string[] flightNumParts = flight.flightNumber.Split(" ");
            string airlineCode = flightNumParts.Length > 0 ? flightNumParts[0] : "Unknown";

            // Retrieve the airline name, or default to "Unknown" if not found
            string airlineName = terminal.Airlines.ContainsKey(airlineCode) ? terminal.Airlines[airlineCode].Name : "Unknown";

            // Determine the assigned boarding gate, default to "Unassigned"
            string boardingGate = "Unassigned";
            foreach (var gate in terminal.boardingGates.Values)
            {
                if (gate.Flight != null && gate.Flight.flightNumber == flight.flightNumber)
                {
                    boardingGate = gate.gateName;
                    break; // Exit loop once the assigned gate is found
                }
            }

            // Format the expected time for display
            string expectedTimeInfo = flight.expectedTime.ToString("dd/MM/yyyy hh:mm:ss tt");

            // Print flight details in a formatted manner
            Console.WriteLine($"{flight.flightNumber,-15}{airlineName,-25}{flight.origin,-25}{flight.destination,-25}{expectedTimeInfo,-35}{flight.status,-15}{boardingGate,-15}");
        }
        catch (Exception ex)
        {
            // Handle any errors that occur while processing a flight
            Console.WriteLine($"Error displaying flight '{flight.flightNumber}': {ex.Message}");
        }
    }
}


void Main()
{
    while (true)
    {
        // Display the menu options
        displayMenu();
        string option = Console.ReadLine()?.Trim(); // Read user input and trim whitespace

        // Validate user input to ensure it is not empty
        if (string.IsNullOrEmpty(option))
        {
            Console.WriteLine("Invalid input. Please enter a valid option.");
            continue;
        }

        // Process the user's selection using if-else instead of switch
        if (option == "1")
        {
            DisplayAllFlights();
        }
        else if (option == "2")
        {
            ListBoardingGates();
        }
        else if (option == "3")
        {
            AssignBoardingGateToFlight();
        }
        else if (option == "4")
        {
            CreateFlight();
        }
        else if (option == "5")
        {
            DisplayFlightDetails();
        }
        else if (option == "6")
        {
            ModifyFlightDetails();
        }
        else if (option == "7")
        {
            displayScheduledflights();
        }
        else if (option == "8")
        {
            ProcessUnassignedFlights();
        }
        else if (option == "9")
        {
            DisplayTotalFeesPerAirline();
        }
        else if (option == "0")
        {
            Console.WriteLine("Goodbye!");
            return; // Exit the program
        }
        else
        {
            Console.WriteLine("Invalid option. Please try again.");
        }
    }
}



//Advanced feature (a)
void ProcessUnassignedFlights()
{
    // Load data
    loadBoardingGates();
    loadFlights();

    Queue<Flight> unassignedFlights = new Queue<Flight>();

    // Identify unassigned flights
    foreach (var flight in terminal.flights.Values)
    {
        if (flight.boardingGate == null)  // Check if flight has no assigned gate
        {
            unassignedFlights.Enqueue(flight);
        }
    }

    Console.WriteLine($"Total Flights without Boarding Gates: {unassignedFlights.Count}");

    // Identify unassigned boarding gates
    List<BoardingGate> unassignedGates = terminal.boardingGates.Values
        .Where(gate => gate.AssignedFlight == null)  // Check if gate has no flight assigned
        .ToList();

    Console.WriteLine($"Total Boarding Gates without Flight Numbers: {unassignedGates.Count}");

    // Display flight details in a formatted table header
    Console.WriteLine("+--------------------+-----------------------+--------------------+--------------------+-----------------------+------------------+-----------------+");
    Console.WriteLine("| Flight Number      | Airline               | Origin             | Destination        | Expected Time         | Special Request  | Boarding Gate   |");
    Console.WriteLine("+--------------------+-----------------------+--------------------+--------------------+-----------------------+------------------+-----------------+");

    // Process each flight
    while (unassignedFlights.Count > 0)
    {
        Flight flight = unassignedFlights.Dequeue();
        BoardingGate assignedGate = null;

        // Check if flight has a special request
        if (flight is CFFTFlight)
        {
            assignedGate = unassignedGates.FirstOrDefault(gate => gate.supportsCFFT);
        }
        else if (flight is DDJBFlight)
        {
            assignedGate = unassignedGates.FirstOrDefault(gate => gate.supportsDDJB);
        }
        else if (flight is LWTTFlight)
        {
            assignedGate = unassignedGates.FirstOrDefault(gate => gate.supportsLWTT);
        }

        // If no special request gate found, assign any available gate
        if (assignedGate == null)
        {
            assignedGate = unassignedGates.FirstOrDefault();
        }

        if (assignedGate != null)
        {
            // Assign the gate to the flight
            flight.boardingGate = assignedGate;
            // Assign the flight to the gate
            assignedGate.AssignedFlight = flight;
            // Remove the assigned gate from the list
            unassignedGates.Remove(assignedGate);

            // Display the flight details in formatted manner
            string specialRequest = !string.IsNullOrEmpty(flight.specialRequestCode) ? flight.specialRequestCode : "None";
            string gateInfo = assignedGate != null ? assignedGate.gateName : "No Gate Assigned";
            string airlineName = flight.airline?.Name ?? "N/A";  // Ensure the Airline name is properly set.

            Console.WriteLine($"| {flight.flightNumber,-18} | {airlineName,-21} | {flight.origin,-18} | {flight.destination,-18} | {flight.expectedTime,-21} | {specialRequest,-16} | {gateInfo,-15} |");
        }
        else
        {
            Console.WriteLine($"No available gate for Flight {flight.flightNumber}");
        }
    }

    // Display footer for table
    Console.WriteLine("+--------------------+-----------------------+--------------------+--------------------+-----------------------+------------------+-----------------+");

    // Summary of results
    int totalFlightsProcessed = terminal.flights.Count(f => f.Value.boardingGate != null);
    int totalGatesProcessed = terminal.boardingGates.Count(g => g.Value.AssignedFlight != null);

    Console.WriteLine($"Total Flights processed: {totalFlightsProcessed}");
    Console.WriteLine($"Total Boarding Gates processed: {totalGatesProcessed}");

    double flightProcessingPercentage = Math.Round((double)totalFlightsProcessed / terminal.flights.Count * 100, 2);
    double gateProcessingPercentage = Math.Round((double)totalGatesProcessed / terminal.boardingGates.Count * 100, 2);

    Console.WriteLine($"Percentage of Flights automatically processed: {flightProcessingPercentage}%");
    Console.WriteLine($"Percentage of Gates automatically processed: {gateProcessingPercentage}%");
}


//Advanced feature (b) - Display Total Fees Per Airline
void CalculateTotalFeesPerAirline()
{
    // Ensure all flights have an assigned boarding gate
    if (terminal.flights.Values.Any(flight => flight.boardingGate == null))
    {
        Console.WriteLine("Not all flights have been assigned boarding gates. Please assign all flights before running this feature again.");
        return;
    }

    // Dictionary to store total fees and discounts per airline
    Dictionary<string, double> totalFeesByAirline = new Dictionary<string, double>();
    Dictionary<string, double> totalDiscountsByAirline = new Dictionary<string, double>();

    foreach (var flight in terminal.flights.Values)
    {
        // Extract airline code from the flight number
        string airlineCode = flight.flightNumber.Split(' ')[0];

        // Base boarding gate fee
        double flightFee = 300;

        // Additional fees based on origin and destination
        if (flight.origin == "Singapore (SIN)")
        {
            flightFee += 800;
        }

        if (flight.destination == "Singapore (SIN)")
        {
            flightFee += 500;
        }

        // Add special request fee if applicable
        if (flight is DDJBFlight ddjbFlight)
        {
            flightFee += ddjbFlight.requestFee;
        }
        else if (flight is CFFTFlight cfftFlight)
        {
            flightFee += cfftFlight.requestFee;
        }
        else if (flight is LWTTFlight lwttFlight)
        {
            flightFee += lwttFlight.requestFee;
        }

        // Initialize airline fee tracking if not already present
        if (!totalFeesByAirline.ContainsKey(airlineCode))
        {
            totalFeesByAirline[airlineCode] = 0;
            totalDiscountsByAirline[airlineCode] = 0;
        }

        // Add flight fee to the airline's total fees
        totalFeesByAirline[airlineCode] += flightFee;

        // Apply eligible discounts
        double discountAmount = 0;

        // Off-peak flight discount ($110)
        if (flight.expectedTime.Hour < 11 || flight.expectedTime.Hour >= 21)
        {
            discountAmount += 110;
        }

        // Specific origin discount ($25)
        if (flight.origin == "Dubai (DXB)" || flight.origin == "Bangkok (BKK)" || flight.origin == "Tokyo (NRT)")
        {
            discountAmount += 25;
        }

        // Discount for flights with no special request code ($50)
        if (!(flight is DDJBFlight || flight is CFFTFlight || flight is LWTTFlight))
        {
            discountAmount += 50;
        }

        // Add the calculated discount to the airline's total discounts
        totalDiscountsByAirline[airlineCode] += discountAmount;
    }

    // Apply bulk discounts based on flight count
    foreach (var airlineCode in totalFeesByAirline.Keys.ToList())
    {
        int airlineFlightCount = terminal.flights.Values.Count(flight => flight.flightNumber.StartsWith(airlineCode));

        // Discount of $350 for every 3 flights
        totalDiscountsByAirline[airlineCode] += (airlineFlightCount / 3) * 350;

        // Additional 3% discount for airlines with more than 5 flights
        if (airlineFlightCount > 5)
        {
            totalDiscountsByAirline[airlineCode] += totalFeesByAirline[airlineCode] * 0.03;
        }
    }

    // Display results
    double grandTotalFees = 0;
    double grandTotalDiscounts = 0;
    double finalCollectedAmount = 0;

    Console.WriteLine("{0,-20} {1,-30} {2,-20} {3,-20}", "Airline", "Total Fees", "Discounts", "Final Amount");

    foreach (var airlineCode in totalFeesByAirline.Keys)
    {
        double finalAirlineFee = totalFeesByAirline[airlineCode] - totalDiscountsByAirline[airlineCode];

        Console.WriteLine("{0,-20} {1,-30:F2} {2,-20:F2} {3,-20:F2}",
                          airlineCode,
                          totalFeesByAirline[airlineCode],
                          totalDiscountsByAirline[airlineCode],
                          finalAirlineFee);

        grandTotalFees += totalFeesByAirline[airlineCode];
        grandTotalDiscounts += totalDiscountsByAirline[airlineCode];
        finalCollectedAmount += finalAirlineFee;
    }

    // Calculate the percentage of discounts over total fees
    double discountPercentage = (grandTotalDiscounts / grandTotalFees) * 100;

    Console.WriteLine("\nTotal Fees Collected: {0:F2}", grandTotalFees);
    Console.WriteLine("Total Discounts Given: {0:F2}", grandTotalDiscounts);
    Console.WriteLine("Final Collected Fees: {0:F2}", finalCollectedAmount);
    Console.WriteLine("Overall Discount Percentage: {0:F2}%", discountPercentage);
}


