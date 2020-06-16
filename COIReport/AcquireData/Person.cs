using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace AcquireData
{
    public class Person
    {
        public int authorshipNumber;
        public string first { get; set; }
        public string middle { get; set; }
        public string last { get; set; }
        private string clincalDegree { get; set; }
        private string otherDegree { get; set; }
        private string USLocation { get; set; }
        private string institution { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        private string  Involvement { get; set; }
        private string otherInvolvement { get; set; }
        private string companiesNumbered { get; set; }
        private string otherCompanies { get; set; }
        private string articleNumber { get; set; }
        private string journal { get; set; }
        public string receiveddate { get; set; }
        private string volume { get; set; }
        private string issue { get; set; } 
        private string pubyear { get; set; }
        private string pubmonth { get; set; }

        private string articleTitle { get; set; }


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
            string freetextentityother3, string articlenumber, string journal, string receiveddate, string volume, string issue, string pubyear, string pubmonth, string title)
        {
            this.authorshipNumber = int.Parse(authorshipnumber);
            this.first = first.ToUpper();
            this.middle = middle.ToUpper();
            this.last = last.ToUpper();
            this.clincalDegree = clinicaldegree;
            this.otherDegree = freetextdegreeother;
            this.USLocation = uslocation;
            this.institution = institution;
            this.city = city.ToUpper();
            this.state = state;
            this.Involvement = type3;
            this.otherInvolvement = freetexttypeother3;
            this.companiesNumbered = entity3;
            this.otherCompanies = freetextentityother3;
            this.articleNumber = articlenumber;
            this.journal = journal;
            this.receiveddate = receiveddate;
            this.volume = volume;
            this.issue = issue;
            this.pubyear = pubyear;
            this.pubmonth = pubmonth;
            this.articleTitle = title;
        }

        /// <summary>
        /// This method uses the BlankCheck method to compare the current variable values to that of the 'new' author.
        /// The 'new' author is really just the same author, but with more information
        /// </summary>
        /// <param name="newAuthor"></param>
        /// <returns></returns>
        internal Person CheckAndMerge(Person newAuthor)
        {
            //if(first.Equals("") && !newAuthor.first.Equals("")) { first = newAuthor.first; }
            first = BlankCheck(first, newAuthor.first);
            middle = BlankCheck(middle, newAuthor.middle);
            last = BlankCheck(last, newAuthor.last);
            clincalDegree = BlankCheck(clincalDegree, newAuthor.clincalDegree);
            otherDegree = BlankCheck(otherDegree, newAuthor.otherDegree);
            USLocation = BlankCheck(USLocation, newAuthor.USLocation);
            institution = BlankCheck(institution, newAuthor.institution);
            city = BlankCheck(city, newAuthor.city);
            state = BlankCheck(state, newAuthor.state);
            Involvement = BlankCheck(Involvement, newAuthor.Involvement);
            otherInvolvement = BlankCheck(otherInvolvement, newAuthor.otherInvolvement);
            companiesNumbered = BlankCheck(companiesNumbered, newAuthor.companiesNumbered);
            otherCompanies = BlankCheck(otherCompanies, newAuthor.otherCompanies);
            articleNumber = BlankCheck(articleNumber, newAuthor.articleNumber);
            journal = BlankCheck(journal, newAuthor.journal);
            receiveddate = BlankCheck(receiveddate, newAuthor.receiveddate);
            volume = BlankCheck(volume, newAuthor.volume);
            issue = BlankCheck(issue, newAuthor.issue);
            pubyear = BlankCheck(pubyear, newAuthor.pubyear);
            pubmonth = BlankCheck(pubmonth, newAuthor.pubmonth);
            articleTitle = BlankCheck(articleTitle, newAuthor.articleTitle);

            return this;

        }

        /// <summary>
        /// This method compares two strings, and if the currentvariable is blank it returns the other variable.
        /// Otherwise, it just returns the currentvariable.
        /// </summary>
        /// <param name="currentVariable"></param>
        /// <param name="newVariable"></param>
        /// <returns></returns>
        private string BlankCheck(string currentVariable, string newVariable)
        {
            if(currentVariable.Equals("") && !(newVariable.Equals("")))
            {
                return newVariable;
            }
            return currentVariable;
        }
    }
}
