using System;

namespace consumer
{
    public class Bank
    {
        public string Name { get; set; }
        public double Quota1 { get; set; }
        public double Quota2 { get; set; }
        public double Quota3 { get; set; }
    
        public void GenerateLoanInterest(){
            var rng = new Random();
            this.Quota1 = (rng.Next(100,200)/10.0);
            this.Quota2 = (rng.Next(100,200)/10.0);
            this.Quota3 = (rng.Next(100,200)/10.0);
        }

        public double TheDecider(double loanAmount){
            switch (loanAmount)
            {
                case double x when (x >= 0 && x <100.0):
                    return this.Quota1;
                case double x when (x >= 100.0 && x <1000.0):
                    return this.Quota2;
                case double x when (x >= 1000.0):
                    return this.Quota3;
            }
            return -1.0;
        }
    
    }
}