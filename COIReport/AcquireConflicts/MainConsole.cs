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
        static string companyOutputFilePath = @"D:\Users\u1205752\Documents\COI\JAMAReal2.csv";
        static string authoroutputFilePath = @"D:\Users\u1205752\Documents\COI\JAMARealAuthor2.csv";

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
            for (int i = 0; i < authors.Count; i++)
            {
                searchResults = new List<String[]>();

                Person author = authors[i];

                 Console.WriteLine("Author #" + (i + 1) + ": " + author.first + " " + author.last + " - " + author.cities + "," + author.states + " Authorship #: " + author.authorshipNumber + " Receive Date: " + author.receiveddate);
                //Console.WriteLine(author.first + " " + author.last + " - " + author.city + "," + GetData.stateDictionary[int.Parse(author.state)]);
                List<String[]> results = new List<String[]>();
                try
                {
                    //While all of these threads are commented out, I will keep them in just in case we need to abandon the SQL method,
                    //At which point threads are the best way of searching all of the years.
                    string[] dates = author.receiveddate.Split('-');
                    int year = int.Parse(dates[0]);

                    //This is to try and find the PhysicianID before doing any major searching.
                    for(int currentYear = year; currentYear >= year-4; currentYear--)
                    {
                        //If the Physician ID has been found, break out of the loop
                        if(GetOPDAndSQLData.PhysicianID != null) { break; }

                        GetOPDAndSQLData.AcquirePhysicianID(author.first, author.last, author.cities,author.states, $"OPD_GNRL_{currentYear}");
                    }
                    //If we find the Physician ID, then we begin searching through all of the tables.
                    if(GetOPDAndSQLData.PhysicianID != null)
                    {
                        if (year == 2018)
                        {
                            OPDYearSearch(2018, author);
                            OPDYearSearch(2017, author);
                            OPDYearSearch(2016, author);
                            OPDYearSearch(2015, author);
                        }
                        else if (year == 2017)
                        {
                            OPDYearSearch(2017, author);
                            OPDYearSearch(2016, author);
                            OPDYearSearch(2015, author);
                            OPDYearSearch(2014, author);
                        }
                        else if (year == 2016)
                        {
                            OPDYearSearch(2016, author);
                            OPDYearSearch(2015, author);
                            OPDYearSearch(2014, author);
                            OPDYearSearch(2013, author);
                        }
                        else if (year == 2015)
                        {
                            OPDYearSearch(2015, author);
                            OPDYearSearch(2014, author);
                            OPDYearSearch(2013, author);
                        }
                        else if (year == 2014)
                        {
                            OPDYearSearch(2014, author);
                            OPDYearSearch(2013, author);
                        }
                        else if (year == 2013)
                        {
                            OPDYearSearch(2013, author);
                        }
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
                    // Console.WriteLine($"Could not find {author.first}, {author.last} at location {author.city}, {author.state}.");
                    string[] row = { author.first, author.last, author.authorshipNumber.ToString(), "Not Found", author.cities.Replace(',',';'), author.states.Replace(',', ';')," ", author.position, author.articleNumber, author.journal, author.receiveddate, "N/A" };
                    OutputToCSV(row, authoroutputFilePath);
                }
                //We reset the PhysicianID to null here so that the next author has to do the process at least once.
                GetOPDAndSQLData.PhysicianID = null;
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
            searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPD(author.first, author.last, author.middle, author.cities, GetData.stateDictionary[int.Parse(author.states)],
                     @$"C:\Users\devin\OneDrive\Documents\COI Report\OPD CSVs\{year}\OP_DTL_GNRL_PGYR{year}_P01172020.csv"));
            //searchResults.AddRange(GetOpdData.FindPeopleFromOPD(author.first, author.last, author.middle, author.city, GetData.stateDictionary[int.Parse(author.state)],
            //        $@"C:\Users\devin\OneDrive\Documents\COI Report\OPD CSVs\{year}\OP_DTL_OWNRSHP_PGYR{year}_P01172020.csv"));
            //searchResults.AddRange(GetOpdData.FindPeopleFromOPD(author.first, author.last, author.middle, author.city, GetData.stateDictionary[int.Parse(author.state)],
            //        $@"C:\Users\devin\OneDrive\Documents\COI Report\OPD CSVs\{year}\OP_DTL_RSRCH_PGYR{year}_P01172020.csv"));

        }

        /// <summary>
        /// This method is to shorten down the main method by searching all the types of the OPD through here.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="author"></param>
        private static void OPDYearSearch(int year, Person author)
        {
            searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, $"OPD_GNRL_{year}", author.receiveddate));
            searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, $"OPD_RSRCH_{year}", author.receiveddate));
            searchResults.AddRange(GetOPDAndSQLData.FindPeopleFromOPDSQL(author.first, author.last, $"OPD_OWNRSHP_{year}", author.receiveddate));
        }

        /// <summary>
        /// This method uses a filewriter to create an excel sheet. It was used for testing and reviewing that
        /// we were finding the right author.
        /// </summary>
        /// <param name="searchResults">the result of searching the OPD </param>
        private static void OutputToCSV(string[] searchResults, string filePath)
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
            string delimiter = ",";

            
            using (System.IO.TextWriter writer = File.AppendText(filePath))
            {
                for (int index = 0; index < searchResults.Length; index++)
                {
                    //   writer.WriteLine(string.Join(delimiter, searchResults[index]));
                    writer.Write(searchResults[index] + delimiter);
                }
                writer.Write(Environment.NewLine);
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
            string position = author.position;
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
                string company = FindCompanyInRow(OPDEntry);

                //This check is because Bausch and Lomb acquired Valeant so when comparing if the company is Bausch or Valeant and the author has either of these 
                //companies reported, we admit it as reported.
                bool BauschCheck = false;
                if(company.Contains("Bausch") && authorCompanies.Contains("Valeant") || company.Contains("Valeant") && authorCompanies.Contains("Bausch")) 
                { 
                    BauschCheck = true;
                }
               
                //If the company is found or satisfies the BauschCheck above, put it in as reported.
                if(findCompany(company, authorCompanies) || BauschCheck)
                {

                    string[] currentLine = FormCSVLine(author, OPDEntry, "Reported", position);
                    OutputToCSV(currentLine, companyOutputFilePath);
                    companyHits.Add(companyHitString);
                }
                //If the above statement is false, then we can know that there was a match, and there was no discrepancy
                else
                {
                    string[] currentLine = FormCSVLine(author, OPDEntry, "Undisclosed", position);
                    OutputToCSV(currentLine, companyOutputFilePath);
                }
            }
            //This if statement is to catch if there were not 'hits', but the author still had companies reported.
            //if(companyHits.Count == 0 && authorCompanies.Count > 0)
            //{
            //    foreach(string company in authorCompanies)
            //    {
            //        if(!(company.Equals("")) && !(company.Equals("Other")) && !(company.Equals(" ")))
            //        {
            //            Console.WriteLine($"DISCREPANCY: author reported company {company}, but said company was either not found within the OPD or payments were not found within the time range.");
            //        }
            //    }
            //}
            //This else statement is for finding the remaining companies that were hits
           // else if(companyHits.Count != 0)
           // {
 
            
            
            //As we find hits, we remove them from the company list.
                foreach(string hit in companyHits)
                {
                    authorCompanies.Remove(hit);
                }
                //If there are any companies left, then there are companies the author reported that were not found within the OPD
                if(authorCompanies.Count > 0)
                {
                    //These checks are for the Bausch/Valeant Check detailed above. This is so that the output doesn't say 
                    if(authorCompanies.Contains("Bausch") && companyHits.Contains("Valeant")) { authorCompanies.Remove("Bausch"); }
                    if(authorCompanies.Contains("Valeant") && companyHits.Contains("Bausch")) { authorCompanies.Remove("Valeant"); }

                //foreach(string company in authorCompanies)
                //{
                //    if (!company.Equals("Other") && !(company.Equals("")))
                //    {
                //    Console.WriteLine($"DISCREPANCY: author reported company {company}, but said company was either not found within the OPD or payments were not found within the time range.");
                //    }
                //}
                string[] row = { author.first, author.last, author.authorshipNumber.ToString(), "Found", author.cities.Replace(',',';'), author.states.Replace(',',';'), String.Join(';', authorCompanies), position, author.articleNumber, author.journal, author.receiveddate, GetOPDAndSQLData.PhysicianID };
                    OutputToCSV(row, authoroutputFilePath);
                }
           // }
        }

        private static string[] FormCSVLine(Person author, string[] OPDEntry, string disclosure, string position)
        {

            if(OPDEntry[OPDEntry.Length-1].Equals("General"))
            {
                string[] output = { author.authorshipNumber.ToString(), OPDEntry[5], author.last, author.first, position, author.articleNumber, author.journal, disclosure, "\"" + OPDEntry[27] + "\"", OPDEntry[26], OPDEntry[45], OPDEntry[31], OPDEntry[OPDEntry.Length - 1], "\"" + OPDEntry[34] + "\"", OPDEntry[33], OPDEntry[30] };
                return output;
            }
            else if(OPDEntry[OPDEntry.Length-1].Equals("Research"))
            {
                
                //Due to the OPD Research file having a format change from 2014 onward, we have two different cases depending on what element 157 contains. If it's a date, then it's 2015+, else it's from 2014 to 2013
                if(!(OPDEntry[157].Contains("No") || OPDEntry[157].Contains("Yes")))
                {
                    string[] output = { author.authorshipNumber.ToString(), OPDEntry[6], author.last, author.first, position, author.articleNumber, author.journal, disclosure, "\"" + OPDEntry[128] + "\"", OPDEntry[127], OPDEntry[170], OPDEntry[158], OPDEntry[OPDEntry.Length - 1], "N/A", OPDEntry[159], OPDEntry[157] };
                    return output;
                }
                else
                {
                    string[] output = { author.authorshipNumber.ToString(), OPDEntry[6], author.last, author.first, position, author.articleNumber, author.journal, disclosure, "\"" + OPDEntry[128] + "\"", OPDEntry[127], OPDEntry[160], OPDEntry[148], OPDEntry[OPDEntry.Length - 1], "N/A", OPDEntry[149], OPDEntry[147] };
                    return output;
                }
            }
           // if(OPDEntry[OPDEntry.Length-1].Equals("Ownership"))
            else
            {
                string[] output = { author.authorshipNumber.ToString(), OPDEntry[1], author.last, author.first, position, author.articleNumber, author.journal, disclosure, "\"" + OPDEntry[23] + "\"", OPDEntry[22], OPDEntry[16], OPDEntry[17], OPDEntry[OPDEntry.Length - 1], "N/A", OPDEntry[20], OPDEntry[19] };
                return output;
            }


        }

        private static void SortAlphabetically(List<string[]> rows)
        {
            rows.Sort(CompareByName);
        }

        private static int CompareByName(string[] firstRow, string[] secondRow)
        {
            string firstCompany = FindCompanyInRow(firstRow);
            string secondCompany = FindCompanyInRow(secondRow);
            return string.Compare(firstCompany,secondCompany);
        }

        private static string FindCompanyInRow(string[] row)
        {
            //General
            if (row[row.Length - 1].Equals("General")) { return row[27]; }
            //Research
            else if (row[row.Length - 1].Equals("Research")) { return row[128]; } 
            //Ownership
            else { return row[23]; }
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
