using System;

namespace ChangeCSVToTSV
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting process.");
                var document = new Aspose.Cells.Workbook(@"C:\Users\devin\OneDrive\Documents\COI Report\OPD CSVs\2018\OP_DTL_GNRL_PGYR2018_P06302020 - Copy.csv");
                document.Save(@"C:\Users\devin\OneDrive\Documents\COI Report\OPD CSVs\2018\OPD2018GNRL.tsv", Aspose.Cells.SaveFormat.TSV);
            Console.WriteLine("Process finsihed.");
        }
    }
}
