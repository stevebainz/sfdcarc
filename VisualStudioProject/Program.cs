using SFDC_API_Demo.SalesforceEnterprise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Protocols;

namespace SFDC_API_Demo
{
    public class Program
    {
        static void Main(string[] args)
        {
            bool isLoggedIn = EnterpriseSOAP.login();
            EnterpriseSOAP.createAccount();
            EnterpriseSOAP.queryRecords();

            bool isLoggedInPartner = PartnerSOAP.login();
            PartnerSOAP.createSample();
        }
    }
}
