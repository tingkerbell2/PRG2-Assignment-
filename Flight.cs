	//==========================================================
	// Student Number	: S10267822
	// Student Name	: Lau Jia Qi
	// Partner Name	: Ee Ting Li
//==========================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2_Assignment
{
    public abstract class Flight:IComparable<Flight>
    {
        public string flightNumber {  get; set; }
        public string origin {  get; set; }
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
            status = "Scheduled";
        }
        public abstract double CalculateFees();
        public override string ToString()
        {
            return $"{flightNumber}\t{origin}\t{destination}\t{expectedTime:dd/MM/yyyy hh:mm tt}";
        }
        public int CompareTo(Flight other)
        {
            return expectedTime.CompareTo(other.expectedTime);
        }
    }
}

