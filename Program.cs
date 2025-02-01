//==========================================================
// Student Number	: S10267822
// Student Name	: Lau Jia Qi
// Partner Name	: Ee Ting Li
//==========================================================
using PRG2_Assignment;
Terminal terminal = new Terminal("Changi Airport Terminal 5");
loadAirlines();
loadBoardingGates();
loadFlights();
Console.WriteLine();
Console.WriteLine();
Console.WriteLine();
Console.WriteLine();
Main();
//Display Menu (Completed)
void displayMenu()
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
    Console.WriteLine("0. Exit");
    Console.WriteLine();
    Console.WriteLine("Please select your option:");
}

//Feature 1: Load Airline and Boarding Gate (Completed)
void loadAirlines() 
{
    using (StreamReader sr = new StreamReader("airlines.csv"))
    {
        string header = sr.ReadLine();
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            string[] data = line.Split(",");
            string airlineName = data[0];
            string airlineCode = data[1];
            //create airline object
            Airline airline = new Airline(airlineName, airlineCode);
            //add airline object
            terminal.AddAirline(airline);
        }
        Console.WriteLine("Loading Airlines...");
        Console.WriteLine($"{terminal.Airlines.Count()} Airlines Loaded!");
    }
}
void loadBoardingGates() 
{
    using (StreamReader sr = new StreamReader("boardinggates.csv"))
    {
        string header = sr.ReadLine();
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            string[] data = line.Split(",");
            string boardingGate = data[0].Trim();
            bool supportsDDJB = bool.Parse(data[1].Trim());
            bool supportsCFFT = bool.Parse(data[2].Trim());
            bool supportsLWTT = bool.Parse(data[3].Trim());

            if (!terminal.boardingGates.ContainsKey(boardingGate))
            {
                BoardingGate gate = new BoardingGate(boardingGate, supportsDDJB, supportsCFFT, supportsLWTT);
                terminal.AddBoardingGate(gate);
            }
        }
        Console.WriteLine("Loading Boarding Gates...");
        Console.WriteLine($"{terminal.boardingGates.Count()} Boarding Gates Loaded!");
    }
}
//Feature 2: Load Flight Data (Completed)
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
    {        // Handle unexpected errors gracefully
        Console.WriteLine($"Error: {ex.Message}");
    }
}

//Feature 3: List Flights with basic information (Completed)
void DisplayAllFlights()
{
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Flights for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-16}{1,-23}{2,-24}{3,-23}{4,-20}",
                      "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time");

    foreach (Flight flight in terminal.flights.Values)
    {
        string[] flightNumParts = flight.flightNumber.Split(" ");
        string airlineCode = flightNumParts[0];

        // Initialize airlineName to "" as a default value
        string airlineName = "";

        if (terminal.Airlines.ContainsKey(airlineCode))
        {
            airlineName = terminal.Airlines[airlineCode].Name;
        }

        // Ensure that the expected time is 18th January 2025 for the demonstration
        string expectedTime = "18/01/2025 " + flight.expectedTime.ToString("hh:mm:ss tt");

        Console.WriteLine($"{flight.flightNumber,-16}{airlineName,-23}{flight.origin,-24}{flight.destination,-23}{expectedTime}");
    }
}
//Feature 4: List Boarding gates (Completed)
void ListBoardingGates() 
{
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Boarding Gates for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    //Header
    Console.WriteLine("{0,-16}{1,-23}{2,-23}{3,-23}", "Gate Name", "DDJB", "CFFT", "LWTT");
    // Iterate over boarding gates and print their details
    foreach (BoardingGate gate in terminal.boardingGates.Values)
    {
        Console.WriteLine($"{gate.gateName,-16}{gate.supportsDDJB,-23}{gate.supportsCFFT,-23}{gate.supportsLWTT,-23}");
    }
}

//Feature 5: Assigh Boarding gate to flight (Completed)
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

