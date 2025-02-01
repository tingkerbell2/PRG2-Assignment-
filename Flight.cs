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
            public string flightNumber { get; set; }
    public string origin { get; set; }
    public string destination { get; set; }
    public DateTime expectedTime { get; set; }
    public string status { get; set; }

    // Added for advanced feature
    public BoardingGate boardingGate { get; set; }

    public string specialRequestCode { get; set; } 

    public Airline airline { get; set; }
    public Flight() { }

    public Flight(string FlightNumber, string Origin, string Destination, DateTime ExpectedTime, string Status, BoardingGate boardingGate, string specialRequestCode = null)
    {
        flightNumber = FlightNumber;
        origin = Origin;
        destination = Destination;
        expectedTime = ExpectedTime;
        status = Status;
        this.boardingGate = boardingGate; 
        this.specialRequestCode = specialRequestCode;
        this.airline = airline;
    }

    public virtual double CalculateFees()
    {
        return 0;
    }

    public override string ToString()
    {
        return $"{flightNumber}\t{origin}\t{destination}\t{expectedTime:dd/MM/yyyy hh:mm tt}\t{specialRequestCode}";
    }

    public int CompareTo(Flight other)
    {
        return expectedTime.CompareTo(other.expectedTime);
    }
}

