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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PRG2_Assignment
{
    class BoardingGate
    {
                //Properties
        public string gateName { get; set; }
        public bool supportsCFFT { get; set; }
        public bool supportsDDJB { get; set; }
        public bool supportsLWTT { get; set; }
        public Flight Flight { get; set; }

        //Added for advanced feature - track assigned flight
        public Flight AssignedFlight { get; set; }

        //Default Constructor
        public BoardingGate()
        {
            //Added for advanced feature
            AssignedFlight = null;
        }

        //Parameterized Constructor
        public BoardingGate(string GateName, bool SupportsDDJB, bool SupportsCFFT, bool SupportsLWTT)
        {
            gateName = GateName;
            supportsDDJB = SupportsDDJB;
            supportsCFFT = SupportsCFFT;
            supportsLWTT = SupportsLWTT;
            this.AssignedFlight = null; // Added for advanced feature

        }

        //Methods
        //Calculate Fees
        public double CalculateFees()
        {
            double baseFee = 300.0;
            if (Flight != null)
            {
                return baseFee + Flight.CalculateFees(); ;
            }
            return baseFee;
        }

        // Advanced Feature: Method to check if gate supports a special request
        public bool SupportsSpecialRequest(string specialRequest)
        {
            return specialRequest switch
            {
                "DDJB" => supportsDDJB,
                "CFFT" => supportsCFFT,
                "LWTT" => supportsLWTT,
                _ => false,
            };
        }
        //ToString() method
        public override string ToString()
        {
            return $"Gate Name: {gateName}\nSupports DDJB: {supportsDDJB}\nSupports CFFT: {supportsCFFT}\nSupports LWTT: {supportsLWTT}\nFlight: {Flight}";
        }
    }
}

