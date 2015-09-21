using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mozu.Api;
using Autofac;
using Mozu.Api.ToolKit.Config;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace Mozu_BED_Training_Exercise_13_1
{
    [TestClass]
    public class MozuDataConnectorTests
    {
        private IApiContext _apiContext;
        private IContainer _container;

        [TestInitialize]
        public void Init()
        {
            _container = new Bootstrapper().Bootstrap().Container;
            var appSetting = _container.Resolve<IAppSetting>();
            var tenantId = int.Parse(appSetting.Settings["TenantId"].ToString());
            var siteId = int.Parse(appSetting.Settings["SiteId"].ToString());

            _apiContext = new ApiContext(tenantId, siteId);
        }

        [TestMethod]
        public void Exercise_13_1_Get_Customers()
        {
            //Create a Customer Account resource
            var customerAccountResource = new Mozu.Api.Resources.Commerce.Customer.CustomerAccountResource(_apiContext);

            //Retrieve an Account by id
            var account = customerAccountResource.GetAccountAsync(1001).Result;

            //Write the Account email
            System.Diagnostics.Debug.WriteLine("Account Email[{0}]: {1}", account.Id, account.EmailAddress);

            //You can also filter the Accounts Get call by email
            var accountByEmail = customerAccountResource.GetAccountsAsync(filter: "EmailAddress eq 'test@customer.com'").Result;

            //write account email
            System.Diagnostics.Debug.WriteLine("Account Email[{0}]: {1}", account.EmailAddress, account.Id);

            //Now, create a Customer Contact resource
            var customerContactResource = new Mozu.Api.Resources.Commerce.Customer.Accounts.CustomerContactResource(_apiContext);

            var customerContactCollection = customerContactResource.GetAccountContactsAsync(accountByEmail.Items[0].Id).Result;

            foreach(var contact in customerContactCollection.Items)
            {
                System.Diagnostics.Debug.WriteLine("Name:");
                System.Diagnostics.Debug.WriteLine(contact.FirstName);
                System.Diagnostics.Debug.WriteLine(contact.MiddleNameOrInitial);
                System.Diagnostics.Debug.WriteLine(contact.LastNameOrSurname);
                System.Diagnostics.Debug.WriteLine("Address:");
                System.Diagnostics.Debug.WriteLine(contact.Address.Address1);
                System.Diagnostics.Debug.WriteLine(contact.Address.Address2);
                System.Diagnostics.Debug.WriteLine(contact.Address.Address3);
                System.Diagnostics.Debug.WriteLine(contact.Address.Address4);
                System.Diagnostics.Debug.WriteLine(contact.Address.CityOrTown);
                System.Diagnostics.Debug.WriteLine(contact.Address.StateOrProvince);
                System.Diagnostics.Debug.WriteLine(contact.Address.PostalOrZipCode);
                System.Diagnostics.Debug.WriteLine(contact.Address.CountryCode);
                System.Diagnostics.Debug.WriteLine(String.Format("Is a validated address? {0}", contact.Address.IsValidated));
            }

            //Create a Customer Credit resource
            var creditResource = new Mozu.Api.Resources.Commerce.Customer.CreditResource(_apiContext);

            //Get credits by customer account id
            var customerCredits = creditResource.GetCreditsAsync(filter: "CustomerId eq '1001'").Result;

            foreach (var customerCredit in customerCredits.Items)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Customer Credit[{0}]: Code({1})Balance ({2})", customerCredit.CustomerId, customerCredit.Code, customerCredit.CurrentBalance));
            }
        }
    }
}
