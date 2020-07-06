OPHTHALMOLOGY CONFLICT OF INTEREST RESEARCH PROGRAM 2020
Author:     Devin Schwehr
Date:       7/6/2020
GitHub ID:  [Your GitHub ID]
Repo:       Actual URL to your GitHub repository

1. Summary

	This program is designed to take a report from RedCap and then compare it to the files taken from the Open Payments Database(OPD) and look for 
	any discrepancies between the list of companies an author has reported receiving payments from, and the list of companies that have reported paying the author. 
	
	It does this by looking through 4 years of files from the receive date of the article and searching by a first and last name basis. After that, it then looks for 
	authors within that list that live in the same city and state as reported by the author. For each match, the entire row in the OPD is saved to a list to be analyzed
	at a later point in the program. If the program cannot thin down the list to one author, that search is skipped. There are three searches per year, as there are 
	three OPD files: General Payments, Research Payments, and Ownership Payments.

	When analyzing the outputted search results, there are 4 different outcomes.
		1. A company reported paying the author, but the author did not list that company in their article.
		2. An author reports receiving payment from the company, but there is no such payment within the OPD.
		3. Both lists match and there is no discrepancy.
		4. The author is not found within the OPD at all, but doesn't report taking payments. (this is an edge case for outcome 3)

	The purpose of this project is more to analyze how many times outcome 1 happens, and why. Every time outcome 1 or 2 occurs, that discrepancy will be output to the 
	main console with some key information:
		1. Discrepancy type (1 or 2)
		2. Author name
		3. Company name
		4. Date of discrepancy
		5. type of payment
		6. Payment amount

	This data should help us find out what payments are most often left out and shine some light into this issue within the Ophthalmology field.

2. References:
	Creating a CSV File from a string Array - https://stackoverflow.com/questions/22655312/creating-csv-file-from-string-array