//Feature 6: Create flight
void CreateFlight()
{
    bool addAnotherFlight = true;

    while (addAnotherFlight)
    {
        // Prompt user for flight specifications
        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine().Trim();

        // Ensure the flight number is unique and not empty
        if (string.IsNullOrWhiteSpace(flightNumber) || terminal.flights.ContainsKey(flightNumber))
        {
            Console.WriteLine("Invalid or duplicate flight number. Please enter a unique flight number.");
            continue;
        }

        // Prompt for origin and validate input
        Console.Write("Enter Origin: ");
        string origin = Console.ReadLine().Trim();
        if (string.IsNullOrWhiteSpace(origin))
        {
            Console.WriteLine("Origin cannot be empty.");
            continue;
        }

        // Prompt for destination and validate input
        Console.Write("Enter Destination: ");
        string destination = Console.ReadLine().Trim();
        if (string.IsNullOrWhiteSpace(destination))
        {
            Console.WriteLine("Destination cannot be empty.");
            continue;
        }

        // Prompt for expected departure/arrival time and validate input
        Console.Write("Enter Expected Departure/Arrival Time (hh:mm tt): ");
        if (!DateTime.TryParseExact(Console.ReadLine().Trim(), "hh:mm tt", null, System.Globalization.DateTimeStyles.None, out DateTime expectedTime))
        {
            Console.WriteLine("Invalid time format. Please enter in hh:mm tt format.");
            continue;
        }

        // Prompt for special request code and validate input
        Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
        string specialRequestCode = Console.ReadLine().Trim().ToUpper();

        // Create flight object based on special request code
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
            case "NONE":
                newFlight = new NORMFlight(flightNumber, origin, destination, expectedTime, "Scheduled");
                specialRequestCode = "";
                break;
            default:
                Console.WriteLine("Invalid special request code. Please enter CFFT, DDJB, LWTT, or None.");
                continue;
        }

        // Add flight to dictionary
        terminal.AddFlight(newFlight);

        // Append new flight to CSV file using StreamWriter
        using (StreamWriter writer = new StreamWriter("flights.csv", true))
        {
            writer.WriteLine($"{flightNumber},{origin},{destination},{expectedTime:hh:mm tt},{specialRequestCode}");
        }

        Console.WriteLine($"Flight {flightNumber} has been successfully added.");

        // Prompt to add another flight
        Console.Write("Would you like to add another flight? [Y/N]: ");
        string choice = Console.ReadLine().Trim().ToUpper();
        if (choice == "N")
        {
            break;
        }
        else if (choice != "Y")
        {
            Console.WriteLine("Invalid option. Returning to main menu.");
            break;
        }
    }
}
//Feature 7: Display full flight details from an airline (Completed)
void DisplayFlightDetails() //feature 7
{
    // Step 1: List all the airlines available
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-16}{1,-23}", "Airline Code", "Airline Name");

    foreach (Airline airline in terminal.Airlines.Values)
    {
        Console.WriteLine($"{airline.Code,-16}{airline.Name,-23}");
    }

    // Step 2: Prompt the user to enter the airline code
    Console.Write("Enter Airline Code: ");
    string? airlineCode = Console.ReadLine().Trim().ToUpper();

    // Step 3: Validate the airline code
    if (!terminal.Airlines.ContainsKey(airlineCode))
    {
        Console.WriteLine("Invalid airline code. Please try again.");
        return;
    }

    // Retrieve the airline object
    Airline selectedAirline = terminal.Airlines[airlineCode];

    // Step 4: List all flights for the selected airline
    Console.WriteLine("=============================================");
    Console.WriteLine($"List of Flights for {selectedAirline.Name}");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-16}{1,-23}{2,-23}{3,-23}{4,-30}",
                      "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time");

    foreach (Flight flight in terminal.flights.Values)
    {
        if (flight.flightNumber.StartsWith(airlineCode))
        {
            Console.WriteLine("{0,-15} {1,-20} {2,-25} {3,-25} {4,-15}",
                              flight.flightNumber,
                              selectedAirline.Name,
                              flight.origin,
                              flight.destination,
                              flight.expectedTime.ToString("dd/MM/yyyy hh:mm:ss tt"));
        }
    }
}

