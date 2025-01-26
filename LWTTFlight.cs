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
    public class LWTTFlight : Flight
    {
        //Properties
        public double requestFee { get; set; }

        //Constructor
        public LWTTFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, double requestFee)
            : base(flightNumber, origin, destination, expectedTime, status)
        {
            this.requestFee = requestFee;
        }

        //CalculateFees method
        public override double CalculateFees()
        {
            double baseFee = 500;
            return baseFee + requestFee;

        }

        //ToString() method
        public override string ToString()
        {
            return base.ToString() + $", Request Fee: {requestFee}";
        }
    }
}

