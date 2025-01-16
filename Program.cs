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
