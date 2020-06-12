using System;
using System.Collections.Generic;
using AcquireData;
using Newtonsoft.Json;
using Redcap;
using Redcap.Models;

namespace RedcapApiDemo
{
    class Program
    {
        static List<Person> authors;
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
                //Repeat this for all 3 categories of payments for every year.
                try
                {
                    List<String[]> results =  (List<String[]>)GetOpdData.FindPeopleFromOPD(author.first, author.last, author.middle, author.city, GetData.stateDictionary[int.Parse(author.state)],
                    "OP_DTL_GNRL_PGYR2018_P01172020.csv");
                    AnalyzeOPDList(results, author);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        //Next step to perform: Analyze the list acquired from the OPD and then compare if all of the companies listed in the OPD match with the companies that they listed.
        static void AnalyzeOPDList(List<String[]> rows, Person author)
        {

        }
    }
}
