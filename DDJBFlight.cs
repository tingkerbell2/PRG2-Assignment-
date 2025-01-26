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
    class DDJBFlight: Flight
    {
        public double requestFee { get; set; }
        public DDJBFlight() { }
        public DDJBFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
            : base(flightNumber, origin, destination, expectedTime, status)
        {
            requestFee = 200; // Fee for DDJB special request
        }
        public override double CalculateFees()
        {
            // base fee for boarding gate
            double totalFee = 300; 
            if (origin == "Singapore (SIN)")
            {
                totalFee += 800; 
            }
            if (destination == "Singapore (SIN)")
            {
                totalFee += 500; 
            }
            totalFee += requestFee;
            return totalFee;
        }
        public override string ToString()
        {
            return $"\t{requestFee}";
        }
    }
}
