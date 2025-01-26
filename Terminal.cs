using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ver1
{
        public class Terminal
    {
        public string TerminalName { get; set; }
        public Dictionary<string, Airline> Airlines { get; private set; }
        public Dictionary<string, Flight> flights { get; private set; }
        public Dictionary<string, BoardingGate> boardingGates { get; private set; }
        public Dictionary<string, double> gateFees { get; private set; }

        public Terminal()
        {
            Airlines = new Dictionary<string, Airline>();
            flights = new Dictionary<string, Flight>();
            boardingGates = new Dictionary<string, BoardingGate>();
            gateFees = new Dictionary<string, double>();
        }

        public Terminal(string terminalName) : this()
        {
            TerminalName = terminalName;
        }

        public bool AddAirline(Airline airline)
        {
            if (Airlines.ContainsKey(airline.Code)) return false;
            Airlines[airline.Code] = airline;
            return true;
        }

        public bool AddFlight(Flight flight)
        {
            if (flights.ContainsKey(flight.flightNumber)) return false;
            flights[flight.flightNumber] = flight;
            return true;
        }

        public bool AddBoardingGate(BoardingGate gate)
        {
            if (boardingGates.ContainsKey(gate.gateName)) return false;
            boardingGates[gate.gateName] = gate;
            return true;
        }

        public Airline GetAirlineFromFlight(Flight flight)
        {
            return Airlines.Values.FirstOrDefault(airline => airline.flights.ContainsKey(flight.flightNumber));
        }

        public void PrintAirlineFees()
        {
            foreach (var airline in Airlines.Values)
            {
                Console.WriteLine($"{airline.Name} Fees: {airline.CalculateFees():C}");
            }
        }

        public override string ToString()
        {
            return $"Terminal: {TerminalName}, Airlines: {Airlines.Count}, Flights: {flights.Count}, Boarding Gates: {boardingGates.Count}";
        }
    }
}



