using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ver1
{
    public class Terminal
    {
        public string terminalName { get; set; }
        public Dictionary<string, Airline> airlines { get; set; }
        public Dictionary<string, Flight> flights { get; set; }
        public Dictionary<string, BoardingGate> boardingGates { get; set; }
        public Dictionary<string, double> gateFees { get; set; }
        public Terminal() { }
        public Terminal(string TerminalName, Dictionary<string, Airline> Airlines, Dictionary<string, Flight> Flights, Dictionary<string, BoardingGate> BoardingGates, Dictionary<string, double> GateFees)
        {
            terminalName = TerminalName;
            airlines = Airlines;
            flights = Flights;
            boardingGates = BoardingGates;
            gateFees = GateFees;
        }
        public void AddAirline(Airline airline)
        {

        }
    }
}

