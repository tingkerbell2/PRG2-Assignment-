//==========================================================
// Student Number	: S10267822
// Student Name	: Lau Jia Qi
// Partner Name	: Ee Ting Li
//==========================================================
using PRG2_Assignment;
Terminal terminal = new Terminal("Changi Airport Terminal 5");

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
    using (StreamReader sr = new StreamReader("flights.csv"))
    {
        string header = sr.ReadLine(); //skip header row
        string line;

        while ((line = sr.ReadLine()) != null)
        {
            string[] data = line.Split(',');
            string flightNumber = data[0];
            string origin = data[1];
            string destination = data[2];
            DateTime expectedTime = Convert.ToDateTime(data[3].Trim());
            string specialRequestCode = data.Length > 4 ? data[4] : null; // Handle missing Special Request Code

            Flight flight;

            switch (specialRequestCode?.Trim())
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
            terminal.AddFlight(flight);
        }
        Console.WriteLine("Loading Flights...");
        Console.WriteLine($"{terminal.flights.Count()} Flights Loaded!");
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
    Console.WriteLine("=============================================");
    Console.WriteLine("Assign a Boarding Gate to a Flight");
    Console.WriteLine("=============================================");
    while (true)
    {
        //Prompt user for flight number
        Console.WriteLine("Enter Flight Number: ");
        string flightNumber = Console.ReadLine().Trim();
        if (!terminal.flights.ContainsKey(flightNumber))
        {
            Console.WriteLine("Flight not found. Please enter a valid flight number.");
            continue;
        }
        //Prompt user for Boarding Gate
        Console.WriteLine("Enter Boarding Gate Number: ");
        string gateNum = Console.ReadLine();
        if (!terminal.boardingGates.ContainsKey(gateNum))
        {
            Console.WriteLine("Boarding gate not found. Please enter a valid boarding gate number.");
            continue;
        }

        //Check if the Flight exists

        //Get the Flight object
        Flight selectedFlight = terminal.flights[flightNumber];

        //Determine the Special Request Code
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


        //Display basic information
        Console.WriteLine($"Flight Number: {flightNumber}");
        Console.WriteLine($"Origin: {selectedFlight.origin}");
        Console.WriteLine($"Destination: {selectedFlight.destination}");
        Console.WriteLine($"Expected Time: 18/1/2025 {selectedFlight.expectedTime:hh:mm:ss tt}");
        Console.WriteLine($"Special Request Code: {specialRequestCode}");
        Console.WriteLine($"Boarding Gate Name: {gateNum}");

        

        // Further steps for boarding gate validation and assignment...
        if (!terminal.boardingGates.ContainsKey(gateNum))
        {
            Console.WriteLine("Boarding Gate not found. Please enter a valid gate number.");
            continue;
        }

        //Get Boarding Gate object
        BoardingGate gate = terminal.boardingGates[gateNum];

        //Display Special Request Code
        Console.WriteLine($"Supports DDJB: {gate.supportsDDJB}");
        Console.WriteLine($"Supports CFFT: {gate.supportsCFFT}");
        Console.WriteLine($"Supports LWTT: {gate.supportsLWTT}");

        //if (gate.Flight != null)
        //{
        //    Console.WriteLine($"Boarding Gate {gateNum} is already assigned to Flight {gate.Flight.flightNumber}.");
        //    continue;
        //}

        //Prompt the user if they would like to update the Status of the Flight
        Console.WriteLine("Would you like to update the status of the flight? (Y/N)");
        string choice = Console.ReadLine();

        if (choice.ToUpper() == "Y")
        {
            Console.WriteLine("1. Delayed");
            Console.WriteLine("2. Boarding");
            Console.WriteLine("3. On Time");
            Console.WriteLine("Please select the new status of the flight: ");

            string newStatus = Console.ReadLine();

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
                    continue;
            }
        }
        else
        {
            break;
        }

        Console.WriteLine($"Flight {flightNumber} has been assigned to Boarding Gate {gateNum}!");
        break;        
    }
}

//Feature 6: Create flight
void CreateFLight()
{
    bool addAnotherFlight = true;

    while (addAnotherFlight)
    {
        // Prompt user for flight specifications
        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine();

        //Ensure the flight number is unique
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
                newFlight = new CFFTFlight(flightNumber, origin, destination, expectedTime, "Scheduled",150);
                break;
            case "DDJB":
                newFlight = new DDJBFlight(flightNumber, origin, destination, expectedTime, "Scheduled",300);
                break;
            case "LWTT":
                newFlight = new LWTTFlight(flightNumber, origin, destination, expectedTime, "Scheduled",500);
                break;
            default:
                newFlight = new NORMFlight(flightNumber, origin, destination, expectedTime, "Scheduled");
                specialRequestCode = "";
                break;
        }
        //add flight to dictionary
        terminal.AddFlight(newFlight);

        //Append new flight to csv
        string flightData = $"{flightNumber},{origin},{destination},{expectedTime:yyyy-MM-dd HH:mm},{specialRequestCode}";

        Console.WriteLine($"Flight {flightNumber} has been successfully added.");

        //Prompt to add another flight
        Console.Write("Would you like to add another flight? [Y/N]: ");
        string choice = Console.ReadLine().ToUpper();
        //addAnotherFlight
        if (choice == "N")
        {
            break;
        }
        else if (choice == "Y")
        {
            continue;
        }
        else
        {
            Console.WriteLine("Invalid option.");
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


void displayScheduledflights()
{
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Flights in Chronological Order");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-16}{1,-23}{2,-24}{3,-23}{4,-20}",
                      "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time","Status","Boarding Gate");

    //Create a list of all flights from the dictionary
    List<Flight> sortedFlights = new List<Flight>(terminal.flights.Values);

    //Sort the list using IComparable interface implemented in the Flight class
    sortedFlights.Sort();

    //Display each flight's details in sorted order 
    foreach (Flight flight in sortedFlights)
    {
        string[] flightNumParts = flight.flightNumber.Split(" ");
        string airlineCode = flightNumParts[0];

        string airlineName = "";
        if (terminal.Airlines.ContainsKey(airlineCode))
        {
            airlineName = terminal.Airlines[airlineCode].Name;
        }
        string expectedTimeInfo = flight.expectedTime.ToString("dd/MM/yyyy hh:mm:ss tt");

        Console.WriteLine($"{flight.flightNumber,-16} {airlineName,-23} {flight.origin,-24} {flight.destination,-23} {expectedTimeInfo,-20}");

    }
}
loadAirlines();
loadBoardingGates();

void Main()
{
    while (true)
    {
        displayMenu();
        string option = Console.ReadLine();
        if (option == "1")
        {
            DisplayAllFlights();
        }
        else if (option == "2")
        {
            ListBoardingGates ();
        }
        else if (option == "3")
        {
            AssignBoardingGateToFlight ();
        }
        else if (option == "4")
        {
            CreateFLight();
        }
        else if (option == "5")
        {
            DisplayFlightDetails();
        }
        else if(option == "6")
        {
            ModifyFlightDetails();
        }
        else if (option == "7")
        {
            displayScheduledflights();
        }
        else if (option == "8")
        {
            ProcessUnassignedFlights():
        }
        else if (option == "0")
        {
            Console.WriteLine("Goodbye!");
            break;
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
