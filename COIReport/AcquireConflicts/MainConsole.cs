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
        static string companyHitString; 
        static void Main(string[] args)
        {
            Console.WriteLine("Hello! Welcome to the OPD searcher.");
            Console.WriteLine("Attempting to acquire report and data dictionary");
            try
            {
                //Here we get all of the authors from the RedCap report.
                authors = (List<Person>)AcquireData.GetData.CreatePeopleList();
                AcquireData.GetData.CreateDictionaries();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Error in acquiring Data. Printing error message" + e.Message);
            }
            
            Console.WriteLine("Successfully acquired Authors and Data Dictionary.");
            //foreach(Person author in authors)
            for (int i = 42; i < authors.Count; i++)
            {
                searchResults = new List<String[]>();

                Person author = authors[i];
                 Console.WriteLine("Author #" + (i + 1) + ": " + author.first + " " + author.last + " - " + author.city + "," + GetData.stateDictionary[int.Parse(author.state)] + " Authorship #: " + author.authorshipNumber + " Receive Date: " + author.receiveddate);
                //Console.WriteLine(author.first + " " + author.last + " - " + author.city + "," + GetData.stateDictionary[int.Parse(author.state)]);
                List<String[]> results = new List<String[]>();
                try
                {
                    //While all of these threads are commented out, I will keep them in just in case we need to abandon the SQL method,
                    //At which point threads are the best way of searching all of the years.
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
                        //Thread2018.Start();
                        //Thread2017.Start();
                        //Thread2016.Start();
                        //Thread2015.Start();
                        //Thread2018.Join();
                        //Thread2017.Join();
                        //Thread2016.Join();
                        //Thread2015.Join();
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2018", author.receiveddate));
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2017", author.receiveddate));
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2016", author.receiveddate));
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2015", author.receiveddate));

                    }
                    else if (year == 2017)
                    {
                        //Thread2017.Start();
                        //Thread2016.Start();
                        //Thread2015.Start();
                        //Thread2014.Start();
                        //Thread2017.Join();
                        //Thread2016.Join();
                        //Thread2015.Join();
                        //Thread2014.Join();
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2017", author.receiveddate));
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2016", author.receiveddate));
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2015", author.receiveddate));
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2014", author.receiveddate));

                    }
                    else if (year == 2016)
                    {
                        //Thread2016.Start();
                        //Thread2015.Start();
                        //Thread2014.Start();
                        //Thread2013.Start();
                        //Thread2016.Join();
                        //Thread2015.Join();
                        //Thread2014.Join();
                        //Thread2013.Join();
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2016", author.receiveddate));
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2015", author.receiveddate));
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2014", author.receiveddate));
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2013", author.receiveddate));

                    }
                    else if (year == 2015)
                    {
                        //Thread2015.Start();
                        //Thread2014.Start();
                        //Thread2013.Start();
                        //Thread2015.Join();
                        //Thread2014.Join();
                        //Thread2013.Join();
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2015", author.receiveddate));
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2014", author.receiveddate));
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2013", author.receiveddate));

                    }
                    else if (year == 2014)
                    {
                        //Thread2014.Start();
                        //Thread2013.Start();
                        //Thread2014.Join();
                        //Thread2013.Join();
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2014", author.receiveddate));
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2013", author.receiveddate));

                    }
                    else if (year == 2013)
                    {
                        //Thread2013.Start();
                        //Thread2013.Join();
                        searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, author.city, GetData.stateDictionary[int.Parse(author.state)], "OPD_GNRL_2013", author.receiveddate));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                //This was just for testing purposes.
                //OutputToCSV(searchResults);
                if (searchResults.Count > 0) { AnalyzeOPDList(searchResults, author); }
                else
                {
                    Console.WriteLine($"Could not find {author.first}, {author.last} at location {author.city}, {author.state}.");
                }
            }
        }

        /// <summary>
        /// this method is used by the threads to search the OPD. right now it is not being used but if the threads have to
        /// be used again this is the method they use.
        /// </summary>
        /// <param name="year">the year to search</param>
        /// <param name="author">the author being searched</param>
        private static void VoidSearchOPD(int year, Person author)
        {
            searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPD(author.first, author.last, author.middle, author.city, GetData.stateDictionary[int.Parse(author.state)],
                     @$"C:\Users\devin\OneDrive\Documents\COI Report\OPD CSVs\{year}\OP_DTL_GNRL_PGYR{year}_P01172020.csv"));
            //searchResults.AddRange(GetOpdData.FindPeopleFromOPD(author.first, author.last, author.middle, author.city, GetData.stateDictionary[int.Parse(author.state)],
            //        $@"C:\Users\devin\OneDrive\Documents\COI Report\OPD CSVs\{year}\OP_DTL_OWNRSHP_PGYR{year}_P01172020.csv"));
            //searchResults.AddRange(GetOpdData.FindPeopleFromOPD(author.first, author.last, author.middle, author.city, GetData.stateDictionary[int.Parse(author.state)],
            //        $@"C:\Users\devin\OneDrive\Documents\COI Report\OPD CSVs\{year}\OP_DTL_RSRCH_PGYR{year}_P01172020.csv"));

        }

        /// <summary>
        /// This method uses a filewriter to create an excel sheet. It was used for testing and reviewing that
        /// we were finding the right author.
        /// </summary>
        /// <param name="searchResults">the result of searching the OPD </param>
        private static void OutputToCSV(List<String[]> searchResults)
        {
            string filePath = @"C:\Users\devin\OneDrive\Documents\COI Report\SearchOutputs\\PaymentOutputsJenniferTest.csv";
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

        /// <summary>
        /// This is the method to analyze the results from the OPD list and find any
        /// Discrepancies. 
        /// 
        /// There are two forms of discrepancies that we are looking for: 
        /// 1) The author did not report the company, but that company was found within the OPD
        /// 2) The author did report the company, but said company was not found within the OPD
        /// 
        /// Any discrepancies will be Output in the console to be viewed.
        /// </summary>
        /// <param name="rows">the OPD entries</param>
        /// <param name="author">the author being analyzed.</param>
        static void AnalyzeOPDList(List<String[]> rows, Person author)
        {
            SortAlphabetically(rows);
            List<String> authorCompanies = new List<String>(author.otherCompanies.Split(','));
            string[] authorNumberedCompanies = author.companiesNumbered.Split(',');
            //we find the author's position here to use later.
            string position = GetOPDAndSQLData.FindAuthorPosition(author.first, author.last);
            //This List will be used to output the results to a csv
            List<String[]> outputs = new List<String[]>();
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

                if(!(findCompany(OPDEntry[25], authorCompanies)))
                {

                    string[] currentLine = { author.authorshipNumber.ToString(), OPDEntry[5], author.last, author.first, position, author.articleNumber, author.journal, "Undisclosed","\""+ OPDEntry[25] + "\"", OPDEntry[26],OPDEntry[45], OPDEntry[31], OPDEntry[OPDEntry.Length - 1], "\"" + OPDEntry[34] + "\"", OPDEntry[33], OPDEntry[30] };
                    outputs.Add(currentLine);                   
                    // Console.WriteLine("DISCREPANCY: Company not reported by author. Company is: " + OPDEntry[25] + ", Date of payment: " + OPDEntry[31] + " Type of Payment: " + OPDEntry[34] + "Payment Amount: " + OPDEntry[30] + " "+ GetOpdData.FindAuthorPosition(author.first, author.last)); ;
                }
                //If the above statement is false, then we can know that there was a match, and there was no discrepancy
                else
                {
                    string[] currentLine = { author.authorshipNumber.ToString(), OPDEntry[5], author.last, author.first, position, author.articleNumber, author.journal, "Reported", "\"" + OPDEntry[25] + "\"", OPDEntry[26], OPDEntry[45], OPDEntry[31], OPDEntry[OPDEntry.Length - 1], "\"" +  OPDEntry[34] + "\"", OPDEntry[33], OPDEntry[30] };
                    outputs.Add(currentLine);
                   // Console.WriteLine($"SUCCESSFUL MATCH: Author reported company {OPDEntry[25]} in their list of companies.");
                    companyHits.Add(companyHitString);
                }
            }
            OutputToCSV(outputs);
            //This if statement is to catch if there were not 'hits', but the author still had companies reported.
            if(companyHits.Count == 0 && !(authorCompanies[0].Equals("")))
            {
                foreach(string company in authorCompanies)
                {
                    Console.WriteLine($"DISCREPANCY: author reported company {company}, but said company was not found within the OPD.");
                }
            }
            //This else statement is for finding the remaining companies that were hits
            else if(companyHits.Count != 0)
            {
                //As we find hits, we remove them from the company list.
                foreach(string hit in companyHits)
                {
                    authorCompanies.Remove(hit);
                }
                //If there are any companies left, then there are companies the author reported that were not found within the OPD
                if(authorCompanies.Count > 0)
                {
                    foreach(string company in authorCompanies)
                    {
                        if (!company.Equals("Other"))
                        {
                        Console.WriteLine($"DISCREPANCY: author reported company {company}, but said company was not found within the OPD.");
                        }
                    }
                }
            }
        }

        //private static string PaymentSubtype(string[] OPDEntry, Person author)
        //{
        //    if (OPDEntry[OPDEntry.Length - 1].Equals("General"))
        //    {
        //       GetData.typeDictionary(author)
        //    }
        //}

        private static void SortAlphabetically(List<string[]> rows)
        {
            rows.Sort(CompareByName);
        }

        private static int CompareByName(string[] firstRow, string[] secondRow)
        {
            return string.Compare(firstRow[25], secondRow[25]);
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
        private static bool findCompany(string rowCompanyEntry, List<String> RedCapCompanies)
        {
            foreach(string company in RedCapCompanies)
            {
                //Revamping the method! I'm flipping it now to see if the Redcap company (which is smaller, one word) is found within the larger
                //OPD Company name. This should make it simpler to check. 
                if (!(company.Equals("Other")))
                {
                    if (rowCompanyEntry.ToUpper().Contains(company.ToUpper()) && !(company.Equals("")))
                    {
                        companyHitString = company;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
