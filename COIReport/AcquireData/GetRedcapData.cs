﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using AcquireData.Properties;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Redcap;
using Redcap.Models;

namespace AcquireData
{
    public class GetRedcapData
    {
        private static String token;
        private static string reportID;
        private static string apiURL;
        private static string RedCapResult;
        private static Dictionary<int, Person> authorshipDictionary;

        /// <summary>
        /// The purpose of this method is to acquire the JSON file from RedCap using the RedCap API
        /// </summary>
        static void AcquireJSON()
        {
            //These lines are for looking at the secrets file and getting the info to make contact with the RedCap API
            var builder = new ConfigurationBuilder();
            builder.AddUserSecrets<GetRedcapData>();
            IConfigurationRoot Configuration = builder.Build();
            var SelectedSecrets = Configuration.GetSection("COIReportDevinSecrets");
            token = SelectedSecrets["APIToken"];
            reportID = SelectedSecrets["DevinReportID"];
            apiURL = SelectedSecrets["APIURL"];

            var redcap_api = new RedcapApi(apiURL);

            //This is all of the RedCapData!
            RedCapResult = redcap_api.ExportReportsAsync(token, int.Parse(reportID), Redcap.Models.ReturnFormat.json).Result;

            //Current problems with receiving data. JSON breaks every line down as an object when it's not supposed to. Because of this,
            //I would have to go into each 'person' created and find what chunk of an actual Person it contains.

        }

        /// <summary>
        /// The purpose of this method is to create the 
        /// </summary>
        /// <returns></returns>
        public static IList<Person> CreatePeopleList() {
            AcquireJSON();
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
        /// </summary>
        public static void ParseOPD()
        {
            using (TextFieldParser parser = new TextFieldParser(@"C:\Users\devin\OneDrive\Documents\COI Report\OP_DTL_GNRL_PGYR2017_P01172020.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    foreach(string field in fields)
                    {

                    }
                }
            }
        }
    }
}