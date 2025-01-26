
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
        //string expectedTimeInfo = "18/1/2025 " + flight.expectedTime.ToString("hh:mm:ss tt");

        Console.WriteLine($"{flight.flightNumber,-16}{airlineName,-23}{flight.origin,-24}{flight.destination,-23}{flight.expectedTime:dd/MM/yyyy hh:mm:ss tt}");
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

    foreach (var airline in airlinesDict.Values)
    {
        Console.WriteLine($"{airline.Code,-16}{airline.Name,-23}");
    }

    // Step 2: Prompt the user to enter the airline code
    Console.Write("Enter Airline Code: ");
    string? airlineCode = Console.ReadLine()?.Trim();

    // Step 3: Validate the airline code
    if (!airlinesDict.ContainsKey(airlineCode))
    {
        Console.WriteLine("Invalid airline code. Please try again.");
        return;
    }

    // Retrieve the airline object
    Airline selectedAirline = airlinesDict[airlineCode];

    // Step 4: List all flights for the selected airline
    Console.WriteLine("=============================================");
    Console.WriteLine($"List of Flights for {selectedAirline.Name}");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-16}{1,-23}{2,-23}{3,-23}{4,-30}",
                      "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time");

    var airlineFlights = flightsDict.Values
        .Where(flight => flight.flightNumber.StartsWith(airlineCode))
        .ToList();

    if (airlineFlights.Count == 0)
    {
        Console.WriteLine("No flights available for this airline.");
        return;
    }

    foreach (var flight in airlineFlights)
    {
        Console.WriteLine($"{flight.flightNumber,-16}{selectedAirline.Name,-23}{flight.origin,-23}{flight.destination,-23}{flight.expectedTime:dd/MM/yyyy hh:mm:ss tt}");
    }
}

//Feature 8: Modify Flight Details (Completed)
void ModifyFlightDetails()
{
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-16}{1,-23}", "Airline Code", "Airline Name");

    foreach (var airline in airlinesDict.Values)
    {
        Console.WriteLine($"{airline.Code,-16}{airline.Name,-23}");
    }

    Console.Write("Enter Airline Code: ");
    string? airlineCode = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(airlineCode) || !airlinesDict.ContainsKey(airlineCode))
    {
        Console.WriteLine("Invalid airline code. Please try again.");
        return; // Changed from continue to return
    }

    Airline selectedAirline = airlinesDict[airlineCode];

    Console.WriteLine($"List of Flights for {selectedAirline.Name}");
    Console.WriteLine("{0,-16}{1,-23}{2,-23}{3,-23}{4,-30}", "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time");

    var airlineFlights = flightsDict.Values
        .Where(flight => flight.flightNumber.StartsWith(airlineCode))
        .ToList();

    if (airlineFlights.Count == 0)
    {
        Console.WriteLine("No flights available for this airline.");
        return; // Changed from continue to return
    }

    foreach (var flight in airlineFlights)
    {
        Console.WriteLine($"{flight.flightNumber,-16}{selectedAirline.Name,-23}{flight.origin,-23}{flight.destination,-23}{flight.expectedTime:dd/MM/yyyy hh:mm:ss tt}");
    }

    Console.Write("Choose an existing Flight to modify or delete: ");
    string? flightNumber = Console.ReadLine()?.Trim();

    if (!flightsDict.ContainsKey(flightNumber))
    {
        Console.WriteLine("Invalid flight number. Please try again.");
        return; // Changed from continue to return
    }

    Flight selectedFlight = flightsDict[flightNumber];

    Console.WriteLine("1. Modify Flight");
    Console.WriteLine("2. Delete Flight");
    Console.Write("Choose an option: ");
    string? actionChoice = Console.ReadLine()?.Trim();

    if (actionChoice == "2")
    {
        flightsDict.Remove(flightNumber);
        Console.WriteLine($"Flight {flightNumber} has been deleted successfully.");
        return; // Changed from continue to return
    }
    else if (actionChoice != "1")
    {
        Console.WriteLine("Invalid option. Returning to menu.");
        return; // Changed from continue to return
    }

    Console.WriteLine("1. Modify Basic Information");
    Console.WriteLine("2. Modify Status");
    Console.WriteLine("3. Modify Special Request Code");
    Console.WriteLine("4. Modify Boarding Gate");
    Console.Write("Choose an option: ");
    string? choice = Console.ReadLine()?.Trim();

    switch (choice)
    {
        case "1":
            // Modify basic information
            Console.Write("Enter new Origin: ");
            selectedFlight.origin = Console.ReadLine()?.Trim();
            Console.Write("Enter new Destination: ");
            selectedFlight.destination = Console.ReadLine()?.Trim();
            Console.Write("Enter new Expected Departure/Arrival Time (dd/MM/yyyy hh:mm): ");
            string? newTimeInput = Console.ReadLine()?.Trim();
            if (DateTime.TryParse(newTimeInput, out DateTime newTime))
            {
                selectedFlight.expectedTime = newTime;
            }
            else
            {
                Console.WriteLine("Invalid time format. No changes made to the time.");
            }
            break;

        case "2":
            // Modify status
            Console.WriteLine("1. Delayed");
            Console.WriteLine("2. Boarding");
            Console.WriteLine("3. On Time");
            Console.Write("Choose a new status: ");
            string? statusOption = Console.ReadLine()?.Trim();
            switch (statusOption)
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
                    Console.WriteLine("Invalid status. No changes made.");
                    break;
            }
            break;

        case "3":
            // Modify special request code (dummy modification since flight types are static)
            Console.WriteLine("Special Request Code cannot be modified.");
            break;

        case "4":
            // Modify boarding gate (handled in external dictionary)
            Console.Write("Enter new Boarding Gate: ");
            string? newGate = Console.ReadLine()?.Trim();
            if (newGate != null)
            {
                BoardingGate gate = new BoardingGate(newGate, true, true, true);
                boardinggatesDict[flightNumber] = gate;
                Console.WriteLine($"Boarding gate updated for Flight {flightNumber}.");
            }
            else
            {
                Console.WriteLine("Invalid gate. No changes made.");
            }
            break;

        default:
            Console.WriteLine("Invalid option. Returning to menu.");
            return; // Changed from continue to return
    }

    // Display updated flight details
    Console.WriteLine("Flight updated!");
    Console.WriteLine($"Flight Number: {selectedFlight.flightNumber}");
    Console.WriteLine($"Airline Name: {selectedAirline.Name}");
    Console.WriteLine($"Origin: {selectedFlight.origin}");
    Console.WriteLine($"Destination: {selectedFlight.destination}");
    Console.WriteLine($"Expected Departure/Arrival Time: {selectedFlight.expectedTime:dd/MM/yyyy hh:mm:ss tt}");
    Console.WriteLine($"Status: {selectedFlight.status}");
    Console.WriteLine($"Special Request Code: CFFT"); // Assuming CFFT is the default code
    Console.WriteLine($"Boarding Gate: {(boardinggatesDict.ContainsKey(flightNumber) ? boardinggatesDict[flightNumber].gateName : "Unassigned")}");
}