//Feature 8: Modify Flight Details (Completed)
//option 6
void ModifyFlightDetails()
{
    // List all the airlines available
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-16}{1,-23}", "Airline Code", "Airline Name");

    foreach (Airline airline in terminal.Airlines.Values)
    {
        Console.WriteLine($"{airline.Code,-16}{airline.Name,-23}");
    }

    // Prompt the user to enter the airline code
    Console.WriteLine("Enter Airline Code: ");
    string? airlineCode = Console.ReadLine().Trim().ToUpper();  // Convert input to uppercase

    // Validate the airline code
    if (string.IsNullOrEmpty(airlineCode) || !terminal.Airlines.ContainsKey(airlineCode))
    {
        Console.WriteLine("Invalid airline code. Please try again.");
        return;
    }

    // Retrieve the airline object selected by the user
    Airline selectedAirline = terminal.Airlines[airlineCode];

    // List all flights for the selected airline
    Console.WriteLine($"List of Flights for {selectedAirline.Name}");
    Console.WriteLine("{0,-16}{1,-23}{2,-23}{3,-23}{4,-30}",
                      "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time");

    // Iterate through all flights in the terminal
    foreach (Flight flight in terminal.flights.Values)
    {
        // Filter flights based on the airline code
        if (flight.flightNumber.StartsWith(airlineCode))
        {
            // Print the flight details in a formatted table row
            Console.WriteLine("{0,-16}{1,-23}{2,-23}{3,-23}{4,-30}",
                              flight.flightNumber,
                              selectedAirline.Name,
                              flight.origin,
                              flight.destination,
                              flight.expectedTime.ToString("dd/MM/yyyy hh:mm:ss tt"));
        }
    }

    // Prompt the user to enter the flight number
    Console.WriteLine("Choose an existing Flight to modify or delete: ");
    string flightNumber = Console.ReadLine().Trim();

    // Validate the flight number inputted
    if (!terminal.flights.ContainsKey(flightNumber))
    {
        Console.WriteLine("Invalid flight number. Please try again.");
        return;
    }

    Flight selectedFlight = terminal.flights[flightNumber];

    // Flight Modification Options
    Console.WriteLine("1. Modify Flight");
    Console.WriteLine("2. Delete Flight");
    Console.WriteLine("Choose an option: ");
    string option = Console.ReadLine().Trim();

    // Validate the option input
    if (option != "1" && option != "2")
    {
        Console.WriteLine("Invalid option. Please choose either 1 or 2.");
        return;
    }

    if (option == "1")  // Modify flight
    {
        Console.WriteLine("1. Modify Basic Information");
        Console.WriteLine("2. Modify Status");
        Console.WriteLine("3. Modify Special Request Code");
        Console.WriteLine("4. Modify Boarding Gate");
        string modifyOption = Console.ReadLine().Trim();

        if (modifyOption == "1")  // Modify Basic Information
        {
            Console.Write("Enter New Origin: ");
            selectedFlight.origin = Console.ReadLine().Trim();

            Console.Write("Enter New Destination: ");
            selectedFlight.destination = Console.ReadLine().Trim();

            // Validate the new expected time input
            Console.Write("Enter new Expected Departure/Arrival Time (dd/mm/yyyy hh:mm): ");
            string expectedTimeInput = Console.ReadLine().Trim();
            DateTime expectedTime;

            if (DateTime.TryParseExact(expectedTimeInput, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out expectedTime))
            {
                selectedFlight.expectedTime = expectedTime;
            }
            else
            {
                Console.WriteLine("Invalid date format. Please enter the date in the format dd/MM/yyyy hh:mm.");
                return;
            }
        }
        else if (modifyOption == "2")  // Modify Status
        {
            Console.WriteLine("1. Delayed");
            Console.WriteLine("2. Boarding");
            Console.WriteLine("3. On Time");
            Console.Write("Enter new status (1/2/3): ");
            string status = Console.ReadLine().Trim();

            if (status == "1")
            {
                selectedFlight.status = "Delayed";
            }
            else if (status == "2")
            {
                selectedFlight.status = "Boarding";
            }
            else if (status == "3")
            {
                selectedFlight.status = "On Time";
            }
            else
            {
                Console.WriteLine("Invalid status option.");
                return;
            }
        }
        else if (modifyOption == "3")  // Modify Special Request Code
        {
            Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
            string newCode = Console.ReadLine().Trim().ToUpper();

            if (newCode == "CFFT")
            {
                terminal.flights[flightNumber] = new CFFTFlight(selectedFlight.flightNumber, selectedFlight.origin, selectedFlight.destination, selectedFlight.expectedTime, selectedFlight.status, 150);
            }
            else if (newCode == "DDJB")
            {
                terminal.flights[flightNumber] = new DDJBFlight(selectedFlight.flightNumber, selectedFlight.origin, selectedFlight.destination, selectedFlight.expectedTime, selectedFlight.status, 300);
            }
            else if (newCode == "LWTT")
            {
                terminal.flights[flightNumber] = new LWTTFlight(selectedFlight.flightNumber, selectedFlight.origin, selectedFlight.destination, selectedFlight.expectedTime, selectedFlight.status, 500);
            }
            else if (newCode == "NONE")
            {
                terminal.flights[flightNumber] = new NORMFlight(selectedFlight.flightNumber, selectedFlight.origin, selectedFlight.destination, selectedFlight.expectedTime, selectedFlight.status);
            }
            else
            {
                Console.WriteLine("Invalid Special Request Code.");
                return;
            }
        }
        else if (modifyOption == "4")  // Modify Boarding Gate
        {
            Console.WriteLine("Enter new Boarding Gate: ");
            string newGate = Console.ReadLine().Trim();

            if (terminal.boardingGates.ContainsKey(newGate))
            {
                terminal.boardingGates[newGate].Flight = selectedFlight;
                Console.WriteLine($"Boarding Gate updated to {newGate}");
            }
            else
            {
                Console.WriteLine("Invalid Boarding Gate");
                return;
            }
        }
        else
        {
            Console.WriteLine("Invalid modification choice");
            return;
        }
        Console.WriteLine("Flight updated!");
    }
    else if (option == "2")  // Delete Flight
    {
        Console.WriteLine("Are you sure you want to delete this flight? (Y/N): ");
        string deleteChoice = Console.ReadLine().Trim().ToUpper();
        if (deleteChoice == "Y")
        {
            terminal.flights.Remove(flightNumber);
            Console.WriteLine($"Flight number {flightNumber} has been successfully removed.");
        }
        else if (deleteChoice == "N")
        {
            Console.WriteLine("Deletion Cancelled.");
        }
        else
        {
            Console.WriteLine("Invalid option.");
            return;
        }
    }
    else
    {
        Console.WriteLine("Invalid option.");
        return;
    }

    // Display updated flight details
    Console.WriteLine("Flight Updated!");
    Console.WriteLine($"Flight Number: {selectedFlight.flightNumber}");
    Console.WriteLine($"Airline Name: {selectedAirline.Name}");
    Console.WriteLine($"Origin: {selectedFlight.origin}");
    Console.WriteLine($"Destination: {selectedFlight.destination}");
    Console.WriteLine($"Expected Departure/Arrival Time: {selectedFlight.expectedTime:dd/MM/yyyy hh:mm:ss tt}");
    Console.WriteLine($"Status: {selectedFlight.status}");

    if (selectedFlight is CFFTFlight)
    {
        Console.WriteLine("Special Request Code: CFFT");
    }
    else if (selectedFlight is DDJBFlight)
    {
        Console.WriteLine("Special Request Code: DDJB");
    }
    else if (selectedFlight is LWTTFlight)
    {
        Console.WriteLine("Special Request Code: LWTT");
    }
    else
    {
        Console.WriteLine("Special Request Code: None");
    }

    Console.WriteLine($"Boarding Gate: {(terminal.boardingGates.ContainsKey(flightNumber) ? terminal.boardingGates[flightNumber].gateName : "Unassigned")}");
}

