
Dictionary<string, Flight> flightsDict = new Dictionary<string, Flight>();
Dictionary<string,Airline> airlinesDict = new Dictionary<string,Airline>();
Dictionary<string,BoardingGate> boardinggatesDict = new Dictionary<string,BoardingGate>();

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

            Airline airline = new Airline(airlineName, airlineCode);
            airlinesDict.Add(airlineCode, airline);
        }
        Console.WriteLine("Loading Airlines...");
        Console.WriteLine($"{airlinesDict.Count()} Airlines Loaded!");
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
            string boardingGate = data[0];
            bool supportsDDJB = bool.Parse(data[1].Trim());
            bool supportsCFFT = bool.Parse(data[2].Trim());
            bool supportsLWTT = bool.Parse(data[3].Trim());

            BoardingGate gate = new BoardingGate(boardingGate, supportsDDJB, supportsCFFT, supportsLWTT);
            boardinggatesDict.Add(boardingGate,gate);
        }
        Console.WriteLine("Loading Boarding Gates...");
        Console.WriteLine($"{boardinggatesDict.Count()} Boarding Gates Loaded!");
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
            flightsDict.Add(flightNumber, flight);
        }
        Console.WriteLine("Loading Flights...");
        Console.WriteLine($"{flightsDict.Count()} Flights Loaded!");
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

    foreach (Flight flight in flightsDict.Values)
    {
        string[] flightNumParts = flight.flightNumber.Split(" ");
        string airlineCode = flightNumParts[0];

        // Initialize airlineName to "" as a default value
        string airlineName = "";

        if (airlinesDict.ContainsKey(airlineCode))
        {
            airlineName = airlinesDict[airlineCode].Name;
        }
        string expectedTimeInfo = "18/1/2025 " + flight.expectedTime.ToString("hh:mm:ss tt");

        Console.WriteLine($"{flight.flightNumber,-16}{airlineName,-23}{flight.origin,-24}{flight.destination,-23}{flight.expectedTime,-20}");
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
    foreach (BoardingGate gate in boardinggatesDict.Values)
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

        //Check if the Flight exists
        if (!flightsDict.ContainsKey(flightNumber))
        {
            Console.WriteLine("Flight not found. Please enter a valid flight number.");
            continue;
        }
        //Get the Flight object
        Flight selectedFlight = flightsDict[flightNumber];

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
        Console.WriteLine();

        //Prompt user for Boarding Gate
        Console.WriteLine("Enter Boarding Gate Number: ");
        string gateNum = Console.ReadLine();

        // Further steps for boarding gate validation and assignment...
        if (!boardinggatesDict.ContainsKey(gateNum))
        {
            Console.WriteLine("Boarding Gate not found. Please enter a valid gate number.");
            continue;
        }

        //Get Boarding Gate object
        BoardingGate gate = boardinggatesDict[gateNum];

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

        Console.WriteLine($"Flight {flightNumber} has been assigned to Boarding Gate {gateNum}");
        Console.WriteLine(selectedFlight.ToString());
        break;        
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

//Feature 8: Modify Flight Details (In progress)
void ModifyFlightDetails()
{
    
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
