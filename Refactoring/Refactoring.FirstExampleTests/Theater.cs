using System;
using System.Globalization;

namespace Refactoring.FirstExampleTests
{
    public class Theater
    {
        private Func<Performance, Play> _playFor;

        public string Statement(Invoice invoice, Func<Performance, Play> playFor)
        {
            _playFor = playFor;

            var totalAmount = 0;
            var volumeCredits = 0;
            var result = $"Statement for {invoice.Customer}\r\n";

            var format = IntlNumberFormat("en-US", "C", "$", 2);

            foreach (var perf in invoice.Performances)
            {
                // add volume credits
                volumeCredits += VolumeCreditsFor(perf);

                // print line for this order
                result += $" {_playFor(perf).Name}: {format(AmountFor(perf) / 100)} ({perf.Audience} seats)\r\n";
                totalAmount += AmountFor(perf);
            }
            result += $"Amount owed is {format(totalAmount / 100)}\r\n";
            result += $"You earned {volumeCredits} credits";
            return result;
        }

        private int VolumeCreditsFor(Performance aPerformance)
        {
            var volumeCredits = Math.Max(aPerformance.Audience - 30, 0);
            // add extra credit for every ten comedy attendees
            if ("comedy" == _playFor(aPerformance).Type) volumeCredits += (int) Math.Floor((double) aPerformance.Audience / 5);
            return volumeCredits;
        }

        private int AmountFor(Performance aPerformance)
        {
            int result;
            switch (_playFor(aPerformance).Type)
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
                    throw new Exception($"unknown type: {_playFor(aPerformance).Type}");
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