using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using AcquireData;
using Newtonsoft.Json;
using Redcap;
using Redcap.Models;

namespace RedcapApiDemo
{
    class Program
    {
        static List<Person> authors;
        static List<String[]> searchResults = new List<String[]>();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello! Welcome to the OPD searcher.");
            Console.WriteLine("Attempting to acquire report and data dictionary");
            try
            {
                authors = (List<Person>) AcquireData.GetData.CreatePeopleList();
                AcquireData.GetData.CreateDictionaries();
            }
            catch(ArgumentException e)
            {
                Console.WriteLine("Error in acquiring Data. Printing error message" + e.Message);
            }
            Console.WriteLine("Successfully acquired Authors and Data Dictionary.");
           // foreach(Person author in authors)
            for(int i = 0; i < 100; i++)
            {
                Person author = authors[i];
                Console.WriteLine("Author #" + (i+1) + ": " + author.first + " " + author.last);
                //What I want to do in this loop.
                //Break down the author to get parameters for FindPeopleFromOPD
                //need to convert state from number to string using metadata.
                //then need to try to do the method and catch any exceptions.
                //if no exceptions caught, then do comparisons.
                //Repeat this for all 3 categories of payments for 4 years back from the receive date
                List<String[]> results = new List<String[]>();
                try
                {
                    string[] dates = author.receiveddate.Split('-');
                    int year = int.Parse(dates[0]);
                        Thread Thread2018 = new Thread(()=>VoidSearchOPD(2018, author));
                        Thread Thread2017 = new Thread(()=>VoidSearchOPD(2017, author));
                        Thread Thread2016 = new Thread(() => VoidSearchOPD(2016, author));
                        Thread Thread2015 = new Thread(() => VoidSearchOPD(2015, author));
                        Thread Thread2014 = new Thread(() => VoidSearchOPD(2014, author));
                        Thread Thread2013 = new Thread(() => VoidSearchOPD(2013, author));
                    if(year== 2018)
                    {
                        //Thread Thread2018 = new Thread(() => SearchOPD(2018, author));
                        //Thread2018.Start();
                        //results.AddRange(SearchOPD(2018, author));
                        //results.AddRange(SearchOPD(2017, author));
                        //results.AddRange(SearchOPD(2016, author));
                        //results.AddRange(SearchOPD(2015, author));
                        Thread2018.Start();
                        Thread2017.Start();
                        Thread2016.Start();
                        Thread2015.Start();
                        Thread2018.Join();
                        Thread2017.Join();
                        Thread2016.Join();
                        Thread2015.Join();
                    }
                    else if(year == 2017)
                    {
                        //results.AddRange(SearchOPD(2017, author));
                        //results.AddRange(SearchOPD(2016, author));
                        //results.AddRange(SearchOPD(2015, author));
                        //results.AddRange(SearchOPD(2014, author));
                        Thread2017.Start();
                        Thread2016.Start();
                        Thread2015.Start();
                        Thread2014.Start();
                        Thread2017.Join();
                        Thread2016.Join();
                        Thread2015.Join();
                        Thread2014.Join();
                    }
                    else if(year == 2016)
                    {
                        //results.AddRange(SearchOPD(2016, author));
                        //results.AddRange(SearchOPD(2015, author));
                        //results.AddRange(SearchOPD(2014, author));
                        //results.AddRange(SearchOPD(2013, author));
                        Thread2016.Start();
                        Thread2015.Start();
                        Thread2014.Start();
                        Thread2013.Start();
                        Thread2016.Join();
                        Thread2015.Join();
                        Thread2014.Join();
                        Thread2013.Join();

                    }
                    else if (year == 2015)
                    {
                        //results.AddRange(SearchOPD(2015, author));
                        //results.AddRange(SearchOPD(2014, author));
                        //results.AddRange(SearchOPD(2013, author));
                        Thread2015.Start();
                        Thread2014.Start();
                        Thread2013.Start();
                        Thread2015.Join();
                        Thread2014.Join();
                        Thread2013.Join();
                    }
                    else if (year == 2014)
                    {
                        //results.AddRange(SearchOPD(2014, author));
                        //results.AddRange(SearchOPD(2013, author));
                        Thread2014.Start();
                        Thread2013.Start();
                        Thread2014.Join();
                        Thread2013.Join();

                    }
                    else if(year == 2013)
                    {
                        //results.AddRange(SearchOPD(2013, author));
                        Thread2013.Start();
                        Thread2013.Join();
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                OutputToCSV(searchResults);
               // AnalyzeOPDList(results, author);
            }
        }

        private static void VoidSearchOPD(int year, Person author)
        {
            searchResults.AddRange(GetOpdData.FindPeopleFromOPD(author.first, author.last, author.middle, author.city, GetData.stateDictionary[int.Parse(author.state)],
                     @$"C:\Users\devin\OneDrive\Documents\COI Report\OPD CSVs\{year}\OP_DTL_GNRL_PGYR{year}_P01172020.csv"));
            //searchResults.AddRange(GetOpdData.FindPeopleFromOPD(author.first, author.last, author.middle, author.city, GetData.stateDictionary[int.Parse(author.state)],
            //        $@"C:\Users\devin\OneDrive\Documents\COI Report\OPD CSVs\{year}\OP_DTL_OWNRSHP_PGYR{year}_P01172020.csv"));
            //searchResults.AddRange(GetOpdData.FindPeopleFromOPD(author.first, author.last, author.middle, author.city, GetData.stateDictionary[int.Parse(author.state)],
            //        $@"C:\Users\devin\OneDrive\Documents\COI Report\OPD CSVs\{year}\OP_DTL_RSRCH_PGYR{year}_P01172020.csv"));
            
        }

        private static IList<String[]> SearchOPD(int year, Person author)
        {
            List<String[]> results = (List<String[]>)GetOpdData.FindPeopleFromOPD(author.first, author.last, author.middle, author.city, GetData.stateDictionary[int.Parse(author.state)],
                     @$"C:\Users\devin\OneDrive\Documents\COI Report\OPD CSVs\{year}\OP_DTL_GNRL_PGYR{year}_P01172020.csv");
            results.AddRange(GetOpdData.FindPeopleFromOPD(author.first, author.last, author.middle, author.city, GetData.stateDictionary[int.Parse(author.state)],
                    $@"C:\Users\devin\OneDrive\Documents\COI Report\OPD CSVs\{year}\OP_DTL_OWNRSHP_PGYR{year}_P01172020.csv"));
            results.AddRange(GetOpdData.FindPeopleFromOPD(author.first, author.last, author.middle, author.city, GetData.stateDictionary[int.Parse(author.state)],
                    $@"C:\Users\devin\OneDrive\Documents\COI Report\OPD CSVs\{year}\OP_DTL_RSRCH_PGYR{year}_P01172020.csv"));
            return results;
        }

        private static void OutputToCSV(List<String[]> searchResults)
        {
            string filePath = @"C:\Users\devin\OneDrive\Documents\COI Report\SearchOutputs\\AuthorSearchOutput.csv";
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
            string delimiter = ",";

            using (System.IO.TextWriter writer = File.AppendText(filePath))
            {
                for(int index = 0; index < searchResults.Count; index++)
                {
                    writer.WriteLine(string.Join(delimiter, searchResults[index]));
                }
            }
        }

        //Next step to perform: Analyze the list acquired from the OPD and then compare if all of the companies listed in the OPD match with the companies that they listed.
        static void AnalyzeOPDList(List<String[]> rows, Person author)
        {
            List<String> authorCompanies = new List<String>(author.otherCompanies.Split(','));
            string[] companyNumbers = author.companiesNumbered.Split(',');
            //Since everything is in numbers, we go through a loop of converting the numbers into companies from the data dictionaries
            //and then we add them to the list of all of the companies the author reported.
            for(int i=0; i<companyNumbers.Length; i++)
            {
                authorCompanies.Add(GetData.companyDictionary[int.Parse(companyNumbers[i])]);
            }
            //Now we begin to analyze the reported companies with the name matches from the OPD
            foreach(string[] OPDEntry in rows)
            {
                //If there is an OPD entry that has a company that is not in the list of companies the author reported,
                //Then there is a discrepancy that must be shown to the user.

                //WARNING: try not to do a direct match. maybe try and split the name apart and compare words together. if majority of words match, then its a hit.
                //what if the author reported a non-profit like the National Eye Institute
                //Maybe only compare the first part of the name together.
                if (!(authorCompanies.Contains(OPDEntry[25])))
                {
                    Console.WriteLine("DISCREPANCY: Company not reported by author. Company is: " + OPDEntry[25] + ", Date of payment: " + OPDEntry[31] + "Type of Payment: " + OPDEntry[34]);
                }
                //If the above statement is false, then we can know that there was a match, and there was no discrepancy
                else
                {
                    //We then remove that company from the list. This will help us later to print out all of the companies that the author reported that were not found in the OPD
                    //ERROR: Cannot delete the author in the case of multiple entries from the company to the author.
                    //FIX: do a loop one way (OPD to author) and then do another loop the other way (author to OPD) to try and find both types of discrepancies.
                    //     this should not be a huge waste of time as the data should still be very small.
                    authorCompanies.Remove(OPDEntry[25]);
                }
            }
            //After the loop, we check the remaining companies reported by the author. If there are any, we report to the user that these companies were not found in the OPD.
            if(authorCompanies.Count > 0)
            {
                foreach(string company in authorCompanies)
                {
                    Console.WriteLine("DISCREPANCY: Company " + company + " was reported by the author, but not found within the OPD.");
                }
            }
        }
    }
}