void displayScheduledflights() //Feature 9 (output different from answer)
{
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Flights in Chronological Order");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-16}{1,-23}{2,-24}{3,-23}{4,-20}",
                      "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time","Status","Boarding Gate");

    //Create a list of all flights from the dictionary
    List<Flight> sortedFlights = new List<Flight>(flightsDict.Values);

    //Sort the list using IComparable interface implemented in the Flight class
    sortedFlights.Sort();

    //Display each flight's details in sorted order 
    foreach (Flight flight in sortedFlights)
    {
        string[] flightNumParts = flight.flightNumber.Split(" ");
        string airlineCode = flightNumParts[0];

        string airlineName = "";
        if (airlinesDict.ContainsKey(airlineCode))
        {
            airlineName = airlinesDict[airlineCode].Name;
        }
        string expectedTimeInfo = flight.expectedTime.ToString("dd/MM/yyyy hh:mm:ss tt");

        Console.WriteLine($"{flight.flightNumber,-16} {airlineName,-23} {flight.origin,-24} {flight.destination,-23} {expectedTimeInfo,-20}");

    }
}
loadAirlines();
loadBoardingGates();

//While true loop (for future use)
while (true)
{
   DisplayMenu();
   int option = Convert.ToInt32(Console.ReadLine());
   if (option == 1)
   {
      DisplayAllFlights();
   }

   else if (option == 2)
   {
      ListBoardingGates();
   }
   else if (option == 3)
   { 
     AssignBoardingGateToFlight();
   }
   else if (option == 4)
   {
     CreateFlight();
   }
   else if (option == 5)
   {
        DisplayFlightDetails();
   }
    else if (option == 6)
    {
      ModifyFlightDetails();
    }
    else if (option == 7)
    {
      //DisplayFlightSchedule();
    }
    else if (option == 0)
    {
        break;
    }
    else
    {
         Console.WriteLine("Invalid option. Please try again.");
    }
}
