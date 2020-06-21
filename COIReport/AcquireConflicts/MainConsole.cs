using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            foreach(Person author in authors)
            {
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
                    if(year== 2018)
                    {
                        //Thread Thread2018 = new Thread(() => SearchOPD(2018, author));
                        //Thread2018.Start();
                        //results.AddRange(SearchOPD(2018, author));
                        //results.AddRange(SearchOPD(2017, author));
                        //results.AddRange(SearchOPD(2016, author));
                        //results.AddRange(SearchOPD(2015, author));
                        Thread Thread2018 = new Thread(()=>VoidSearchOPD(2018, author));
                        Thread Thread2017 = new Thread(()=>VoidSearchOPD(2017, author));
                        Thread Thread2016 = new Thread(() => VoidSearchOPD(2016, author));
                        Thread Thread2015 = new Thread(() => VoidSearchOPD(2015, author));
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
                        results.AddRange(SearchOPD(2017, author));
                        results.AddRange(SearchOPD(2016, author));
                        results.AddRange(SearchOPD(2015, author));
                        results.AddRange(SearchOPD(2014, author));
                    }
                    else if(year == 2016)
                    {
                        results.AddRange(SearchOPD(2016, author));
                        results.AddRange(SearchOPD(2015, author));
                        results.AddRange(SearchOPD(2014, author));
                        results.AddRange(SearchOPD(2013, author));
                    }
                    else if (year == 2015)
                    {
                        results.AddRange(SearchOPD(2015, author));
                        results.AddRange(SearchOPD(2014, author));
                        results.AddRange(SearchOPD(2013, author));
                    }
                    else if (year == 2014)
                    {
                        results.AddRange(SearchOPD(2014, author));
                        results.AddRange(SearchOPD(2013, author));
                    }
                    else if(year == 2013)
                    {
                        results.AddRange(SearchOPD(2013, author));
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                AnalyzeOPDList(results, author);
            }
        }

        private static void VoidSearchOPD(int year, Person author)
        {
            searchResults.AddRange(GetOpdData.FindPeopleFromOPD(author.first, author.last, author.middle, author.city, GetData.stateDictionary[int.Parse(author.state)],
                     @$"C:\Users\devin\OneDrive\Documents\COI Report\OPD CSVs\{year}\OP_DTL_GNRL_PGYR{year}_P01172020.csv"));
            searchResults.AddRange(GetOpdData.FindPeopleFromOPD(author.first, author.last, author.middle, author.city, GetData.stateDictionary[int.Parse(author.state)],
                    $@"C:\Users\devin\OneDrive\Documents\COI Report\OPD CSVs\{year}\OP_DTL_OWNRSHP_PGYR{year}_P01172020.csv"));
            searchResults.AddRange(GetOpdData.FindPeopleFromOPD(author.first, author.last, author.middle, author.city, GetData.stateDictionary[int.Parse(author.state)],
                    $@"C:\Users\devin\OneDrive\Documents\COI Report\OPD CSVs\{year}\OP_DTL_RSRCH_PGYR{year}_P01172020.csv"));
            
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

        //Next step to perform: Analyze the list acquired from the OPD and then compare if all of the companies listed in the OPD match with the companies that they listed.
        static void AnalyzeOPDList(List<String[]> rows, Person author)
        {
            //These two lines of code get us a list with all of the companies that this person has reported receiving payments from.
            //After this, we will use the rows that we received to 
            List<String> authorReportedCompanies = new List<String>(author.companiesNumbered.Split(','));
            authorReportedCompanies.AddRange(author.otherCompanies.Split(','));
        }
    }
}
