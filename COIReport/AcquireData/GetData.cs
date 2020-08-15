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


            //Current problems with receiving data. JSON breaks every line down as an object when it's not supposed to. Because of this,
            //I would have to go into each 'person' created and find what chunk of an actual Person it contains.

        }

        /// <summary>
        /// The purpose of this method is to create the 
        /// </summary>
        /// <returns></returns>
        public static IList<Person> CreatePeopleList() {
            AcquireJSONAndMetaData();
            List<Person> authors = new List<Person>();
            StringReader baseReader = new StringReader(RedCapResult);
            authorshipDictionary = new Dictionary<int, Person>();
            //we use the textReader here to be able to step through every object on its own in the large list of names
             using (JsonTextReader jsonReader = new JsonTextReader(baseReader))
            {
                //Read moves it to the next token
                while (jsonReader.Read())
                {
                    if(jsonReader.TokenType == JsonToken.StartObject)
                    {
                        //grab the current token that we're on from the JSonReader
                        JObject currentToken = JObject.Load(jsonReader);
                        JsonSerializer serializer = new JsonSerializer();
                        //What to do here
                        //Get the person
                        //find the person in a dicionary
                        //If present, update the person with the parts that are present
                        //When changing from one authorship to the next, add the (hopefully) complete person to the list

                        //This line is operating as expected, now need to look at how to combine people
                        Person newAuthor = (Person)serializer.Deserialize(new JTokenReader(currentToken), typeof(Person));
                        if (authorshipDictionary.ContainsKey(newAuthor.authorshipNumber) && authorshipDictionary.TryGetValue(newAuthor.authorshipNumber, out Person oldAuthor))
                        {
                            oldAuthor = oldAuthor.CheckAndMerge(newAuthor);
                        }
                        else
                        {
                            authorshipDictionary.Add(newAuthor.authorshipNumber, newAuthor);
                        }
                    }
                }
            }
             foreach(Person author in authorshipDictionary.Values)
            {
                authors.Add(author);
            }

            return authors;

        }

        /// <summary>
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
            //Next Steps:
            //Break apart the three elements containing the actual pairs, separating by '|'
            //then add elements into respective dictionary by breaking apart with ',', element 0 is key, element 2 is value.
            string[] degrees = splitString[3].Split('|');
            //Have to do some special scenarios for degrees as there are values with commas within them
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

        private static string TypoCheck(string company)
        {
            if(company.Equals("Cambrium")) { return "Cambium"; }
            if(company.Equals("Nidex")) { return "Nidek"; }
            if(company.Equals("Aldevra")) { return "Aldeyra"; }
            if(company.Equals("ophthea")) { return "opthea"; }
            if(company.Equals("Abbot")) { return "Abbott";   }
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
    public static class GetOpdData
    {
        public static readonly string connectionString;
        static GetOpdData()
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
        /// This method right now is pretty barebones. As of now it just looks through the one OPD file I have, and just starts pulling names
        /// and putting it into a string array. This is just the base, in the future I'll be implementing search restrictions
        /// and will be searching through more than just the one file.
        /// 
        /// I am going to retrofit this method to be the groundwork for the search protocol. It is going to find all of the names, put them
        /// into a list, and then return it to be reviewed.
        /// 
        /// Things to know for parsing (Post 2016)
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
            //    bool allSameID = true;
            //   string physicianID = matches[0][5];
            //This private helper method will check if all of the ID's in the list are the same.
            //   allSameID = IDChecker(physicianID, matches);
            //If all of the physicians have the same ID, then return the list.
            //ERROR. do not return list yet. we want to confirm the person we found is the right person.
            //if (allSameID)
            //{
            //    return matches;
            //}
            //Otherwise, get more specific
            //   else
            // {
            // List<string[]> duplicateMatches = matches;
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
            DateTime RedcapDate = DateTime.Parse(receiveDate);
            List<String[]> OPDOutputs = new List<String[]>();
            List<String[]> sameCityState = new List<String[]>();
            List<String[]> matchingID = new List<String[]>();
            //The following try block is the process of querying the database to get the authors.
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    //This command will query the database for any author with the same first and last name
                    using (SqlCommand command = new SqlCommand($"select * from {table} where upper(Physician_First_Name) = upper('{first}') and upper(Physician_Last_Name) = upper('{last}')", con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                String[] fields = new string[reader.FieldCount];
                                for (int i = 0; i < fields.Length; i++)
                                {
                                    //due to the nature of the Table, all entries have leading and ending quotes. These couple of lines are to remove those quotes.
                                    //string currentField = reader[i].ToString();
                                    //currentField = currentField.Replace("\"", "");
                                    fields[i] = reader[i].ToString();
                                }
                                //We don't check the timeframe here because it will get sorted later.
                                OPDOutputs.Add(fields);
                            }
                        }
                    }

                    foreach (string[] row in OPDOutputs)
                    {
                        if (row[12].Equals(city) && row[13].Equals(state)) { sameCityState.Add(row); }
                    }
                    //Now that we can ascertain the accuracy of the author down to what we hope is one person, we will then make another 
                    //Search using the Physician ID of the author
                    if (sameCityState.Count > 0)
                    {
                        using (SqlCommand command = new SqlCommand($"select * from {table} where Physician_Profile_ID = '{sameCityState[0][5]}'", con))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    String[] fields = new string[reader.FieldCount];
                                    for (int i = 0; i < fields.Length; i++)
                                    {
                                        fields[i] = reader[i].ToString();
                                    }
                                    //Now we check to make sure the date is within the current timeframe.
                                    DateTime OpdDate = DateTime.Parse(fields[31]);
                                    if (WithinTimeFrame(RedcapDate, OpdDate))
                                    {
                                        matchingID.Add(fields);
                                    }
                                }
                            }
                        }
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

        private static bool WithinTimeFrame(DateTime RedcapDate, DateTime opdDate)
        {
            //First, we must check if the OPD date is newer than the Redcap date, if it is, throw it out. 
            if(DateTime.Compare(opdDate, RedcapDate) > 0 )
            {
                //If this is true, then the date is later, so we throw it out.
                return false;
            }
            //Now we must make the check to see if the entry is too old to be accepted. If it is, toss it. 
            else if(RedcapDate.Year - opdDate.Year ==2 )
            {
                // If the OPD month is smaller than the Redcap month, then it is outside of the period and should be tossed.
                if(opdDate.Month.CompareTo(RedcapDate) < 0)
                {
                    return false; 
                }
            }
            //If it hasn't returned false, then it is in the period. return true. 
            return true;
        }

        private static bool IDChecker(string ID, List<String[]> authors)
        {
            foreach (string[] row in authors)
            {
                if (!(row[5].Equals(ID)))
                {
                    return false;
                }
            }
            return true;

        }

        public static string FindAuthorPosition(string first, string last)
        {
            string position = "";
            try
            {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand($"select position from OphthAuthorPositions where UPPER(last) = upper('{last}') and upper(first) = upper('{first}')", con))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            position = reader.GetString(0); //
                        }
                    }
                }
            }
            }
            catch(SqlException e)
            {
                Console.WriteLine($"Error in SQL Connection: {e.Message}");
            }
            if(position.Equals("f")) { return "first"; }
            else if(position.Equals("m")) { return "middle"; }
            else if (position.Equals("l")) { return "last"; }
            else { return "Position not found"; }
        }

    }
}