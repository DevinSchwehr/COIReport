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

            }
        }

        //Next step to perform: Analyze the list acquired from the OPD and then compare if all of the companies listed in the OPD match with the companies that they listed.
        static void AnalyzeOPDList(List<String[]> rows, Person author)
        {

        }
    }
}
