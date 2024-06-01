using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Protocols;
using SFDC_API_Demo.SalesforceEnterprise;

namespace SFDC_API_Demo
{
    public class EnterpriseSOAP
    {
        private static SforceService binding;

        public static bool login()
        {
            Boolean success = false;
            string username = Properties.Settings.Default.username;
            string password = Properties.Settings.Default.password + Properties.Settings.Default.token;

            // Create a service object 
            binding = new SforceService();

            LoginResult lr;
            try
            {
                Console.WriteLine("\nLogging in...\n");
                lr = binding.login(username, password);

                /** 
                   * The login results contain the endpoint of the virtual server instance 
                   * that is servicing your org. Set the URL of the binding 
                   * to this endpoint.
                   */
                // Save old authentication end point URL
                String authEndPoint = binding.Url;
                // Set returned service endpoint URL
                binding.Url = lr.serverUrl;

                /** Get the session ID from the login result and set it for the 
                   * session header that will be used for all subsequent calls.
                   */
                binding.SessionHeaderValue = new SessionHeader();
                binding.SessionHeaderValue.sessionId = lr.sessionId;

                // Print user and session info
                GetUserInfoResult userInfo = lr.userInfo;
                Console.WriteLine("UserID: " + userInfo.userId);
                Console.WriteLine("User Full Name: " +
                      userInfo.userFullName);
                Console.WriteLine("User Email: " +
                      userInfo.userEmail);
                Console.WriteLine();
                Console.WriteLine("SessionID: " +
                      lr.sessionId);
                Console.WriteLine("Auth End Point: " +
                      authEndPoint);
                Console.WriteLine("Service End Point: " +
                      lr.serverUrl);
                Console.WriteLine();

                // Return true to indicate that we are logged in, pointed  
                // at the right URL and have our security token in place.     
                success = true;
            }
            catch (SoapException e)
            {
                Console.WriteLine("An unexpected error has occurred: " +
                                           e.Message + "\n" + e.StackTrace);
            }
            return success;
        }

        public static void createAccount()
        {
            Account a = new Account();
            a.Name = "New Test Account";
            a.ARC101_Demo__c = "Test value";

            SaveResult[] results = binding.create(new sObject[] { a });

            // Check results.
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].success)
                {
                    Console.WriteLine("Successfully created ID: "
                          + results[i].id);
                }
                else
                {
                    Console.WriteLine("Error: could not create sobject "
                          + "for array element " + i + ".");
                    Console.WriteLine("   The error reported was: "
                          + results[i].errors[0].message + "\n");
                }
            }

        }

        public static void queryRecords()
        {
            QueryResult qResult = null;
            try
            {
                String soqlQuery = "SELECT FirstName, LastName FROM Contact";
                qResult = binding.query(soqlQuery);
                Boolean done = false;
                if (qResult.size > 0)
                {
                    Console.WriteLine("Logged-in user can see a total of "
                       + qResult.size + " contact records.");
                    while (!done)
                    {
                        sObject[] records = qResult.records;
                        for (int i = 0; i < records.Length; ++i)
                        {
                            Contact con = (Contact)records[i];
                            String fName = con.FirstName;
                            String lName = con.LastName;
                            if (fName == null)
                            {
                                Console.WriteLine("Contact " + (i + 1) + ": " + lName);
                            }
                            else
                            {
                                Console.WriteLine("Contact " + (i + 1) + ": " + fName
                                      + " " + lName);
                            }
                        }
                        if (qResult.done)
                        {
                            done = true;
                        }
                        else
                        {
                            qResult = binding.queryMore(qResult.queryLocator);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No records found.");
                }
                Console.WriteLine("\nQuery succesfully executed.");
            }
            catch (SoapException e)
            {
                Console.WriteLine("An unexpected error has occurred: " +
                                           e.Message + "\n" + e.StackTrace);
            }
        }
    }
}
