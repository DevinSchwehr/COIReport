using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Redcap;
using System;
using System.Collections.Generic;
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
        private static Dictionary<int, Person> authorshipDictionary;
        static Dictionary<int, string> clinicalDegrees;
        static Dictionary<int, string> stateDictionary;
        static Dictionary<int ,string> companyDictionary;
        static Dictionary<int, string> typeDictionary;

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

        public static void CreateDictionaries()
        {
            AcquireJSONAndMetaData();
            String[] splitString = MetaDataResult.Split('"');
            //Next Steps:
            //Break apart the three elements containing the actual pairs, separating by '|'
            //then add elements into respective dictionary by breaking apart with ',', element 0 is key, element 2 is value.


        }


    }

    /// <summary>
    /// This class is responsible for pulling data from the OPD and will be responsible for searching through it when looking for a specific name.
    /// </summary>
    public class GetOpdData
    {
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
            bool allSameID = true;
            string physicianID = matches[0][5];
            //This loop is to check if the list has only one physician or not.
            foreach(string[] row in matches)
            {
                if (!(row[5].Equals(physicianID)))
                {
                    allSameID = false;
                    break;
                }
            }
            //If all of the physicians have the same ID, then return the list.
            if (allSameID)
            {
                return matches;
            }
            return matches;
            //If they don't let's remove those from the list who don't come from the same city and state
            //Note: we have to bring in the dictionary.. The person's city and state will be in numbers.

        }
    }
}