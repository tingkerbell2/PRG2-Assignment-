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
            string airlineCode = data[0];
            string airlineName = data[1];

            Airline airline = new Airline(airlineCode, airlineName);
            airlinesDict[airlineCode] = airline;
        }
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
            bool supportsDDJB = bool.Parse(data[1]);
            bool supportsCFFT = bool.Parse(data[2]);
            bool supportsLWTT = bool.Parse(data[3]);

            BoardingGate gate = new BoardingGate(boardingGate, supportsDDJB, supportsCFFT, supportsLWTT);
            boardinggatesDict[boardingGate] = gate;
        }
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
            DateTime expectedTime = DateTime.Parse(data[3]);
            string specialRequestCode = data[4];

            //Extract airline code from the first 2 characters of the flight number
            string airlineCode = flightNumber.Substring(0, 2);
        }
    }
}

//Feature 3: List Flights with basic information (Completed but the date time format wrong + am pm needs to be lower case)
void ListFlights()
{
    //New Dictionary to store airline name
    var airlineNames = new Dictionary<string, string>
    {
        { "SQ 115", "Singapore Airlines" },
        { "MH 298", "Malaysia Airlines" },
        { "JL 401", "Japan Airlines" },
        { "CX 453", "Cathay Pacific" },
        { "QF 672", "Qantas Airways" },
        { "TR 123", "AirAsia" },
        { "EK 870", "Emirates" },
        { "BA 450", "British Airways" },
        { "SQ 221", "Singapore Airlines" },
        { "MH 652", "Malaysia Airlines" },
        { "JL 198", "Japan Airlines" },
        { "CX 334", "Cathay Pacific" },
        { "QF 821", "Qantas Airways" },
        { "TR 789", "AirAsia" },
        { "EK 342", "Emirates" },
        { "BA 652", "British Airways" },
        { "SQ 687", "Singapore Airlines" },
        { "MH 223", "Malaysia Airlines" },
        { "JL 324", "Japan Airlines" },
        { "CX 918", "Cathay Pacific" },
        { "QF 456", "Qantas Airways" },
        { "TR 231", "AirAsia" },
        { "EK 123", "Emirates" },
        { "BA 981", "British Airways" },
        { "SQ 512", "Singapore Airlines" },
        { "MH 781", "Malaysia Airlines" },
        { "JL 900", "Japan Airlines" },
        { "CX 329", "Cathay Pacific" },
        { "QF 897", "Qantas Airways" },
        { "TR 786", "AirAsia" }
    };

    var flightDetails = new List<(string flightNumber, string origin, string destination, DateTime expectedTime)>
    {
        ("SQ 115", "Tokyo (NRT)", "Singapore (SIN)", new DateTime(2025, 1, 18, 11, 45, 0)),
        ("MH 298", "Kuala Lumpur (KUL)", "Singapore (SIN)", new DateTime(2025, 1, 18, 12, 30, 0)),
        ("JL 401", "Tokyo (NRT)", "Singapore (SIN)", new DateTime(2025, 1, 18, 13, 10, 0)),
        ("CX 453", "Singapore (SIN)", "Hong Kong (HKD)", new DateTime(2025, 1, 18, 14, 20, 0)),
        ("QF 672", "Singapore (SIN)", "Sydney (SYD)", new DateTime(2025, 1, 18, 15, 50, 0)),
        ("TR 123", "Bangkok (BKK)", "Singapore (SIN)", new DateTime(2025, 1, 18, 10, 0, 0)),
        ("EK 870", "Dubai (DXB)", "Singapore (SIN)", new DateTime(2025, 1, 18, 21, 30, 0)),
        ("BA 450", "Singapore (SIN)", "London (LHR)", new DateTime(2025, 1, 18, 19, 15, 0)),
        ("SQ 221", "Manila (MNL)", "Singapore (SIN)", new DateTime(2025, 1, 18, 8, 50, 0)),
        ("MH 652", "Singapore (SIN)", "Jakarta (CGK)", new DateTime(2025, 1, 18, 12, 40, 0)),
        ("JL 198", "Singapore (SIN)", "Tokyo (NRT)", new DateTime(2025, 1, 18, 13, 30, 0)),
        ("CX 334", "Singapore (SIN)", "Hong Kong (HKD)", new DateTime(2025, 1, 18, 18, 15, 0)),
        ("QF 821", "Singapore (SIN)", "Melbourne (MEL)", new DateTime(2025, 1, 18, 9, 10, 0)),
        ("TR 789", "Bangkok (BKK)", "Singapore (SIN)", new DateTime(2025, 1, 18, 11, 20, 0)),
        ("EK 342", "Dubai (DXB)", "Singapore (SIN)", new DateTime(2025, 1, 18, 8, 50, 0)),
        ("BA 652", "London (LHR)", "Singapore (SIN)", new DateTime(2025, 1, 18, 10, 0, 0)),
        ("SQ 687", "Singapore (SIN)", "Sydney (SYD)", new DateTime(2025, 1, 18, 14, 25, 0)),
        ("MH 223", "Kuala Lumpur (KUL)", "Singapore (SIN)", new DateTime(2025, 1, 18, 16, 0, 0)),
        ("JL 324", "Tokyo (NRT)", "Singapore (SIN)", new DateTime(2025, 1, 18, 17, 45, 0)),
        ("CX 918", "Singapore (SIN)", "Hong Kong (HKD)", new DateTime(2025, 1, 18, 19, 30, 0)),
        ("QF 456", "Singapore (SIN)", "Jakarta (CGK)", new DateTime(2025, 1, 18, 13, 20, 0)),
        ("TR 231", "Singapore (SIN)", "Bangkok (BKK)", new DateTime(2025, 1, 18, 15, 50, 0)),
        ("EK 123", "Dubai (DXB)", "Singapore (SIN)", new DateTime(2025, 1, 18, 6, 45, 0)),
        ("BA 981", "Singapore (SIN)", "Melbourne (MEL)", new DateTime(2025, 1, 18, 11, 15, 0)),
        ("SQ 512", "Manila (MNL)", "Singapore (SIN)", new DateTime(2025, 1, 18, 8, 35, 0)),
        ("MH 781", "Singapore (SIN)", "Sydney (SYD)", new DateTime(2025, 1, 18, 21, 40, 0)),
        ("JL 900", "Tokyo (NRT)", "Singapore (SIN)", new DateTime(2025, 1, 18, 10, 20, 0)),
        ("CX 329", "Singapore (SIN)", "Hong Kong (HKD)", new DateTime(2025, 1, 18, 12, 50, 0)),
        ("QF 897", "Singapore (SIN)", "Sydney (SYD)", new DateTime(2025, 1, 18, 13, 40, 0)),
        ("TR 786", "Bangkok (BKK)", "Singapore (SIN)", new DateTime(2025, 1, 18, 14, 55, 0))
    };


    Console.WriteLine("=============================================");
    Console.WriteLine("List of Flights for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");

    //Header
    Console.WriteLine("Flight Number  Airline Name        Origin                Destination           Expected Departure/Arrival Time");
    foreach (var (flightNumber, Origin, Destination, ExpectedTime) in flightDetails)
    {
        // Lookup the airline name from the dictionary
        string airlineName = airlineNames.ContainsKey(flightNumber) ? airlineNames[flightNumber] : "Unknown Airline";
        Console.WriteLine($"{flightNumber,-15}{airlineName,-20}{Origin,-22}{Destination,-22}{ExpectedTime:dd/MM/yyyy hh:mm:ss tt}");
    }

}

//Feature 4: List all boarding gates (Completed)
void ListBoardingGates()
{
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Boarding Gates for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    //Header
    Console.WriteLine("Gate Name\tDDJB\t\tCFFT\t\tLWTT");
    // Iterate over boarding gates and print their details
    foreach (var gate in boardinggatesDict.Values)
    {
        Console.WriteLine($"{gate.gateName}\t\t{gate.supportsDDJB}\t\t{gate.supportsCFFT}\t\t{gate.supportsLWTT}");
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
Console.WriteLine("Loading Flights...");
Console.WriteLine($"{flightsDict.Count} Flights Loaded!");

//While true loop (for future use)
while (true)
{
   DisplayMenu();
   int option = Convert.ToInt32(Console.ReadLine());
   if (option == 1)
   {
      ListFlights();
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
