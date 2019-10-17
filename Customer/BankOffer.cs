using System;
using System.Diagnostics.CodeAnalysis;

namespace producer
{
    public class BankOffer : IComparable<BankOffer>
    {
        public string Name { get; set; }
        public double Offer { get; set; }


        public int CompareTo([AllowNull] BankOffer other)
        {
            if(other == null)
                return 1;
            
            return this.Offer.CompareTo(other.Offer);
        }
    }
}