using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ver1
{
    public class CFFTFlight:Flight
    {
        public double requestFee {  get; set; }
        public CFFTFlight() { }
        public CFFTFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
            : base(flightNumber, origin, destination, expectedTime, status)
        {
            requestFee = 150; // Fee for CFFT special request
        }
        public override double CalculateFees() 
        {
            double totalFee = 300; // base fee for boarding gate
            if(origin == "Singapore (SIN)")
            {
                totalFee += 800; //Departing flight fee
            }
            if (destination == "Singapore (SIN)")
            {
                totalFee += 500; //arriving flight fee
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