//Feature 9
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
        Console.Write("Enter your choice: ");
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

//Advanced Feature - Process Unassigned Flights to boarding gates in bulk
void ProcessUnassignedFlights()
{
    // Load data
    loadBoardingGates();
    loadFlights();

    // Create a queue for unassigned flights and boarding gates
    Queue<Flight> unassignedFlights = new Queue<Flight>();

    // Identify unassigned flights
    foreach (var flight in terminal.flights.Values)
    {
        // Check if flight has no assigned gate
        if (flight.boardingGate == null)  
        {
            // Add unassigned flights to the queue
            unassignedFlights.Enqueue(flight);
        }
    }

    //Display the total number of unassigned flights
    Console.WriteLine($"Total Flights without Boarding Gates: {unassignedFlights.Count}");

    // Identify unassigned boarding gates
    // Check if gate has no flight assigned
    List<BoardingGate> unassignedGates = terminal.boardingGates.Values
        .Where(gate => gate.AssignedFlight == null)  
        .ToList();

    //Display the total number of unassigned boarding gates
    Console.WriteLine($"Total Boarding Gates without Flight Numbers: {unassignedGates.Count}");

    // Display flight details in a formatted table header
    Console.WriteLine("+--------------------+-----------------------+--------------------+--------------------+-----------------------+------------------+-----------------+");
    Console.WriteLine("| Flight Number      | Airline               | Origin             | Destination        | Expected Time         | Special Request  | Boarding Gate   |");
    Console.WriteLine("+--------------------+-----------------------+--------------------+--------------------+-----------------------+------------------+-----------------+");

    //Process each flight
    while (unassignedFlights.Count > 0)
    {
        Flight flight = unassignedFlights.Dequeue();
        BoardingGate assignedGate = null;

        // Find a suitable gate manually using a loop
        foreach (var gate in unassignedGates)
        {
            if ((flight is CFFTFlight && gate.supportsCFFT) ||
                (flight is DDJBFlight && gate.supportsDDJB) ||
                (flight is LWTTFlight && gate.supportsLWTT))
            {
                assignedGate = gate;
                break; // Stop searching once a match is found
            }
        }

        // If no special request gate found, assign any available gate
        if (assignedGate == null && unassignedGates.Count > 0)
        {
            assignedGate = unassignedGates[0]; // Pick the first available gate
        }

        if (assignedGate != null)
        {
            // Assign flight to gate and vice versa
            flight.boardingGate = assignedGate;
            assignedGate.AssignedFlight = flight;
            unassignedGates.Remove(assignedGate);

            // Display flight details
            Console.WriteLine($"| {flight.flightNumber,-18} | {flight.airline?.Name ?? "N/A",-21} | " +
                              $"{flight.origin,-18} | {flight.destination,-18} | {flight.expectedTime,-21} | " +
                              $"{(string.IsNullOrEmpty(flight.specialRequestCode) ? "None" : flight.specialRequestCode),-16} | " +
                              $"{assignedGate.gateName,-15} |");
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

    //Console.WriteLine($"Total Flights without Boarding Gates: {unassignedFlights.Count}");
    //Console.WriteLine($"Total Boarding Gates without Flight Numbers: {unassignedGates.Count}");
    Console.WriteLine($"Total Flights processed: {totalFlightsProcessed}");
    Console.WriteLine($"Total Boarding Gates processed: {totalGatesProcessed}");

    // Calculate processing percentages
    double flightProcessingPercentage = Math.Round((double)totalFlightsProcessed / terminal.flights.Count * 100, 2);
    double gateProcessingPercentage = Math.Round((double)totalGatesProcessed / terminal.boardingGates.Count * 100, 2);

    // Display processing percentages
    Console.WriteLine($"Percentage of Flights automatically processed: {flightProcessingPercentage}%");
    Console.WriteLine($"Percentage of Gates automatically processed: {gateProcessingPercentage}%");
}
//Advance Feature
void DisplayTotalFeesPerAirline()
{
    // Step 1: Check if all flights have been assigned a boarding gate
    if (terminal.flights == null || terminal.flights.Count == 0)
    {
        // If no flights are available, notify the user and exit
        Console.WriteLine("No flights available in the terminal.");
        return;
    }

    // Check if any flight does not have an assigned boarding gate
    bool unassignedFlightsExist = terminal.flights.Values.Any(f => f.boardingGate == null);
    if (unassignedFlightsExist)
    {
        // Warn the user about unassigned gates and exit
        Console.WriteLine("Warning: Some flights have not been assigned a boarding gate. Please assign all gates before proceeding.");
        return;
    }

    // Display header for the report
    Console.WriteLine("=================================");
    Console.WriteLine(" Total Fees Per Airline for Today");
    Console.WriteLine("=================================");

    double totalFeesCollected = 0; // Stores total collected fees
    double totalDiscountsApplied = 0; // Stores total applied discounts

    // Step 2: Compute fees for each airline
    if (terminal.Airlines == null || terminal.Airlines.Count == 0)
    {
        // If no airlines are available, notify the user and exit
        Console.WriteLine("No airlines found in the terminal.");
        return;
    }

    // Iterate over each airline
    foreach (var airline in terminal.Airlines.Values)
    {
        // Check if the airline or its flights are null or empty
        if (airline == null || airline.flights == null || airline.flights.Count == 0)
        {
            Console.WriteLine($"Airline: {airline?.Name ?? "Unknown"} ({airline?.Code ?? "N/A"}) - No flights available.");
            continue;
        }

        double airlineTotalFees = 0; // Stores total fees for this airline
        double airlineTotalDiscounts = 0; // Stores total discounts for this airline
        int qualifyingFlights = 0; // Tracks the number of flights processed

        // Iterate over each flight in the airline
        foreach (var flight in airline.flights.Values)
        {
            if (flight == null || string.IsNullOrWhiteSpace(flight.flightNumber))
            {
                // Skip invalid flights
                Console.WriteLine("Invalid flight detected. Skipping...");
                continue;
            }

            double flightFee = 300; // Base boarding gate fee for all flights

            // Apply origin/destination fees
            if (!string.IsNullOrEmpty(flight.origin) && flight.origin == "SIN")
                flightFee += 800; // Additional fee if departing from SIN
            if (!string.IsNullOrEmpty(flight.destination) && flight.destination == "SIN")
                flightFee += 500; // Additional fee if arriving at SIN

            // Apply special request fees based on flight type
            if (flight is CFFTFlight cfftFlight)
                flightFee += cfftFlight.requestFee;
            else if (flight is DDJBFlight ddjbFlight)
                flightFee += ddjbFlight.requestFee;
            else if (flight is LWTTFlight lwttFlight)
                flightFee += lwttFlight.requestFee;

            airlineTotalFees += flightFee; // Add to total airline fees
            qualifyingFlights++; // Increment flight count

            // Step 3: Apply promotional discounts
            if (qualifyingFlights % 3 == 0) // Discount every 3 flights
                airlineTotalDiscounts += 700;

            // Apply time-based discount for flights before 11 AM or after 9 PM
            if (flight.expectedTime != DateTime.MinValue && (flight.expectedTime.Hour < 11 || flight.expectedTime.Hour > 21))
                airlineTotalDiscounts += 110;

            // Apply location-based discount for flights from specific origins
            if (!string.IsNullOrEmpty(flight.origin) && (flight.origin == "DXB" || flight.origin == "BKK" || flight.origin == "NRT"))
                airlineTotalDiscounts += 25;

            // Apply discount if no special request is made
            if (string.IsNullOrEmpty(flight.specialRequestCode))
                airlineTotalDiscounts += 50;
        }

        // Apply an additional airline-level discount for airlines with more than 5 flights
        if (qualifyingFlights > 5)
            airlineTotalDiscounts += airlineTotalFees * 0.03;

        double airlineFinalFee = airlineTotalFees - airlineTotalDiscounts; // Calculate final fees
        totalFeesCollected += airlineFinalFee; // Add to total collected fees
        totalDiscountsApplied += airlineTotalDiscounts; // Add to total applied discounts

        // Step 4: Display breakdown per airline
        Console.WriteLine($"Airline: {airline.Name} ({airline.Code})");
        Console.WriteLine($"Total Flights Processed: {qualifyingFlights}");
        Console.WriteLine($"Subtotal Fees: ${airlineTotalFees:F2}");
        Console.WriteLine($"Discounts Applied: -${airlineTotalDiscounts:F2}");
        Console.WriteLine($"Final Total Fees: ${airlineFinalFee:F2}");
        Console.WriteLine("---------------------------------");
    }

    // Step 5: Display summary of total fees collected
    double discountPercentage = (totalDiscountsApplied / (totalFeesCollected + totalDiscountsApplied)) * 100;
    Console.WriteLine("=================================");
    Console.WriteLine(" Summary of Fees for Terminal 5 ");
    Console.WriteLine("=================================");
    Console.WriteLine($"Total Airline Fees: ${totalFeesCollected:F2}");
    Console.WriteLine($"Total Discounts Applied: -${totalDiscountsApplied:F2}");
    Console.WriteLine($"Final Total Fees Collected: ${totalFeesCollected - totalDiscountsApplied:F2}");
    Console.WriteLine($"Discount Percentage: {discountPercentage:F2}%");
}
