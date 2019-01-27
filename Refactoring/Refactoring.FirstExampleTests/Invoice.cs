using System.Collections.Generic;

namespace Refactoring.FirstExampleTests
{
    public class Invoice
    {
        public string Customer { get; set; }
        public IEnumerable<Performance> Performances { get; set; }

        public Invoice(string customer, Performance[] performances)
        {
            Customer = customer;
            Performances = performances;
        }
    }
}