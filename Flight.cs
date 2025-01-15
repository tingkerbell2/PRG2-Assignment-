using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ver1
{
    public abstract class Flight
    {
        public string flightNumber { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public DateTime expectedTime { get; set; }
        public string status { get; set; }
        public Flight() { }
        public Flight(string FlightNumber, string Origin, string Destination, DateTime ExpectedTime, string Status)
        {
            flightNumber = FlightNumber;
            origin = Origin;
            destination = Destination;
            expectedTime = ExpectedTime;
            status = Status;
        }
        public abstract double CalculateFees();
        public override string ToString()
        {
            return $"{flightNumber}\t{origin}\t{destination}\t{expectedTime:dd/MM/yyyy hh:mm tt}\t{status}";
        }
    }
}

