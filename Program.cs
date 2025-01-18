//Completed features (Ting Li) : 1, 3, 4
//Incompleted features (Ting Li): 7,8 + input validations for all

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

//Feature 2: Load Flight Data (Figuring out)
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

//Feature 3: List Flights with basic information (Completed but the date time format wrong + am pm needs to be lower case)
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

//Feature 4: List all boarding gates (Error: Boolean)
void ListBoardingGates()
{
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Boarding Gates for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    //Header
    Console.WriteLine("{0,-16}{1,-23}{2,-23}{3,-23}", "Gate Name", "DDJB", "CFFT", "LWTT");
    // Iterate over boarding gates and print their details
    foreach (var gate in boardinggatesDict.Values)
    {
        Console.WriteLine($"{gate.gateName,-16}{gate.supportsDDJB,-23}{gate.supportsCFFT,-23}{gate.supportsLWTT,-23}");
    }
}

//Feature 7: Display full flight details from an airline (In progress)
void DisplayFlightDetails()
{
    //List all the airlines available
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine("Airline Code\tAirline Name");
    foreach (var Airline in airlinesDict.Values)
    {
        Console.WriteLine($"{Airline.Code}\t\t{Airline.Name}");
    }
    //Prompt user to enter airline code
    Console.Write("Enter Airline Code:");
    string? airlineCode = Console.ReadLine();
}
            
loadAirlines();
loadBoardingGates();
//Print out message
Console.WriteLine("Loading Airlines...");
Console.WriteLine($"{airlinesDict.Count} Airlines Loaded!");
Console.WriteLine("Loading Boarding Gates...");
Console.WriteLine($"{boardinggatesDict.Count} Boarding Gates Loaded!");

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
     //AssignBoardingGate();
   }
   else if (option == 4)
   {
     //CreateFlight();
   }
   else if (option == 5)
   {
        DisplayFlightDetails();
   }
    else if (option == 6)
    {
      //ModifyFlightDetails();
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
