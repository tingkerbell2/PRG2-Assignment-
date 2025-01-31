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
    public class CFFTFlight : Flight
    {
    // Properties
    public double requestFee { get; set; }

    // Constructor with BoardingGate support
    public CFFTFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, double requestFee, BoardingGate boardingGate = null)
        : base(flightNumber, origin, destination, expectedTime, status, boardingGate)
    {
        this.requestFee = requestFee;
    }

    // Calculate Fees method
    public override double CalculateFees()
    {
        double baseFee = 150;
        return baseFee + requestFee;
    }

    // ToString() method
    public override string ToString()
    {
        string gateInfo = this.boardingGate != null ? $"Assigned Gate: {this.boardingGate.gateName}" : "No Gate Assigned";
        return base.ToString() + $", Request Fee: {requestFee}, {gateInfo}";
    }
}
