using System;
using System.Collections.Generic;
using System.Text;

namespace AcquireData
{
    public class Person
    {
        private string authorshipNumber;
        private string first { get; set; }
        private string middle { get; set; }
        private string last { get; set; }
        private string clincalDegree { get; set; }
        private string otherDegree { get; set; }
        private string USLocation { get; set; }
        private string institution { get; set; }
        private string city { get; set; }
        private string state { get; set; }
        private string  Involvement { get; set; }
        private string otherInvolvement { get; set; }
        private string companiesNumbered { get; set; }
        private string otherCompanies { get; set; }
        private string articleNumber { get; set; }
        private string journal { get; set; }
        private string receivedate { get; set; }

        /// <summary>
        /// This is a public constructor that will be creating different Person objects from the JSON parsing
        /// </summary>
        /// <param name="authorshipnumber">this is what we will be using for merging all the different 'people' that are actually one Person</param>
        /// <param name="first">their first name</param>
        /// <param name="middle">their middle name</param>
        /// <param name="last">their last name </param>
        /// <param name="clinicaldegree">their medical degree</param>
        /// <param name="freetextdegreeother">any other degree they may have (?)</param>
        /// <param name="uslocation">an identifier for if they live in the US or not</param>
        /// <param name="institution">the institution they work with</param>
        /// <param name="city">their city of residence</param>
        /// <param name="state">their state of residence</param>
        /// <param name="type3">the type of work they did with the company/companies</param>
        /// <param name="freetexttypeother3"> other type of work they might have done</param>
        /// <param name="entity3">the companies they worked with (companies represented as numbers)</param>
        /// <param name="freetextentityother3">any other companies listed</param>
        /// <param name="articlenumber">the article the author is associated with</param>
        /// <param name="journal">the journal the article is in</param>
        /// <param name="receivedate">the date the journal was received</param>
        public Person(string authorshipnumber, string first, string middle, string last, string clinicaldegree, string freetextdegreeother,
            string uslocation, string institution, string city, string state, string type3, string freetexttypeother3, string entity3,
            string freetextentityother3, string articlenumber, string journal, string receivedate)
        {
            this.authorshipNumber = authorshipnumber;
            this.first = first;
            this.middle = middle;
            this.last = last;
            this.clincalDegree = clinicaldegree;
            this.otherDegree = freetextdegreeother;
            this.USLocation = uslocation;
            this.institution = institution;
            this.city = city;
            this.state = state;
            this.Involvement = type3;
            this.otherInvolvement = freetexttypeother3;
            this.companiesNumbered = entity3;
            this.otherCompanies = freetextentityother3;
            this.articleNumber = articlenumber;
            this.journal = journal;
            this.receivedate = receivedate;
        }
    }
}
