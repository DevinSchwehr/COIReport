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
                authors = (List<Person>)AcquireData.GetData.CreatePeopleList();
                AcquireData.GetData.CreateDictionaries();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Error in acquiring Data. Printing error message" + e.Message);
            }
            Console.WriteLine("Successfully acquired Authors and Data Dictionary.");
            // foreach(Person author in authors)
            for (int i = 4; i < 100; i++)
            {
                //This is just for the test. Do not let this stay in!!
                searchResults = new List<String[]>();

                Person author = authors[i];
                Console.WriteLine("Author #" + (i + 1) + ": " + author.first + " " + author.last + " - " + author.city + "," + GetData.stateDictionary[int.Parse(author.state)]);
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
                    //Thread Thread2018 = new Thread(()=>VoidSearchOPD(2018, author));
                    //Thread Thread2017 = new Thread(()=>VoidSearchOPD(2017, author));
                    //Thread Thread2016 = new Thread(() => VoidSearchOPD(2016, author));
                    //Thread Thread2015 = new Thread(() => VoidSearchOPD(2015, author));
                    //Thread Thread2014 = new Thread(() => VoidSearchOPD(2014, author));
                    //Thread Thread2013 = new Thread(() => VoidSearchOPD(2013, author));
                    if (year == 2018)
                    {
                        //Thread Thread2018 = new Thread(() => SearchOPD(2018, author));
                        //Thread2018.Start();
                        //results.AddRange(SearchOPD(2018, author));
                        //results.AddRange(SearchOPD(2017, author));
                        //results.AddRange(SearchOPD(2016, author));
                        //results.AddRange(SearchOPD(2015, author));
                        //Thread2018.Start();
                        //Thread2017.Start();
                        //Thread2016.Start();
                        //Thread2015.Start();
                        //Thread2018.Join();
                        //Thread2017.Join();
                        //Thread2016.Join();
                        //Thread2015.Join();
                        searchResults.AddRange(GetOpdData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2018"));
                        searchResults.AddRange(GetOpdData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2017"));
                        searchResults.AddRange(GetOpdData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2016"));

                    }
                    else if (year == 2017)
                    {
                        //results.AddRange(SearchOPD(2017, author));
                        //results.AddRange(SearchOPD(2016, author));
                        //results.AddRange(SearchOPD(2015, author));
                        //results.AddRange(SearchOPD(2014, author));
                        //Thread2017.Start();
                        //Thread2016.Start();
                        //Thread2015.Start();
                        //Thread2014.Start();
                        //Thread2017.Join();
                        //Thread2016.Join();
                        //Thread2015.Join();
                        //Thread2014.Join();
                        searchResults.AddRange(GetOpdData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2017"));
                        searchResults.AddRange(GetOpdData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2016"));
                        searchResults.AddRange(GetOpdData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2015"));

                    }
                    else if (year == 2016)
                    {
                        //results.AddRange(SearchOPD(2016, author));
                        //results.AddRange(SearchOPD(2015, author));
                        //results.AddRange(SearchOPD(2014, author));
                        //results.AddRange(SearchOPD(2013, author));
                        //Thread2016.Start();
                        //Thread2015.Start();
                        //Thread2014.Start();
                        //Thread2013.Start();
                        //Thread2016.Join();
                        //Thread2015.Join();
                        //Thread2014.Join();
                        //Thread2013.Join();
                        searchResults.AddRange(GetOpdData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2016"));
                        searchResults.AddRange(GetOpdData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2015"));
                        searchResults.AddRange(GetOpdData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2014"));

                    }
                    else if (year == 2015)
                    {
                        //results.AddRange(SearchOPD(2015, author));
                        //results.AddRange(SearchOPD(2014, author));
                        //results.AddRange(SearchOPD(2013, author));
                        //Thread2015.Start();
                        //Thread2014.Start();
                        //Thread2013.Start();
                        //Thread2015.Join();
                        //Thread2014.Join();
                        //Thread2013.Join();
                        searchResults.AddRange(GetOpdData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2015"));
                        searchResults.AddRange(GetOpdData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2014"));
                        searchResults.AddRange(GetOpdData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2013"));

                    }
                    else if (year == 2014)
                    {
                        //results.AddRange(SearchOPD(2014, author));
                        //results.AddRange(SearchOPD(2013, author));
                        //Thread2014.Start();
                        //Thread2013.Start();
                        //Thread2014.Join();
                        //Thread2013.Join();
                        searchResults.AddRange(GetOpdData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2014"));
                        searchResults.AddRange(GetOpdData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2013"));

                    }
                    else if (year == 2013)
                    {
                        //results.AddRange(SearchOPD(2013, author));
                        //Thread2013.Start();
                        //Thread2013.Join();
                        searchResults.AddRange(GetOpdData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2013"));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                //OutputToCSV(searchResults);
                if (searchResults.Count > 0) { AnalyzeOPDList(searchResults, author); }
                else
                {
                    Console.WriteLine($"Could not find {author.first}, {author.last} at location {author.city}, {author.state}.");
                }
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
            string filePath = @"C:\Users\devin\OneDrive\Documents\COI Report\SearchOutputs\\AuthorSearchOutput2.csv";
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
            string delimiter = ",";

            using (System.IO.TextWriter writer = File.AppendText(filePath))
            {
                for (int index = 0; index < searchResults.Count; index++)
                {
                    writer.WriteLine(string.Join(delimiter, searchResults[index]));
                }
            }
        }

        //Next step to perform: Analyze the list acquired from the OPD and then compare if all of the companies listed in the OPD match with the companies that they listed.
        static void AnalyzeOPDList(List<String[]> rows, Person author)
        {
            List<String> authorCompanies = new List<String>(author.otherCompanies.Split(','));
            string[] authorNumberedCompanies = author.companiesNumbered.Split(',');
            //This list is to do a reverse comparison from author reported companies to the OPD to see if any companies did not 
            //Report a payment where the author did
            HashSet<String> companyHits = new HashSet<string>();
            //Since everything is in numbers, we go through a loop of converting the numbers into companies from the data dictionaries
            //and then we add them to the list of all of the companies the author reported.
            if (!(authorNumberedCompanies[0].Equals("")))
            {
                for (int i = 0; i < authorNumberedCompanies.Length; i++)
                {
                    authorCompanies.Add(GetData.companyDictionary[int.Parse(authorNumberedCompanies[i])].Trim());
                }
            }
            //Now we begin to analyze the reported companies with the name matches from the OPD
            foreach (string[] OPDEntry in rows)
            {
                //If there is an OPD entry that has a company that is not in the list of companies the author reported,
                //Then there is a discrepancy that must be shown to the user.

                //WARNING: try not to do a direct match. maybe try and split the name apart and compare words together. if majority of words match, then its a hit.
                //what if the author reported a non-profit like the National Eye Institute
                //Maybe only compare the first part of the name together.
              //  if (!(authorCompanies.Contains(OPDEntry[25])))
                if(!(findCompany(OPDEntry[25], authorCompanies)))
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
                    //OTHER METHOD: create a list of companies that are a 'hit' and then against the author list to see any companies that are not a hit and report those
                    //              companies as discrepancies.
                    companyHits.Add(OPDEntry[25]);
                }
            }
            //After the loop, we check to see if there are any companies that were reported by the author, but not reported by the 
            if(companyHits.Count > 0)
            {
                //The method here is to go through the set and remove any hits from the authorCompany list.
                //After that, if the redcap list is not empty, then there are discrepancies.
                foreach(string company in companyHits)
                {
                    if(findCompany(company, authorCompanies))
                    {
                        authorCompanies.Remove(company);
                    }
                }
                //after the string, we check to see if there are any companies left that aren't 'other'
                if(authorCompanies.Count > 0)
                {
                    //If there are more companies, report them as discrepancies
                    foreach(string remainingCompany in authorCompanies)
                    {
                        if (!(remainingCompany.Equals("other")))
                        {
                            Console.WriteLine($"DISCREPANCY: Company {remainingCompany} was reported by author but not found within the OPD.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The purpose of this method is to try and match the correct companies from the RedCap entry to the entry from the OPD row. 
        /// I will be looking for keywords in the terms of the example below: 
        /// in Redcap, Allergen is reported as so, but in the OPD it is reported as Allergen Inc., or Allergen Pharmaceuticals. We 
        /// just want to find the Allergen Keyword. 
        /// </summary>
        /// <param name="rowCompanyEntry"></param>
        /// <param name="RedCapCompanies"></param>
        /// <returns></returns>
        static bool findCompany(string rowCompanyEntry, List<String> RedCapCompanies)
        {
            foreach(string company in RedCapCompanies)
            {
                if (!(company.Equals("")))
                {
                    string[] rowEntrySplit = rowCompanyEntry.Split(' ');
                    if(RedCapCompanies.Contains(rowEntrySplit[0])) { return true; }  
                }
            }
            return false;
        }
    }
}
