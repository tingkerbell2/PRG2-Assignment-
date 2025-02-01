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
    class Airline
    {
            //Properties
    public string Name { get; set; }
    public string Code { get; set; }
    public Dictionary<string, Flight> flights;

    //Default Constructor
    public Airline() 
    {
        flights = new Dictionary<string, Flight>();
    }

    //Parameterized Constructor
    public Airline(string Name, string Code)
    {
        this.Name = Name;
        this.Code = Code;
        flights = new Dictionary<string, Flight>();
    }

    //Methods 
    //Add Flight to airline
    public bool AddFlight(Flight flight)
    {
        if (flights.ContainsKey(flight.flightNumber))
        {
            Console.WriteLine($"Flight {flight.flightNumber} already exists in the airline");
            return false;
        }

        // Ensure the flight has an airline assigned before adding it to the collection
        if (flight.airline == null)
        {
            flight.airline = this;  // Set the current airline as the flight's airline
        }

        flights[flight.flightNumber] = flight;
        Console.WriteLine($"Flight {flight.flightNumber} successfully added to the airline");
        return true;
    }


    //Calculate Fees
    public double CalculateFees()
    {
        double totalFees = 0;
        foreach (Flight flight in flights.Values)
        {
            totalFees += flight.CalculateFees();
        }
        return totalFees;
    }

    //Remove Flight from airline
    public bool RemoveFlight(Flight flight)
    {
        if (!flights.ContainsKey(flight.flightNumber))
        {
            Console.WriteLine($"Error occurred. Flight {flight.flightNumber} does not exist in the airline");
            return false;
        }

        flights.Remove(flight.flightNumber);
        return true;
    }

    //ToString()
    public override string ToString()
    {
        string str = $"Airline: {Name} ({Code})\n";
        if (flights.Count > 0)
        {
            foreach (Flight flight in flights.Values)
            {
                str += flight.ToString() + "\n";
            }
        }
        else
        {
            str += "No flights available.\n";
        }
        return str;
    }
}
