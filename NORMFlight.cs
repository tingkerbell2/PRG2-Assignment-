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
    public class NORMFlight : Flight
    {
        //Constructor
        public NORMFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
        : base(flightNumber, origin, destination, expectedTime, status)
        {
        }

        //CalculateFees method
        public override double CalculateFees()
        {
            //baseFee depends if its arrival or departure
            double baseFee = (destination == "SIN") ? 500 : 800;
            //constant boardingGateFee
            const double boardingGateFee = 300;
            return  baseFee + boardingGateFee;
        }

        //ToString method
        public override string ToString()
        {
            return base.ToString();
        }

    }
}

