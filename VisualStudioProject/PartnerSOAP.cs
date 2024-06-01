using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using System.Collections;
using SFDC_API_Demo.SalesforcePartner;
using System.Configuration;

namespace SFDC_API_Demo
{
    public class PartnerSOAP
    {
        private static SforceService binding;

        public static bool login()
        {
            string username = Properties.Settings.Default.username;
            string password = Properties.Settings.Default.password + Properties.Settings.Default.token;

            // Create a service object 
            binding = new SforceService();

            // Timeout after a minute 
            binding.Timeout = 60000;

            // Try logging in   
            LoginResult lr;
            try
            {

                Console.WriteLine("\nLogging in...\n");
                lr = binding.login(username, password);
            }

            // ApiFault is a proxy stub generated from the WSDL contract when     
            // the web service was imported 
            catch (SoapException e)
            {
                // Write the fault code to the console 
                Console.WriteLine(e.Code);

                // Write the fault message to the console 
                Console.WriteLine("An unexpected error has occurred: " + e.Message);

                // Write the stack trace to the console 
                Console.WriteLine(e.StackTrace);

                // Return False to indicate that the login was not successful 
                return false;
            }

            // Check if the password has expired 
            if (lr.passwordExpired)
            {
                Console.WriteLine("An error has occurred. Your password has expired.");
                return false;
            }

            // Set the returned service endpoint URL
            binding.Url = lr.serverUrl;

            // Set the SOAP header with the session ID returned by
            // the login result. This will be included in all
            // API calls.
            binding.SessionHeaderValue = new SessionHeader();
            binding.SessionHeaderValue.sessionId = lr.sessionId;

            // Return true to indicate that we are logged in, pointed  
            // at the right URL and have our security token in place.     
            return true;
        }

        public static void createSample()
        {
            try
            {
                // Create a new sObject of type Contact
                // and fill out its fields.
                sObject contact = new sObject();
                System.Xml.XmlElement[] contactFields = new System.Xml.XmlElement[6];

                // Create the contact's fields
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                contactFields[0] = doc.CreateElement("FirstName");
                contactFields[0].InnerText = "Otto";
                contactFields[1] = doc.CreateElement("LastName");
                contactFields[1].InnerText = "Jespersen";
                contactFields[2] = doc.CreateElement("Salutation");
                contactFields[2].InnerText = "Professor";
                contactFields[3] = doc.CreateElement("Phone");
                contactFields[3].InnerText = "(999) 555-1234";
                contactFields[4] = doc.CreateElement("Title");
                contactFields[4].InnerText = "Philologist";

                contact.type = "Contact";
                contact.Any = contactFields;

                // Add this sObject to an array
                sObject[] contactList = new sObject[1];
                contactList[0] = contact;

                // Make a create call and pass it the array of sObjects 
                SaveResult[] results = binding.create(contactList);
                // Iterate through the results list
                // and write the ID of the new sObject
                // or the errors if the object creation failed.
                // In this case, we only have one result
                // since we created one contact.
                for (int j = 0; j < results.Length; j++)
                {
                    if (results[j].success)
                    {
                        Console.Write("\nA contact was created with an ID of: "
                                        + results[j].id);
                    }
                    else
                    {
                        // There were errors during the create call,
                        // go through the errors array and write
                        // them to the console
                        for (int i = 0; i < results[j].errors.Length; i++)
                        {
                            Error err = results[j].errors[i];
                            Console.WriteLine("Errors were found on item " + j.ToString());
                            Console.WriteLine("Error code is: " + err.statusCode.ToString());
                            Console.WriteLine("Error message: " + err.message);
                        }
                    }
                }
            }
            catch (SoapException e)
            {
                Console.WriteLine("An unexpected error has occurred: " + e.Message +
                    " Stack trace: " + e.StackTrace);
            }
        }
    }
}