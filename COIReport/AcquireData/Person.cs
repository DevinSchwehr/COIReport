using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

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
        public string cities { get; set; }
        public string states { get; set; }
        private string  Involvement { get; set; }
        private string otherInvolvement { get; set; }
        public string companiesNumbered { get; set; }
        public string otherCompanies { get; set; }
        public string articleNumber { get; set; }
        public string journal { get; set; }
        public string receiveddate { get; set; }
        private string volume { get; set; }
        private string issue { get; set; } 
        private string pubyear { get; set; }
        private string pubmonth { get; set; }

        private string articleTitle { get; set; }
        public string position { get; set; }


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
        public Person(string authorshipnumber, string articlenumber, string last, string first, string uslocation, string clinicaldegree, string city, string state, string any_disclosures,
            string entity, string freetextentityother3, string journal, string volume, string issue, string startpage, string pubyear, string pubmonth, string receiveddate, string title,
            string middle, string position, string freetextdegreeother)
        {
            try
            {
            this.authorshipNumber = int.Parse(authorshipnumber);
            }
            catch(Exception e) { Console.WriteLine(e); }
            this.first = first.ToUpper();
            this.middle = middle.ToUpper();
            this.last = last.ToUpper();
            //this.clincalDegree = clinicaldegree;
            //this.otherDegree = freetextdegreeother;
            this.USLocation = uslocation;
            //this.institution = institution;
            this.cities = city.ToUpper();
            this.states = state;
            //this.Involvement = type3;
            //this.otherInvolvement = freetexttypeother3;
            this.companiesNumbered = entity;
            this.otherCompanies = freetextentityother3;
            this.articleNumber = articlenumber;
            this.journal = journal;
            this.receiveddate = receiveddate;
            this.volume = volume;
            this.issue = issue;
            this.pubyear = pubyear;
            this.pubmonth = pubmonth;
            this.articleTitle = title;
            this.position = position;
        }

        /// <summary>
        /// This method uses the BlankCheck method to compare the current variable values to that of the 'new' author.
        /// The 'new' author is really just the same author, but with more information
        /// </summary>
        /// <param name="newAuthor"></param>
        /// <returns></returns>
        internal Person CheckAndMerge(Person newAuthor)
        {
            first = BlankCheck(first, newAuthor.first);
            middle = BlankCheck(middle, newAuthor.middle);
            last = BlankCheck(last, newAuthor.last);
           // clincalDegree = BlankCheck(clincalDegree, newAuthor.clincalDegree);
            //otherDegree = BlankCheck(otherDegree, newAuthor.otherDegree);
            USLocation = BlankCheck(USLocation, newAuthor.USLocation);
            //institution = BlankCheck(institution, newAuthor.institution);
            cities = OtherEntityCheck(cities, newAuthor.cities);
            if(!(newAuthor.states.Equals(""))) { states += newAuthor.states + ','; }
           // states = NumberEntityCheck(states, newAuthor.states);
           // Involvement = BlankCheck(Involvement, newAuthor.Involvement);
           // otherInvolvement = BlankCheck(otherInvolvement, newAuthor.otherInvolvement);
            companiesNumbered = NumberEntityCheck(companiesNumbered, newAuthor.companiesNumbered);
            otherCompanies = OtherEntityCheck(otherCompanies, newAuthor.otherCompanies);
            articleNumber = BlankCheck(articleNumber, newAuthor.articleNumber);
            journal = BlankCheck(journal, newAuthor.journal);
            receiveddate = BlankCheck(receiveddate, newAuthor.receiveddate);
            volume = BlankCheck(volume, newAuthor.volume);
            //issue = BlankCheck(issue, newAuthor.issue);
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
            //if(newVariable == null) { return currentVariable; }
            //if(currentVariable == null && newVariable != null) { currentVariable = newVariable; }
            if(currentVariable.Equals("") && !(newVariable.Equals("")))
            {
                return newVariable;
            }
            return currentVariable;
        }

        /// <summary>
        /// This is used for getting all of the 'other' companies or cities. We don't use BlankCheck here
        /// because there can be multiple companies, all on their own individual row. So we add them
        /// to the string as we find them instead of doing a BlankCheck.
        /// </summary>
        /// <param name="currentList">the current list of other companies</param>
        /// <param name="newCompany">the company that is being checked to be added in or not</param>
        /// <returns>the string either with the new company added, or the string as it was.</returns>
        private string OtherEntityCheck(string currentList, string newCompany)
        {
            if (currentList.Contains(newCompany))
            {
                return currentList;
            }
            else
            {
                if (currentList.Equals("")) { return newCompany; }
                currentList += $",{newCompany}";
                return currentList;
            }
        }

       /// <summary>
       /// This method is used to merge incoming lists of companies so that some aren't overwritten.
       /// This is similar to the OtherCompanyCheck method but is not the same due to the fact that 
       /// newCompanyList can have multiple numbers in it.
       /// </summary>
       /// <param name="currentList">the current list of numbered companies</param>
       /// <param name="newCompanyList">a new list of numbered companies</param>
       /// <returns>a company list that has all the numbers of the old list plus all of the new companies
       ///  in the newCompanyList</returns>
       private string NumberEntityCheck(string currentList, string newCompanyList)
        {
            HashSet<string> current = currentList.Split(',').ToHashSet();
            string[] newlist = newCompanyList.Split(',');
            for(int i = 0; i < newlist.Length; i++)
            {
                current.Add(newlist[i]);
            }
            string output = string.Join(',', current.ToArray());
            if(output.Length > 0 && output[0].Equals(',')) { output = output.Substring(1, output.Length - 1); }
            return output;
        }

    }
}
