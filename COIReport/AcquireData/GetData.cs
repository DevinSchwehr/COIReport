using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Redcap;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace AcquireData
{
    /// <summary>
    /// The purpose of this class is to acquire data from redcap. This includes the report you are seeking, 
    /// as well as the metadata. There is another class within this file that is for getting the OPD Data.
    /// </summary>
    public class GetData
    {
        private static String token;
        private static string reportID;
        private static string apiURL;
        private static string RedCapResult;
        private static string MetaDataResult;
        public static readonly string connectionString;
        private static Dictionary<int, Person> authorshipDictionary;
        public static Dictionary<int, string> clinicalDegrees;
        public static Dictionary<int, string> stateDictionary;
        public static Dictionary<int ,string> companyDictionary;
        public static Dictionary<int, string> typeDictionary;


        /// <summary>
        /// The purpose of this method is to acquire the JSON file from RedCap using the RedCap API
        /// </summary>
        static void AcquireJSONAndMetaData()
        {
            //These lines are for looking at the secrets file and getting the info to make contact with the RedCap API
            var builder = new ConfigurationBuilder();
            builder.AddUserSecrets<GetData>();
            IConfigurationRoot Configuration = builder.Build();
            var SelectedSecrets = Configuration.GetSection("COIReportDevinSecrets");
            token = SelectedSecrets["APIToken"];
            reportID = SelectedSecrets["DevinReportID"];
            apiURL = SelectedSecrets["APIURL"];

            var redcap_api = new RedcapApi(apiURL);

            string[] metadataFields = { "clinicaldegree", "entity3", "state", "type3" };

            //This is all of the RedCapData, including the data dictionary!
            RedCapResult = redcap_api.ExportReportsAsync(token, int.Parse(reportID), Redcap.Models.ReturnFormat.json).Result;
            MetaDataResult = redcap_api.ExportMetaDataAsync(token, Redcap.Models.ReturnFormat.csv, metadataFields).Result;
        }

        /// <summary>
        /// The purpose of this method is to create the list of authors by breaking down the 
        /// RedCapResult JSON. It breaks down every JSON into a person object, and then does a check
        /// with a dictionary that we are creating to combine partial people objects into one complete 'Person'
        /// </summary>
        /// <returns>a list of all of the authors</returns>
        public static IList<Person> CreatePeopleList()
        {
            //We run the method to get the JSON file.
            AcquireJSONAndMetaData();
            List<Person> authors = new List<Person>();
            StringReader baseReader = new StringReader(RedCapResult);
            authorshipDictionary = new Dictionary<int, Person>();
            //Creating our specific list of people
            List<string[]> specificAuthors = GetOPDAndSQLData.getSpecificAuthors();

            //we use the textReader here to be able to step through every object on its own in the large list of names
            using (JsonTextReader jsonReader = new JsonTextReader(baseReader))
            {
                //Read moves it to the next token
                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.StartObject)
                    {
                        //grab the current token that we're on from the JSonReader
                        JObject currentToken = JObject.Load(jsonReader);
                        JsonSerializer serializer = new JsonSerializer();
                        //We create a person object from the current JSON token
                        Person newAuthor = (Person)serializer.Deserialize(new JTokenReader(currentToken), typeof(Person));
                        if (authorshipDictionary.ContainsKey(newAuthor.authorshipNumber) && authorshipDictionary.TryGetValue(newAuthor.authorshipNumber, out Person oldAuthor))
                        {
                            //If the author is already in the dictionary, check to merge the newAuthor with the currently existing Author
                            oldAuthor = oldAuthor.CheckAndMerge(newAuthor);
                        }
                        else
                        {
                            //Add the current author into the dictionary if they are not in it right now
                            authorshipDictionary.Add(newAuthor.authorshipNumber, newAuthor);
                        }
                    }
                }
            }
            //After finishing merging all of the authors, add them into a list and return the list.
            foreach (Person author in authorshipDictionary.Values)
            {
                //If they are in the set of sample data, then add them to the list.
                if(InTestSample(specificAuthors, author))
                {
                    authors.Add(author);
                }
            }

            return authors;

        }


        /// <summary>
        /// This is the method that will create all of the dictionaries from the metadata.
        /// This will be useful later for the analyzation of the OPD returns, particularly the 
        /// state and company dictionaries.
        /// element 3 = Clinical Degree
        /// Element 9 = States
        /// Element 21, 23, 25 = Type
        /// Element 37 = Companies
        /// </summary>
        public static void CreateDictionaries()
        {
            clinicalDegrees = new Dictionary<int, string>();
            stateDictionary = new Dictionary<int, string>();
            companyDictionary = new Dictionary<int, string>();
            typeDictionary = new Dictionary<int, string>();
            AcquireJSONAndMetaData();
            String[] splitString = MetaDataResult.Split('"');
            string[] degrees = splitString[3].Split('|');
            //Have to do some special scenarios for degrees as there are values with commas within them

            //TAKE ANOTHER LOOK AT THIS. It might no longer be necessary with the new way things are put in the OPD
            foreach(string pair in degrees)
            {
                string[] splitPair = pair.Split(',');
                if(splitPair.Length > 2)
                {
                    string ConcateValue = "";
                    for(int counter = 1; counter <splitPair.Length; counter++)
                    {
                        ConcateValue += splitPair[counter];
                    }
                    clinicalDegrees.Add(int.Parse(splitPair[0]), ConcateValue);
                }
                else
                {
                clinicalDegrees.Add(int.Parse(splitPair[0]), splitPair[1]);
                }
            }
            //Repeating the same process for states, however it is easier as it will always fit into an array of 2 if you split on comma.
            string[] states = splitString[9].Split('|');
            foreach(string pair in states)
            {
                string[] splitPair = pair.Split(",");
                stateDictionary.Add(int.Parse(splitPair[0]), GetStateAbbreviation(splitPair[1]));
            }
            //Same routine as was used for states, but for the types of involvement with companies.
            string allTypes = splitString[21] + splitString[23] + splitString[25];
            string[] types = allTypes.Split('|');
            foreach(string pair in types)
            {
                string[] splitPair = pair.Split(',');
                typeDictionary.Add(int.Parse(splitPair[0]), splitPair[1]);
            }
            //Same deal for the past two, but with the companies.
            string[] companies = splitString[37].Split('|');
            foreach(string pair in companies)
            {
                string[] splitPair = pair.Split(',');
                splitPair[1] = TypoCheck(splitPair[1]);
                companyDictionary.Add(int.Parse(splitPair[0]), splitPair[1]);
            }

        }

        private static bool InTestSample(List<string[]> specificAuthors, Person author)
        {
            foreach(string[] set in specificAuthors)
            {
              if(author.first.Equals(set[0].ToUpper()) && author.last.Equals(set[1].ToUpper())) { return true; }  
            }
            return false;
        }
        /// <summary>
        /// Within the redcap there is some typos for the companies. If you find any typos, put them
        /// in here.
        /// </summary>
        /// <param name="company">the company that needs to be checked.</param>
        /// <returns>if the company is typoed, it returns the fixed company. Else, it returns the company as is.</returns>
        private static string TypoCheck(string company)
        {
            if(company.Equals("Cambrium")) { return "Cambium"; }
            if(company.Equals("Nidex")) { return "Nidek"; }
            if(company.Equals("Aldevra")) { return "Aldeyra"; }
            if(company.Equals("ophthea")) { return "opthea"; }
            if(company.Equals("Abbot")) { return "Abbott";   }
            if(company.Equals("Abbot Medical Optics aka AMO")) { return "Abbott"; }
            if(company.Equals("Dutch Ophthalmic Research Center (DORC)")) { return "Dutch Ophthalmic"; }
            if(company.Equals("Notal Vision")) { return "NotalVision"; }
            if(company.Equals("Bausch and Lomb")) { return "Bausch"; }
            return company;
        }

        /// <summary>
        /// The purpose of this method is to convert the full state names from the metadata into their abbreviations to be found within the OPD, as it uses abbreviations.
        /// </summary>
        /// <param name="state">the state</param>
        /// <returns>its abbreviation</returns>
        private static string GetStateAbbreviation(string state)
        {
            switch (state) 
            {
                case " Alabama ": return "AL";
                case " Alaska ": return "AK";
                case " Arizona ": return "AZ";
                case " Arkansas ": return "AR";
                case " California ": return "CA";
                case " Colorado ": return "CO";
                case " Connecticut ":return "CT";
                case " Delaware ": return "DE";
                case " Florida ": return "FL";
                case " Georgia ": return "GA";
                case " Hawaii ": return "HI";
                case " Idaho ": return "ID";
                case " Illinois ": return "IL";
                case " Indiana ": return "IN";
                case " Iowa ": return "IA";
                case " Kansas ": return "KS";
                case " Kentucky ": return "KY";
                case " Louisiana ": return "LA";
                case " Maine ": return "ME";
                case " Maryland ": return "MD";
                case " Massachusetts ": return "MA";
                case " Michigan ": return "MI";
                case " Minnesota ": return "MN";
                case " Mississippi ": return "MS";
                case " Missouri ": return "MS";
                case " Montana ": return "MT";
                case " Nebraska ": return "NE";
                case " Nevada ": return "NV";
                case " New Hampshire ": return "NH";
                case " New Jersey ": return "NJ";
                case " New Mexico ": return "NM";
                case " New York ": return "NY";
                case " North Carolina ": return "NC";
                case " North Dakota ": return "ND";
                case " Ohio ": return "OH";
                case " Oklahoma ": return "OK";
                case " Oregon ": return "OR";
                case " Pennsylvania ": return "PA";
                case " Rhode Island ": return "RI";
                case " South Carolina ": return "SC";
                case " South Dakota ": return "SD";
                case " Tennessee ": return "TN";
                case " Texas ": return "TX";
                case " Utah ": return "UT";
                case " Vermont ": return "VT";
                case " Virginia ": return "VA";
                case " Washington ": return "WA";
                case " West Virginia ": return "WV";
                case " Wisconsin ": return "WI";
                case " Wyoming ": return "WY";
                case " American Samoa ": return "AS";
                case " District of Columbia ": return "DC";
                case " Federated States of Micronesia ": return "FM";
                case " Guam ": return "GU";
                case " Marshall Islands ": return "MH";
                case " Northern Mariana Islands ": return "MP";
                case " Palau": return "PW";
                case " Puerto Rico ": return "PR";
                case " Virgin Islands": return "VI";
            }
            throw new Exception("State not found");
        }
    }

    /// <summary>
    /// This class is responsible for pulling data from the OPD and will be responsible for searching through it when looking for a specific name.
    /// </summary>
    public static class GetOPDAndSQLData
    {
        public static readonly string connectionString;
        /// <summary>
        /// this is used to get the connection string with the OPD that we will use
        /// for future commands.
        /// </summary>
        static GetOPDAndSQLData()
        {
            var builder = new ConfigurationBuilder();

            builder.AddUserSecrets<GetData>();
            IConfigurationRoot Configuration = builder.Build();
            var SelectedSecrets = Configuration.GetSection("COIReportDevinSecrets");

            connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = "cs3500.eng.utah.edu, 14330",
                InitialCatalog = SelectedSecrets["DataBaseName"],
                UserID = SelectedSecrets["SQLUsername"],
                Password = SelectedSecrets["SQLPassword"]
            }.ConnectionString;

        }
        /// <summary>
        /// This method has become obsolete now that we are using SQL for searching, but we are going
        /// to keep it around in cae the SQL method fails in the long run.
        /// 
        /// Things to know for parsing
        /// 5 = Physician ID
        /// 6 = First Name
        /// 7 = Middle Name
        /// 8 = Last Name
        /// 12 = Recipient City
        /// 13 = Recipient State
        /// </summary>
        public static IList<String[]> FindPeopleFromOPD(string first,string last, string middle, string city, string state, string filepath)
        {
            List<String[]> matches = new List<String[]>();
            using (TextFieldParser parser = new TextFieldParser(filepath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //fields is an array that contains all of the data for the current line in the csv.
                    string[] fields = parser.ReadFields();
                    //If the first and last name in the row match the given name, put it in the list.
                    if(fields[6].Equals(first) && fields[8].Equals(last))
                    {
                        matches.Add(fields);
                    }
                }
            }
            //If the size of the list is zero it will simply throw an error saying it could not find anybody
            //EDIT: We are simply going to return the empty list so that it doesn't exit in the other method.
            if(matches.Count == 0)
            {
                // throw new ArgumentException("Error. Did Not find anybody with the same first and last name from OPD.");
                return matches;
            }

            List<String[]> duplicateMatches = new List<string[]>(matches);
                foreach(string[] author in duplicateMatches)
                {
                    //If the author doesn't have the same city or state, remove them. This will ensure that only those from the same city and state will stay in the list.
                  //  if(!(author[12].Equals(city)) || !(author[13].Equals(state))) { matches.Remove(author); }
                  if(!(author[12].Equals(city) && author[13].Equals(state))) { matches.Remove(author); }
                }
                //Now do another check to see if everyone has the same physician id

           // }
           if(matches.Count == 0) { return matches; }

            //string physicianID = matches[0][5];
            //bool allSameID = IDChecker(physicianID, matches);
            ////If they all now have the same ID, then return the list
            //if (allSameID) { return matches; }
            // else
            // {
            //     //If you still cannot narrow down the list to one physician, throw an error.
            //     throw new ArgumentException("Error: Could not narrow list down to one candidate.");
            // }
            return matches;


        }
        /// <summary>
        /// The purpose of this method is to get correct authors from the OPD using the SQL Database. This should be faster than 
        /// the current way of searching for the author.
        /// 
        /// Things to know for parsing (Post 2016)
        /// 5 = Physician ID
        /// 6 = First Name
        /// 7 = Middle Name
        /// 8 = Last Name
        /// 12 = Recipient City
        /// 13 = Recipient State

        /// </summary>
        /// <param name="first">the author's first name</param>
        /// <param name="last">the author's last name</param>
        /// <param name="middle">the author's middle name</param>
        /// <param name="city">the author's city</param>
        /// <param name="state">the author's state</param>
        /// <returns>a list of all the author's with the same first name, last name, and either the same city and state or the same state if an author in the same city cannot be found</returns>
        public static IList<String[]> FindPeopleFromOPDSQL(string first, string last, string city, string state,string table, string receiveDate)
        {
            //We use the Redcapdate to make sure that it's within the proper timeframe.
            DateTime RedcapDate = DateTime.Parse(receiveDate);
            List<String[]> OPDOutputs = new List<String[]>();
            List<String[]> sameCityState = new List<String[]>();
            List<String[]> matchingID = new List<String[]>();
            List<String[]> specificAuthors = new List<String[]>();
            //The following try block is the process of querying the database to get the authors.
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    //This command will query the database for any author with the same first and last name
                    using (SqlCommand command = new SqlCommand($"select * from {table} where upper(Physician_First_Name) = upper('{first}') and upper(Physician_Last_Name) = upper('{last}')", con))
                    {
                        command.CommandTimeout = 5000;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                String[] fields = new string[reader.FieldCount+1];
                                for (int i = 0; i < fields.Length-1; i++)
                                {
                                    fields[i] = reader[i].ToString();
                                }
                                if(table.Contains("GNRL")) { fields[fields.Length - 1] = "General"; }
                                else if(table.Contains("RSRCH")) { fields[fields.Length - 1] = "Research"; }
                                else if(table.Contains("OWNRSHP")) { fields[fields.Length - 1] = "Ownership"; }
                                //We don't check the timeframe here because it will get sorted later.
                                OPDOutputs.Add(fields);
                            }
                        }
                    }
                    con.Close();
                    //Now we do a check to make sure that they are in the same city and state as the author.
                    foreach (string[] row in OPDOutputs)
                    {
                        if (row[12].Equals(city) && row[13].Equals(state)) { sameCityState.Add(row); }
                    }
                    //Now that we can ascertain the accuracy of the author down to what we hope is one person, we will then make another 
                    //Search using the Physician ID of the author
                    if (sameCityState.Count > 0)
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand($"select * from {table} where Physician_Profile_ID = '{sameCityState[0][5]}'", con))
                        {
                            command.CommandTimeout = 5000;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    String[] fields = new string[reader.FieldCount+1];
                                    for (int i = 0; i < fields.Length-1; i++)
                                    {
                                        fields[i] = reader[i].ToString();
                                    }
                                    if (table.Contains("GNRL")) { fields[fields.Length - 1] = "General"; }
                                    else if (table.Contains("RSRCH")) { fields[fields.Length - 1] = "Research"; }
                                    else if (table.Contains("OWNRSHP")) { fields[fields.Length - 1] = "Ownership"; }
                                    //Now we check to make sure the date is within the current timeframe.
                                    DateTime OpdDate = DateTime.Parse(fields[31]);
                                    if (WithinTimeFrame(RedcapDate, OpdDate))
                                    {
                                        matchingID.Add(fields);
                                    }
                                }
                            }
                        }
                        con.Close();
                    }

                }
            }
            catch(SqlException exception)
            {
                Console.WriteLine($"Error in SQL Connection: {exception.Message}");
            }
            //We return matching ID here regardless of if there is anything in it. As it is empty, so is SameCityState.
            return matchingID;
        }

        /// <summary>
        /// This is the method to make sure that the OPD entry is within the timeframe of relevancy for the journal. 
        /// If it isn't the method returns false and the entry will not be added to the final list of entries.
        /// </summary>
        /// <param name="RedcapDate">the receivedate of the article</param>
        /// <param name="opdDate"> the OPD date that is being checked</param>
        /// <returns>true if the OPD date is within the timeframe, false if not.</returns>
        private static bool WithinTimeFrame(DateTime RedcapDate, DateTime opdDate)
        {
            //First, we must check if the OPD date is newer than the Redcap date, if it is, throw it out. 
            if(DateTime.Compare(opdDate, RedcapDate) > 0 )
            {
                //If this is true, then the date is later, so we throw it out.
                return false;
            }
            //Now we must make the check to see if the entry is too old to be accepted. If it is, toss it. 
            else if(RedcapDate.Year - opdDate.Year ==3 )
            {
                // If the OPD month is smaller than the Redcap month, then it is outside of the period and should be tossed.
                if(opdDate.Month.CompareTo(RedcapDate.Month) < 0)
                {
                    return false; 
                }
            }
            //If it hasn't returned false, then it is in the period. return true. 
            return true;
        }

        /// <summary>
        /// This method uses Eileen's program that is on the SQL database and tries to find their author
        /// position. This is used in the analysis method in the main console method.
        /// </summary>
        /// <param name="first">the author's first name</param>
        /// <param name="last">the author's last name</param>
        /// <returns>their position, if it can be found</returns>
        public static string FindAuthorPosition(string first, string last)
        {
            string position = "";
            try
            {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand($"select position from OphthAuthorPositions2019 where UPPER(last) = upper('{last}') and upper(first) = upper('{first}')", con))
                {
                    command.CommandTimeout = 1000;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            position = reader.GetString(0); 
                        }
                    }
                }
                con.Close();
            }
            }
            catch(SqlException e)
            {
                Console.WriteLine($"Error in SQL Connection: {e.Message}");
            }
            if(position.Equals("f")) { return "Position: First"; }
            else if(position.Equals("m")) { return "Position: Middle"; }
            else if (position.Equals("l")) { return "Position: Last"; }
            else { return "Position not found"; }
        }

        /// <summary>
        /// This is the primary difference from the SQL Branch as this is going to be used to find
        /// a specific list of authors.
        /// </summary>
        /// <returns></returns>
        public static List<string[]> getSpecificAuthors()
        {
            List<string[]> specificAuthors = new List<string[]>();
            using (SqlConnection con = new SqlConnection(connectionString))
            { 
                con.Open();
                using (SqlCommand command = new SqlCommand("Select first, last from TestSample", con))
                {
                    command.CommandTimeout = 500;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string[] fields = new string[reader.FieldCount];
                            for (int i = 0; i < fields.Length; i++)
                            {
                                fields[i] = reader[i].ToString();
                            }
                            specificAuthors.Add(fields);
                        }
                    }
                }
            }
            return specificAuthors;
        }

    }
}