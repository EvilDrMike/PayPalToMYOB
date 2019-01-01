using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper.Configuration;

namespace PayPalToMYOB
{
    public class FormatConverter : IFormatConverter
    {
        private readonly string _creditAccount;
        private readonly string _debitAccount;

        public FormatConverter(string creditAccount, string debitAccount)
        {
            _creditAccount = creditAccount;
            _debitAccount = debitAccount;
        }

        public string Convert(string payPallFormat)
        {
            var configuration = GetConfiguration();

            var list = ReadString(payPallFormat, configuration);

            return WriteString(list);
        }

        private string WriteString(IEnumerable<PayPalFormat> list)
        {
            var writer = new StringWriter();

            writer.WriteLine("{},,,,,,,,,");
            writer.WriteLine("Journal Number,Date,Memo,Account Number,Is Credit,Amount,Job,Allocation Memo,Category,Is Year-End Adjustment");

            var first = true;
            foreach (var row in list.Where(i => string.Compare(i.Type, "General Withdrawal", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                if (!first)
                {
                    writer.WriteLine(",,,,,,,,,");
                }

                writer.WriteLine(ToDebitLine(row));
                writer.WriteLine(ToCreditLine(row));

                first = false;
            }

            return writer.ToString();
        }

        private string ToCreditLine(PayPalFormat item)
        {
            return $",{item.Date},\"{item.TransactionId}\",{_creditAccount},Y,{decimal.Parse(item.Net) * -1},,,,";
        }

        private string ToDebitLine(PayPalFormat item)
        {
            return $",{item.Date},\"{item.TransactionId}\",{_debitAccount},N,{decimal.Parse(item.Net) * -1},,,,";
        }

        private List<PayPalFormat> ReadString(string civiFormat, Configuration configuration)
        {
            using (var reader = new StringReader(civiFormat))
            {
                var csv = new CsvHelper.CsvReader(reader, configuration);
                csv.Configuration.RegisterClassMap(new PayPalFormatMap());
                csv.Configuration.MissingFieldFound = null;
                var records = csv.GetRecords<PayPalFormat>();
                return records.ToList();
            }
        }

        private static Configuration GetConfiguration()
        {
            var configuration = new Configuration
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                HeaderValidated = (opt1, opt2, opt3, opt4) => { },
                TrimOptions = TrimOptions.Trim
            };
            return configuration;
        }
    }
}
