@RestResource(urlMapping='/ARC101Account/*')
global with sharing class MyRestResource {

    @HttpDelete
    global static void doDelete() {
        RestRequest req = RestContext.request;
        RestResponse res = RestContext.response;
        String accountId = req.requestURI.substring(req.requestURI.lastIndexOf('/')+1);
        Account account = [SELECT Id FROM Account WHERE Id = :accountId];
        delete account;
    }
  
    @HttpGet
    global static String doGet() {
        RestRequest req = RestContext.request;
        RestResponse res = RestContext.response;
        String accountId = req.requestURI.substring(req.requestURI.lastIndexOf('/')+1);
        String results = '';
        Account account = null;
        try { 
            account = [SELECT Id, Name, Phone, Website FROM Account WHERE Id = :accountId];
            results = 'Account found: ' + accountId;
        }
        catch (Exception ex) {
            results = 'Account not found with Id ' + accountId;
        }
        return results;
    }
  
    @HttpPost
    global static Account doPost(String name,
        String phone, String website, String accountIndustry) {
        Account account = new Account();
        account.Name = name;
        account.phone = phone;
        account.website = website;
        account.Industry = accountIndustry;
        account.Description = 'This account was created with an Apex REST Service on March 18';
        account.Rating = 'Hot';
        insert account;
        /*
        custom logic here
        */
        RestContext.response.addHeader('Content-Type', 'application/json');
        return account;
    }
}

/*
{
  "name" : "ARC101 Demo April 11",
  "phone" : "707-555-1234",
  "website" : "www.arc101.com"
}
*/