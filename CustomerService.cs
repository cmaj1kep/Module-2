using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using System.Linq;
using System;

namespace Module_2
{
    class CustomerService
    {
        private readonly CrmServiceClient svc;
        private readonly Entity entity;

        public CustomerService(CrmServiceClient svc, Entity entity)
        {
            this.svc = svc;
            this.entity = entity;
        }

        public void CustomerAssign()
        {
            using (OrganizationServiceContext context = new OrganizationServiceContext(svc))
            {
                var accounts = (from _accounts in context.CreateQuery("account")
                                select _accounts
                            ).ToArray();

                EntityReference customerRef = new EntityReference("account", accounts.ElementAt(new Random().Next(0, accounts.Length)).Id);
                entity["drn_customer"] = customerRef;
            }
        }
    }
}
