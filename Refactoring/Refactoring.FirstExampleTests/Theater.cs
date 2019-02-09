using System;
using System.Collections.Generic;
using System.Globalization;

namespace Refactoring.FirstExampleTests
{
    public class Theater
    {
        public string Statement(Invoice invoice, Dictionary<string, Play> plays, Func<Performance, Play> playFor)
        {
            var totalAmount = 0;
            var volumeCredits = 0;
            var result = $"Statement for {invoice.Customer}\r\n";

            var format = IntlNumberFormat("en-US", "C", "$", 2);

            foreach (var perf in invoice.Performances)
            {
                var thisAmount = 0;

                thisAmount = AmountFor(perf, playFor(perf));

                // add volume credits
                volumeCredits += Math.Max(perf.Audience - 30, 0);
                // add extra credit for every ten comedy attendees
                if ("comedy" == playFor(perf).Type) volumeCredits += (int)Math.Floor((double)perf.Audience / 5);

                // print line for this order
                result += $" {playFor(perf).Name}: {format(thisAmount / 100)} ({perf.Audience} seats)\r\n";
                totalAmount += thisAmount;
            }
            result += $"Amount owed is {format(totalAmount / 100)}\r\n";
            result += $"You earned {volumeCredits} credits";
            return result;
        }

        private static int AmountFor(Performance aPerformance, Play play)
        {
            int result;
            switch (play.Type)
            {
                case "tragedy":
                    result = 40000;
                    if (aPerformance.Audience > 30)
                    {
                        result += 1000 * (aPerformance.Audience - 30);
                    }

                    break;
                case "comedy":
                    result = 30000;
                    if (aPerformance.Audience > 20)
                    {
                        result += 10000 + 500 * (aPerformance.Audience - 20);
                    }

                    result += 300 * aPerformance.Audience;
                    break;
                default:
                    throw new Exception($"unknown type: {play.Type}");
            }

            return result;
        }

        /// <summary>
        /// In Fowler's Refactoring book the example was written in Javascript.
        /// The format function was implemented as follows:
        /// <code>const format = new Intl.NumberFormat(...)</code>
        /// To keep the example more or less the same I used the Format method to format the amounts.
        /// </summary>
        /// <param name="locale"></param>
        /// <param name="style"></param>
        /// <param name="currency"></param>
        /// <param name="minimalFractionDigits"></param>
        /// <returns></returns>
        private Func<int, string> IntlNumberFormat(string locale, string style, string currency, int minimalFractionDigits)
        {
            var format = new CultureInfo(locale, false).NumberFormat;
            format.CurrencyDecimalDigits = minimalFractionDigits;
            format.CurrencySymbol = currency;
            format.CurrencyDecimalSeparator = ".";
            format.CurrencyGroupSeparator = ",";

            return s => s.ToString(style, format);
        }
    }
}