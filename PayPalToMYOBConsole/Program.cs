using Fclp;
using PayPalToMYOB;

namespace PayPalToMYOBConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFileName = "";
            var outputFileName = "";
            var creditAccount = "";
            var debitAccount = "";

            var parser = new FluentCommandLineParser();
            parser.Setup<string>('i').Callback(val => inputFileName = val);
            parser.Setup<string>('o').Callback(val => outputFileName = val);
            parser.Setup<string>('c').Callback(val => creditAccount = val);
            parser.Setup<string>('d').Callback(val => debitAccount = val);
            parser.Parse(args);

            var outfile = new System.IO.StreamWriter(outputFileName);
            outfile.Write(new FormatConverter(creditAccount, debitAccount).Convert(System.IO.File.ReadAllText(inputFileName)));
            outfile.Flush();
        }
    }
}
