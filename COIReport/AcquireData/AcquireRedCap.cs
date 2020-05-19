using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Redcap;
using Redcap.Models;

namespace AcquireData
{
    class AcquireRedCap
    {
        private static String token;
        private static string reportID;
        private static string apiURL;
        private static string RedCapResult;

        /// <summary>
        /// The purpose of this method is to acquire the JSON file from RedCap using the RedCap API
        /// </summary>
        static void AcquireJSON()
        {
            //These lines are for looking at the secrets file and getting the info to make contact with the RedCap API
            var builder = new ConfigurationBuilder();
            builder.AddUserSecrets<AcquireRedCap>();
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
        static IList<String> CreatePeopleList() {
            AcquireJSON();
            List<String> authors = new List<String>();
             StreamReader streamReader = new StreamReader(RedCapResult);
             using (JsonTextReader textReader = new JsonTextReader(streamReader))
            {
                while (textReader.Read())
                {
                    if(textReader.TokenType == JsonToken.StartObject)
                    {
                        //What to do here
                        //Get the person
                        //find the person in a dicionary
                        //If present, update the person with the parts that are present
                        //When changing from one authorship to the next, add the (hopefully) complete person to the list
                        Person newAuthor = 
                    }
                }
            }

            return authors;

        }


    }
}