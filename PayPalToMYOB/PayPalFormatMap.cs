using CsvHelper.Configuration;

namespace PayPalToMYOB
{
    public sealed class PayPalFormatMap : ClassMap<PayPalFormat>
    {
        public PayPalFormatMap()
        {
            Map(m => m.Date).Name("Date");
            Map(m => m.TransactionId).Name("Transaction ID");
            Map(m => m.Type);
            Map(m => m.Net);
        }
    }
}