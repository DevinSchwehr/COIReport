using System;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Redcap;
using Redcap.Models;

namespace RedcapApiDemo
{
    class TestsWithConsole
    {
        private static String token;
        private static string reportID;
        private static string apiURL;
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddUserSecrets<TestsWithConsole>();
            IConfigurationRoot Configuration = builder.Build();
            var SelectedSecrets = Configuration.GetSection("COIReportDevinSecrets");
            token = SelectedSecrets["APIToken"];
            reportID = SelectedSecrets["DevinReportID"];
            apiURL = SelectedSecrets["APIURL"];


            Console.WriteLine("Redcap Api Demo Started!");
            // Use your own API Token here...
            var redcap_api = new RedcapApi(apiURL);

            Console.WriteLine("Exporting Report.");
            var result = redcap_api.ExportReportsAsync(token, int.Parse(reportID)).Result;

            //Current problems with receiving data. JSON breaks every line down as an object when it's not supposed to. Because of this,
            //I would have to go into each 'person' created and find what chunk of an actual Person it contains.
            var data = JsonConvert.DeserializeObject(result);
            Console.WriteLine(data);
            Console.ReadLine();

        }
    }
}